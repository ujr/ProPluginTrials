using Core;

namespace AppWinForms
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			var plugin = Plugins.LoadPlugin("ProPlugin.dll");
			plugin.Initialize();
			var s = plugin.DoPluginWork(@"C:\Work\Data\Esri\California.gdb", "Schools");

			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			Application.Run(new Form1(plugin));
		}
	}
}