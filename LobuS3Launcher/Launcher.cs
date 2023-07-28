using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace LobuS3Launcher
{
	class Launcher
	{
		public static readonly string gamePath = @"D:\Programs\Electronic Arts\The Sims 3\Game\Bin";
		private static readonly string oldGame = "TS3W.exe";
		private static readonly string newGame = "TS3L.exe";

		private static readonly int startupAffinity = 0b1;
		private static readonly int startupDelayMs = 5000;

		public static void SingleCoreLaunch()
		{
			Thread thread = new Thread(new ThreadStart(SingleCoreLaunchThread));
			thread.Start();
		}

		private static void SingleCoreLaunchThread()
		{
			string newGamePath = Path.Combine(gamePath, newGame);
			string oldGamePath = Path.Combine(gamePath, oldGame);

			// Select which TS3*.exe to run.
			string TS3Path = File.Exists(newGamePath) ? newGamePath : oldGamePath;
			
			Trace.WriteLine("Starting " + TS3Path);

			Process ts3 = new Process();
			ts3.StartInfo.FileName = TS3Path;

			// Start the game.
			try
			{
				// Start the game.
				Trace.WriteLine(ts3.Start() ? "Started" : "Not started");
			}
			catch
			{
				return;
			}

			// Limit core usage until the game is started.
			// Inspired by Miaa234's core limiting script.
			// https://answers.ea.com/t5/Technical-Issues-PC/Sims-3-won-t-open-Alder-Lake-Intel-12th-gen-CPU/td-p/11057820/page/5.
			// Miaa245 https://answers.ea.com/t5/user/viewprofilepage/user-id/7950707
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
