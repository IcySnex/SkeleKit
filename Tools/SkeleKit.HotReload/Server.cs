using System.Net;
using System.Net.Sockets;

namespace SkeleKit.HotReload;

sealed class Server
{
	readonly TcpListener listener;
	TcpClient? client;

	public bool EverConnected { get; private set; }
	public DateTime LastActivity { get; private set; } = DateTime.UtcNow;

	public bool HasClient => client is { Connected: true };

	Server(
		TcpListener listener)
	{
		this.listener = listener;
	}

	public static Server? Bind(
		int port)
	{
		try
		{
			TcpListener listener = new(IPAddress.Loopback, port);
			listener.Start();

			return new(listener);
		}
		catch (SocketException)
		{
			return null;
		}
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
					EverConnected = true;
					LastActivity = DateTime.UtcNow;
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

			LastActivity = DateTime.UtcNow;
			return true;
		}
		catch
		{
			client = null;
			return false;
		}
	}
}
