using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Autofac;
using Debugger.Common.EnC;
using JetBrains.Lifetimes;
using JetBrains.Util;
using Mono.Debugging.Autofac;
using Mono.Debugging.Soft;
using Mono.Debugging.Soft.Connections.StartArgs;

namespace SkeleKit.Rider.DebuggerWorker;

// Rider already knows how to update its symbol reader and rebind breakpoints when Mono reports an
// EnC update. That code expects the matching EnCDelta to be staged in the debugger worker first.
// The SkeleKit backend runs in a different process, so this small loopback receiver is the missing
// handoff between our compiler and Rider's existing EnC machinery.
[DebuggerSessionComponent(typeof(SoftDebuggerType))]
public sealed class DeltaReceiver : IStartable
{
	const int Magic = 0x534B454C; // SKEL
	const int ProtocolVersion = 1;
	const int MaxBlobBytes = 64 * 1024 * 1024;
	const int MaxItems = 100_000;

	readonly Lifetime lifetime;
	readonly SoftDebuggerSession session;
	readonly ILogger logger;

	TcpListener? listener;
	string? discoveryFile;
	string? secret;

	public DeltaReceiver(
		Lifetime lifetime,
		SoftDebuggerSession session,
		ILogger logger)
	{
		this.lifetime = lifetime;
		this.session = session;
		this.logger = logger;
	}

	public void Start()
	{
		lifetime.OnTermination(Stop);

		Thread thread = new(WaitForDebuggerPort)
		{
			IsBackground = true,
			Name = "skele-enc-bootstrap"
		};
		thread.Start();
	}

	void WaitForDebuggerPort()
	{
		while (lifetime.IsAlive)
		{
			int? debuggerPort = TryGetDebuggerPort();
			if (debuggerPort.HasValue)
			{
				Publish(debuggerPort.Value);
				return;
			}

			Thread.Sleep(10);
		}
	}

	void Publish(
		int debuggerPort)
	{
		try
		{
			listener = new(IPAddress.Loopback, 0);
			listener.Start();

			secret = Guid.NewGuid().ToString("N");
			discoveryFile = DiscoveryFile(debuggerPort);
			Directory.CreateDirectory(Path.GetDirectoryName(discoveryFile));
			File.WriteAllText(
				discoveryFile,
				$"{ProtocolVersion}:{((IPEndPoint)listener.LocalEndpoint).Port}:{secret}");

			Thread thread = new(Accept)
			{
				IsBackground = true,
				Name = "skele-enc-receiver"
			};
			thread.Start();

			logger.Info($"SkeleKit EnC receiver ready for Mono debugger port {debuggerPort}.");
		}
		catch (Exception exception)
		{
			logger.Error(exception, "SkeleKit could not start its EnC receiver.");
			Stop();
		}
	}

	int? TryGetDebuggerPort()
	{
		if (session.MonoListeningPortNumber is { } listeningPort)
			return listeningPort.Port;

		// Rider's Xamarin iOS WiFi start args own the listening socket but do not populate
		// SoftDebuggerSession.MonoListeningPortNumber. Find the socket without depending on
		// Rider's private field names, which have changed between releases.
		SoftDebuggerStartArgs? startArgs = FindFieldValue<SoftDebuggerStartArgs>(session);
		Socket? socket = startArgs is null ? null : FindFieldValue<Socket>(startArgs);
		return socket?.LocalEndPoint is IPEndPoint endpoint ? endpoint.Port : null;
	}

	static T? FindFieldValue<T>(
		object instance)
		where T : class
	{
		for (Type? type = instance.GetType(); type is not null; type = type.BaseType)
		{
			foreach (FieldInfo field in type.GetFields(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
			{
				if (typeof(T).IsAssignableFrom(field.FieldType)
					&& field.GetValue(instance) is T value)
					return value;
			}
		}

		return null;
	}

	void Accept()
	{
		try
		{
			while (lifetime.IsAlive)
			{
				TcpClient client = listener!.AcceptTcpClient();
				ThreadPool.QueueUserWorkItem(_ => Receive(client));
			}
		}
		catch (SocketException) when (!lifetime.IsAlive)
		{
		}
		catch (ObjectDisposedException)
		{
		}
		catch (Exception exception)
		{
			logger.Error(exception, "SkeleKit EnC receiver stopped unexpectedly.");
		}
	}

	void Receive(
		TcpClient client)
	{
		using (client)
		{
			try
			{
				client.ReceiveTimeout = 3000;
				client.SendTimeout = 3000;

				using NetworkStream stream = client.GetStream();
				using BinaryReader reader = new(stream);
				using BinaryWriter writer = new(stream);

				if (reader.ReadInt32() != Magic
					|| reader.ReadInt32() != ProtocolVersion
					|| reader.ReadString() != secret)
					throw new InvalidDataException("invalid SkeleKit EnC handshake");

				string projectName = reader.ReadString();
				Guid mvid = new(reader.ReadBytes(16));
				byte[] il = ReadBlob(reader);
				byte[] metadata = ReadBlob(reader);
				byte[] pdb = ReadBlob(reader);
				int[] updatedMethods = ReadInts(reader);
				int[] updatedTypes = ReadInts(reader);
				LineEdit[] lineEdits = ReadLineEdits(reader);

				EnCDelta delta = new(
					projectName,
					mvid,
					il,
					metadata,
					pdb,
					lineEdits,
					updatedMethods,
					updatedTypes,
					Array.Empty<ActiveStatementDelta>(),
					Array.Empty<ManagedExceptionRegionUpdate>());

				// A failed runtime apply can leave the previous delta pending. Replace it so the next
				// edit cannot be paired with stale symbols.
				session.EnCDeltaStorage.TryRemovePending(mvid, out _);
				bool staged = session.EnCDeltaStorage.TryAddPending(delta);

				writer.Write(staged);
				writer.Flush();
			}
			catch (Exception exception)
			{
				logger.Error(exception, "SkeleKit could not stage an EnC delta.");
			}
		}
	}

	static byte[] ReadBlob(
		BinaryReader reader)
	{
		int length = reader.ReadInt32();
		if (length < 0 || length > MaxBlobBytes)
			throw new InvalidDataException($"invalid SkeleKit EnC blob length {length}");

		byte[] bytes = reader.ReadBytes(length);
		if (bytes.Length != length)
			throw new EndOfStreamException();

		return bytes;
	}

	static int[] ReadInts(
		BinaryReader reader)
	{
		int count = ReadCount(reader);
		int[] values = new int[count];
		for (int index = 0; index < values.Length; index++)
			values[index] = reader.ReadInt32();

		return values;
	}

	static LineEdit[] ReadLineEdits(
		BinaryReader reader)
	{
		int count = ReadCount(reader);
		LineEdit[] edits = new LineEdit[count];

		for (int editIndex = 0; editIndex < count; editIndex++)
		{
			string path = reader.ReadString();
			int changeCount = ReadCount(reader);
			LineChange[] changes = new LineChange[changeCount];
			for (int changeIndex = 0; changeIndex < changeCount; changeIndex++)
				changes[changeIndex] = new(reader.ReadInt32(), reader.ReadInt32());

			edits[editIndex] = new(path, changes);
		}

		return edits;
	}

	static int ReadCount(
		BinaryReader reader)
	{
		int count = reader.ReadInt32();
		if (count < 0 || count > MaxItems)
			throw new InvalidDataException($"invalid SkeleKit EnC item count {count}");

		return count;
	}

	void Stop()
	{
		try { listener?.Stop(); }
		catch { }

		try
		{
			if (discoveryFile is not null
				&& secret is not null
				&& File.Exists(discoveryFile)
				&& File.ReadAllText(discoveryFile).EndsWith($":{secret}", StringComparison.Ordinal))
				File.Delete(discoveryFile);
		}
		catch { }
	}

	static string DiscoveryFile(
		int debuggerPort) =>
		Path.Combine(Path.GetTempPath(), "skelekit-rider", $"enc-{debuggerPort}.endpoint");
}
