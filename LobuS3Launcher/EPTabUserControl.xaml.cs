using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace LobuS3Launcher.Tabs
{
	/// <summary>
	/// Interaction logic for EPTabUserControl.xaml
	/// </summary>
	public partial class EPTabUserControl : UserControl
	{
		public EPTabUserControl()
		{
			InitializeComponent();
		}

		private void EnableEPButton_Click(object sender, RoutedEventArgs e)
		{
			SetEPSelectionEnabled(true, true, true);
		}

		private void DisableEPButton_Click(object sender, RoutedEventArgs e)
		{
			SetEPSelectionEnabled(false, true, true);
		}

		private static void SetEPSelectionEnabled(bool enable, bool elevated, bool hideWindow)
		{
			Process process = new Process();
			process.StartInfo.FileName = "ExpansionEnabler.exe";

			process.StartInfo.ArgumentList.Add(enable ? "enable" : "disable");
			process.StartInfo.ArgumentList.Add(Launcher.gamePath);
			process.StartInfo.ArgumentList.Add(@"SOFTWARE\WOW6432Node");

			// Run as administrator.
			if (elevated)
			{
				process.StartInfo.Verb = "runas";
				process.StartInfo.UseShellExecute = true;
			}

			// Hide the console window.
			if (hideWindow)
			{
				process.StartInfo.ArgumentList.Add("quiet");
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			}

			try
			{
				process.Start();
			}
			catch { }
		}
	}
}
