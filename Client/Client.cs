using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FileClient2
{
	class Client
	{
		TcpClient tcpClient;

		MainWindow window;

		bool listening;

		public Client(MainWindow window)
		{
			this.window = window;

			listening = true;

			tcpClient = new TcpClient();
		}

		public void Connect(IPAddress ip, int port)
		{			
			if (!tcpClient.Connected)
			{
				tcpClient.Connect(ip.ToString(), port);
			}
		}

		private void Listen()
		{
			while (listening)
			{
				byte[] data = new byte[1024];
				tcpClient.GetStream().Read(data, 0, 1024);
				string message = Encoding.ASCII.GetString(data);
				message = message.Replace("\0", String.Empty);
				window.Write(message);
			}
		}

		public void SendMessage(string message)
		{
			byte[] data = new byte[2048];
			data = Encoding.ASCII.GetBytes(message);
			tcpClient.GetStream().Write(data, 0, data.Length);
		}
	}
}
