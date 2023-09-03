using Common;
using ModernWpf.Controls;
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
			// Get all available mods.
			Dictionary<string, bool> mods = ModSelectionManager.GetAvailableAndSelected();

			// Create a CheckBox for each mod.
			List<CheckBox> checkBoxes = new List<CheckBox>();
			foreach (KeyValuePair<string, bool> mod in mods)
			{
				checkBoxes.Add(new CheckBox
				{
					Content = mod.Key,
					IsChecked = mod.Value,
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

			// Add a context menu to all CheckBoxes.
			foreach (CheckBox checkBox in checkBoxes)
			{
				ContextMenu contextMenu = new ContextMenu();
				checkBox.ContextMenu = contextMenu;

				MenuItem menuDelete = new MenuItem
				{
					Header = "Remove",
					Icon = new SymbolIcon(Symbol.Delete),
					CommandParameter = checkBox,
				};
				menuDelete.Click += MenuDelete_Click;
				contextMenu.Items.Add(menuDelete);
			}

			// Add the new CheckBoxes to the window.
			modsContainer.Children.Clear();
			foreach (CheckBox checkBox in checkBoxes)
				modsContainer.Children.Add(checkBox);
		}

		private void MenuDelete_Click(object sender, RoutedEventArgs e)
		{
			if (sender is not MenuItem menuItem)
				return;

			if (menuItem.CommandParameter is not CheckBox checkBox)
				return;

			if (checkBox.Content is not string name)
				return;

			ModSelectionManager.Delete(name);

			// Update the tab.
			UpdateCheckBoxes();
		}

		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			// Get the mod name.
			CheckBox checkBox = (CheckBox)sender;
			string name = (string)checkBox.Content;

			// Deselect the mod.
			ModSelectionManager.Deselect(name);

			// Update the tab.
			UpdateCheckBoxes();
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			// Get the mod name.
			CheckBox checkBox = (CheckBox)sender;
			string name = (string)checkBox.Content;

			// Deselect the mod.
			ModSelectionManager.Select(name);

			// Update the tab.
			UpdateCheckBoxes();
		}

		private void AddModButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.OpenFileDialog();
			dialog.DefaultExt = ".package";
			dialog.Filter = "Package mods |*.package";

			bool? result = dialog.ShowDialog();

			if (result == true)
			{
				string filename = dialog.FileName;

				ModSelectionManager.Add(filename);

				UpdateCheckBoxes();
			}
		}
	}
}
