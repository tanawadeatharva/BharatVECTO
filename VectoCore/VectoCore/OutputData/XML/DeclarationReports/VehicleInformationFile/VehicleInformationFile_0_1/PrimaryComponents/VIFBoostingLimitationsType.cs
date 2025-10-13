using System.Data;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFBoostingLimitationsType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFBoostingLimitationsType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var boostingLimitations = inputData.JobInputData.Vehicle.BoostingLimitations;
			if (boostingLimitations == null)
				return null;
			
			var boostingLimitationsXElement = new XElement(_vif + XMLNames.Vehicle_BoostingLimitation,
				new XAttribute(_xsi + XMLNames.XSIType, "BoostingLimitationsType"),
				new XAttribute("xmlns", _v27));
			foreach (DataRow row in boostingLimitations.Rows)
			{
				boostingLimitationsXElement.Add(new XElement(_v27 + XMLNames.BoostingLimitation_Entry,
					new XAttribute(XMLNames.BoostingLimitation_RotationalSpeed, row[MaxBoostingTorqueReader.Fields.MotorSpeed]),
					new XAttribute(XMLNames.BoostingLimitation_BoostingTorque, row[MaxBoostingTorqueReader.Fields.DrivingTorque])
				));
			}
			
			return boostingLimitationsXElement;
		}
		
		#endregion
	}
}
