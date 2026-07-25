using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection.Metadata;

namespace SkeleKit;

internal static class HotReload
{
	const int Port = 9988;


	static void Listen()
	{
		while (true)
		{
			try
			{
				using TcpClient client = new();
				client.Connect("127.0.0.1", Port);

				using NetworkStream stream = client.GetStream();
				while (stream.ReadByte() >= 0)
					ForceReload();
			}
			catch
			{
				Thread.Sleep(1000);
			}
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
			Name = "skele-rider-refresh"
		};
		thread.Start();

		Debug.WriteLine("[SkeleKit] Hot reload started.");
	}
}
