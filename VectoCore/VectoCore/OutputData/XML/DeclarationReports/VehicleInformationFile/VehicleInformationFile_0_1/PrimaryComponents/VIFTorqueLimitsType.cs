using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
    public class VIFTorqueLimitsType : AbstractVIFXmlType, IXmlTypeWriter
    {
		public VIFTorqueLimitsType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			if (inputData.JobInputData.Vehicle.TorqueLimits == null ||
				inputData.JobInputData.Vehicle.TorqueLimits.Count == 0)
				return null;

			var torqueLimits = inputData.JobInputData.Vehicle.TorqueLimits;

			return new XElement(_vif + XMLNames.Vehicle_TorqueLimits,
				new XAttribute(_xsi + XMLNames.XSIType, "TorqueLimitsType"),
				new XAttribute("xmlns", _v20.NamespaceName),
				torqueLimits
					.OrderBy(x => x.Gear)
					.Select(entry => new XElement(_v20 + XMLNames.Vehicle_TorqueLimits_Entry,
							new XAttribute(XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr, entry.Gear),
							new XAttribute(XMLNames.Vehicle_TorqueLimits_Entry_MaxTorque_Attr, entry.MaxTorque.ToXMLFormat(0))
						)
					));
		}

		#endregion
	}
}
