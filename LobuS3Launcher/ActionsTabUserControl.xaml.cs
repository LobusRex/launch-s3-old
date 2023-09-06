using Common;
using System;
using System.Collections.Generic;
using System.IO;
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

			Loaded += ActionsTabUserControl_Loaded;
		}

		private void ActionsTabUserControl_Loaded(object sender, RoutedEventArgs e)
		{
			activeSavesButton.IsEnabled = Directory.Exists(DocumentFolders.Game.Path);
			backupSavesButton.IsEnabled = Directory.Exists(DocumentFolders.Launcher.Path);
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

		private void BackupButton_Click(object sender, RoutedEventArgs e)
		{
			// Ask the user if they are sure.
			string message = "Do you want to make a backup of all saves? Make sure that the disk has enough space to fit the backup.";
			string caption = "Create backup";
			MessageBoxImage icon = MessageBoxImage.Question;
			MessageBoxButton button = MessageBoxButton.YesNoCancel;
			bool doBackup = MessageBox.Show(message, caption, button, icon).Equals(MessageBoxResult.Yes);

			if (!doBackup)
				return;

			// Make the backup.
			DocumentFolders.BackupSaves();
		}

		private void BackupSavesButton_Click(object sender, RoutedEventArgs e)
		{
			Folder.OpenWithExplorer(DocumentFolders.Launcher.SavePath);
		}

		private void ActiveSavesButton_Click(object sender, RoutedEventArgs e)
		{
			Folder.OpenWithExplorer(DocumentFolders.Game.SavePath);
		}

		private void EnableModsButton_Click(object sender, RoutedEventArgs e)
		{
			// Ask the user if they are sure.
			string message = "Do you want to set up modding? Doing so requires an internet connection to download FrameworkSetup.zip from modthesims.info.";
			string caption = "Set up modding";
			MessageBoxImage icon = MessageBoxImage.Question;
			MessageBoxButton button = MessageBoxButton.YesNoCancel;
			bool doSetup = MessageBox.Show(message, caption, button, icon).Equals(MessageBoxResult.Yes);

			if (!doSetup)
				return;

			// Download and extract FrameworkSetup.zip.
			Task<ModSelectionManager.DownloadExtractResult> download = ModSelectionManager.EnableSelection();

			// Print error messages if anything fails.
			Task temp = download.ContinueWith(t =>
			{
				// The download failed.
				if (t.Result == ModSelectionManager.DownloadExtractResult.DownloadFailed)
					ErrorBox.Show($"Unable to download FrameworkSetup.zip from {ModSelectionManager.FrameworkSetupUrl}.");

				// The extraction failed.
				if (t.Result == ModSelectionManager.DownloadExtractResult.ExtractionFailed)
					ErrorBox.Show($"Unable to extract FrameworkSetup.zip to {DocumentFolders.Game.Path}.");
			});
		}
	}
}
