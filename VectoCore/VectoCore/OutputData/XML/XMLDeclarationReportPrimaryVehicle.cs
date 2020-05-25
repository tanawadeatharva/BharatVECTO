using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML {
	public class XMLDeclarationReportPrimaryVehicle : XMLDeclarationReport
	{
		private XMLPrimaryVehicleReport _primaryReport;


		public XMLDeclarationReportPrimaryVehicle(IReportWriter writer, bool writePIF = false) : base(writer)
		{
			_primaryReport = new XMLPrimaryVehicleReport();
		}

		public override XDocument CustomerReport
		{
			get { return null; }
		}

		public override XDocument PrimaryVehicleReport
		{
			get { return _primaryReport?.Report; }
		}

		
		#region Overrides of XMLDeclarationReport

		protected override void InstantiateReports(VectoRunData modelData)
		{
			ManufacturerRpt = new XMLManufacturerReportPrimaryBus();
			CustomerRpt = new XMLCustomerReport();
		}

		public override void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			base.InitializeReport(modelData, fuelModes);
			_primaryReport.Initialize(modelData,fuelModes);
		}



		protected override void WriteResult(ResultEntry result)
		{
			base.WriteResult(result);
			_primaryReport.WriteResult(result);
		}

		protected override void GenerateReports()
		{
			ManufacturerRpt.GenerateReport();
			var fullReportHash = GetSignature(ManufacturerRpt.Report);
			CustomerRpt.GenerateReport(fullReportHash);
			_primaryReport.GenerateReport(fullReportHash);
		}

	

		protected override void OutputReports()
		{
			Writer.WriteReport(ReportType.DeclarationReportManufacturerXML, ManufacturerRpt.Report);
			Writer.WriteReport(ReportType.DeclarationReportPrimaryVehicleXML, _primaryReport.Report);
		}

		#endregion
	}
}