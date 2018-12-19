using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{

	public struct ADASCombination
	{
		public string ID;
	}
	public sealed class ADASCombinations : LookupData<bool, bool, bool, PredictiveCruiseControlType, ADASCombination>
	{
		private readonly List<Entry> _combinations = new List<Entry>();

		#region Overrides of LookupData

		public override ADASCombination Lookup(bool enginestopstart, bool ecorollwithoutenginestop, bool ecorollwithenginestop, PredictiveCruiseControlType pcc)
		{
			try {
				var entry = _combinations.First(
					x => x.EngineStopStart == enginestopstart && x.EcoRollWithoutEngineStop == ecorollwithoutenginestop &&
						x.EcorollWithEngineStop == ecorollwithenginestop && x.PCCType == pcc);
				return new ADASCombination {ID = entry.ADASCombination};
			} catch (Exception ) { 
				throw new VectoException(string.Format(ErrorMessage, enginestopstart, ecorollwithoutenginestop, ecorollwithenginestop, pcc));
			}
		}

		protected override string ResourceId
		{
			get { return DeclarationData.DeclarationDataResourcePrefix + ".ADAS.ADAS_Combinations.csv"; }
		}

		protected override string ErrorMessage
		{
			get {
				return
					"ADAS Combination Lookup Error: No entry found for engine stop/start: {0}, eco roll w/o engine stop: {1}, eco roll w engine stop: {2}, PCC: {3}";
			}
		}

		protected override void ParseData(DataTable table)
		{
			foreach (DataRow row in table.Rows) {
				_combinations.Add(new Entry() {
					EngineStopStart = row.ParseBoolean("enginestopstart"),
					EcoRollWithoutEngineStop = row.ParseBoolean("ecorollwithoutenginestop"),
					EcorollWithEngineStop = row.ParseBoolean("ecorollwithenginestop"),
					PCCType = PredictiveCruiseControlTypeHelper.Parse(row.Field<string>("predictivecruisecontrol")),
					ADASCombination = row.Field<string>("adascombination")
				});
			}
		}

		#endregion

		public struct Entry
		{
			public bool EngineStopStart;
			public bool EcoRollWithoutEngineStop;
			public bool EcorollWithEngineStop;
			public PredictiveCruiseControlType PCCType;
			public string ADASCombination;

		}

		public ADASCombination Lookup(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			return Lookup(
				adas.EngineStopStart, adas.EcoRollWitoutEngineStop, adas.EcoRollWithEngineStop, adas.PredictiveCruiseControl);
		}
	}
}
