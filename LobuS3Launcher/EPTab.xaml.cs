using Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace LobuS3Launcher.Tabs
{
	/// <summary>
	/// Interaction logic for EPTab.xaml
	/// </summary>
	public partial class EPTab : UserControl
	{
		private readonly List<ExpansionPack> expansionPacks;

		public TabItem? TabItemActions { get; set; } = null;

		public EPTab()
		{
			InitializeComponent();

			expansionPacks = new List<ExpansionPack>()
			{
				new ExpansionPack(EP01CheckBox, "The Sims 3 World Adventures"),
				new ExpansionPack(EP02CheckBox, "The Sims 3 Ambitions"),
				new ExpansionPack(EP03CheckBox, "The Sims 3 Late Night"),
				new ExpansionPack(EP04CheckBox, "The Sims 3 Generations"),
				new ExpansionPack(EP05CheckBox, "The Sims 3 Pets"),
				new ExpansionPack(EP06CheckBox, "The Sims 3 Showtime"),
				new ExpansionPack(EP07CheckBox, "The Sims 3 Supernatural"),
				new ExpansionPack(EP08CheckBox, "The Sims 3 Seasons"),
				new ExpansionPack(EP09CheckBox, "The Sims 3 University Life"),
				new ExpansionPack(EP10CheckBox, "The Sims 3 Island Paradise"),
				new ExpansionPack(EP11CheckBox, "The Sims 3 Into The Future"),
			};

			Loaded += EPTab_Loaded;
		}

		private void EPTab_Loaded(object sender, RoutedEventArgs e)
		{
			UpdateCheckBoxes();
		}

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			if (TabItemActions == null)
				return;

			try
			{
				TabItem tabItem = (TabItem)Parent;
				TabControl tabControl = (TabControl)tabItem.Parent;

				Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedItem = TabItemActions));
			}
			catch { return; }
		}

		private void UpdateCheckBoxes()
		{
			bool EPSelectionEnabled = EPSelectionManager.GetSelectionEnabled();

			foreach (ExpansionPack expansion in expansionPacks)
			{
				string gameKey = expansion.GameKey;

				// Determine if this check box should be enabled and checked.
				bool enable = EPSelectionEnabled && EPSelectionManager.GetInstalled(gameKey);
				bool check = EPSelectionEnabled && EPSelectionManager.GetSelected(gameKey);

				// Update the check box.
				Dispatcher.Invoke(() =>
				{
					expansion.CheckBox.IsEnabled = enable;
					expansion.SetCheckBoxChecked(check);
				});
			}
		}

		private class ExpansionPack
		{
			public CheckBox CheckBox { get; private set; }
			public string GameKey { get; private set; }

			public ExpansionPack(CheckBox checkBox, string gameKey)
			{
				CheckBox = checkBox;
				GameKey = gameKey;

				CheckBox.Checked += CheckBox_Checked;
				CheckBox.Unchecked += CheckBox_Unchecked;
			}

			public void SetCheckBoxChecked(bool check)
			{
				// This function makes it possible to change IsChecked without causing an event.

				// Remove checked events.
				CheckBox.Checked -= CheckBox_Checked;
				CheckBox.Unchecked -= CheckBox_Unchecked;

				CheckBox.IsChecked = check;

				// Add checked events back.
				CheckBox.Checked += CheckBox_Checked;
				CheckBox.Unchecked += CheckBox_Unchecked;
			}

			private void CheckBox_Checked(object sender, RoutedEventArgs e)
			{
				try
				{
					EPSelectionManager.Select(GameKey);
				}
				catch (RegistryKeyNotFoundException)
				{
					ErrorBox.Show("Unable to read the required Windows Registry key to enable this expansion.");
					SetCheckBoxChecked(false);
				}
			}

			private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
			{
				EPSelectionManager.Deselect(GameKey);
			}
		}
	}
}
