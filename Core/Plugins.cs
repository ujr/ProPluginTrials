using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Core
{
	public abstract class Plugins
	{
		public static IPlugin LoadPlugin(string fileName)
		{
			if (fileName is null)
				throw new ArgumentNullException(nameof(fileName));

			var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
			if (string.IsNullOrEmpty(ext))
				fileName = Path.ChangeExtension(fileName, ".dll");

			string pluginPath = fileName;
			var directory = Path.GetDirectoryName(fileName);
			if (string.IsNullOrEmpty(directory))
			{
				// look relative to current assembly
				var current = Assembly.GetExecutingAssembly();
				directory = Path.GetDirectoryName(current.Location) ?? ".";
				pluginPath = Path.Combine(directory, fileName);
			}

			var assembly = Assembly.LoadFrom(pluginPath);
			// TODO simplistic (there may be many plugins in one assembly)
			var pluginType = assembly.GetTypes().Single(type => type.GetInterfaces().Contains(typeof(IPlugin)));
			var plugin = (IPlugin)Activator.CreateInstance(pluginType);

			return plugin;
		}
	}
}
