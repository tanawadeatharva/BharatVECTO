using System;
using System.Xml.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;

namespace TUGraz.VectoCore.OutputData.XML {
	public class XMLDeclarationReportPrimaryVehicle : XMLDeclarationReport
	{
		protected IXMLVehicleInformationFile VehicleInformationFile;


		public XMLDeclarationReportPrimaryVehicle(IReportWriter writer) : base(writer, true)
		{
			throw new NotSupportedException("Use new implementation!");
		}

		public override XDocument CustomerReport => null;

		public override XDocument PrimaryVehicleReport => VehicleInformationFile?.Report;


		#region Overrides of XMLDeclarationReport

		protected override void InstantiateReports(VectoRunData modelData)
		{
			if (modelData.Exempted) {
				ManufacturerRpt = new XMLManufacturerReportExeptedPrimaryBus();
				CustomerRpt = new XMLCustomerReportExemptedPrimaryBus();
				VehicleInformationFile = new XMLExemptedPrimaryBusVehicleReport();

			} else {
				ManufacturerRpt = new XMLManufacturerReportPrimaryBus();
				CustomerRpt = new XMLCustomerReport();
				VehicleInformationFile = new XMLPrimaryBusVehicleReport();
			}


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
			CustomerRpt.GenerateReport(fullReportHash);
			VehicleInformationFile.GenerateReport(fullReportHash);
		}

	

		protected override void OutputReports()
		{
			Writer.WriteReport(ReportType.DeclarationReportManufacturerXML, ManufacturerRpt.Report);
			Writer.WriteReport(ReportType.DeclarationReportPrimaryVehicleXML, VehicleInformationFile.Report);
		}

		#endregion
	}
}