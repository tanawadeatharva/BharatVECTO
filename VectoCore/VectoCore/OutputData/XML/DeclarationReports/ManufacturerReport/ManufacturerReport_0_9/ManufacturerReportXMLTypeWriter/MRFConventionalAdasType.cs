using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{

	public class MRFConventionalAdasType : AbstractMrfXmlType
    {
		#region Overrides of AbstractMRFComponentWriter

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var adas = inputData.JobInputData.Vehicle.ADAS;
			return new XElement(_mrf + XMLNames.Vehicle_ADAS,
				new XElement(_mrf + XMLNames.Vehicle_ADAS_EngineStopStart, adas.EngineStopStart),
				new XElement(_mrf + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop,
					adas.EcoRoll == EcoRollType.WithoutEngineStop),
				new XElement(_mrf + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart,
					adas.EcoRoll == EcoRollType.WithEngineStop),
				new XElement(_mrf + XMLNames.Vehicle_ADAS_PCC,
					adas.PredictiveCruiseControl != PredictiveCruiseControlType.None));
		}

		#endregion

		public MRFConventionalAdasType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }
	}
}
