using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;

namespace TUGraz.VectoCore.OutputData.XML
{
	/// <summary>
	/// Create VIF of an interim (or the complete(d) step
	/// </summary>
	public class XMLDeclarationReportMultistageBusVehicle : XMLDeclarationReport
	{
		protected IXMLMultistepIntermediateReport _multistageBusReport;

		public XMLDeclarationReportMultistageBusVehicle(IReportWriter writer)
			: base(writer, null,null)
		{
			throw new NotImplementedException();
			//throw new VectoException("Used here");
			//_multistageBusReport = new XMLMultistageBusReport();
		}
		
		public override void InitializeReport(VectoRunData modelData)
		{
			_multistageBusReport =
				modelData.Exempted ? new XMLMultistageExemptedBusReport() : new XMLMultistageBusReport();
			_multistageBusReport.Initialize(modelData);
		}
		
		protected override void GenerateReports()
		{
			_multistageBusReport.GenerateReport();
		}
		
		protected override void OutputReports()
		{
			Writer.WriteReport(ReportType.DeclarationReportMultistageVehicleXML, _multistageBusReport.Report);
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
