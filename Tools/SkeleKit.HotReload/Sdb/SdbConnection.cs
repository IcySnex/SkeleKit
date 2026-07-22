using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SkeleKit.HotReload.Sdb;

// A Mono soft-debugger client speaking the wire protocol to the app's debugger agent. It handles the
// Microsoft.iOS "start debugger: sdb" preamble, the DWP-Handshake, then command/reply framing. See
// Docs/hot-reload-debugging.md for the protocol notes.
sealed class SdbConnection : IDisposable
{
	const byte CmdSetVm = 1;
	const byte CmdSetAppDomain = 20;
	const byte CmdSetAssembly = 21;
	const byte CmdSetModule = 24;

	static readonly byte[] Handshake = "DWP-Handshake"u8.ToArray();

	readonly Socket socket;
	readonly ConcurrentDictionary<int, TaskCompletionSource<(int Error, byte[] Data)>> pending = [];

	int nextId = 1;

	SdbConnection(
		Socket socket)
	{
		this.socket = socket;
	}

	// Accepts the app's debug connection, sends the length-prefixed "start debugger: sdb" command, and
	// completes the handshake so the socket is left speaking raw sdb.
	public static SdbConnection AcceptApp(
		int port)
	{
		Socket listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		listener.Bind(new IPEndPoint(IPAddress.Loopback, port));
		listener.Listen(1);

		Socket app = listener.Accept();
		listener.Dispose();

		byte[] command = "start debugger: sdb"u8.ToArray();
		app.Send([(byte)command.Length, .. command]);

		ReadExactly(app, Handshake.Length);
		app.Send(Handshake);

		SdbConnection connection = new(app);
		connection.StartReader();

		return connection;
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
				byte[] header = ReadExactly(socket, 11);
				int length = BinaryPrimitives.ReadInt32BigEndian(header);
				int id = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(4));
				byte flags = header[8];

				byte[] payload = length > 11 ? ReadExactly(socket, length - 11) : [];

				// a reply carries the reply flag; anything else is an agent command (an event) we ignore
				if ((flags & 0x80) != 0)
				{
					int error = BinaryPrimitives.ReadInt16BigEndian(header.AsSpan(9));
					if (pending.TryRemove(id, out TaskCompletionSource<(int, byte[])>? waiter))
						waiter.TrySetResult((error, payload));
				}
			}
		}
		catch
		{
			foreach (TaskCompletionSource<(int, byte[])> waiter in pending.Values)
				waiter.TrySetException(new IOException("sdb connection closed"));
		}
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

		lock (socket)
			socket.Send(packet);

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

	public int[] Assemblies(
		int domain)
	{
		(_, byte[] data) = Command(CmdSetAppDomain, 3, Int(domain));
		int offset = 0;
		int count = ReadInt(data, ref offset);

		int[] ids = new int[count];
		for (int index = 0; index < count; index++)
			ids[index] = ReadInt(data, ref offset);

		return ids;
	}

	public string AssemblyName(
		int assembly)
	{
		(_, byte[] data) = Command(CmdSetAssembly, 6, Int(assembly));
		int offset = 0;

		return ReadString(data, ref offset);
	}

	public int ManifestModule(
		int assembly)
	{
		(_, byte[] data) = Command(CmdSetAssembly, 3, Int(assembly));
		int offset = 0;

		return ReadInt(data, ref offset);
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

	public void Dispose() => socket.Dispose();
}
