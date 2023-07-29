using Common;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace ExpansionEnabler
{
	internal class Program
	{
		private static readonly string oldGame = "TS3W.exe";
		private static readonly string newGame = "TS3L.exe";
		private static readonly string oldTarget = @"Software\Sims";
		private static readonly string newTarget = @"Software\SimL";

		private static bool showOutput = true;

		private static void Main(string[] args)
		{
			//foreach (string arg in args)
			//{
			//	Console.WriteLine(arg);
			//}

			string task = args[0];
			string gamePath = args[1];
			string volume = args.Length > 2 ? args[2] : "";

			showOutput = volume.ToLower() != "quiet";

			Log("");
			Log("Old Base: " + MachineRegistry.GetFullKey(MachineRegistry.SimsKey, MachineRegistry.BaseGameKey));
			Log("New Base: " + MachineRegistry.GetFullKey(MachineRegistry.SimLKey, MachineRegistry.BaseGameKey));
			// TODO: Add logs for the game path...
			Log("");

			if (task.ToLower() == "enable")
			{
				// Create the SimL registry key.
				MachineRegistry.CreateOpenKey(MachineRegistry.SimLKey);
				Log("Created " + MachineRegistry.GetFullKey(MachineRegistry.SimLKey));

				// Copy the "The Sims 3" subkey from Sims to SimL.
				MachineRegistry.CopyKey(MachineRegistry.SimsKey, MachineRegistry.SimLKey, MachineRegistry.BaseGameKey);
				Log("Copied to " + MachineRegistry.GetFullKey(MachineRegistry.SimLKey, MachineRegistry.BaseGameKey));

				// Create the new game TS3L.exe.
				CreateTS3L(gamePath);
				Log("Created " + Path.Combine(gamePath, newGame));
			}
			else if (task.ToLower() == "disable")
			{
				// Delete the new game TS3L.exe.
				DeleteTS3L(gamePath);
				Log("Deleted " + Path.Combine(gamePath, newGame));

				// Delete the new registry key SimL.
				MachineRegistry.DeleteKey(MachineRegistry.SimLKey);
				Log("Deleted " + MachineRegistry.GetFullKey(MachineRegistry.SimLKey));
			}

			if (showOutput)
			{
				Console.ReadLine();
			}
		}

		private static void CreateTS3L(string gamePath)
		{
			string newGamePath = Path.Combine(gamePath, newGame);
			string oldGamePath = Path.Combine(gamePath, oldGame);

			// Translate "Software\Sims" and "Software\SimL to bytes"
			byte[] oldTargetBytes = Encoding.UTF8.GetBytes(oldTarget);
			byte[] newTargetBytes = Encoding.UTF8.GetBytes(newTarget);

			// Read TS3W.exe.
			byte[] newGameContent = File.ReadAllBytes(oldGamePath);

			// Replace "Software\Sims" with "Software\SimL"
			newGameContent.ReplaceAll(oldTargetBytes, newTargetBytes);

			// Save as TS3L.exe.
			File.WriteAllBytes(newGamePath, newGameContent);
		}

		private static void DeleteTS3L(string gamePath)
		{
			// Delete TS3L.exe.
			string newGamePath = Path.Combine(gamePath, newGame);
			File.Delete(newGamePath);
		}

		private static void Log(string message)
		{
			if (showOutput)
			{
				Console.WriteLine(message);
			}
		}
	}
}
