// Copyright 2017 European Union.
// Licensed under the EUPL (the 'Licence');
// 
// * You may not use this work except in compliance with the Licence.
// * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
// * Unless required by applicable law or agreed to in writing,
// software distributed under the Licence is distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the LICENSE.txt for the specific language governing permissions and limitations.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.PneumaticSystem;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
{
	public class PneumaticActuationsMap : IPneumaticActuationsMAP
	{
		private Dictionary<ActuationsKey, int> map;
		private string filePath;


		public int GetNumActuations(ActuationsKey key)
		{
			if (map == null || !map.ContainsKey(key))
				throw new ArgumentException(string.Format("Pneumatic Actuations map does not contain the key '{0}'.", key.CycleName + ":" + key.ConsumerName));

			return map[key];
		}


		public PneumaticActuationsMap(string filePath)
		{
			this.filePath = filePath;

			if (filePath.Trim().Length == 0)
				throw new ArgumentException("A filename for the Pneumatic Actuations Map has not been supplied");

			Initialise();
		}

		public bool Initialise()
		{
			ActuationsKey newKey;
			int numActuations;

			if (File.Exists(filePath)) {
				using (var sr = new StreamReader(filePath)) {
					// get array of lines from csv
					var lines = sr.ReadToEnd().Split(new [] { Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

					// Must have at least 2 entries in map to make it usable [dont forget the header row]
					if (lines.Length < 3)
						throw new ArgumentException("Pneumatic Actuations Map does not have sufficient rows in file to build a usable map");

					map = new Dictionary<ActuationsKey, int>();
					var firstline = true;

					foreach (var line in lines) {
						if (!firstline) {
							// split the line
							var elements = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							// 3 entries per line required
							if ((elements.Length != 3))
								throw new ArgumentException("Pneumatic Actuations Map has Incorrect number of values in file");

							// add values to map


							if (!int.TryParse(elements[2], out numActuations))
								throw new ArgumentException("Pneumatic Actuations Map Contains Non Integer values in actuations column");

							// Should throw exception if ConsumerName or CycleName are empty.
							newKey = new ActuationsKey(elements[0].ToString(), elements[1].ToString());

							map.Add(newKey, int.Parse(elements[2], CultureInfo.InvariantCulture));
						} else
							firstline = false;
					}
				}
			} else
				throw new ArgumentException(string.Format(" Pneumatic Acutations map '{0}' supplied  does not exist", filePath));

			// If we get here then all should be well and we can return a True value of success.
			return true;
		}
	}
}
