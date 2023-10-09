using System.Diagnostics;

namespace Common
{
	public static class EPSelectionManager
	{
		public static bool GetInstalled(string gameKey)
		{
			// Check if this expansion has a sub key in Sims.
			return MachineRegistry.KeyExists(MachineRegistry.SimsKey, gameKey);
		}

		public static bool GetSelected(string gameKey)
		{
			// Check if this expansion has a sub key in SimL.
			return MachineRegistry.KeyExists(MachineRegistry.SimLKey, gameKey);
		}

		public static void Select(string gameKey)
		{
			// Copy the expansion key to SimL when the box is checked.
			MachineRegistry.CopyKey(MachineRegistry.SimsKey, MachineRegistry.SimLKey, gameKey);
		}

		public static void Deselect(string gameKey)
		{
			// Remove the expansion key from SimL when the box is unchecked.
			MachineRegistry.DeleteKey(MachineRegistry.SimLKey, gameKey);
		}

		public static bool GetSelectionEnabled()
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
			bool TS3LExists = File.Exists(Path.Combine(baseGamePath, GameDirectory.NewGame));

			return SimLExists && TS3LExists;
		}

		public static Process SetSelectionEnabled(bool enable, bool elevated, bool hideWindow)
		{
			// Get the path to the base game installation.
			string baseGamePath = MachineRegistry.GetBaseBinPath();

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

			// The process is returned to allow the caller to add exited events to it.
			return process;
		}
	}
}
