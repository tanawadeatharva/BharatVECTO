using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLDeclarationReport09 : IDeclarationReport
	{
		private readonly IReportWriter _writer;
		private readonly IManufacturerReportFactory _mrfFactory;
		private readonly ICustomerInformationFileFactory _cifFactory;

		private IXMLManufacturerReport _manufacturerReport;
		private IXMLCustomerReport _customerReport;

		#region Implementation of IDeclarationReport

		public XMLDeclarationReport09(IReportWriter writer, IManufacturerReportFactory mrfFactory, ICustomerInformationFileFactory cifFactory)
		{
			_writer = writer;
			_mrfFactory = mrfFactory;
			_cifFactory = cifFactory;
		}

		public void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			var vehicleData = modelData.VehicleData.InputData;
			var iepc = vehicleData.Components.IEPC != null;
			var ihpc =
				vehicleData.Components.ElectricMachines?.Entries?.Count(e => e.ElectricMachine.IHPCType != "None") > 0;

			_manufacturerReport = _mrfFactory.GetManufacturerReport(vehicleData.VehicleCategory,
				vehicleData.VehicleType,
				vehicleData.ArchitectureID,
				vehicleData.ExemptedVehicle,
				iepc,
				ihpc);
			_customerReport = _cifFactory.GetCustomerReport(vehicleData.VehicleCategory,
				vehicleData.VehicleType,
				vehicleData.ArchitectureID,
				vehicleData.ExemptedVehicle,
				iepc,
				ihpc);
		}

		public void PrepareResult(LoadingType loading, Mission mission, int fuelMode, VectoRunData runData)
		{
			throw new System.NotImplementedException();
		}

		public void AddResult(LoadingType loadingType, Mission mission, int fuelMode, VectoRunData runData,
			IModalDataContainer modData)
		{
			throw new System.NotImplementedException();
		}

		

		#endregion
	}
}