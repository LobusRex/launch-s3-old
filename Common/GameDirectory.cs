namespace Common
{
	public static class GameDirectory
	{
		public static string OldGame { get; } = "TS3W.exe";
		public static string NewGame { get; } = "TS3L.exe";

		/// <summary>
		/// The path to the directory where the game executable is located.
		/// </summary>
		public static string? BaseGamePath
		{
			get
			{
				// Get the path from MachineRegistry.
				try
				{
					 return MachineRegistry.GetBaseBinPath();
				}
				catch { return null; }
			}
		}
	}
}
