using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration.Auxiliaries
{
    public abstract class AbstractAuxiliaryFullyElectricLookup : LookupData<string, bool>,  IDeclarationAuxiliaryFullyElectricTable
    {
		#region Implementation of IDeclarationAuxiliaryFullyElectricTable

		public bool IsFullyElectric(string technology)
		{
			return Lookup(technology);
		}

		public string[] FullyElectricTechnologies()
		{
			return Data.Where(keyVal => keyVal.Value == true).Select(keyVal => keyVal.Key).ToArray();
		}

		#endregion

		#region Overrides of LookupData

		protected override void ParseData(DataTable table)
		{
			foreach (DataRow row in table.Rows) {
				var name = row["technology"].ToString();
				var electric = row.ParseBoolean("fullyelectric");
				Data[name] = electric;
			}
		}
		#endregion
	}
}
