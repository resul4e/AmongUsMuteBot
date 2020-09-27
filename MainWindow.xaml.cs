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

		public MainWindowViewModel ViewModel => (MainWindowViewModel) DataContext;

		public MainWindow()
		{
			InitializeComponent();
			DataContext = new MainWindowViewModel();

			var isOpen = MemLib.OpenProcess("Among Us.exe");

			if (isOpen)
			{
				Thread = new Thread(new ThreadStart(ThreadProc));
				Thread.IsBackground = true;
				Thread.Start();

				m_bot = new DiscordBot();
			}
			else
			{
				Debug.WriteLine("Could not Open Among us");
			}
		}

		private void ThreadProc()
		{
			while (true)
			{
				var inEmergencyMeeting = MemLib.ReadByte("UnityPlayer.dll+012A7A14,0x64,0x34,0x8,0xC,0x3C,0x18");
				if (inEmergencyMeeting == 1 && !inMeeting)
				{
					inMeeting = true;
					m_bot.UnMute();
				}
				else if (inEmergencyMeeting != 1 && inMeeting)
				{
					inMeeting = false;
					m_bot.Mute();
				}
				Thread.Sleep(100);

				Debug.WriteLine(inMeeting);

				if (MemLib.theProc.HasExited)
				{
					m_bot.UnMute();
					return;
				}
			}
		}

		private void OnApplyTokenButtonClicked(object sender, RoutedEventArgs e)
		{
			m_bot.Start(TokenBox.Password);
		}

		private void OnTargetSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string selectedPID = (string)e.AddedItems[0];
			ViewModel.StartScrapingAmongUs(selectedPID);
		}

		
		private DiscordBot m_bot;
	}


}
