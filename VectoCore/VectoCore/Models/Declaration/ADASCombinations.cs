/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
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

		internal ADASCombination Lookup(VehicleData.ADASData adas)
		{
			return Lookup(
				adas.EngineStopStart, adas.EcoRollWithoutEngineStop, adas.EcoRollWithEngineStop, adas.PredictiveCruiseControl);
		}
	}
}
