namespace Common
{
	public static class DocumentFolders
	{
		private static string Path { get; }

		public static DocumentFolder Game { get; }
		public static DocumentFolder Launcher { get; }

		static DocumentFolders()
		{
			Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			Game = new DocumentFolder(System.IO.Path.Combine(Path, @"Electronic Arts\The Sims 3"), "Saves", @"Mods\Packages");
			Launcher = new DocumentFolder(System.IO.Path.Combine(Path, "LobuS3Launcher"), "Saves", "Mods");
		}

		public static void MoveFile(string from, string to, string name)
		{
			string fromPath = System.IO.Path.Combine(from, name);
			string toPath = System.IO.Path.Combine(to, name);

			try
			{
				File.Move(fromPath, toPath, true);
			}
			catch { }
		}

		public static void CopyFile(string from, string to, string name)
		{
			string fromPath = System.IO.Path.Combine(from, name);
			string toPath = System.IO.Path.Combine(to, name);

			try
			{
				File.Copy(fromPath, toPath, true);
			}
			catch { }
		}

		public static void DeleteFile(string from, string name)
		{
			string path = System.IO.Path.Combine(from, name);

			try
			{
				File.Delete(path);
			}
			catch { }
		}

		public static void BackupSaves()
		{
			string backupName = "Backup " + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");

			RecursiveSaveBackup(Game.Path, System.IO.Path.Combine(Launcher.SavePath, backupName));
		}

		// Inspired by a question and answer from Stack Overflow.
		// https://stackoverflow.com/q/58744/13798212
		// Asked by Keith https://stackoverflow.com/users/905/keith
		// Answered by Konrad Rudolph https://stackoverflow.com/users/1968/konrad-rudolph
		private static void RecursiveSaveBackup(string sourcePath, string targetPath)
		{
			DirectoryInfo source = new DirectoryInfo(sourcePath);
			DirectoryInfo target = new DirectoryInfo(targetPath);

			// Copy all non backup save directories.
			foreach (DirectoryInfo dir in source.GetDirectories())
			{
				if (dir.Extension == ".backup")
					continue;

				RecursiveSaveBackup(dir.FullName, target.CreateSubdirectory(dir.Name).FullName);
			}

			// Copy all files within the save directory.
			foreach (FileInfo file in source.GetFiles())
				file.CopyTo(System.IO.Path.Combine(target.FullName, file.Name));
		}
	}
}
