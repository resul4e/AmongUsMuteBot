using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AmongUsBot
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public ProcessFinder ProcessFinder;
		public string Token;

		App()
		{
			ProcessFinder = new ProcessFinder();
			if (File.Exists("Token"))
			{
				Token = File.ReadAllText("Token");
			}
		}
	}
}
