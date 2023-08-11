using Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.DirectoryServices;
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
	/// Interaction logic for EPTabUserControl.xaml
	/// </summary>
	public partial class EPTabUserControl : UserControl
	{
		private readonly List<ExpansionPack> expansionPacks;

		public EPTabUserControl()
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

		public static bool GetEPSelectionEnabled()
		{
			// EP Selection is considered enabled if both the "SimL\The Sims 3" registry key and TS3L.exe exist.

			// Check if the SimL registry key exists.
			bool SimLExists = MachineRegistry.KeyExists(MachineRegistry.SimLKey, MachineRegistry.BaseGameKey);

			// Get the path to the base game.
			string baseGamePath;
			try
			{
				baseGamePath = MachineRegistry.GetBaseBinPath();
			}
			catch
			{ return false; }

			// Check if TS3L.exe exists.
			bool TS3LExists = File.Exists(Path.Combine(baseGamePath, Launcher.NewGame));

			return SimLExists && TS3LExists;
		}

		private void UpdateCheckBoxes()
		{
			bool EPSelectionEnabled = GetEPSelectionEnabled();

			foreach (ExpansionPack expansion in expansionPacks)
			{
				// Check if this expansion has sub keys in Sims and SimL.
				bool EPExists = MachineRegistry.KeyExists(MachineRegistry.SimsKey, expansion.GameKey);
				bool EPActive = MachineRegistry.KeyExists(MachineRegistry.SimLKey, expansion.GameKey);

				// Determine if this check box should be enabled and checked.
				bool enable = EPSelectionEnabled && EPExists;
				bool check = EPSelectionEnabled && EPActive;

				// Update the check box.
				Dispatcher.Invoke(() =>
				{
					expansion.CheckBox.IsEnabled = enable;
					expansion.SetCheckBoxChecked(check);
				});
			}
		}

		private void EnableEPButton_Click(object sender, RoutedEventArgs e)
		{
			SetEPSelectionEnabled(true, true, true);
		}

		private void DisableEPButton_Click(object sender, RoutedEventArgs e)
		{
			SetEPSelectionEnabled(false, true, true);
		}

		private void SetEPSelectionEnabled(bool enable, bool elevated, bool hideWindow)
		{
			// Get the path to the base game installation.
			string baseGamePath;
			try
			{
				baseGamePath = MachineRegistry.GetBaseBinPath();
			}
			catch (RegistryKeyNotFoundException)
			{
				ErrorBox.Show("Unable to get the game location from the Windows Registry.");
				return;
			}

			// Create the expansion enabler process.
			Process process = new Process();
			process.StartInfo.FileName = "ExpansionEnabler.exe";

			process.StartInfo.ArgumentList.Add(enable ? "enable" : "disable");
			process.StartInfo.ArgumentList.Add(baseGamePath);

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

			// Start the process.
			try
			{
				process.Start();
			}
			catch { }

			// Add an event to occur when the process is finished.
			process.EnableRaisingEvents = true;
			process.Exited += Process_Exited;
		}

		private void Process_Exited(object? sender, EventArgs e)
		{
			UpdateCheckBoxes();
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
				// Copy the expansion key to SimL when the box is checked.
				try
				{
					MachineRegistry.CopyKey(MachineRegistry.SimsKey, MachineRegistry.SimLKey, GameKey);
				}
				catch (RegistryKeyNotFoundException)
				{
					ErrorBox.Show("Unable to read the required Windows Registry key to enable this expansion.");
					SetCheckBoxChecked(false);
				}
			}

			private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
			{
				// Remove the expansion key from SimL when the box is unchecked.
				MachineRegistry.DeleteKey(MachineRegistry.SimLKey, GameKey);
			}
		}
	}
}
