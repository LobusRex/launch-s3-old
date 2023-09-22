using System.Windows;

namespace LobuS3Launcher
{
	public static class ErrorBox
	{
		private static string Caption { get; } = "Error";
		private static MessageBoxImage Icon { get; } = MessageBoxImage.Error;
		private static MessageBoxButton Button { get; } = MessageBoxButton.OK;

		/// <summary>
		/// Show a MessageBox, styled for errors.
		/// </summary>
		/// <param name="message">The error message to display.</param>
		public static void Show(string message)
		{
			MessageBox.Show(message, Caption, Button, Icon);
		}
	}
}
