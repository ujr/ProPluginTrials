using System.Reflection;
using System.Runtime.CompilerServices;
using Core;
using ArcGIS.Core.Hosting;
using ArcGIS.Core.Data;
using Microsoft.Win32;

namespace ProPlugin;

public class ProPlugin : IPlugin
{
	private const string DefaultProInstallPath = @"C:\Program Files\ArcGIS\Pro";

	private readonly Assembly _thisAssembly;
	private string? _proInstallPath;

	public string Name => "TrialProPlugin";

	public ProPlugin()
	{
		_thisAssembly = Assembly.GetExecutingAssembly();

		var current = AppDomain.CurrentDomain;
		current.AssemblyResolve += ResolveProAssemblyPath;
	}

	public void Initialize()
	{
		_proInstallPath = FindProInstallPath();

		InitializeProCoreHost();
	}

	public string DoPluginWork(string fgdbPath, string datasetName)
	{
		var uri = new Uri(fgdbPath, UriKind.Absolute);
		var connector = new FileGeodatabaseConnectionPath(uri);
		using var gdb = new Geodatabase(connector);

		using var fc = gdb.OpenDataset<FeatureClass>(datasetName);
		using var defn = fc.GetDefinition();

		var sref = defn.GetSpatialReference();
		var shapeField = defn.GetShapeField();
		var oidField = defn.GetObjectIDField();

		var rowCount = fc.GetCount();
		long vertices = 0;

		using var cursor = fc.Search();
		while (cursor.MoveNext())
		{
			var feature = (Feature)cursor.Current;
			var shape = feature.GetShape();
			vertices += shape.PointCount;
		}

		return $"{datasetName}: SRID={sref.Wkid}, #rows={rowCount}, #vertices={vertices}";
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void InitializeProCoreHost()
	{
		// Must be called before any ArcGIS.Core access
		Host.Initialize();
	}

	private Assembly? ResolveProAssemblyPath(object? sender, ResolveEventArgs args)
	{
		if (_thisAssembly != args.RequestingAssembly)
			return null; // not our dependency
		if (_proInstallPath is null)
			return null; // not initialized
		var name = new AssemblyName(args.Name);
		var fileName = name.Name + ".dll";
		string path = Path.Combine(_proInstallPath, "bin", fileName);
		var assembly = Assembly.LoadFrom(path);
		return assembly;
	}

	private static string FindProInstallPath()
	{
		// TODO simplistic (at least log)
		try
		{
			const string regPath = @"SOFTWARE\ESRI\ArcGISPro";

			var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			var esriKey = baseKey.OpenSubKey(regPath);

			if (esriKey is null)
			{
				baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
				esriKey = baseKey.OpenSubKey(regPath);
			}

			if (esriKey is null)
			{
				throw new InvalidOperationException(
					"Cannot find install location of ArcGIS Pro in registry: " +
					$"cannot open key {baseKey.Name}\\{regPath}");
			}

			const string installDirName = "InstallDir";
			var path = (string?)esriKey.GetValue(installDirName);
			if (string.IsNullOrEmpty(path))
				throw new InvalidOperationException(
					"Cannot find install location of ArcGIS Pro in registry: " +
					$"no value for {installDirName} in {esriKey.Name}");

			const string versionName = "Version";
			var version = esriKey.GetValue(versionName) as string;
			if (string.IsNullOrEmpty(version))
				throw new InvalidOperationException(
					"Cannot find version of ArcGIS Pro in registry: " +
					$"no value for {versionName} in {esriKey.Name}");

			const string buildNumberName = "BuildNumber";
			var buildNo = esriKey.GetValue(buildNumberName) as string;
			if (string.IsNullOrEmpty(buildNo))
				throw new InvalidOperationException(
					"Cannot find build number of ArcGIS Pro in registry: " +
					$"no value for {buildNumberName} in {esriKey.Name}");

			return path; // TODO also return version and buildNo?
		}
		catch
		{
			return DefaultProInstallPath; // fallback
		}
	}
}
