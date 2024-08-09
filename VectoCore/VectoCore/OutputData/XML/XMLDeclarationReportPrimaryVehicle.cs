using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.MonitoringReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCore.OutputData.XML
{
	/// <summary>
	/// Create MRF and VIF for primary bus
	/// </summary>
	public class XMLDeclarationReportPrimaryVehicle : XMLDeclarationReport
	{
		private readonly IVIFReportFactory _vifFactory;

		protected IXMLVehicleInformationFile VehicleInformationFile;


		public override XDocument CustomerReport => null;

		public override XDocument PrimaryVehicleReport => VehicleInformationFile?.Report;

		
		public XMLDeclarationReportPrimaryVehicle(IReportWriter writer,
			IManufacturerReportFactory mrfFactory,
			IVIFReportFactory vifFactory) : base(writer, mrfFactory, null)
		{
			_vifFactory = vifFactory;
		}
		


		#region Overrides of XMLDeclarationReport
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

			_monitoringReport = new XMLMonitoringReport(ManufacturerRpt);	
		}

		public override void InitializeReport(VectoRunData modelData)
		{
			base.InitializeReport(modelData);
			VehicleInformationFile.Initialize(modelData);
		}
		protected override void WriteResult(ResultEntry result)
		{
			base.WriteResult(result);
			VehicleInformationFile.WriteResult(result);
		}

		protected override void GenerateReports()
		{
			ManufacturerRpt.GenerateReport();
			var fullReportHash = GetSignature(ManufacturerRpt.Report);
			//CustomerRpt.GenerateReport(fullReportHash);
			VehicleInformationFile.GenerateReport(fullReportHash);
			_monitoringReport.GenerateReport();
		}



		protected override void OutputReports()
		{
			Writer.WriteReport(ReportType.DeclarationReportManufacturerXML, ManufacturerRpt.Report);
			Writer.WriteReport(ReportType.DeclarationReportPrimaryVehicleXML, VehicleInformationFile.Report);
			Writer.WriteReport(ReportType.DeclarationReportMonitoringXML, _monitoringReport.Report);
		}
		#endregion
	}
}