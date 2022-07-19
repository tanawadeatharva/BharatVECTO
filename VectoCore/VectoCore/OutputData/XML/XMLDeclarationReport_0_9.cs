using System.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;

namespace TUGraz.VectoCore.OutputData.XML
{
	/// <summary>
	/// Create MRF and VIF for primary bus
	/// </summary>
	public class XMLDeclarationReportPrimaryVehicle_09 : XMLDeclarationReportPrimaryVehicle
	{
				private readonly IManufacturerReportFactory _mrfFactory;
		private readonly IVIFReportFactory _vifFactory;

		public XMLDeclarationReportPrimaryVehicle_09(IReportWriter writer,
			IManufacturerReportFactory mrfFactory,
			ICustomerInformationFileFactory cifFactory,
			IVIFReportFactory vifFactory,
			bool writePIF = false) : base(writer, writePIF)
		{
			_mrfFactory = mrfFactory;
			//_cifFactory = cifFactory;
			_vifFactory = vifFactory;
		}




		protected override void InstantiateReports(VectoRunData modelData)
		{
			var vehicleData = modelData.VehicleData.InputData;
			var iepc = vehicleData.Components?.IEPC != null;
			var ihpc =
				vehicleData.Components?.ElectricMachines?.Entries?.Count(e => e.ElectricMachine.IHPCType != "None") > 0;

			//if (modelData.Exempted) {
			//	PrimaryReport = new XMLExemptedPrimaryBusVehicleReport();
			//}
			//PrimaryReport = new XMLPrimaryBusVehicleReport();
			
			ManufacturerRpt = _mrfFactory.GetManufacturerReport(vehicleData.VehicleCategory,
				vehicleData.VehicleType,
				vehicleData.ArchitectureID,
				vehicleData.ExemptedVehicle,
				iepc,
				ihpc);

			VehicleInformationFile = _vifFactory.GetVIFReport(vehicleData.VehicleCategory,
                vehicleData.VehicleType,
                vehicleData.ArchitectureID,
                vehicleData.ExemptedVehicle,
                iepc,
                ihpc);


		}

	}

	// --------------------------------------------------

	/// <summary>
	/// Create VIF
	/// </summary>
	//public class XMLDeclarationReportInterimVehicle_09 : XMLDeclarationReportMultistageBusVehicle
	//{

	//}

	// --------------------------------------------------

	/// <summary>
	/// Create MRF and CIF for lorries
	/// </summary>
	public class XMLDeclarationReport09 : XMLDeclarationReport
	{
		private readonly IManufacturerReportFactory _mrfFactory;
		private readonly ICustomerInformationFileFactory _cifFactory;



		public XMLDeclarationReport09(IReportWriter writer, IManufacturerReportFactory mrfFactory, ICustomerInformationFileFactory cifFactory) : base(writer)
		{
			_mrfFactory = mrfFactory;
			_cifFactory = cifFactory;
		}

		protected override void InstantiateReports(VectoRunData modelData)
		{
			var vehicleData = modelData.VehicleData.InputData;
			var iepc = vehicleData.Components?.IEPC != null;
			var ihpc =
				vehicleData.Components?.ElectricMachines?.Entries?.Count(e => e.ElectricMachine.IHPCType != "None") > 0;

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







	}

}