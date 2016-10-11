using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.OutputData;

namespace TUGraz.VectoCommon.Utils
{
	public class PluginRegistry
	{
		private static PluginRegistry _instance;

		private readonly Dictionary<string, IExportPlugin> _exportPlugins = new Dictionary<string, IExportPlugin>();
		private readonly Dictionary<string, IImportPlugin> _importPlugins = new Dictionary<string, IImportPlugin>();
		private readonly Dictionary<string, IInputDataPlugin> _inputDataPlugins = new Dictionary<string, IInputDataPlugin>();

		public static PluginRegistry Instance
		{
			get { return _instance ?? (_instance = new PluginRegistry()); }
		}

		public void RegisterPlugin(IExportPlugin plugin)
		{
			_exportPlugins.Add(plugin.Key, plugin);
		}

		public void RegisterPlugin(IImportPlugin plugin)
		{
			_importPlugins.Add(plugin.Key, plugin);
		}

		public void RegisterPlugin(IInputDataPlugin plugin)
		{
			_inputDataPlugins.Add(plugin.Key, plugin);
		}

		public IExportPlugin GetExportPlugin(string key)
		{
			return _exportPlugins.ContainsKey(key) ? _exportPlugins[key] : null;
		}

		public IImportPlugin GetImportPlugin(string key)
		{
			return _importPlugins.ContainsKey(key) ? _importPlugins[key] : null;
		}

		public IEnumerable<KeyValuePair<string, IInputDataPlugin>> GetInputDataPlugins()
		{
			return _inputDataPlugins.Select(x => x).ToList();
		}

		public IInputDataPlugin GetInputDataPlugin(string key)
		{
			return _inputDataPlugins.ContainsKey(key) ? _inputDataPlugins[key] : null;
		}

		public IEnumerable<string> GetKnownInputExtensions()
		{
			var retVal = new List<string>();
			foreach (var entry in _inputDataPlugins) {
				retVal.AddRange(entry.Value.KnownExtensions);
			}
			return retVal;
		}
	}
}