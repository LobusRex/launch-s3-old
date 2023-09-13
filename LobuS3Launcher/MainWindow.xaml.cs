using System.Windows;

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
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			EPTab.TabItemActions = tabItemActions;
			modsTab.TabItemActions = tabItemActions;
		}

		private void LaunchButton_Click(object sender, RoutedEventArgs e)
		{
			Launcher.SingleCoreLaunch();
		}
    }
}
