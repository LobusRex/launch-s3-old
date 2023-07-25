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

namespace LobuS3Launcher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			Loaded += MainWindow_Loaded;
			tabControl.SelectionChanged += TabControl_SelectionChanged;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			FixExtraSpaceWidth();
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.Source is TabControl)
			{
				FixExtraSpaceWidth();
			}
		}

		private void FixExtraSpaceWidth()
		{
			extraSpace.Width = MathF.Min((float)tabItem1.ActualWidth, (float)tabItem2.ActualWidth);
		}

		private void LaunchButton_Click(object sender, RoutedEventArgs e)
		{
			Launcher.SingleCoreLaunch();
		}
    }
}
