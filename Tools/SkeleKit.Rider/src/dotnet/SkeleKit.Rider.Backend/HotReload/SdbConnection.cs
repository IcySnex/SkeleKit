using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace SkeleKit.Rider.Backend.HotReload;

// One socket the app opened, wired through to Rider.
//
// The app makes several connections (the soft-debugger one plus stdout and stderr) and they are not
// distinguishable by order, so each one identifies itself: only the debugger connection opens with the
// DWP handshake. That one gets frame-parsed, which lets us inject apply-changes commands of our own
// while everything the app and Rider say to each other passes through untouched, so breakpoints and
// stepping are unaffected. Injected commands take ids from a reserved high range and their replies are
// consumed here, so Rider never sees traffic it did not ask for.
sealed class SdbConnection
{
	const byte CmdSetAppDomain = 20;
	const byte CmdSetAssembly = 21;
	const byte CmdSetModule = 24;

	const int InjectedIdBase = 0x40000000;

	static readonly byte[] Handshake = "DWP-Handshake"u8.ToArray();
	static readonly TimeSpan CommandTimeout = TimeSpan.FromSeconds(10);

	readonly Socket app;
	readonly Socket ide;
	readonly ConcurrentDictionary<int, TaskCompletionSource<(int Error, byte[] Data)>> pending = [];
	readonly object ideLock = new();

	readonly Action<SdbConnection> onSdbIdentified;
	readonly Action onSdbClosed;
	readonly Action<SdbConnection> onOutput;

	bool isSdb;
	int nextId = InjectedIdBase;
	int closed;

	SdbConnection(
		Socket app,
		Socket ide,
		Action<SdbConnection> onSdbIdentified,
		Action onSdbClosed,
		Action<SdbConnection> onOutput)
	{
		this.app = app;
		this.ide = ide;
		this.onSdbIdentified = onSdbIdentified;
		this.onSdbClosed = onSdbClosed;
		this.onOutput = onOutput;
	}

	public static SdbConnection Mitm(
		Socket appSocket,
		Socket riderSocket,
		Action<SdbConnection> onSdbIdentified,
		Action onSdbClosed,
		Action<SdbConnection> onOutput)
	{
		SdbConnection connection = new(appSocket, riderSocket, onSdbIdentified, onSdbClosed, onOutput);

		Start(connection.PumpIdeToApp, "skele-sdb-rider");
		Start(connection.ReadApp, "skele-sdb-mitm");

		return connection;
	}

	// Write raw bytes to Rider on this (output) connection, serialized against the relay pump so host
	// console notices cannot interleave mid-write with the app's own output.
	public void SendToIde(
		byte[] data)
	{
		lock (ideLock)
		{
			try { ide.Send(data); } catch { }
		}
	}

	void ReadApp()
	{
		try
		{
			byte[] first = ReadExactly(app, Handshake.Length);
			SendToIde(first);

			if (!first.AsSpan().SequenceEqual(Handshake))
			{
				RelayOutput();
				return;
			}

			isSdb = true;
			onSdbIdentified(this);

			ReadFrames();
		}
		catch
		{
			FailPending();
		}
		finally
		{
			if (isSdb)
			{
				// hand Rider a clean VM_DEATH so it ends the session instead of hanging on the drop
				SendToIde(VmDeath());
				try { onSdbClosed(); } catch { }
			}

			CloseBoth();
		}
	}

	void RelayOutput()
	{
		onOutput(this);

		byte[] buffer = new byte[8192];
		while (true)
		{
			int read = app.Receive(buffer);
			if (read == 0)
				break;

			SendToIde(buffer.AsSpan(0, read).ToArray());
		}
	}

	void ReadFrames()
	{
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

			// forward everything, including the ENC/METHOD_UPDATE events our apply triggers
			SendToIde([.. header, .. payload]);
		}
	}

	void PumpIdeToApp()
	{
		try
		{
			byte[] chunk = new byte[8192];
			while (true)
			{
				int read = ide.Receive(chunk);
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

	void FailPending()
	{
		foreach (int id in pending.Keys)
			if (pending.TryRemove(id, out TaskCompletionSource<(int, byte[])>? waiter))
				waiter.TrySetException(new IOException("the debug connection closed"));
	}

	void CloseBoth()
	{
		if (Interlocked.Exchange(ref closed, 1) == 1)
			return;

		FailPending();

		try { app.Dispose(); } catch { }
		try { ide.Dispose(); } catch { }
	}

	(int Error, byte[] Data) Command(
		byte commandSet,
		byte command,
		byte[] payload)
	{
		int id = Interlocked.Increment(ref nextId);
		TaskCompletionSource<(int, byte[])> waiter = new(TaskCreationOptions.RunContinuationsAsynchronously);
		pending[id] = waiter;

		try
		{
			byte[] packet = new byte[11 + payload.Length];
			BinaryPrimitives.WriteInt32BigEndian(packet, packet.Length);
			BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(4), id);
			packet[8] = 0;
			packet[9] = commandSet;
			packet[10] = command;
			payload.CopyTo(packet.AsSpan(11));

			lock (app)
				app.Send(packet);

			if (!waiter.Task.Wait(CommandTimeout))
				throw new TimeoutException(
					$"the app did not answer sdb command {commandSet}/{command} within {CommandTimeout.TotalSeconds:0}s; it is probably stopped at a breakpoint");

			return waiter.Task.Result;
		}
		finally
		{
			pending.TryRemove(id, out _);
		}
	}

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
			if (ReadString(name, ref nameOffset).StartsWith($"{assemblyName},", StringComparison.Ordinal))
			{
				(_, byte[] module) = Command(CmdSetAssembly, 3, Int(assembly));
				int moduleOffset = 0;

				return ReadInt(module, ref moduleOffset);
			}
		}

		return 0;
	}

	// MODULE_GET_INFO answers name, scopename, fqname, guid, assembly. The guid is the module's MVID,
	// which says whether the app is running the same build we baselined against.
	public Guid ModuleMvid(
		int module)
	{
		(_, byte[] data) = Command(CmdSetModule, 1, Int(module));
		int offset = 0;

		ReadString(data, ref offset);
		ReadString(data, ref offset);
		ReadString(data, ref offset);

		return Guid.TryParse(ReadString(data, ref offset), out Guid mvid) ? mvid : Guid.Empty;
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

	int CreateByteArray(
		int domain,
		byte[] bytes)
	{
		(_, byte[] data) = Command(CmdSetAppDomain, 8, [.. Int(domain), .. Int(bytes.Length), .. bytes]);
		int offset = 0;

		return ReadInt(data, ref offset);
	}

	static void Start(
		ThreadStart body,
		string name)
	{
		Thread thread = new(body)
		{
			IsBackground = true,
			Name = name
		};
		thread.Start();
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
