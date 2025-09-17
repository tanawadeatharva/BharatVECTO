using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    public class TransmissionTypeWriter : AbstractMrfXmlType, IXmlTypeWriter, IXmlAxlePowertrainTypeWriter
	{
		public TransmissionTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IAxlePowertrainDeclarationInputData axlePt)
		{
            return GetElement(axlePt.GearboxInputData, axlePt.PTOTransmissionInputData);
        }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleComponents = inputData.JobInputData.Vehicle.Components;
            return GetElement(vehicleComponents.GearboxInputData, vehicleComponents.PTOTransmissionInputData);
		}
		
		private XElement GetElement(IGearboxDeclarationInputData gearboxInputData, IPTOTransmissionInputData ptoTransmissionInputData)
		{
            var result = new XElement(_mrf + XMLNames.Component_Transmission,
                new XElement(_mrf + XMLNames.Component_Model, gearboxInputData.Model),
                new XElement(_mrf + XMLNames.Component_CertificationNumber,
                    gearboxInputData.CertificationNumber),
                new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue,
                    gearboxInputData.DigestValue?.DigestValue ?? ""),
                new XElement(_mrf + XMLNames.Component_CertificationMethod, gearboxInputData.CertificationMethod.ToXMLFormat()),
                new XElement(_mrf + "Type",
                    gearboxInputData.Type.ToXMLFormat()),
                new XElement(_mrf + "NrOfGears", gearboxInputData.Gears.Count),
                new XElement(_mrf + "FinalGearRatio",
                    gearboxInputData.Gears.Last().Ratio.ToXMLFormat(3)),
                //new XElement(_mrf + XMLNames.Vehicle_RetarderType,
                //	vehicleComponents.RetarderInputData.Type.ToXMLFormat()),
                (ptoTransmissionInputData != null ? new XElement(_mrf + "PowerTakeOff",
                    ptoTransmissionInputData.PTOTransmissionType != "None") : null));
            return result;
        }

	}
}
