﻿using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Common
{
	public static class MachineRegistry
	{
		public static string SoftwarePath { get; } = @"SOFTWARE\WOW6432Node";
		public static string SimsKey { get; } = "Sims";
		public static string SimLKey { get; } = "SimL";
		public static string BaseGameKey { get; } = "The Sims 3";

		public static string GetBaseBinPath()
		{
			using RegistryKey gameKey = Registry.LocalMachine.OpenSubKey(GetFullKey(SimsKey, BaseGameKey)) ?? throw new RegistryKeyNotFoundException();

			string name = "ExePath";

			// TODO: Consider replacing the RegistryKeyNotFoundException with some other exception.
			object value = gameKey.GetValue(name) ?? throw new RegistryKeyNotFoundException();
			
			return ((string)value).Replace(@"\TS3.exe", "");
		}

		public static string GetFullKey(string simsKey, string gameKey)
		{
			return Path.Combine(SoftwarePath, simsKey, gameKey);
		}

		public static string GetFullKey(string simsKey)
		{
			return Path.Combine(SoftwarePath, simsKey);
		}

		public static bool KeyExists(string simKey, string gameKey)
		{
			try
			{
				// Open the key.
				string key = GetFullKey(simKey, gameKey);
				using RegistryKey? regKey = Registry.LocalMachine.OpenSubKey(key);

				// The key exists if regKey is not null.
				return regKey != null;
			}
			catch
			{ return false; }
		}

		// Inspired by a question and answer from Stack Overflow.
		// https://stackoverflow.com/q/12262536/13798212
		// Asked by Edgar https://stackoverflow.com/users/1479536/edgar
		// Answered by Wallace Kelly https://stackoverflow.com/users/167920/wallace-kelly
		public static void CopyKey(string fromSimKey, string toSimKey, string gameKey)
		{
			// Open the old key.
			using RegistryKey fromKey = Registry.LocalMachine.OpenSubKey(GetFullKey(fromSimKey, gameKey)) ?? throw new RegistryKeyNotFoundException();

			// Create a new key.
			using RegistryKey toKey = Registry.LocalMachine.CreateSubKey(GetFullKey(toSimKey, gameKey));

			// Copy the values.
			foreach (string name in fromKey.GetValueNames())
			{
				object? value = fromKey.GetValue(name);

				if (value != null)
					toKey.SetValue(name, value, fromKey.GetValueKind(name));
			}
		}

		public static void DeleteKey(string simKey, string gameKey)
		{
			// Delete the key.
			Registry.LocalMachine.DeleteSubKeyTree(GetFullKey(simKey, gameKey), false);
		}

		public static void DeleteKey(string key)
		{
			// Delete the key.
			Registry.LocalMachine.DeleteSubKeyTree(GetFullKey(key), false);
		}

		public static void CreateOpenKey(string simKey)
		{
			// Create the key.
			using RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(GetFullKey(simKey));

			// Give all users write permission.
			AllowAllUsers(registryKey);
		}

		// Inspired by a question and answer from Stack Overflow.
		// https://stackoverflow.com/q/2151074/13798212
		// Asked by Jared Knipp https://stackoverflow.com/users/39803/jared-knipp
		// Answered by KevinC https://stackoverflow.com/users/1940560/kevinc
		private static void AllowAllUsers(RegistryKey registryKey)
		{
			// Get the "Everyone" user.
			SecurityIdentifier identifier = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
			NTAccount? user = identifier.Translate(typeof(NTAccount)) as NTAccount;
			if (user == null)
				return;

			// Write permission for the user.
			RegistryAccessRule accessRule = new RegistryAccessRule(
				user.ToString(),
				RegistryRights.FullControl,
				InheritanceFlags.ContainerInherit,
				PropagationFlags.None,
				AccessControlType.Allow);

			// Add new permission to the key.
			RegistrySecurity security = registryKey.GetAccessControl();
			security.AddAccessRule(accessRule);
			registryKey.SetAccessControl(security);
		}
	}
}
