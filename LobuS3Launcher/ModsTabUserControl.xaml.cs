using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace LobuS3Launcher.Tabs
{
    /// <summary>
    /// Interaction logic for ModsTabUserControl.xaml
    /// </summary>
    public partial class ModsTabUserControl : UserControl
    {
        public ModsTabUserControl()
        {
            InitializeComponent();

			Loaded += ModsTabUserControl_Loaded;
        }

		private void ModsTabUserControl_Loaded(object sender, RoutedEventArgs e)
		{
			UpdateCheckBoxes();
		}

		private void UpdateCheckBoxes()
		{
			List<CheckBox> checkBoxes = new List<CheckBox>();

			DirectoryInfo launcherModsFolder = new DirectoryInfo(DocumentFolders.Launcher.ModPath);
			DirectoryInfo gameModsFolder = new DirectoryInfo(DocumentFolders.Game.ModPath);

			// Get the mods from the games folder.
			foreach (FileInfo file in gameModsFolder.GetFiles("*.package"))
			{
				// Create a new CheckBox for the mod.
				checkBoxes.Add(new CheckBox
				{
					IsChecked = true,
					Content = Path.GetFileNameWithoutExtension(file.Name),
				});
			}

			// Get additional mods from the launcher folder.
			foreach (FileInfo file in launcherModsFolder.GetFiles("*.package"))
			{
				bool isDuplicate = false;

				for (int i = 0; i < checkBoxes.Count; i++)
				{
					CheckBox checkBox = checkBoxes[i];

					if ((string)checkBox.Content == Path.GetFileNameWithoutExtension(file.Name))
					{
						isDuplicate = true;
						break;
					}
				}

				if (isDuplicate)
					continue;

				checkBoxes.Add(new CheckBox
				{
					IsChecked = false,
					Content = Path.GetFileNameWithoutExtension(file.Name),
				});
			}

			// Sort the list by name.
			checkBoxes = checkBoxes.OrderBy(x => x.Content).ToList();

			// Add CheckBox events.
			foreach (CheckBox checkBox in checkBoxes)
			{
				checkBox.Checked += CheckBox_Checked;
				checkBox.Unchecked += CheckBox_Unchecked;
			}

			// Add the new CheckBoxes to the window.
			modsContainer.Children.Clear();
			foreach (CheckBox checkBox in checkBoxes)
				modsContainer.Children.Add(checkBox);
		}

		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			// Get the mod name.
			CheckBox checkBox = (CheckBox)sender;
			string name = (string)checkBox.Content + ".package";

			// Get mods folders for both game and launcher.
			string gameFolder = DocumentFolders.Game.ModPath;
			string launcherFolder = DocumentFolders.Launcher.ModPath;

			// Move the mod to the launcher mods folder.
			MoveFile(gameFolder, launcherFolder, name);

			// Update the tab.
			UpdateCheckBoxes();
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			// Get the mod name.
			CheckBox checkBox = (CheckBox)sender;
			string name = (string)checkBox.Content + ".package";

			// Get mods folders for both game and launcher.
			string gameFolder = DocumentFolders.Game.ModPath;
			string launcherFolder = DocumentFolders.Launcher.ModPath;

			// Move the mod to the game mods folder.
			MoveFile(launcherFolder, gameFolder, name);

			// Update the tab.
			UpdateCheckBoxes();
		}

		private static void MoveFile(string from, string to, string name)
		{
			string fromPath = Path.Combine(from, name);
			string toPath = Path.Combine(to, name);

			try
			{
				File.Move(fromPath, toPath, true);
			}
			catch { }
		}
	}
}
