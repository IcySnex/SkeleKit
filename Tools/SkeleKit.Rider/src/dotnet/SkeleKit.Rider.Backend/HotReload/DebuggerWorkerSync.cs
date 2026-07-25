using System.Net;
using System.Net.Sockets;

namespace SkeleKit.Rider.Backend.HotReload;

// Sends a freshly emitted delta to the debugger worker before Mono applies it. The worker then lets
// Rider's built-in EnC event processor update its symbol reader and rebind breakpoints.
sealed class DebuggerWorkerSync
{
	const int Magic = 0x534B454C; // SKEL
	const int ProtocolVersion = 1;

	readonly int debuggerPort;

	public DebuggerWorkerSync(
		int debuggerPort)
	{
		this.debuggerPort = debuggerPort;
	}

	public bool Stage(
		string projectName,
		Guid mvid,
		byte[] il,
		byte[] metadata,
		byte[] pdb,
		int[] updatedMethods,
		int[] updatedTypes,
		IReadOnlyList<LineMapping> lineMappings,
		out string reason)
	{
		string file = DiscoveryFile(debuggerPort);
		reason = "the debugger worker did not publish its EnC endpoint";

		for (int attempt = 0; attempt < 5; attempt++)
		{
			try
			{
				if (!TryReadEndpoint(file, out int port, out string secret))
				{
					Thread.Sleep(20);
					continue;
				}

				using TcpClient client = new();
				client.ReceiveTimeout = 3000;
				client.SendTimeout = 3000;
				client.Connect(IPAddress.Loopback, port);

				using NetworkStream stream = client.GetStream();
				using BinaryWriter writer = new(stream);
				using BinaryReader reader = new(stream);

				writer.Write(Magic);
				writer.Write(ProtocolVersion);
				writer.Write(secret);
				writer.Write(projectName);
				writer.Write(mvid.ToByteArray());
				WriteBlob(writer, il);
				WriteBlob(writer, metadata);
				WriteBlob(writer, pdb);
				WriteInts(writer, updatedMethods);
				WriteInts(writer, updatedTypes);
				WriteLineMappings(writer, lineMappings);
				writer.Flush();

				if (reader.ReadBoolean())
				{
					reason = "";
					return true;
				}

				reason = "the debugger worker rejected the EnC delta";
			}
			catch (Exception exception)
			{
				reason = exception.Message;
			}

			Thread.Sleep(20);
		}

		return false;
	}

	static bool TryReadEndpoint(
		string file,
		out int port,
		out string secret)
	{
		port = 0;
		secret = "";

		if (!File.Exists(file))
			return false;

		string[] parts = File.ReadAllText(file).Trim().Split(':');
		return parts.Length == 3
			&& parts[0] == ProtocolVersion.ToString()
			&& int.TryParse(parts[1], out port)
			&& port is > IPEndPoint.MinPort and <= IPEndPoint.MaxPort
			&& !string.IsNullOrWhiteSpace(secret = parts[2]);
	}

	static void WriteBlob(
		BinaryWriter writer,
		byte[] bytes)
	{
		writer.Write(bytes.Length);
		writer.Write(bytes);
	}

	static void WriteInts(
		BinaryWriter writer,
		int[] values)
	{
		writer.Write(values.Length);
		foreach (int value in values)
			writer.Write(value);
	}

	static void WriteLineMappings(
		BinaryWriter writer,
		IReadOnlyList<LineMapping> mappings)
	{
		writer.Write(mappings.Count);
		foreach (LineMapping mapping in mappings)
		{
			writer.Write(mapping.Path);
			writer.Write(mapping.Changes.Count);
			foreach ((int oldLine, int newLine) in mapping.Changes)
			{
				writer.Write(oldLine);
				writer.Write(newLine);
			}
		}
	}

	static string DiscoveryFile(
		int debuggerPort) =>
		Path.Combine(Path.GetTempPath(), "skelekit-rider", $"enc-{debuggerPort}.endpoint");
}

readonly record struct LineMapping(
	string Path,
	IReadOnlyList<(int OldLine, int NewLine)> Changes);
