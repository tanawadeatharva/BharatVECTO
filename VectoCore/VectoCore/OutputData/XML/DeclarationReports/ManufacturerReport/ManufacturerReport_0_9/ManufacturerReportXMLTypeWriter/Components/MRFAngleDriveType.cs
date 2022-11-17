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
    internal class AngleDriveTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public AngleDriveTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var angleDriveData = inputData.JobInputData.Vehicle.Components.AngledriveInputData;
			if (angleDriveData == null || angleDriveData.Type == AngledriveType.None) {
				return null;
			}
			return new XElement(_mrf + XMLNames.Component_Angledrive,
				new XElement(_mrf + XMLNames.Component_Model, angleDriveData.Model),
				new XElement(_mrf + XMLNames.Component_CertificationNumber, angleDriveData.CertificationNumber),
				new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, angleDriveData.DigestValue?.DigestValue ?? ""),
				new XElement(_mrf + XMLNames.Component_CertificationMethod, angleDriveData.CertificationMethod.ToXMLFormat()),
				new XElement(_mrf + "AngledriveRatio", angleDriveData.Ratio.ToXMLFormat(3)));
		}

		#endregion
	}
}
