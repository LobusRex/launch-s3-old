using Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace LobuS3Launcher
{
	class Launcher
	{
		/// <summary>
		/// Launches the game.
		/// </summary>
		/// <param name="baseGamePath">The path to the game Bin directory.</param>
		/// <param name="startSingleCore">Whether or not the game should start
		/// with the Intel CPU fix.</param>
		public static void Launch(string baseGamePath, bool startSingleCore)
		{
			// Select which TS3*.exe to run.
			bool selectionEnabled = EPSelectionManager.GetSelectionEnabled();
			string newGamePath = Path.Combine(baseGamePath, GameDirectory.NewGame);
			string oldGamePath = Path.Combine(baseGamePath, GameDirectory.OldGame);
			string TS3Path = selectionEnabled ? newGamePath : oldGamePath;

			Trace.WriteLine("Starting " + TS3Path);

			// Start the game.
			Process ts3 = new Process();
			ts3.StartInfo.FileName = TS3Path;
			try
			{
				bool started = ts3.Start();

				Trace.WriteLine(started ? "Started" : "Not started");
			}
			catch
			{ return; }

			// Help the game start on new Intel processors.
			if (startSingleCore)
				new Thread(() => LimitProcessCores(ts3)).Start();
		}

		// Inspired by Miaa245's core limiting script.
		// https://answers.ea.com/t5/Technical-Issues-PC/Sims-3-won-t-open-Alder-Lake-Intel-12th-gen-CPU/td-p/11057820/page/5.
		// Miaa245 https://answers.ea.com/t5/user/viewprofilepage/user-id/7950707
		/// <summary>
		/// Limits the number of cores used by a process for a short amount of time.
		/// </summary>
		/// <param name="process">The process to limit.</param>
		private static void LimitProcessCores(Process process)
		{
			// The processor affinity used when the game starts.
			// Each binary digit corresponds to one core.
			int startupAffinity = 0b1;

			// The duration to limit the processor affinity.
			int limiterDurationMs = 5000;

			try
			{
				// Save core usage and limit to a single core.
				IntPtr affinity = process.ProcessorAffinity;
				process.ProcessorAffinity = new IntPtr(startupAffinity);

				// Wait for the game to start.
				Thread.Sleep(limiterDurationMs);

				Trace.WriteLine(process.HasExited ? "Not running" : "Running");

				// Return if the process is turned off. This is the case if multiple instances were launched.
				if (process.HasExited)
				{
					return;
				}

				// Restore previous core usage.
				process.ProcessorAffinity = affinity;
			}
			catch { }
		}
	}
}
