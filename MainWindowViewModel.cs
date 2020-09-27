using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Annotations;

namespace AmongUsBot
{
	class MainWindowViewModel : INotifyPropertyChanged
	{
		public IReadOnlyCollection<string> ProcessIDs => m_processFinder.ProcessIDs;


		public MainWindowViewModel()
		{
			m_processFinder = ((App) App.Current).ProcessFinder;
			m_processFinder.PropertyChanged += OnProcessFinderPropertyChanged;
			m_processFinder.FindAllRunningProcesses();
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

		private ProcessFinder m_processFinder;

	}
}
