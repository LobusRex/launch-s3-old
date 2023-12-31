﻿using System.IO.Compression;

namespace Common
{
	public static class ModSelectionManager
	{
		public static string FrameworkSetupUrl { get; set; } = "https://chii.modthesims.info/FrameworkSetup.zip";

		public static Dictionary<string, bool> GetAvailableAndSelected()
		{
			Dictionary<string, bool> mods = new Dictionary<string, bool>();

			// Get the mods from the games folder.
			DirectoryInfo gameModsFolder = new DirectoryInfo(DocumentFolders.Game.ModPath);
			foreach (FileInfo file in gameModsFolder.GetFiles("*.package"))
			{
				string name = Path.GetFileNameWithoutExtension(file.Name);

				// Add the mod to the dictionary.
				// This mod is selected.
				mods.Add(name, true);
			}

			// Get additional mods from the launcher folder.
			DirectoryInfo launcherModsFolder = new DirectoryInfo(DocumentFolders.Launcher.ModPath);
			foreach (FileInfo file in launcherModsFolder.GetFiles("*.package"))
			{
				string name = Path.GetFileNameWithoutExtension(file.Name);

				// Add the mod if it is not already in the dictionary.
				// This mod is not selected.
				mods.TryAdd(name, false);
			}

			return mods;
		}

		public static void Select(string name)
		{
			string packageName = name + ".package";

			// Get mods folders for both game and launcher.
			string gameMods = DocumentFolders.Game.ModPath;
			string launcherMods = DocumentFolders.Launcher.ModPath;

			// Move the mod to the game mods folder.
			DocumentFolders.MoveFile(launcherMods, gameMods, packageName);
		}

		public static void Deselect(string name)
		{
			string packageName = name + ".package";

			// Get mods folders for both game and launcher.
			string gameMods = DocumentFolders.Game.ModPath;
			string launcherMods = DocumentFolders.Launcher.ModPath;

			// Move the mod to the launcher mods folder.
			DocumentFolders.MoveFile(gameMods, launcherMods, packageName);
		}

		public static void Add(string packagePath)
		{
			// Get mods folders for the game.
			string gameMods = DocumentFolders.Game.ModPath;

			FileInfo file = new FileInfo(packagePath);

			if (file.DirectoryName == null)
				return;

            // Copy the mod to the game mods folder.
            DocumentFolders.CopyFile(file.DirectoryName, gameMods, file.Name);
		}

		public static void Delete(string name)
		{
			string packageName = name + ".package";

			// Get mods folders for both game and launcher.
			string gameMods = DocumentFolders.Game.ModPath;
			string launcherMods = DocumentFolders.Launcher.ModPath;

			// Delete the mod from both/either mod folder.
			DocumentFolders.DeleteFile(gameMods, packageName);
			DocumentFolders.DeleteFile(launcherMods, packageName);
		}

		public enum DownloadExtractResult
		{
			Succeeded,
			DownloadFailed,
			ExtractionFailed,
		}

		public static Task<DownloadExtractResult> EnableSelection()
		{
			return DownloadFrameworkSetup();
		}

		private static async Task<DownloadExtractResult> DownloadFrameworkSetup()
		{
			try
			{
				// Download FrameworkSetup.zip.
				using HttpClient HttpClient = new HttpClient();
				using Stream data = await HttpClient.GetStreamAsync(FrameworkSetupUrl);

				// Extract the archive to the game documents folder.
				ZipArchive archive = new ZipArchive(data, ZipArchiveMode.Read);
				archive.ExtractToDirectory(DocumentFolders.Game.Path, true);
			}
			catch (Exception e)
			{
				if (e is InvalidOperationException ||
					e is HttpRequestException ||
					e is TaskCanceledException)
				{
					// The Download failed.
					return DownloadExtractResult.DownloadFailed;
				}
				else
				{
					// The extraction failed.
					return DownloadExtractResult.ExtractionFailed;
				}
			}

			// The set up was successfull.
			return DownloadExtractResult.Succeeded;
		}
	}
}
