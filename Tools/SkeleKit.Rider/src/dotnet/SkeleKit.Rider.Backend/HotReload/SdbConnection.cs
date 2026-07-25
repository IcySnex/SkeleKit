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
	const byte CmdSetVm = 1;
	const byte CmdSetAppDomain = 20;
	const byte CmdSetAssembly = 21;
	const byte CmdSetModule = 24;

	const int InjectedIdBase = 0x40000000;
	public const int InvalidObjectError = 20;

	static readonly byte[] Handshake = "DWP-Handshake"u8.ToArray();
	static readonly TimeSpan CommandTimeout = TimeSpan.FromSeconds(10);

	readonly Socket app;
	readonly Socket ide;
	readonly ConcurrentDictionary<int, TaskCompletionSource<(int Error, byte[] Data)>> pending = [];
	readonly object ideLock = new();

	readonly Action<SdbConnection> onSdbIdentified;
	readonly Action<SdbConnection> onSdbClosed;

	bool isSdb;
	int nextId = InjectedIdBase;
	int logThreadId;
	int closed;

	SdbConnection(
		Socket app,
		Socket ide,
		Action<SdbConnection> onSdbIdentified,
		Action<SdbConnection> onSdbClosed)
	{
		this.app = app;
		this.ide = ide;
		this.onSdbIdentified = onSdbIdentified;
		this.onSdbClosed = onSdbClosed;
	}

	public static SdbConnection Mitm(
		Socket appSocket,
		Socket riderSocket,
		Action<SdbConnection> onSdbIdentified,
		Action<SdbConnection> onSdbClosed)
	{
		SdbConnection connection = new(appSocket, riderSocket, onSdbIdentified, onSdbClosed);

		Start(connection.PumpIdeToApp, "skele-sdb-rider");
		Start(connection.ReadApp, "skele-sdb-mitm");

		return connection;
	}

	void SendToIde(
		byte[] data)
	{
		lock (ideLock)
		{
			try { SendAll(ide, data); } catch { }
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
				try { onSdbClosed(this); } catch { }
			}

			CloseBoth();
		}
	}

	void RelayOutput()
	{
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
			if (length < 11 || length > 256 * 1024 * 1024)
				throw new InvalidDataException($"invalid sdb packet length {length}");

			int id = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(4));
			byte flags = header[8];

			byte[] payload = length > 11 ? ReadExactly(app, length - 11) : [];

			if ((flags & 0x80) != 0 && pending.TryRemove(id, out TaskCompletionSource<(int, byte[])>? waiter))
			{
				waiter.TrySetResult((BinaryPrimitives.ReadInt16BigEndian(header.AsSpan(9)), payload));
				continue;
			}

			// Forward every real runtime event. APPLY_CHANGES does not emit ENC_UPDATE on this Mono
			// runtime, so ReloadEngine sends that standard event explicitly after staging Rider's delta.
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
					SendAll(app, chunk, read);
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
				SendAll(app, packet);

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

	// USER_LOG is the debugger protocol event used by Debug.WriteLine. Injecting the same event keeps
	// hot-reload feedback in Rider's existing Debug output, without borrowing stdout or creating a
	// second UI surface.
	public void UserLog(
		string message)
	{
		int thread = Volatile.Read(ref logThreadId);
		if (thread == 0)
		{
			thread = FirstThread();
			Volatile.Write(ref logThreadId, thread);
		}

		SendToIde(UserLogEvent(thread, $"[SkeleKit] {message}\n"));
	}

	int FirstThread()
	{
		(int error, byte[] data) = Command(CmdSetVm, 2, []);
		if (error != 0)
			throw new InvalidOperationException($"ALL_THREADS failed with sdb error {error}");

		int offset = 0;
		int count = ReadInt(data, ref offset);
		if (count < 1)
			throw new InvalidOperationException("the app has no debugger-visible threads");

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

	// Mono applies deltas injected through MODULE/APPLY_CHANGES but does not publish the ENC_UPDATE
	// event that Rider's own hot-reload path relies on. The delta is already staged in Rider's
	// debugger worker; this event tells its built-in processor to consume it, update the PDB reader,
	// and rebind breakpoints. The event's metadata/PDB fields are intentionally empty because Rider
	// reads the complete EnCDelta from that staged storage.
	public void NotifyEnCUpdate(
		int module)
	{
		SendToIde(EnCUpdateEvent(module));
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

	// A COMPOSITE event (set 64, cmd 100) carrying one USER_LOG. Object ids are four bytes in
	// Mono's negotiated protocol used by Rider and by every command above.
	static byte[] UserLogEvent(
		int thread,
		string message)
	{
		byte[] category = Encoding.UTF8.GetBytes("");
		byte[] text = Encoding.UTF8.GetBytes(message);
		byte[] packet = new byte[37 + category.Length + text.Length];

		CompositeHeader(packet, 0x10);
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(21), thread);
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(25), 0);
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(29), category.Length);
		category.CopyTo(packet.AsSpan(33));
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(33 + category.Length), text.Length);
		text.CopyTo(packet.AsSpan(37 + category.Length));

		return packet;
	}

	// ENC_UPDATE is event kind 18. Its payload is thread id, module id, metadata delta and PDB delta;
	// ids are four bytes in the protocol negotiated by this iOS runtime.
	static byte[] EnCUpdateEvent(
		int module)
	{
		byte[] packet = new byte[37];

		CompositeHeader(packet, 0x12);
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(21), 0);
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(25), module);
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(29), 0);
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(33), 0);

		return packet;
	}

	// A clean VM_DEATH keeps Rider from hanging when the app socket disappears. Every COMPOSITE event,
	// including VM_DEATH, contains a thread id before its event-specific payload.
	static byte[] VmDeath()
	{
		byte[] packet = new byte[29];
		CompositeHeader(packet, 0x01);
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(21), 0);
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(25), 0);

		return packet;
	}

	static void CompositeHeader(
		byte[] packet,
		byte eventKind)
	{
		BinaryPrimitives.WriteInt32BigEndian(packet, packet.Length);
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(4), 0);
		packet[8] = 0;
		packet[9] = 64;
		packet[10] = 100;
		packet[11] = 0;
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(12), 1);
		packet[16] = eventKind;
		BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(17), 0);
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

	static void SendAll(
		Socket socket,
		byte[] data,
		int count = -1)
	{
		if (count < 0)
			count = data.Length;

		int sent = 0;
		while (sent < count)
		{
			int chunk = socket.Send(data, sent, count - sent, SocketFlags.None);
			if (chunk == 0)
				throw new EndOfStreamException();

			sent += chunk;
		}
	}
}
