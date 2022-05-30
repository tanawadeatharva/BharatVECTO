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
    internal class XMLDeclarationMockupReportCompletedVehicle : XMLDeclarationReportCompletedVehicle
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

			var arch = inputData.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID;

			inputData.JobInputData.PrimaryVehicle.Vehicle.VehicleCategory.GetVehicleType();// HEV/PEV - Sx/Px


			var ihpc = (inputData.JobInputData.PrimaryVehicle.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (inputData.JobInputData.PrimaryVehicle.Vehicle.Components.IEPC != null);
			ManufacturerRpt = _mrfFactory.GetManufacturerReport(
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

		protected internal override void DoWriteReport()
		{
			base.DoWriteReport();
		}

		#endregion

		#region Overrides of XMLDeclarationReport

		protected override void DoStoreResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			base.DoStoreResult(entry, runData, modData);
		}

		protected override void WriteResult(ResultEntry result)
		{
			base.WriteResult(result);
		}

		protected override void GenerateReports()
		{
			base.GenerateReports();
		}

		#endregion
	}
}
