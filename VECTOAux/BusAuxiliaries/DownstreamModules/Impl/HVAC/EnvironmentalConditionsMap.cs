using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;
using TUGraz.VectoCore.BusAuxiliaries.Util;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class EnvironmentalConditionsMap : IEnvironmentalConditionsMap
	{
		private string filePath;
		private string vectoDirectory;

		private List<IEnvironmentalCondition> _map = new List<IEnvironmentalCondition>();

		public EnvironmentalConditionsMap(string filepath, string vectoDirectory)
		{
			this.filePath = filepath;
			this.vectoDirectory = vectoDirectory;

			Initialise();
		}

		public bool Initialise()
		{
			if ((!string.IsNullOrWhiteSpace(filePath))) {
				filePath = FilePathUtils.ResolveFilePath(vectoDirectory, filePath);

				if (File.Exists(filePath)) {
					using (var sr = new StreamReader(filePath)) {

						// get array og lines fron csv
						var lines = sr.ReadToEnd().Split(new [] { Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

						// Must have at least 1 entries to make it usable [dont forget the header row]
						if ((lines.Count() < 2))
							return false;

						var firstline = true;

						foreach (var line in lines) {
							if (!firstline) {

								// split the line
								var elements = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

								// 3 entries per line required
								if ((elements.Length != 4))
									return false;

								// Add environment condition
								var newCondition = new EnvironmentalCondition(double.Parse(elements[1], CultureInfo.InvariantCulture), double.Parse(elements[2], CultureInfo.InvariantCulture), double.Parse(elements[3], CultureInfo.InvariantCulture));

								_map.Add(newCondition);
							} else
								firstline = false;
						}
					}
				} else
					return false;
			}

			return true;
		}

		public List<IEnvironmentalCondition> GetEnvironmentalConditions()
		{
			return _map;
		}
	}
}
