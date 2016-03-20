using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;

namespace FileClient2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Client client;

		internal string outputBoxText
		{
			get { return outputBox.Text; }
			set { Dispatcher.Invoke(new Action(() => { outputBox.AppendText("\n" + value); })); }
		}

		public MainWindow()
		{
			InitializeComponent();
			client = new Client(this);
		}

		private void ExecuteCommand(object sender, RoutedEventArgs e)
		{
			string fullCommand = commandBox.Text;

			if (String.IsNullOrEmpty(fullCommand) || String.IsNullOrWhiteSpace(fullCommand))
			{
				commandBox.Text = "";
				return;
			}

			Command command = Command.IntParse(fullCommand);

			Write("> " + fullCommand);

			try {
				switch (command.GetCommand())
				{
					case "connect":
						Write("Connecting to " + command.GetArgs()[0] + " on port " + command.GetArgs()[1]);
						client.Connect(IPAddress.Parse(command.GetArgs()[0].ToString()), int.Parse(command.GetArgs()[1].ToString()));
						break;
					case "m":
						client.SendMessage("m" + Command.ParseOneArgument(fullCommand).GetArgs()[0].ToString());
						break;
				}
			}
			catch (ArgumentOutOfRangeException)
			{
				Write("ERROR: Insufficient arguments");
			}
			catch
			{
				Write("ERROR");
			}
		}

		private string CombineArgs(string[] args)
		{
			string result = "";

			foreach(string arg in args)
			{
				result = result + arg + " ";
			}

			return result.Trim();
		}

		public void Write(string text)
		{
			outputBoxText = text;
		}

		private void OutputChanged(object sender, TextChangedEventArgs e)
		{
			outputBox.ScrollToEnd();
		}
	}
}
