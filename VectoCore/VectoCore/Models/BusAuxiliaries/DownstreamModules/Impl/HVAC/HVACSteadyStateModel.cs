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
using System.IO;
using System.Linq;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class HVACSteadyStateModel : IHVACSteadyStateModel
	{
		public float HVACElectricalLoadPowerWatts { get; set; }
		public float HVACFuellingLitresPerHour { get; set; }
		public float HVACMechanicalLoadPowerWatts { get; set; }

		// Constructors
		public HVACSteadyStateModel()
		{
		}

		public HVACSteadyStateModel(float elecPowerW, float mechPowerW, float fuellingLPH)
		{
			HVACElectricalLoadPowerWatts = elecPowerW;
			HVACFuellingLitresPerHour = mechPowerW;
			HVACMechanicalLoadPowerWatts = fuellingLPH;
		}

		// Implementation
		public bool SetValuesFromMap(string filePath, ref string message)
		{
			string myData;
			string[] linesArray;

			var vbLf = '\n';

			// Check map file can be found.
			try {
				myData = System.IO.File.ReadAllText(filePath, System.Text.Encoding.UTF8);
			} catch (FileNotFoundException ) {
				message = "HVAC Steady State Model : The map file was not found";
				return false;
			}


			linesArray = (from s in myData.Split(vbLf)
						  select s.Trim()).ToArray();

			// Check count is at least 2 rows
			if (linesArray.Count() < 2) {
				message = "HVAC Steady State Model : Insufficient Lines in this File";
				return false;
			}

			// validate headers
			var headers = linesArray[0].Split(',');
			if (headers.Length != 3 || headers[0].Trim() != "[Electrical Power (w)]" || headers[1].Trim() != "[Mechanical Power (w)]" || headers[2].Trim() != "[Fuelling (L/H)]") {
				message = "HVAC Steady State Model : Column headers in  *.AHSM file being read are incompatable.";
				return false;
			}

			// validate values
			var values = linesArray[1].Split(',');
			double unused;
			if (headers.Length != 3 || !double.TryParse(values[0], out unused) || !double.TryParse(values[1], out unused) || !double.TryParse(values[2], out unused)) {
				message = "Steady State Model : Unable to confirm numeric values in the *.AHSM file being read.";
				return false;
			}

			// OK we have the values so lets set the  properties
			float out1, out2, out3;
			out1 = HVACElectricalLoadPowerWatts;
			out2 = HVACMechanicalLoadPowerWatts;
			out3 = HVACFuellingLitresPerHour;
			try {
				HVACElectricalLoadPowerWatts = float.Parse(values[0]);
				HVACMechanicalLoadPowerWatts = float.Parse(values[1]);
				HVACFuellingLitresPerHour = float.Parse(values[2]);
			} catch (Exception ) {

				// Restore in the event of failure to fully assign
				HVACElectricalLoadPowerWatts = out1;
				HVACMechanicalLoadPowerWatts = out2;
				HVACFuellingLitresPerHour = out3;

				// Return result
				message = "Steady State Model : Unable to parse the values in the *.AHSM file being read no values were harmed in reading of this file.";
				return false;
			}


			return true;
		}
	}
}
