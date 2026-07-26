using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection.Metadata;

namespace SkeleKit;

internal static class HotReload
{
	const int Port = 9988;


	// ReSharper disable once FunctionNeverReturns
	static void Listen()
	{
		while (true)
		{
			bool connected = false;

			try
			{
				using TcpClient client = new();
				client.Connect("127.0.0.1", Port);
				connected = true;

				Debug.WriteLine("[SkeleKit] Hot reload connected.");

				using NetworkStream stream = client.GetStream();
				while (stream.ReadByte() >= 0)
					ForceReload();
			}
			catch
			{
				// Rider may not be up yet, or may disappear with the debug session
			}

			if (connected)
				Debug.WriteLine("[SkeleKit] Hot reload disconnected.");

			Thread.Sleep(1000);
		}
	}

	internal static void ForceReload() =>
		UIApplication.SharedApplication.InvokeOnMainThread(PageHost.ReloadLive);

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
			Name = "skele-hot-reload"
		};

		Debug.WriteLine("[SkeleKit] Hot reload started.");
		thread.Start();
	}
}
