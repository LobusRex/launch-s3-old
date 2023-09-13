using Common;
using System.Text;

namespace ExpansionEnabler
{
	internal static class Program
	{
		private static string OldTarget { get; } = @"Software\Sims";
		private static string NewTarget { get; } = @"Software\SimL";

		private static bool ShowOutput { get; set; } = true;

		/// <summary>
		/// The entry point of the ExpansionEnabler. Running this program requires
		/// elevated permissions to modify a Windows registry key and to modify the
		/// game installation. When EP selection is enabled, a user can select
		/// Expansion Packs without elevated permissions. However, the user must
		/// start the game through TS3L.exe to use the selected packs.
		/// </summary>
		/// <param name="args"></param>
		private static void Main(string[] args)
		{
			string task = args[0];
			string gamePath = args[1];
			string volume = args.Length > 2 ? args[2] : "";

			ShowOutput = volume.ToLower() != "quiet";

			string simsKey = MachineRegistry.SimsKey;
			string simLKey = MachineRegistry.SimLKey;
			string baseGameKey = MachineRegistry.BaseGameKey;
			string oldGameName = GameDirectory.OldGame;
			string newGameName = GameDirectory.NewGame;

			Log("Old base key:  " + MachineRegistry.GetFullKey(simsKey, baseGameKey));
			Log("New base key:  " + MachineRegistry.GetFullKey(simLKey, baseGameKey));
			Log("Old game path: " + Path.Combine(gamePath, oldGameName));
			Log("New game path: " + Path.Combine(gamePath, newGameName));
			Log("");

			if (task.ToLower() == "enable")
			{
				// Create the SimL registry key.
				MachineRegistry.CreateOpenKey(simLKey);
				Log("Created " + MachineRegistry.GetFullKey(simLKey));

				// Copy the "The Sims 3" subkey from Sims to SimL.
				MachineRegistry.CopyKey(simsKey, simLKey, baseGameKey);
				Log("Copied to " + MachineRegistry.GetFullKey(simLKey, baseGameKey));

				// Create the new game TS3L.exe.
				CreateTS3L(gamePath);
				Log("Created " + Path.Combine(gamePath, newGameName));
			}
			else if (task.ToLower() == "disable")
			{
				// Delete the new game TS3L.exe.
				DeleteTS3L(gamePath);
				Log("Deleted " + Path.Combine(gamePath, newGameName));

				// Delete the new registry key SimL.
				MachineRegistry.DeleteKey(simLKey);
				Log("Deleted " + MachineRegistry.GetFullKey(simLKey));
			}

			if (ShowOutput)
			{
				Console.ReadLine();
			}
		}

		/// <summary>
		/// Creates a copy of TS3W.exe called TS3L.exe. The copy uses different Windows Registry keys for finding Expansion Packs.
		/// </summary>
		/// <param name="gamePath">The path to the parent directory of TS3W.exe.</param>
		/// <exception cref="Exception">Thrown if anything unexpected occurs.</exception>
		private static void CreateTS3L(string gamePath)
		{
			string newGamePath = Path.Combine(gamePath, GameDirectory.NewGame);
			string oldGamePath = Path.Combine(gamePath, GameDirectory.OldGame);

			// Translate "Software\Sims" and "Software\SimL to bytes"
			byte[] oldTargetBytes = Encoding.UTF8.GetBytes(OldTarget);
			byte[] newTargetBytes = Encoding.UTF8.GetBytes(NewTarget);

			// Read TS3W.exe.
			byte[] newGameContent = File.ReadAllBytes(oldGamePath);

			// Replace "Software\Sims" with "Software\SimL"
			newGameContent.ReplaceAll(oldTargetBytes, newTargetBytes);

			// Save as TS3L.exe.
			File.WriteAllBytes(newGamePath, newGameContent);
		}

		/// <summary>
		/// Deletes TS3L.exe.
		/// </summary>
		/// <param name="gamePath">The path to the parent directory of TS3W.exe.</param>
		/// <exception cref="Exception">Thrown if anything unexpected occurs.</exception>
		private static void DeleteTS3L(string gamePath)
		{
			// Delete TS3L.exe.
			string newGamePath = Path.Combine(gamePath, GameDirectory.NewGame);
			File.Delete(newGamePath);
		}

		/// <summary>
		/// Writes a message to the Console if ShowOutput is true.
		/// </summary>
		/// <param name="message">The message to log.</param>
		private static void Log(string message)
		{
			if (ShowOutput)
			{
				Console.WriteLine(message);
			}
		}
	}
}
