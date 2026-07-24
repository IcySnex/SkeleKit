using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata;

[assembly: MetadataUpdateHandler(typeof(SkeleKit.HotReload))]

namespace SkeleKit;

internal static class HotReload
{
	const int Port = 9988;


	// ReSharper disable once FunctionNeverReturns
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

		if (module == Guid.Empty)
		{
			ForceReload();
			return;
		}

		if (Debugger.IsAttached)
		{
			Debug.WriteLine("[SkeleKit] Hot reload is off while the debugger is attached.");
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

				Debug.WriteLine("[SkeleKit] Hot reloaded: {0}.", target.GetName().Name);
			}
			catch (Exception exception) when (exception.Message.Contains("debugger", StringComparison.OrdinalIgnoreCase))
			{
				Debug.WriteLine("[SkeleKit] Hot reload is off while the debugger is attached.");
			}
			catch (Exception exception)
			{
				Debug.WriteLine($"[SkeleKit] Hot reload failed: {exception.Message}");
			}
		});
	}

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


	internal static void UpdateApplication(
		Type[]? updatedTypes) =>
		ForceReload();

	internal static void ForceReload() =>
		UIApplication.SharedApplication.InvokeOnMainThread(() =>
		{
			PageHost.ReloadLive();
			Debug.WriteLine("[SkeleKit] Hot reloaded: live UI rebuilt.");
		});

	internal static void Start()
	{
		if (!MetadataUpdater.IsSupported)
		{
			Debug.WriteLine("[SkeleKit] Hot reload could not be started: Metadata updater is not supported.");
			return;
		}

		Thread thread = new(Listen)
		{
			IsBackground = true,
			Name = "skele-hotreload"
		};
		thread.Start();

		Debug.WriteLine("[SkeleKit] Hot reload started.");
	}
}
