using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    public class MRFTransmissionType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFTransmissionType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var vehicleComponents = inputData.JobInputData.Vehicle.Components;
			var result = new XElement(_mrf + XMLNames.Component_Transmission,
				new XElement(_mrf + XMLNames.Component_Model, vehicleComponents.GearboxInputData.Model),
				new XElement(_mrf + XMLNames.Component_CertificationNumber,
					vehicleComponents.GearboxInputData.CertificationNumber),
				new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue,
					vehicleComponents.GearboxInputData.DigestValue.DigestValue),
				new XElement(_mrf + XMLNames.Gearbox_TransmissionType,
					vehicleComponents.GearboxInputData.Type.ToXMLFormat()),
				new XElement(_mrf + "NrOfGears", vehicleComponents.GearboxInputData.Gears.Count),
				new XElement(_mrf + "FinalGearRatio",
					vehicleComponents.GearboxInputData.Gears.Last().Ratio.ToXMLFormat(3)),
				new XElement(_mrf + XMLNames.Vehicle_RetarderType,
					vehicleComponents.RetarderInputData.Type.ToXMLFormat()),
				(vehicleComponents.PTOTransmissionInputData != null ? new XElement(_mrf + "PowerTakeOff",
					vehicleComponents.PTOTransmissionInputData.PTOTransmissionType != "None") : null));
			return result;
		}


		#endregion
	}
}
