using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{

	public sealed class IMCTechnologyLookup : LookupData<IMCTechnology, IMCTechnologyLookup.Entry>
	{

		#region Overrides of LookupData

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".IMC-Tech.csv";

		protected override void ParseData(DataTable table)
		{
			Data = table.Rows.Cast<DataRow>().Select(r => {
				return new Entry(
					r.Field<string>("technology").ParseEnum<IMCTechnology>(),
					r.ParseDouble("deltacdxa").SI<SquareMeter>(),
					r.ParseBoolean("heavylorries"),
					r.ParseBoolean("mediumlorries"),
					r.ParseBoolean("heavybuses"));
			}).ToDictionary(x => x.Technology);
		}

		#endregion

		public struct Entry
		{

			public readonly IMCTechnology Technology;

			public readonly SquareMeter deltaCdxA;

			public readonly bool HeavyLorry;

			public readonly bool MediumLorry;

			public readonly bool HeavyBus;

			public Entry(IMCTechnology tech, SquareMeter dCdxA, bool hl, bool ml, bool hb)
			{
				Technology = tech;
				deltaCdxA = dCdxA;
				HeavyLorry = hl;
				MediumLorry = ml;
				HeavyBus = hb;
			}
		}
	}
}