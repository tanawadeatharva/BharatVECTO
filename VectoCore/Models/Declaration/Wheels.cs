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
using System.Linq;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class Wheels : LookupData<string, Wheels.WheelsEntry>
	{
		protected const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.Wheels.csv";

		public Wheels()
		{
			ParseData(ReadCsvResource(ResourceId));
		}

		public override WheelsEntry Lookup(string key)
		{
			return base.Lookup(key.Replace(" ", ""));
		}

		protected override sealed void ParseData(DataTable table)
		{
			Data = (from DataRow row in table.Rows
				select new WheelsEntry {
					WheelType = row.Field<string>(0).Replace(" ", ""),
					Inertia = row.ParseDouble(1).SI<KilogramSquareMeter>(),
					DynamicTyreRadius = row.ParseDouble(2).SI().Milli.Meter.Cast<Meter>(),
					SizeClass = row.Field<string>(3)
				}).ToDictionary(e => e.WheelType);
		}

		public class WheelsEntry
		{
			public string WheelType;
			public KilogramSquareMeter Inertia;
			public Meter DynamicTyreRadius;
			public string SizeClass;
		}
	}
}