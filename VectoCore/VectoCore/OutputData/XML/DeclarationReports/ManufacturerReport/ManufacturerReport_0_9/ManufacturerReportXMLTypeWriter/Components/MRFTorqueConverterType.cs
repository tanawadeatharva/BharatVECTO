using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    public class TorqueConverterTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public TorqueConverterTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var torqueConverterInputData = inputData.JobInputData.Vehicle.Components.TorqueConverterInputData;
			if (torqueConverterInputData == null) {
				return null;
			}
			return new XElement(_mrf + XMLNames.Component_TorqueConverter,
				new XElement(_mrf + XMLNames.Component_Model, torqueConverterInputData.Model),
				new XElement(_mrf + XMLNames.Component_CertificationNumber,
					torqueConverterInputData.CertificationNumber),
				new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, torqueConverterInputData.DigestValue?.DigestValue ?? ""),
				new XElement(_mrf + XMLNames.Component_CertificationMethod,
					torqueConverterInputData.CertificationMethod.ToXMLFormat()));
		}

		#endregion
	}
}
