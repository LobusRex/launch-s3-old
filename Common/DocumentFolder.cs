namespace Common
{
	public class DocumentFolder
	{
		public string Path { get; private set; }
		public string SavePath { get; private set; }
		public string ModPath { get; private set; }

		public DocumentFolder(string path, string saveFolder, string modFolder)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));

			if (saveFolder == null)
				throw new ArgumentNullException(nameof(saveFolder));

			if (modFolder == null)
				throw new ArgumentNullException(nameof(modFolder));

			Path = path;
			SavePath = System.IO.Path.Combine(path, saveFolder);
			ModPath = System.IO.Path.Combine(path, modFolder); ;
		}
	}
}
