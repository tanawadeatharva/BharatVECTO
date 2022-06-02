using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoMockup.Reports
{
    public class XMLDeclarationMockupReportCompletedVehicle : XMLDeclarationReportCompletedVehicle
	{
		private readonly IManufacturerReportFactory _mrfFactory;
		private readonly ICustomerInformationFileFactory _cifFactory;

		public XMLDeclarationMockupReportCompletedVehicle(IReportWriter writer, IManufacturerReportFactory mrfFactory, ICustomerInformationFileFactory cifFactory, bool writePIF = false) : base(writer,
			writePIF)
		{
			_mrfFactory = mrfFactory;
			_cifFactory = cifFactory;
		}

		#region Overrides of XMLDeclarationReportCompletedVehicle

		protected override void InstantiateReports(VectoRunData modelData)
		{
			var inputData = modelData.InputData as IXMLMultistageInputDataProvider;


			var ihpc = (inputData.JobInputData.PrimaryVehicle.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (inputData.JobInputData.PrimaryVehicle.Vehicle.Components.IEPC != null);
			ManufacturerRpt = _mrfFactory.GetManufacturerReport(
				inputData.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory,
				inputData.JobInputData.JobType,
				inputData.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID,
				inputData.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle,
				iepc,
				ihpc);
			CustomerRpt = _cifFactory.GetCustomerReport(
				inputData.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory,
				inputData.JobInputData.JobType,
				inputData.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID,
				inputData.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle,
				iepc,
				ihpc);
			//base.InstantiateReports(modelData);
		}

		public override void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			base.InitializeReport(modelData, fuelModes);
		}

		#endregion

		#region Overrides of XMLDeclarationReport

		#region Overrides of XMLDeclarationReportCompletedVehicle

		protected internal override void DoWriteReport()
		{
			(ManufacturerRpt as IXMLMockupReport).WriteMockupSummary(Results.First());
            (CustomerRpt as IXMLMockupReport).WriteMockupSummary(Results.First());
			GenerateReports();
			if (Writer != null)
			{
				OutputReports();
			}
		}

		#endregion

		protected override void DoStoreResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			base.DoStoreResult(entry, runData, modData);
		}

		protected override void WriteResult(ResultEntry result)
		{
			(ManufacturerRpt as IXMLMockupReport).WriteMockupResult(result);
			(CustomerRpt as IXMLMockupReport).WriteMockupResult(result);
		}

		protected override void GenerateReports()
		{
			base.GenerateReports();	
		}

		protected override void OutputReports()
		{
			base.OutputReports();
		}



		#endregion
	}
}
