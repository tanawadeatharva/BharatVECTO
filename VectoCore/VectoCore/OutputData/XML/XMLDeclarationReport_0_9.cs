using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLDeclarationReportPrimaryVehicle_09 : XMLDeclarationReportPrimaryVehicle
	{
		private readonly ICustomerInformationFileFactory _cifFactory;
		private readonly IManufacturerReportFactory _mrfFactory;

		public XMLDeclarationReportPrimaryVehicle_09(IReportWriter writer,
			IManufacturerReportFactory mrfFactory,
			ICustomerInformationFileFactory cifFactory,
			bool writePIF = false) : base(writer, writePIF)
		{
			_mrfFactory = mrfFactory;
			_cifFactory = cifFactory;
		}




		protected override void InstantiateReports(VectoRunData modelData)
		{
			var vehicleData = modelData.VehicleData.InputData;
			var iepc = vehicleData.Components.IEPC != null;
			var ihpc =
				vehicleData.Components.ElectricMachines?.Entries?.Count(e => e.ElectricMachine.IHPCType != "None") > 0;

			if (modelData.Exempted) {
				throw new NotImplementedException();
			}
			PrimaryReport = new XMLPrimaryBusVehicleReport();
			
			//PrimaryRpt = _vifFactory.GetVIF(vehicleData.VehicleCategory,
			//	vehicleData.VehicleType,
			//	vehicleData.ArchitectureID,
			//	vehicleData.ExemptedVehicle,
			//	iepc,
			//	ihpc);


			ManufacturerRpt = _mrfFactory.GetManufacturerReport(vehicleData.VehicleCategory,
				vehicleData.VehicleType,
				vehicleData.ArchitectureID,
				vehicleData.ExemptedVehicle,
				iepc,
				ihpc);
		}

		protected override void DoStoreResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			base.DoStoreResult(entry, runData, modData);
		}

		protected override void WriteResult(ResultEntry result)
		{

			//if (Mockup)
			//{

			//	(ManufacturerRpt as IXMLMockupReport).WriteMockupResult(result);
			//	(CustomerRpt as IXMLMockupReport).WriteMockupResult(result);

			//}
			//else
			//{
				base.WriteResult(result);
			//}
		}


	}


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







	}

}