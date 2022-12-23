using System;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;

namespace TUGraz.VectoCore.OutputData.XML
{
	/// <summary>
	/// Create MRF and VIF for primary bus
	/// </summary>
	public class XMLDeclarationReportPrimaryVehicle_09 : XMLDeclarationReport09
	{
		private readonly IManufacturerReportFactory _mrfFactory;
		private readonly IVIFReportFactory _vifFactory;

		protected IXMLVehicleInformationFile VehicleInformationFile;

		public override XDocument CustomerReport => null;

		public override XDocument PrimaryVehicleReport => VehicleInformationFile?.Report;

		public XMLDeclarationReportPrimaryVehicle_09(IReportWriter writer,
			IManufacturerReportFactory mrfFactory,
			ICustomerInformationFileFactory cifFactory,
			IVIFReportFactory vifFactory) : base(writer, mrfFactory, cifFactory)
		{
			_mrfFactory = mrfFactory;
			//_cifFactory = cifFactory;
			_vifFactory = vifFactory;
		}

		public override void InitializeReport(VectoRunData modelData)
		{
			base.InitializeReport(modelData);
			VehicleInformationFile.Initialize(modelData);
		}



		protected override void WriteResult(ResultEntry result)
		{
			ManufacturerRpt.WriteResult(result);
			//base.WriteResult(result);
			VehicleInformationFile.WriteResult(result);
		}

		protected override void GenerateReports()
		{
			ManufacturerRpt.GenerateReport();
			var fullReportHash = GetSignature(ManufacturerRpt.Report);
			//CustomerRpt.GenerateReport(fullReportHash);
			VehicleInformationFile.GenerateReport(fullReportHash);
		}



		protected override void OutputReports()
		{
			Writer.WriteReport(ReportType.DeclarationReportManufacturerXML, ManufacturerRpt.Report);
			Writer.WriteReport(ReportType.DeclarationReportPrimaryVehicleXML, VehicleInformationFile.Report);
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
    /// Create VIF of an interim (or the complete(d) step
    /// </summary>
    public class XMLDeclarationReportInterimVehicle_09 : XMLDeclarationReport09
	{
		protected readonly IVIFReportFactory _vifFactory;

		protected IXMLMultistepIntermediateReport MultistepIntermediateBusReport;
		protected readonly IVIFReportInterimFactory _interimFactory;

		public XMLDeclarationReportInterimVehicle_09(IReportWriter writer,
			IManufacturerReportFactory mrfFactory,
			ICustomerInformationFileFactory cifFactory,
			IVIFReportFactory vifFactory, IVIFReportInterimFactory interimFactory) : base(writer, mrfFactory, cifFactory)
		{
			_vifFactory = vifFactory;
			_interimFactory = interimFactory;
		}

		#region Overrides of XMLDeclarationReport

		protected override void InstantiateReports(VectoRunData modelData)
		{
			var vehicleData = modelData.VehicleData.InputData;
			var iepc = vehicleData.Components?.IEPC != null;
			var ihpc =
				vehicleData.Components?.ElectricMachines?.Entries?.Count(e => e.ElectricMachine.IHPCType != "None") > 0;

			MultistepIntermediateBusReport = _interimFactory.GetInterimVIFReport(vehicleData.VehicleCategory,
				vehicleData.VehicleType,
				vehicleData.ArchitectureID,
				vehicleData.ExemptedVehicle,
				iepc,
				ihpc);
		}

		#endregion

		public override void InitializeReport(VectoRunData modelData)
		{
			//_multistageBusReport =
			//	modelData.Exempted ? new XMLMultistageExemptedBusReport() : new XMLMultistageBusReport();
			
			InstantiateReports(modelData);

			MultistepIntermediateBusReport.Initialize(modelData);
		}

		protected override void GenerateReports()
		{
			MultistepIntermediateBusReport.GenerateReport();
		}

		protected override void OutputReports()
		{
			Writer.WriteReport(ReportType.DeclarationReportMultistageVehicleXML, MultistepIntermediateBusReport.Report);
		}

		protected override void DoStoreResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			throw new NotSupportedException();
		}
		protected override void WriteResult(ResultEntry result)
		{
			throw new NotSupportedException();
		}
	}


	// --------------------------------------------------

	/// <summary>
	/// Create MRF and CIF of the complete(d) step
	/// </summary>
	public class XMLDeclarationReportCompletedVehicle_09 : XMLDeclarationReportCompletedVehicle
	{
		protected readonly ICustomerInformationFileFactory _cifFactory;
		protected readonly IManufacturerReportFactory _mrfFactory;

		public XMLDeclarationReportCompletedVehicle_09(IReportWriter writer, IManufacturerReportFactory mrfFactory,
			ICustomerInformationFileFactory cifFactory,
			IVIFReportFactory vifFactory) : base(writer)
		{
            _cifFactory = cifFactory;
			_mrfFactory = mrfFactory;
		}

		#region Overrides of XMLDeclarationReportCompletedVehicle

		protected override void InstantiateReports(VectoRunData modelData)
		{
			var inputData = modelData.InputData as IXMLMultistageInputDataProvider;
			var primaryVehicle = inputData.JobInputData.PrimaryVehicle.Vehicle;

			var ihpc = (primaryVehicle.Components?.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (primaryVehicle.Components?.IEPC != null);
			ManufacturerRpt = _mrfFactory.GetManufacturerReport(
				inputData.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory,
				inputData.JobInputData.JobType,
				primaryVehicle.ArchitectureID,
				primaryVehicle.ExemptedVehicle,
				iepc,
				ihpc);

			CustomerRpt = _cifFactory.GetCustomerReport(
				inputData.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory,
				inputData.JobInputData.JobType,
				primaryVehicle.ArchitectureID,
				primaryVehicle.ExemptedVehicle,
				iepc,
				ihpc);

		}

		#endregion
	}
	// --------------------------------------------------

	/// <summary>
	/// Create MRF and CIF for lorries
	/// </summary>
	public class XMLDeclarationReport09 : XMLDeclarationReport
	{
		protected readonly IManufacturerReportFactory _mrfFactory;
		protected readonly ICustomerInformationFileFactory _cifFactory;

		public XMLDeclarationReport09(IReportWriter writer, IManufacturerReportFactory mrfFactory, ICustomerInformationFileFactory cifFactory) : base(writer, true)
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


	public class XMLDeclarationReportSingleBus09 : XMLDeclarationReport09
	{
		
		public XMLDeclarationReportSingleBus09(IReportWriter writer, IManufacturerReportFactory mrfFactory, ICustomerInformationFileFactory cifFactory) : base(writer, mrfFactory, cifFactory)
		{ }

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
			// do not instantiate customer report for single bus - not fully implemented, not needed in the final application
		}

	}

}