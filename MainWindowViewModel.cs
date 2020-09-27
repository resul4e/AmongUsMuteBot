using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AmongUsBot.Properties;

namespace AmongUsBot
{
	public class MainWindowViewModel : INotifyPropertyChanged, IDisposable
	{
		public IReadOnlyCollection<string> ProcessIDs => m_processFinder.ProcessIDs;

		public bool IsInMeeting
		{
			get => m_isInMeeting;
			set
			{
				if (m_isInMeeting != value)
				{
					m_isInMeeting = value;
					OnPropertyChanged();
				}
			}
		}
		private bool m_isInMeeting = false;

		public Vector Player1Pos
		{
			get => m_player1Pos;
			set
			{
				if (m_player1Pos != value)
				{
					m_player1Pos = value;
					OnPropertyChanged();
				}
			}
		}
		private Vector m_player1Pos;

		public MainWindowViewModel()
		{
			m_processFinder = ((App) App.Current).ProcessFinder;
			m_processFinder.PropertyChanged += OnProcessFinderPropertyChanged;
			m_processFinder.FindAllRunningProcesses();
		}

		public void Dispose()
		{
			m_disposing = true;
			m_pollingThread.Join();

		}

		public void StartScrapingAmongUs(string _pid)
		{
			m_scraper = new AmongUsScraper(_pid);

			m_pollingThread = new Thread(PollData);
			m_pollingThread.IsBackground = true;
			m_pollingThread.Start();
		}

		private void OnProcessFinderPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ProcessFinder.ProcessIDs))
			{
				OnPropertyChanged(nameof(ProcessIDs));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void PollData()
		{
			while (!m_disposing)
			{
				IsInMeeting = m_scraper.GetIsInMeeting();
				Player1Pos = m_scraper.GetPlayer1Position();
			}
		}

		private bool m_disposing = false;
		private int m_pollingRate = 30;
		private Thread m_pollingThread;
		private ProcessFinder m_processFinder;
		private AmongUsScraper m_scraper;


	}
}
