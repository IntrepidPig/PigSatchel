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

namespace FileServer2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Server server;

		internal string outputBoxText
		{
			get { return outputBox.Text; }
			set { Dispatcher.Invoke(new Action(() => { outputBox.AppendText("\n" + value); })); }
		}

		public MainWindow()
		{
			InitializeComponent();

			server = new Server(new IPAddress(new byte[] { 192, 168, 0, 2 }), 9999, this);
		}

		private void ExecuteCommand(object sender, RoutedEventArgs e)
		{
			outputBox.AppendText("> " + commandBox.Text);
		}

		public void Write(string text)
		{
			outputBoxText = text;
		}

		private void OutputChanged(object sender, TextChangedEventArgs e)
		{
			outputBox.ScrollToEnd();
		}

		private void ShutdownServer(object sender, EventArgs e)
		{
			server.Shutdown();
		}
	}
}
