using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    internal class RetarderTypeWriter : AbstractMrfXmlType, IXmlTypeWriter, IXmlAxlePowertrainTypeWriter
	{
		public RetarderTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return GetElement(inputData.JobInputData.Vehicle.Components.RetarderInputData);
		}

        public XElement GetElement(IAxlePowertrainDeclarationInputData axlePt)
        {
            return GetElement(axlePt.RetarderInputData);
        }

        private XElement GetElement(IRetarderInputData retarderData)
		{
            if (retarderData == null)
            {
                return null;
            }

            var result = new XElement(_mrf + XMLNames.Component_Retarder,
                new XElement(_mrf + XMLNames.Vehicle_RetarderType, retarderData.Type.ToXMLFormat()));

            if (retarderData.Type.IsDedicatedComponent())
            {
                result.Add(
                    new XElement(_mrf + XMLNames.Component_Model, retarderData.Model),
                    new XElement(_mrf + XMLNames.Component_CertificationNumber, retarderData.CertificationNumber),
                    new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, retarderData.DigestValue?.DigestValue ?? ""),
                    new XElement(_mrf + XMLNames.Component_CertificationMethod, retarderData.CertificationMethod.ToXMLFormat()));
            }

            return result;
        }

	}
}
