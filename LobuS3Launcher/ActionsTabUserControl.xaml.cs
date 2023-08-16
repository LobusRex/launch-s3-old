using Common;
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

namespace LobuS3Launcher.Tabs
{
	/// <summary>
	/// Interaction logic for ActionsTabUserControl.xaml
	/// </summary>
	public partial class ActionsTabUserControl : UserControl
	{
		public ActionsTabUserControl()
		{
			InitializeComponent();
		}

		private void EnableEPButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				EPSelectionManager.SetSelectionEnabled(true, true, true);
			}
			catch (RegistryKeyNotFoundException)
			{
				ErrorBox.Show("Unable to get the game location from the Windows Registry.");
			}
		}

		private void DisableEPButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				EPSelectionManager.SetSelectionEnabled(false, true, true);
			}
			catch (RegistryKeyNotFoundException)
			{
				ErrorBox.Show("Unable to get the game location from the Windows Registry.");
			}
		}
	}
}
