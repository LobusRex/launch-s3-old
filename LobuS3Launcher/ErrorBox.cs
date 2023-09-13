using System.Windows;

namespace LobuS3Launcher
{
	public static class ErrorBox
	{
		private static string Caption { get; } = "Error";
		private static MessageBoxImage Icon { get; } = MessageBoxImage.Error;
		private static MessageBoxButton Button { get; } = MessageBoxButton.OK;

		public static void Show(string message)
		{
			MessageBox.Show(message, Caption, Button, Icon);
		}
	}
}
