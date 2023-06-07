using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    internal class AxleGearTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public AxleGearTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var axleGearInputData = inputData.JobInputData.Vehicle.Components.AxleGearInputData;
			return new XElement(_mrf + XMLNames.Component_Axlegear,
				new XElement(_mrf + XMLNames.Component_Model, axleGearInputData.Model),
				new XElement(_mrf + XMLNames.Component_CertificationNumber, axleGearInputData.CertificationNumber),
				new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue,
					axleGearInputData.DigestValue?.DigestValue ?? ""),
				new XElement(_mrf + XMLNames.Component_CertificationMethod,
					axleGearInputData.CertificationMethod.ToXMLFormat()),
				new XElement(_mrf + "AxleType", axleGearInputData.LineType.ToXMLFormat()),
				new XElement(_mrf + XMLNames.Axlegear_Ratio, axleGearInputData.Ratio.ToXMLFormat(3)));
		}

		#endregion
	}
}
