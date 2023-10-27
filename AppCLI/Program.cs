// See https://aka.ms/new-console-template for more information

using Core;

Console.WriteLine("Hello, World!");

var plugin = Plugins.LoadPlugin("ProPlugin.dll");

plugin.Initialize();
var s = plugin.DoPluginWork(@"C:\Work\Data\Esri\California.gdb", "Schools");
Console.WriteLine($"SRef: {s}");

Console.WriteLine("Press any key to finish...");
Console.ReadKey();
