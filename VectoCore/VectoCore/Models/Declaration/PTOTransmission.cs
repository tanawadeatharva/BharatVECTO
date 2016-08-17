using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class PTOTransmission : LookupData<string, Watt>
	{
		protected override string ResourceId
		{
			get { return "TUGraz.VectoCore.Resources.Declaration.VAUX.PTO-tech.csv"; }
		}

		protected override string ErrorMessage
		{
			get { return "PTO Transmission Lookup Error: No value found for PTO Transmission. Technology: '{0}'"; }
		}

		protected override void ParseData(DataTable table)
		{
			Data = table.Rows.Cast<DataRow>().ToDictionary(
				r => r.Field<string>("technology"),
				r => r.ParseDouble("powerloss").SI<Watt>());
		}
	}
}