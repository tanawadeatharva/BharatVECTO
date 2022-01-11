using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    internal class MRFAngleDriveType : AbstractMrfXmlType
    {
		public MRFAngleDriveType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var angleDriveData = inputData.JobInputData.Vehicle.Components.AngledriveInputData;
			return new XElement(_mrf + XMLNames.Component_Angledrive,
				new XElement(_mrf + XMLNames.Component_Model, angleDriveData.Model),
				new XElement(_mrf + XMLNames.Component_CertificationNumber, angleDriveData.CertificationNumber),
				new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, angleDriveData.DigestValue.DigestValue),
				new XElement(_mrf + XMLNames.Component_CertificationMethod, angleDriveData.CertificationMethod),
				new XElement(_mrf + XMLNames.AngleDrive_Ratio, angleDriveData.Ratio.ToXMLFormat(3)));
		}

		#endregion
	}
}
