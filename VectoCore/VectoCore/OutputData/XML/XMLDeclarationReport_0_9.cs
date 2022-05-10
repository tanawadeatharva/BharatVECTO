using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLDeclarationReport09 : XMLDeclarationReport
	{
		private readonly IReportWriter _writer;
		private readonly IManufacturerReportFactory _mrfFactory;
		private readonly ICustomerInformationFileFactory _cifFactory;


		#region Implementation of IDeclarationReport

		public XMLDeclarationReport09(IReportWriter writer, IManufacturerReportFactory mrfFactory, ICustomerInformationFileFactory cifFactory) : base(writer)
		{
			_writer = writer;
			_mrfFactory = mrfFactory;
			_cifFactory = cifFactory;
		}

		protected override void InstantiateReports(VectoRunData modelData)
		{
			var vehicleData = modelData.VehicleData.InputData;
			var iepc = vehicleData.Components.IEPC != null;
			var ihpc =
				vehicleData.Components.ElectricMachines?.Entries?.Count(e => e.ElectricMachine.IHPCType != "None") > 0;

			ManufacturerRpt = _mrfFactory.GetManufacturerReport(vehicleData.VehicleCategory,
				vehicleData.VehicleType,
				vehicleData.ArchitectureID,
				vehicleData.ExemptedVehicle,
				iepc,
				ihpc);
			CustomerRpt = _cifFactory.GetCustomerReport(vehicleData.VehicleCategory,
				vehicleData.VehicleType,
				vehicleData.ArchitectureID,
				vehicleData.ExemptedVehicle,
				iepc,
				ihpc);
		}

		#region Overrides of XMLDeclarationReport

		protected override void InitializeReports(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes, IDeclarationInputDataProvider inputData)
		{
			var customerReport = CustomerRpt as AbstractCustomerReport;
			var manufacturerReport = ManufacturerRpt as AbstractManufacturerReport;
			customerReport.InitializeVehicleData(inputData);
			manufacturerReport.InitializeVehicleData(inputData);
		}

		#endregion

		#endregion
	}
}