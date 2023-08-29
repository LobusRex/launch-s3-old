using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
