using System.Diagnostics;

namespace Common
{
	public static class Folder
	{
		public static void OpenWithExplorer(string path)
		{
			Process process = new Process();
			process.StartInfo.FileName = "explorer.exe";

			process.StartInfo.ArgumentList.Add(path);

			// Start the process.
			try
			{
				process.Start();
			}
			catch { }
		}
	}
}
