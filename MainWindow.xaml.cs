using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using Discord;
using Memory;

namespace AmongUsBot
{
	public partial class MainWindow : Window
	{
		public bool inMeeting = false;
		public Thread Thread = null;

		public Mem MemLib = new Mem();
		private DiscordBot bot;

		public MainWindow()
		{
			InitializeComponent();

			var isOpen = MemLib.OpenProcess("Among Us.exe");

			if (isOpen)
			{
				var posX = MemLib.ReadFloat("UnityPlayer.dll+01277F00,0x20,0x2C,0x58,0x0,0x4,0x5C,0x2C");

				Debug.WriteLine(posX);


				Thread = new Thread(new ThreadStart(ThreadProc));
				Thread.Start();

				bot = new DiscordBot();
			}
			else
			{
				Debug.WriteLine("Could not Open Among us");
			}
		}

		private void ThreadProc()
		{

			var inEmergencyMeeting = MemLib.ReadByte("UnityPlayer.dll+012A7A14,0x64,0x34,0x8,0xC,0x3C,0x18");
			if (inEmergencyMeeting == 1 && !inMeeting)
			{
				inMeeting = true;
				bot.UnMute();
			}
			else if(inEmergencyMeeting != 1 && inMeeting)
			{
				inMeeting = false;
				bot.Mute();
			}
			Thread.Sleep(100);

			Debug.WriteLine(inMeeting);

			if (MemLib.theProc.HasExited)
			{
				bot.UnMute();
				return;
			}

			Thread = new Thread(new ThreadStart(ThreadProc));
			Thread.IsBackground = true;
			Thread.Start();
		}

		private void OnApplyTokenButtonClicked(object sender, RoutedEventArgs e)
		{
			bot.Start(TokenBox.Text);
		}
	}


}
