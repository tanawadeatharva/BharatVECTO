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
	internal class MRFPrimaryBusPneumaticSystemType : AbstractMrfXmlType
    {
		#region Implementation of IMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var busAux = inputData.JobInputData.Vehicle.Components.BusAuxiliaries;
			return new XElement(_mrf + XMLNames.BusAux_PneumaticSystem,
				new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, busAux.PneumaticSupply.CompressorSize),
				new XElement(_mrf + XMLNames.Bus_CompressorRatio, busAux.PneumaticSupply.Ratio.ToXMLFormat(3)),
				
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_SmartcompressionSystem, busAux.PneumaticSupply.SmartAirCompression),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem, busAux.PneumaticSupply.SmartAirCompression),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_AirsuspensionControl, busAux.PneumaticConsumers.AirsuspensionControl),
				new XElement(_mrf + "ReagentDosing", busAux.PneumaticConsumers.AdBlueDosing)
				);
		}

		#endregion

		public MRFPrimaryBusPneumaticSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }
	}
}
