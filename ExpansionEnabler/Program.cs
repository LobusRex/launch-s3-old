using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
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

		private static readonly string oldKey = "Sims";
		private static readonly string newKey = "SimL";
		private static readonly string baseGameKey = "The Sims 3";

		private static bool showOutput = true;

		private static void Main(string[] args)
		{
			//foreach (string arg in args)
			//{
			//	Console.WriteLine(arg);
			//}

			string task = args[0];
			string gamePath = args[1];
			string regPath = args[2];
			string volume = args.Length > 3 ? args[3] : "";

			showOutput = volume.ToLower() != "quiet";

			Log("");

			Log("Path:     " + regPath);
			Log("Old:      " + Path.Combine(regPath, oldKey));
			Log("New:      " + Path.Combine(regPath, newKey));
			Log("Old Base: " + Path.Combine(regPath, oldKey, baseGameKey));
			Log("New Base: " + Path.Combine(regPath, newKey, baseGameKey));

			Log("");

			if (task.ToLower() == "enable")
			{
				// Create the new registry sub key.
				CreateReg(regPath);
				Log("Created " + Path.Combine(regPath, newKey));

				// Create the new game TS3L.exe.
				CreateTS3L(gamePath);
				Log("Created " + Path.Combine(gamePath, newGame));
			}
			else if (task.ToLower() == "disable")
			{
				// Delete the new game TS3L.exe.
				DeleteTS3L(gamePath);
				Log("Deleted " + Path.Combine(gamePath, newGame));

				// Delete the new registry sub key SimL.
				DeleteReg(regPath);
				Log("Deleted " + Path.Combine("HKEY_LOCAL_MACHINE", regPath, newKey));
			}

			if (showOutput)
			{
				Console.ReadLine();
			}
		}

		private static void CreateReg(string regPath)
		{
			// Create the SimL sub key.
			RegistryKey newRegistryKey = Registry.LocalMachine.CreateSubKey(Path.Combine(regPath, newKey));

			// Give all users write permission.
			AllowAllUsers(newRegistryKey);
			ShowSecurity(newRegistryKey);
			newRegistryKey.Close();

			// Copy the Sims\The Sims 3 subkey to SimL\The Sims 3.
			RegistryKey oldBaseGameKey = Registry.LocalMachine.OpenSubKey(Path.Combine(regPath, oldKey, baseGameKey));
			RegistryKey newBaseGameKey = Registry.LocalMachine.CreateSubKey(Path.Combine(regPath, newKey, baseGameKey));
			oldBaseGameKey.CopyTo(newBaseGameKey);
			newBaseGameKey.Close();
		}

		private static void DeleteReg(string regPath)
		{
			// Delete HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\SimL.
			string newRegKey = Path.Combine(regPath, newKey);
			Registry.LocalMachine.DeleteSubKeyTree(newRegKey, false);
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

		private static void ShowSecurity(RegistryKey registryKey)
		{
			RegistrySecurity security = registryKey.GetAccessControl();

			Log("Current access rules:");

			foreach (RegistryAccessRule ar in security.GetAccessRules(true, true, typeof(NTAccount)))
			{
				Log("        User: " + ar.IdentityReference);
				Log("        Type: " + ar.AccessControlType);
				Log("      Rights: " + ar.RegistryRights);
				Log(" Inheritance: " + ar.InheritanceFlags);
				Log(" Propagation: " + ar.PropagationFlags);
				Log("   Inherited: " + ar.IsInherited);
				Log("");
			}
		}

		// Inspired by a question and answer from Stack Overflow.
		// https://stackoverflow.com/q/2151074/13798212
		// Asked by Jared Knipp https://stackoverflow.com/users/39803/jared-knipp
		// Answered by KevinC https://stackoverflow.com/users/1940560/kevinc
		private static void AllowAllUsers(RegistryKey registryKey)
		{
			// Get the "Everyone" user.
			SecurityIdentifier identifier = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
			NTAccount user = identifier.Translate(typeof(NTAccount)) as NTAccount;

			// Write permission for the user.
			RegistryAccessRule accessRule = new RegistryAccessRule(
				user.ToString(),
				RegistryRights.FullControl,
				InheritanceFlags.ContainerInherit,
				PropagationFlags.None,
				AccessControlType.Allow);

			// Add new permissions to the key.
			RegistrySecurity security = registryKey.GetAccessControl();
			security.AddAccessRule(accessRule);
			registryKey.SetAccessControl(security);
		}
	}
}
