using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FileServer2
{
	class Server
	{
		IPAddress ip;
		int port;
		TcpListener tcpListener;

		bool running;

		List<Client> clients;
		
		Thread listenerThread;
		Thread checkerThread;

		MainWindow window;

		public Server(IPAddress ip, int port, MainWindow window)
		{
			this.window = window;

			running = true;

			this.ip = ip;
			this.port = port;
			tcpListener = new TcpListener(ip, port);

			clients = new List<Client>();

			listenerThread = new Thread(ListenForConnections);
			listenerThread.Name = "Listener Thread";
			listenerThread.Start();
		}

		public void Shutdown()
		{
			foreach (Client client in clients)
			{
				client.Shutdown();
			}

			running = false;

			listenerThread.Abort();
			checkerThread.Abort();
		}

		// Writes text to the output
		public void Write(string text)
		{
			window.Write(text);
		}

		// Listens for connections from clients
		private void ListenForConnections()
		{
			tcpListener.Start();
			window.Write("Listening for connections to " + ip.ToString() + " on port " + port);

			while (running)
			{
				Client newClient = new Client(tcpListener.AcceptTcpClient(), this);
				clients.Add(newClient);
				Write("Connection from " + newClient.name);
			}			
		}

		private void CheckClients()
		{
			while (running)
			{
				foreach (Client client in clients)
				{
					if (!client.IsConnected)
					{
						client.Shutdown();
					}
				}
			}
		}
	}

	class Client : ISerializable
	{
		IPAddress ip;
		int port;
		TcpClient tcpClient;
		Thread thread;

		Server server;

		public string name;

		bool listening;

		public bool IsConnected
		{
			get
			{
				try
				{
					if (tcpClient != null && tcpClient.Client != null && tcpClient.Client.Connected)
					{

						if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
						{
							byte[] buff = new byte[1];
							if (tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
							{
								return false;
							}
							else
							{
								return true;
							}
						}

						return true;
					}
					else
					{
						return false;
					}
				}
				catch
				{
					return false;
				}
			}
		}

		public Client(TcpClient tcpClient, Server server)
		{
			this.tcpClient = tcpClient;
			IPEndPoint ipep = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
			ip = ipep.Address;
			name = ip.ToString();
			port = ipep.Port;
			this.server = server;
			listening = true;

			thread = new Thread(Listen);
			thread.Name = name + " Thread";
			thread.Start();
		}

		// Listens for messages from the client
		private void Listen()
		{
			while (listening)
			{
				if (tcpClient.Client.Poll(100, SelectMode.SelectRead))
				{
					byte[] data = new byte[1024];
					try
					{
						tcpClient.GetStream().Read(data, 0, data.Length);
					}
					catch
					{
						server.Write("ERROR: Error reading netstream");
					}
					string message = Encoding.ASCII.GetString(data);
					message = message.Replace("\0", String.Empty);
					HandleMessage(message);
				}
			}			
		}

		public void Shutdown()
		{
			tcpClient.GetStream().Close();
			tcpClient.Close();
			thread.Abort();
		}

		void HandleMessage(string unclippedMessage)
		{
			if (unclippedMessage.StartsWith("m"))
			{
				server.Write(name + ": " + unclippedMessage.Substring(1));
			}
			else if (unclippedMessage.StartsWith("^"))
			{
				HandleCommand(unclippedMessage.Substring(1));
			}
		}

		void HandleCommand(string fullCommand)
		{

		}

		// I'm lost here
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			Stream stream = new FileStream(@"C:\Users\virus\Documents\FileServer2\data.dat", FileMode.Create);
			IFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, this);
		}

		// Returns the tcpclient of this client
		public TcpClient GetTcpClient()
		{
			return tcpClient;
		}
	}
}
