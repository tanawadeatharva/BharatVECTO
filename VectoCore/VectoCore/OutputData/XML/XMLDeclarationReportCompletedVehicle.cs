using System;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML {
	public class XMLDeclarationReportCompletedVehicle : XMLDeclarationReport
	{
		public XMLDeclarationReportCompletedVehicle(IReportWriter writer, bool writePIF = false) : base(writer) { }

		public IPrimaryVehicleInformationInputDataProvider PrimaryResults { get; set; }

		#region Overrides of XMLDeclarationReport

		protected override void InstantiateReports(VectoRunData modelData)
		{
			ManufacturerRpt = new XMLManufacturerReportCompletedBus();
			CustomerRpt = new XMLCustomerReportCompletedBus();
		}

		#endregion

		protected internal override void DoWriteReport()
		{
			foreach (var specificResult in Results.Where(x => VehicleClassHelper.IsCompletedBus(x.VehicleClass)).OrderBy(x => x.VehicleClass)
												.ThenBy(x => x.FuelMode).ThenBy(x => x.Mission)) {

				var genericResult = Results.First(x => x.VehicleClass.IsPrimaryBus() && x.FuelMode == specificResult.FuelMode &&
						x.Mission == specificResult.Mission && x.LoadingType == specificResult.LoadingType);
				var primaryResult = SelectPrimaryResult(genericResult);

				(ManufacturerRpt as XMLManufacturerReportCompletedBus).WriteResult(genericResult, specificResult, primaryResult);
				(CustomerRpt as XMLCustomerReportCompletedBus).WriteResult(genericResult, specificResult, primaryResult);
			}

			GenerateReports();

			if (Writer != null) {
				OutputReports();
			}
		}

		private IResult SelectPrimaryResult(ResultEntry genericResult)
		{
			var isDualModeEngine = Results.Select(x => x.FuelMode).Distinct().Count() > 1;
			var fuelMode = "single fuel mode";
			if (isDualModeEngine && genericResult.FuelMode > 0) {
				fuelMode = "dual fuel mode";
			}
			return PrimaryResults.ResultsInputData.Results.First(
				x => x.VehicleGroup == genericResult.VehicleClass &&
					(x.SimulationParameter.Payload - genericResult.Payload).IsEqual(0, 1) && x.Mission == genericResult.Mission &&
					x.SimulationParameter.FuelMode.Equals(fuelMode, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}