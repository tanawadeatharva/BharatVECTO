using System;
using System.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;

namespace TUGraz.VectoCore.OutputData.XML
{
	/// <summary>
	/// Create VIF of an interim (or the complete(d) step
	/// </summary>
	public class XMLDeclarationReportInterimVehicle : XMLDeclarationReport
	{
		protected readonly IVIFReportFactory _vifFactory;

		protected IXMLMultistepIntermediateReport MultistepIntermediateBusReport;
		protected readonly IVIFReportInterimFactory _interimFactory;

		public XMLDeclarationReportInterimVehicle(IReportWriter writer,
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
}