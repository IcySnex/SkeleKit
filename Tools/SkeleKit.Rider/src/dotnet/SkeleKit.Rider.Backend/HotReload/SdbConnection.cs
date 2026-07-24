using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace SkeleKit.Rider.Backend.HotReload;

// A Mono soft-debugger client speaking the wire protocol to the app's debugger agent. It handles the
// Microsoft.iOS "start debugger: sdb" preamble, the DWP-Handshake, then command/reply framing.
//
// Two modes:
//   • self-drive — we are the only debugger; replies route to our waiters, events are ignored.
//   • relay      — an IDE is also on the wire. Our injected commands use a high id range; their
//                  replies are consumed here, everything else (the IDE's replies, events) is forwarded
//                  to the IDE. The IDE's own commands are pumped straight through to the app.
sealed class SdbConnection
{
	const byte CmdSetVm = 1;
	const byte CmdSetAppDomain = 20;
	const byte CmdSetAssembly = 21;
	const byte CmdSetModule = 24;

	const int InjectedIdBase = 0x40000000;

	static readonly byte[] Handshake = "DWP-Handshake"u8.ToArray();

	readonly Socket app;
	readonly ConcurrentDictionary<int, TaskCompletionSource<(int Error, byte[] Data)>> pending = [];
	readonly List<byte[]> buffered = [];

	Socket? ide;
	Action? onClosed;
	bool buffering;
	int nextId;

	SdbConnection(
		Socket app)
	{
		this.app = app;
	}

	public static SdbConnection Adopt(
		Socket socket)
	{
		byte[] command = "start debugger: sdb"u8.ToArray();
		socket.Send([(byte)command.Length, .. command]);

		ReadExactly(socket, Handshake.Length);
		socket.Send(Handshake);

		SdbConnection connection = new(socket)
		{
			buffering = true,
			nextId = 1
		};
		connection.StartReader();

		return connection;
	}

	public static void PipeOutput(
		Socket socket)
	{
		byte[] command = "connect output"u8.ToArray();
		socket.Send([(byte)command.Length, .. command]);

		Thread thread = new(() =>
		{
			try
			{
				using StreamReader reader = new(new NetworkStream(socket, ownsSocket: true));
				string? line;
				while ((line = reader.ReadLine()) is not null)
					Console.WriteLine($"[app] {line}");
			}
			catch { }
		})
		{
			IsBackground = true,
			Name = "skele-app-output"
		};
		thread.Start();
	}

	// MITM: sit between the app (accepted on our port) and Rider (we connected to its listener). Rider
	// runs the debugger; we relay everything transparently and inject apply-changes on the side. The
	// app->Rider stream begins with the 13-byte DWP-Handshake, then sdb frames; we forward the handshake
	// raw, then frame-parse so we can swallow our own injected replies + the ENC events.
	public static SdbConnection Mitm(
		Socket appSocket,
		Socket riderSocket,
		Action onClosed)
	{
		SdbConnection connection = new(appSocket)
		{
			ide = riderSocket,
			onClosed = onClosed,
			buffering = false,
			nextId = InjectedIdBase
		};

		// Rider -> app: pure passthrough (Rider drives the debug session)
		Thread pump = new(connection.PumpIdeToApp)
		{
			IsBackground = true,
			Name = "skele-sdb-rider"
		};
		pump.Start();

		// app -> Rider: forward the handshake raw, then frame-parse
		Thread reader = new(connection.ReadMitm)
		{
			IsBackground = true,
			Name = "skele-sdb-mitm"
		};
		reader.Start();

		return connection;
	}

	void ReadMitm()
	{
		try
		{
			byte[] handshake = ReadExactly(app, Handshake.Length);
			ide!.Send(handshake);

			while (true)
			{
				byte[] header = ReadExactly(app, 11);
				int length = BinaryPrimitives.ReadInt32BigEndian(header);
				int id = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(4));
				byte flags = header[8];

				byte[] payload = length > 11 ? ReadExactly(app, length - 11) : [];

				if ((flags & 0x80) != 0 && pending.TryRemove(id, out TaskCompletionSource<(int, byte[])>? waiter))
				{
					waiter.TrySetResult((BinaryPrimitives.ReadInt16BigEndian(header.AsSpan(9)), payload));
					continue;
				}

				// forward everything (incl. the ENC/METHOD_UPDATE events our apply triggers) so Rider
				// learns of the new method versions and re-syncs its symbols for edited methods
				ide!.Send([.. header, .. payload]);
			}
		}
		catch
		{
			foreach (TaskCompletionSource<(int, byte[])> waiter in pending.Values)
				waiter.TrySetException(new IOException("sdb connection closed"));
		}
	}

	public void SelfDrive() => buffering = false;

	public void Relay(
		Socket ideSocket)
	{
		ide = ideSocket;
		nextId = InjectedIdBase;

		ideSocket.Send(Handshake);
		ReadExactly(ideSocket, Handshake.Length);

		lock (buffered)
		{
			buffering = false;
			foreach (byte[] packet in buffered)
				ideSocket.Send(packet);

			buffered.Clear();
		}

		Thread pump = new(PumpIdeToApp)
		{
			IsBackground = true,
			Name = "skele-sdb-ide"
		};
		pump.Start();
	}

	void PumpIdeToApp()
	{
		try
		{
			byte[] chunk = new byte[8192];
			while (true)
			{
				int read = ide!.Receive(chunk);
				if (read == 0)
					break;

				lock (app)
					app.Send(chunk.AsSpan(0, read).ToArray());
			}
		}
		catch { }
		finally
		{
			CloseBoth();
		}
	}

	int closed;

	void CloseBoth()
	{
		if (Interlocked.Exchange(ref closed, 1) == 1)
			return;

		try { app.Dispose(); } catch { }
		try { ide?.Dispose(); } catch { }
		try { onClosed?.Invoke(); } catch { }
	}

	void StartReader()
	{
		Thread thread = new(Read)
		{
			IsBackground = true,
			Name = "skele-sdb-reader"
		};
		thread.Start();
	}

	void Read()
	{
		try
		{
			while (true)
			{
				byte[] header = ReadExactly(app, 11);
				int length = BinaryPrimitives.ReadInt32BigEndian(header);
				int id = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(4));
				byte flags = header[8];

				byte[] payload = length > 11 ? ReadExactly(app, length - 11) : [];

				// our injected reply — consume it, never let the IDE see it
				if ((flags & 0x80) != 0 && pending.TryRemove(id, out TaskCompletionSource<(int, byte[])>? waiter))
				{
					waiter.TrySetResult((BinaryPrimitives.ReadInt16BigEndian(header.AsSpan(9)), payload));
					continue;
				}

				if (buffering)
				{
					lock (buffered)
						if (buffering)
						{
							buffered.Add([.. header, .. payload]);
							continue;
						}
				}

				// relay: forward the app's own replies/events to the IDE (dropping the EnC events our
				// apply triggers, which the IDE doesn't expect)
				if (ide is Socket target && !IsEncEvent(header, payload))
					target.Send([.. header, .. payload]);
			}
		}
		catch
		{
			foreach (TaskCompletionSource<(int, byte[])> waiter in pending.Values)
				waiter.TrySetException(new IOException("sdb connection closed"));
		}
		finally
		{
			// the app died/detached; hand Rider a clean VM_DEATH so it ends the session instead of
			// hanging on the dropped socket
			try { ide?.Send(VmDeath()); } catch { }
			CloseBoth();
		}
	}

	// a COMPOSITE event (set 64, cmd 100) carrying one VMDeath (kind 0x01, exit_code 0)
	static byte[] VmDeath()
	{
		byte[] packet = new byte[25];
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(0), packet.Length);
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(4), 0);
		packet[8] = 0;
		packet[9] = 64;
		packet[10] = 100;
		packet[11] = 0;
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(12), 1);
		packet[16] = 0x01;
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(17), 0);
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(21), 0);

		return packet;
	}

	static bool IsEncEvent(
		byte[] header,
		byte[] payload)
	{
		// a COMPOSITE event (cmd set 64 EVENT, cmd 100) whose first event is ENC_UPDATE(18) or
		// METHOD_UPDATE(19), sent with no suspend — safe to swallow
		if ((header[8] & 0x80) != 0 || header[9] != 64 || header[10] != 100 || payload.Length < 6)
			return false;

		byte suspend = payload[0];
		byte kind = payload[5];

		return suspend == 0 && kind is 18 or 19;
	}

	(int Error, byte[] Data) Command(
		byte commandSet,
		byte command,
		byte[] payload)
	{
		int id = Interlocked.Increment(ref nextId);
		TaskCompletionSource<(int, byte[])> waiter = new(TaskCreationOptions.RunContinuationsAsynchronously);
		pending[id] = waiter;

		byte[] packet = new byte[11 + payload.Length];
		BinaryPrimitives.WriteInt32BigEndian(packet, packet.Length);
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(4), id);
		packet[8] = 0;
		packet[9] = commandSet;
		packet[10] = command;
		payload.CopyTo(packet.AsSpan(11));

		lock (app)
			app.Send(packet);

		if (!waiter.Task.Wait(TimeSpan.FromSeconds(10)))
			throw new TimeoutException($"sdb command {commandSet}/{command} timed out");

		return waiter.Task.Result;
	}


	public (string Name, int Major, int Minor) Version()
	{
		(_, byte[] data) = Command(CmdSetVm, 1, []);
		int offset = 0;
		string name = ReadString(data, ref offset);
		int major = ReadInt(data, ref offset);
		int minor = ReadInt(data, ref offset);

		return (name, major, minor);
	}

	public void SetProtocolVersion(
		int major,
		int minor) =>
		Command(CmdSetVm, 8, [.. Int(major), .. Int(minor)]);

	public void Resume() => Command(CmdSetVm, 4, []);

	public int RootDomain()
	{
		(_, byte[] data) = Command(CmdSetAppDomain, 1, []);
		int offset = 0;

		return ReadInt(data, ref offset);
	}

	public int FindModule(
		int domain,
		string assemblyName)
	{
		(_, byte[] data) = Command(CmdSetAppDomain, 3, Int(domain));
		int offset = 0;
		int count = ReadInt(data, ref offset);

		for (int index = 0; index < count; index++)
		{
			int assembly = ReadInt(data, ref offset);

			(_, byte[] name) = Command(CmdSetAssembly, 6, Int(assembly));
			int nameOffset = 0;
			if (ReadString(name, ref nameOffset).Contains($"{assemblyName},", StringComparison.Ordinal))
			{
				(_, byte[] module) = Command(CmdSetAssembly, 3, Int(assembly));
				int moduleOffset = 0;

				return ReadInt(module, ref moduleOffset);
			}
		}

		return 0;
	}

	int CreateByteArray(
		int domain,
		byte[] bytes)
	{
		(_, byte[] data) = Command(CmdSetAppDomain, 8, [.. Int(domain), .. Int(bytes.Length), .. bytes]);
		int offset = 0;

		return ReadInt(data, ref offset);
	}

	// Applies an EnC delta over the debugger: pushes the byte arrays into the runtime, then calls
	// MODULE APPLY_CHANGES with their object ids. Returns the sdb error code (0 is success).
	public int ApplyChanges(
		int domain,
		int module,
		byte[] metadata,
		byte[] il,
		byte[] pdb)
	{
		int meta = CreateByteArray(domain, metadata);
		int ilId = CreateByteArray(domain, il);
		int pdbId = CreateByteArray(domain, pdb);

		(int error, _) = Command(CmdSetModule, 2, [.. Int(module), .. Int(meta), .. Int(ilId), .. Int(pdbId)]);

		return error;
	}


	static byte[] Int(
		int value)
	{
		byte[] buffer = new byte[4];
		BinaryPrimitives.WriteInt32BigEndian(buffer, value);

		return buffer;
	}

	static int ReadInt(
		byte[] data,
		ref int offset)
	{
		int value = BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(offset));
		offset += 4;

		return value;
	}

	static string ReadString(
		byte[] data,
		ref int offset)
	{
		int length = ReadInt(data, ref offset);
		string value = Encoding.UTF8.GetString(data, offset, length);
		offset += length;

		return value;
	}

	static byte[] ReadExactly(
		Socket socket,
		int count)
	{
		byte[] buffer = new byte[count];

		int read = 0;
		while (read < count)
		{
			int chunk = socket.Receive(buffer, read, count - read, SocketFlags.None);
			if (chunk == 0)
				throw new EndOfStreamException();

			read += chunk;
		}

		return buffer;
	}
}
