using System.Net;
using System.Net.Sockets;

namespace SkeleKit.HotReload;

sealed class Server
{
	readonly TcpListener listener;
	TcpClient? client;

	public Server(
		int port)
	{
		listener = new(IPAddress.Loopback, port);
		listener.Start();
	}

	public void Accept()
	{
		Thread thread = new(() =>
		{
			while (true)
			{
				try
				{
					client = listener.AcceptTcpClient();
					Console.WriteLine("app connected");
				}
				catch
				{
					Thread.Sleep(500);
				}
			}
		})
		{
			IsBackground = true
		};

		thread.Start();
	}

	public bool Send(
		Guid module,
		byte[] metadata,
		byte[] il,
		byte[] pdb)
	{
		if (client is not { Connected: true } target)
			return false;

		try
		{
			NetworkStream stream = target.GetStream();
			stream.Write(module.ToByteArray());
			stream.Write(BitConverter.GetBytes(metadata.Length));
			stream.Write(BitConverter.GetBytes(il.Length));
			stream.Write(BitConverter.GetBytes(pdb.Length));
			stream.Write(metadata);
			stream.Write(il);
			stream.Write(pdb);
			stream.Flush();

			return true;
		}
		catch
		{
			client = null;
			return false;
		}
	}
}
