using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Runtime.CompilerServices;
using AmongUsBot.Properties;
using Memory;

namespace AmongUsBot
{
	public class ProcessFinder : INotifyPropertyChanged
	{
		public IReadOnlyCollection<string> ProcessIDs
		{
			get { return m_processIDs.AsReadOnly(); }
		}


		public ProcessFinder()
		{
			EventQuery query = new EventQuery();
			query.QueryString = "SELECT TargetInstance" +
			                    "  FROM __InstanceCreationEvent " +
			                    "WITHIN  .025 " +
			                    " WHERE TargetInstance ISA 'Win32_Process' "
			                    + "   AND TargetInstance.Name like '%'";
			ManagementEventWatcher mgmtWatcher = new ManagementEventWatcher(query);
			mgmtWatcher.EventArrived += OnMgmtWatcherEventArrived;
			mgmtWatcher.Start();
		}

		public void FindAllRunningProcesses()
		{
			Process[] allProcesses = Process.GetProcesses();
			string processName = s_amongUsName.Remove(s_amongUsName.Count() - 4);
			foreach (var process in allProcesses)
			{
				if (string.Compare(process.ProcessName, processName, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					m_processIDs.Add(process.Id.ToString());
				}
			}
			OnPropertyChanged(nameof(ProcessIDs));
		}

		private void OnMgmtWatcherEventArrived(object sender, EventArrivedEventArgs e)
		{
			ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
			string processName = targetInstance.Properties["Name"].Value.ToString();
			if (processName == s_amongUsName)
			{
				m_processIDs.Add(targetInstance.Properties["ProcessId"].Value.ToString());
				OnPropertyChanged(nameof(ProcessIDs));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private const string s_amongUsName = "Among Us.exe";
		private List<string> m_processIDs = new List<string>();
		
		

	}
}
