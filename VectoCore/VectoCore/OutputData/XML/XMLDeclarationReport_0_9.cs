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

namespace TUGraz.VectoCore.OutputData.XML
{
	public interface IMockupReport
	{ 
		bool Mockup { set; }
	}
	public class XMLDeclarationReportPrimaryVehicle_09 : XMLDeclarationReportPrimaryVehicle, IMockupReport
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
		
		#region Implementation of IMockupReport

		public bool Mockup { get; set; }

		#endregion

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

		protected override void DoStoreResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			if (!Mockup)
			{
				base.DoStoreResult(entry, runData, modData);
				return;
			}
		}

		protected override void WriteResult(ResultEntry result)
		{

			if (Mockup)
			{

				(ManufacturerRpt as IXMLMockupReport).WriteMockupResult(result);
				(CustomerRpt as IXMLMockupReport).WriteMockupResult(result);

			}
			else
			{
				base.WriteResult(result);
			}
		}


	}


	public class XMLDeclarationReport09 : XMLDeclarationReport, IMockupReport
	{
		private readonly IManufacturerReportFactory _mrfFactory;
		private readonly ICustomerInformationFileFactory _cifFactory;


		#region Implementation of IDeclarationReport

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


		#region Overrides of XMLDeclarationReport


		protected override void WriteResult(ResultEntry result)
		{

			if (Mockup) {
				
				(ManufacturerRpt as IXMLMockupReport).WriteMockupResult(result);
				(CustomerRpt as IXMLMockupReport).WriteMockupResult(result);
				
				
			} else {
				base.WriteResult(result);
			}
		}

		#endregion

		#endregion



		#region Implementation of IMockupReport

		public bool Mockup { private get; set; }

		#endregion
	}

}