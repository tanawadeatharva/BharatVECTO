using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    public class AxleWheelsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public AxleWheelsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var result = new XElement(_mrf + XMLNames.Component_AxleWheels);
			var axleWheelsInputdata = inputData.JobInputData.Vehicle.Components.AxleWheels;
			var axleCount = 0;
			foreach (var axleDeclaration in axleWheelsInputdata.AxlesDeclaration) {
				var axle = new XElement(_mrf + XMLNames.AxleWheels_Axles_Axle,
					new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, ++axleCount));

				if (axleCount != 1) {
					axle.Add(new XElement(_mrf + XMLNames.AxleWheels_Axles_Axle_TwinTyres, axleDeclaration.TwinTyres));
				}
				axle.Add(new XElement(_mrf + XMLNames.AxleWheels_Axles_Axle_Tyre, 
					new XElement(_mrf + XMLNames.Report_Tyre_TyreDimension, axleDeclaration.Tyre.Dimension),
					new XElement(_mrf + XMLNames.Component_CertificationNumber, axleDeclaration.Tyre.CertificationNumber),
					new XElement(_mrf + "SpecificRRC", axleDeclaration.Tyre.RollResistanceCoefficient.ToXMLFormat(4)),
					new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, axleDeclaration.Tyre.DigestValue?.DigestValue ?? "")));

				if (axleDeclaration.WheelEndFriction != null)
				{
					axle.Add(new XElement(_mrf + "WheelEnd",
						new XElement(_mrf + "WheelEndFriction", axleDeclaration.WheelEndFriction.ToXMLFormat(1)),
						new XElement(_mrf + "CertificationNumber", axleDeclaration.WheelEndCertificationNumber)));
				}

				result.Add(axle);
			}

			return result;
		}

		#endregion
	}
}
