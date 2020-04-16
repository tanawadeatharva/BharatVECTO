using System;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
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
			ManufacturerRpt = new XMLManufacturerReportCompletedBus() {
				PrimaryVehicle = PrimaryResults.Vehicle
			};
			CustomerRpt = new XMLCustomerReportCompletedBus();
		}

		#endregion

		protected internal override void DoWriteReport()
		{
			foreach (var specificResult in Results.Where(x => VehicleClassHelper.IsCompletedBus(x.VehicleClass)).OrderBy(x => x.VehicleClass)
												.ThenBy(x => x.FuelMode).ThenBy(x => x.Mission)) {

				var genericResult = Results.First(x => x.VehicleClass.IsPrimaryBus() && x.FuelMode == specificResult.FuelMode &&
						x.Mission == specificResult.Mission && x.LoadingType == specificResult.LoadingType);
				var primaryResult = genericResult.PrimaryResult ?? specificResult.PrimaryResult;
				if (primaryResult == null) {
					throw new VectoException(
						"no primary result entry set for simulation run vehicle class: {0}, mission: {1}, payload: {2}",
						genericResult.VehicleClass, genericResult.Mission, genericResult.Payload);
				}

				(ManufacturerRpt as XMLManufacturerReportCompletedBus).WriteResult(genericResult, specificResult, primaryResult);
				(CustomerRpt as XMLCustomerReportCompletedBus).WriteResult(genericResult, specificResult, primaryResult);
			}

			GenerateReports();

			if (Writer != null) {
				OutputReports();
			}
		}
	}
}