using System;
using System.Diagnostics;
using System.Threading;

namespace LobuS3Launcher
{
    class Launcher
    {
		private static readonly string gamePath = "D:\\Programs\\Electronic Arts\\The Sims 3\\Game\\Bin\\TS3W.exe";
		private static readonly int startupAffinity = 0b1;
		private static readonly int startupDelayMs = 5000;

		public static void SingleCoreLaunch()
		{
			Thread thread = new Thread(new ThreadStart(StartS3));
			thread.Start();
		}

		private static void StartS3()
		{
			Process ts3 = new Process();

			// Start the game.
			try
			{
				ts3.StartInfo.FileName = gamePath;
				bool test = ts3.Start();

				Trace.WriteLine(test ? "Started" : "Not started");
			}
			catch
			{
				return;
			}

			// Limit core usage until the game is started.
			// Inspired by the script by miaa234 found at https://answers.ea.com/t5/Technical-Issues-PC/Sims-3-won-t-open-Alder-Lake-Intel-12th-gen-CPU/td-p/11057820/page/5.
			try
			{
				// Save core usage and limit to a single core.
				IntPtr affinity = ts3.ProcessorAffinity;
				ts3.ProcessorAffinity = new IntPtr(startupAffinity);

				// Wait for the game to start.
				Thread.Sleep(startupDelayMs);

				Trace.WriteLine(ts3.HasExited ? "Not running" : "Running");

				// Return if the process is turned off. This is the case if multiple instances were launched.
				if (ts3.HasExited)
				{
					return;
				}

				// Restore previous core usage.
				ts3.ProcessorAffinity = affinity;
			}
			catch
			{
				return;
			}

			// Kill the game (just for testing)
			//Thread.Sleep(5000);
			//ts3.Kill();
		}
	}
}
