# ArcGIS Pro Plugin Trials

How to write a runtime-loadable plugin that uses ArcGIS.Core functionality.

Projects in solution:

- Core: `IPlugin` interface
- ProPlugin: the plugin, using `ArcGIS.Core`
- AppCLI: command line executable loading the plugin
- AppWinForms WinForms application loading the plugin

References

- <https://github.com/esri/arcgis-pro-sdk/wiki/proconcepts-CoreHost>
- <https://github.com/Esri/arcgis-pro-sdk-community-samples/blob/master/CoreHost/CoreHostResolveAssembly/ReadMe.md>
- <https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly.loadfrom>
