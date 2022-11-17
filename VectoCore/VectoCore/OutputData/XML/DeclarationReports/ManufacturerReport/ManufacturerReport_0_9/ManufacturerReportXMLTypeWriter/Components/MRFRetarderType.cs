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
    internal class RetarderTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public RetarderTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var retarderData = inputData.JobInputData.Vehicle.Components.RetarderInputData;
			var iepcData = inputData.JobInputData.Vehicle.Components.IEPC;
			var retarderPossible = iepcData == null || !iepcData.DesignTypeWheelMotor;
			if (retarderData == null || !retarderPossible) {
				return null;
			}
			var result = new XElement(_mrf + XMLNames.Component_Retarder,
                new XElement(_mrf + XMLNames.Vehicle_RetarderType,
                    retarderData.Type.ToXMLFormat()));
			if (retarderData.Type.IsDedicatedComponent()) {
				result.Add(new XElement(_mrf + XMLNames.Component_Model,
						inputData.JobInputData.Vehicle.Components.RetarderInputData.Model),
					new XElement(_mrf + XMLNames.Component_CertificationNumber, retarderData.CertificationNumber),
					new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue,
						retarderData.DigestValue?.DigestValue ?? ""),
					new XElement(_mrf + XMLNames.Component_CertificationMethod,
						retarderData.CertificationMethod.ToXMLFormat()));
			}

			return result;
		}

		#endregion
	}
}
