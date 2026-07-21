using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata;

namespace SkeleKit;

// Dev-only C# hot reload. Receives .NET metadata/IL deltas from the SkeleKit.HotReload host tool over
// localhost TCP (the sim reaches the host on 127.0.0.1), applies them with MetadataUpdater.ApplyUpdate,
// then rebuilds every live page through PageHost so the change shows without a reinstall. Mono does not
// invoke [MetadataUpdateHandler]s itself, so the rebuild is driven straight from here. Inert unless
// MetadataUpdater.IsSupported, which is only true on an -p:EnableHotReload=true build, so the whole
// thing trims away in Release: zero cost when not developing.
internal static class HotReload
{
	const int Port = 9988;


	internal static void Start()
	{
		if (MetadataUpdater.IsSupported is false)
			return;

		Thread thread = new(Listen)
		{
			IsBackground = true,
			Name = "skele-hotreload"
		};
		thread.Start();
	}

	static void Listen()
	{
		while (true)
		{
			try
			{
				using TcpClient client = new();
				client.Connect("127.0.0.1", Port);

				using NetworkStream stream = client.GetStream();
				while (true)
					Receive(stream);
			}
			catch
			{
				Thread.Sleep(1000);
			}
		}
	}

	static void Receive(
		NetworkStream stream)
	{
		Guid module = new(ReadExactly(stream, 16));
		int metadataLength = BitConverter.ToInt32(ReadExactly(stream, 4));
		int ilLength = BitConverter.ToInt32(ReadExactly(stream, 4));
		int pdbLength = BitConverter.ToInt32(ReadExactly(stream, 4));

		byte[] metadata = ReadExactly(stream, metadataLength);
		byte[] il = ReadExactly(stream, ilLength);
		byte[] pdb = ReadExactly(stream, pdbLength);

		// the runtime forbids ApplyUpdate under a debugger (it owns EnC), so a Rider "Debug" launch
		// blocks hot reload while "Run" allows it — say so once instead of throwing on every edit
		if (System.Diagnostics.Debugger.IsAttached)
		{
			if (!warnedDebugger)
			{
				warnedDebugger = true;
				Console.WriteLine("[SkeleKit] hot reload is off while the debugger is attached — launch with Run (not Debug) to hot reload.");
			}

			return;
		}

		Assembly? target = AppDomain.CurrentDomain
			.GetAssemblies()
			.FirstOrDefault(assembly => !assembly.IsDynamic && assembly.ManifestModule.ModuleVersionId == module);
		if (target is null)
			return;

		UIApplication.SharedApplication.InvokeOnMainThread(() =>
		{
			try
			{
				MetadataUpdater.ApplyUpdate(target, metadata, il, pdb);
				PageHost.ReloadLive();
			}
			catch (Exception exception) when (exception.Message.Contains("debugger", StringComparison.OrdinalIgnoreCase))
			{
				if (!warnedDebugger)
				{
					warnedDebugger = true;
					Console.WriteLine("[SkeleKit] hot reload is off while the debugger is attached — launch with Run (not Debug) to hot reload.");
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine($"[SkeleKit] hot reload failed: {exception.Message}");
			}
		});
	}

	static bool warnedDebugger;

	static byte[] ReadExactly(
		NetworkStream stream,
		int count)
	{
		byte[] buffer = new byte[count];

		int read = 0;
		while (read < count)
		{
			int chunk = stream.Read(buffer, read, count - read);
			if (chunk == 0)
				throw new EndOfStreamException();

			read += chunk;
		}

		return buffer;
	}
}
