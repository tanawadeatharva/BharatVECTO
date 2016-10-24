/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

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