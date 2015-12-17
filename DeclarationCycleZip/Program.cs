/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.Utils;

namespace DeclarationCycleZip
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var cycleData = DrivingCycleDataReader.ReadFromFileDistanceBased(args[0]);

			var table = new DataTable();
			table.Columns.Add("<s>");
			table.Columns.Add("<v>");
			table.Columns.Add("<grad>");
			table.Columns.Add("<stop>");

			var lastDistance = cycleData.Entries.First().Distance - 1.SI<Meter>();
			foreach (var x in cycleData.Entries) {
				if (x.Distance.IsEqual(lastDistance)) {
					continue;
				}
				var row = table.NewRow();
				row["<s>"] = x.Distance.Value().ToString(CultureInfo.InvariantCulture);
				row["<v>"] = x.VehicleTargetSpeed.ConvertTo().Kilo.Meter.Per.Hour.Value().ToString(CultureInfo.InvariantCulture);
				row["<grad>"] = x.RoadGradientPercent.ToString(CultureInfo.InvariantCulture);
				row["<stop>"] = x.StoppingTime.Value().ToString(CultureInfo.InvariantCulture);
				table.Rows.Add(row);
				lastDistance = x.Distance;
			}

			VectoCSVFile.Write(Path.GetFileName(args[0]), table);
		}
	}
}