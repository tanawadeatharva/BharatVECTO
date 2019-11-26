using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration {
	public sealed class SteeringPumpBus : IDeclarationAuxiliaryTable
	{
		private readonly SteeringPumpBaseLine _baseLine = new SteeringPumpBaseLine();

		private readonly SteeringTubingFactors _tubingFactors = new SteeringTubingFactors();

		private readonly SteeringAxleFactors _axleFactors = new SteeringAxleFactors();

		public Watt LookupMechanicalPowerDemand(MissionType mission, IList<string> technologies, Meter vehicleLength)
		{
			return LookupPowerDemand(mission, technologies, vehicleLength, false);
		}

		public Watt LookupElectricalPowerDemand(MissionType mission, IList<string> technologies, Meter vehicleLength)
		{
			return LookupPowerDemand(mission, technologies, vehicleLength, true);
		}

		public Watt LookupPowerDemand(MissionType mission, IList<string> technologies, Meter vehicleLength, bool electrical)
		{
			var powerDemand = 0.SI<Watt>();

			for (var i = 0; i < technologies.Count; i++) {
				var techLookup = _tubingFactors.Lookup(technologies[i], mission);
				if (techLookup.Value.Item1 != electrical) {
					continue;
				}
				var baseDemand = _baseLine.Lookup(mission, i + 1).Value;
				var powerDemandTubing = (Constants.BusParameters.Auxiliaries.SteeringPump.TubingLoss * 2 *
										(vehicleLength - Constants.BusParameters.Auxiliaries.SteeringPump.LengthBonus) *
										Constants.BusParameters.Auxiliaries.SteeringPump.VolumeFlow).Cast<Watt>();
				var tubingFactor = i == 0 ? techLookup.Value.Item2 : 1.0;
				var axleFactor = i == 0 ? 1.0 : _axleFactors.Lookup(technologies[i], mission).Value;

				powerDemand += (baseDemand + powerDemandTubing) * tubingFactor * axleFactor;
			}

			return powerDemand;
		}

		#region Implementation of IDeclarationAuxiliaryTable

		public string[] GetTechnologies()
		{
			return _tubingFactors.GetTechnologies();
		}

		#endregion

		private sealed class SteeringPumpBaseLine : LookupData<MissionType, int, LookupValues<Watt>>
		{
			#region Overrides of LookupData

			protected override string ResourceId { get { return DeclarationData.DeclarationDataResourcePrefix + ".VAUXBus.SP-Axles.csv"; } }
			protected override string ErrorMessage { get { return "Auxiliary Lookup Error: No value found for Steering Pump. Mission: '{0}', HDVClass: '{1}'"; } }
			protected override void ParseData(DataTable table)
			{
				foreach (DataRow row in table.Rows) {
					var axleNumber = int.Parse(row.Field<string>("axlenbr"));
					foreach (DataColumn col in table.Columns) {
						if (col.Caption == "axlenbr" || string.IsNullOrWhiteSpace(row.Field<string>(col.Caption))) {
							continue;
						}

						Data[Tuple.Create(col.Caption.ParseEnum<MissionType>(), axleNumber)] =
							new LookupValues<Watt> { Value = row.ParseDouble(col.Caption).SI<Watt>() };
					}
				}
			}

			#endregion
		}

		private sealed class SteeringTubingFactors : LookupData<string, MissionType, LookupValues<Tuple<bool, double>>>, IDeclarationAuxiliaryTable
		{
			#region Overrides of LookupData

			protected override string ResourceId { get { return DeclarationData.DeclarationDataResourcePrefix + ".VAUXBus.SP-TubingFactor.csv"; } }
			protected override string ErrorMessage { get { return "Auxiliary Lookup Error: No value found for Steering Pump. Mission: '{0}', HDVClass: '{1}'"; } }
			protected override void ParseData(DataTable table)
			{
				foreach (DataRow row in table.Rows) {
					var axleNumber = row.Field<string>("technology");
					foreach (DataColumn col in table.Columns) {
						if (col.Caption == "technology" || col.Caption == "fullyelectric" || string.IsNullOrWhiteSpace(row.Field<string>(col.Caption))) {
							continue;
						}

						Data[Tuple.Create(axleNumber, col.Caption.ParseEnum<MissionType>())] =
							new LookupValues<Tuple<bool,double>> { Value = Tuple.Create(!row.Field<string>("fullyelectric").Equals("0"), row.ParseDouble(col.Caption)) };
					}
				}
			}

			#endregion

			#region Implementation of IDeclarationAuxiliaryTable

			public string[] GetTechnologies()
			{
				return Data.Keys.Select(x => x.Item1).Distinct().ToArray();
			}

			#endregion
		}

		private sealed class SteeringAxleFactors : LookupData<string, MissionType, LookupValues<double>>
		{
			#region Overrides of LookupData

			protected override string ResourceId { get { return DeclarationData.DeclarationDataResourcePrefix + ".VAUXBus.SP-AxleFactor.csv"; } }
			protected override string ErrorMessage { get { return "Auxiliary Lookup Error: No value found for Steering Pump. Mission: '{0}', HDVClass: '{1}'"; } }
			protected override void ParseData(DataTable table)
			{
				foreach (DataRow row in table.Rows) {
					var axleNumber = row.Field<string>("technology");
					foreach (DataColumn col in table.Columns) {
						if (col.Caption == "technology" || string.IsNullOrWhiteSpace(row.Field<string>(col.Caption))) {
							continue;
						}

						Data[Tuple.Create(axleNumber, col.Caption.ParseEnum<MissionType>())] =
							new LookupValues<double> { Value = row.ParseDouble(col.Caption) };
					}
				}
			}

			#endregion
		}

		private struct LookupValues<T>
		{
			public T Value;
		}
	}
}