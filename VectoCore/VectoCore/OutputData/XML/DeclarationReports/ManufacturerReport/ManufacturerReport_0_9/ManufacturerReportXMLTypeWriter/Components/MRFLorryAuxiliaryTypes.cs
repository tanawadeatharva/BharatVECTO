using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
	public interface IMRFLorryAuxiliariesType
	{
		XElement GetXmlType(IAuxiliariesDeclarationInputData auxData);
	}
	internal class MRFConventionalLorryAuxiliariesType : AbstractMrfXmlType, IMRFLorryAuxiliariesType
	{
		public MRFConventionalLorryAuxiliariesType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }
		public XElement GetXmlType(IAuxiliariesDeclarationInputData auxData)
		{
			var fanData = auxData.Auxiliaries.SingleOrDefault(aux => aux.Type == AuxiliaryType.Fan);
			var steeringPumpData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.SteeringPump);
			var electricSystemData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.ElectricSystem);
			var pneumaticSystemData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.PneumaticSystem);

			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				fanData == null ? null : new XElement(_mrf + "CoolingFanTechnology", fanData.Technology.Single()),
				steeringPumpData.Technology.Select(x => new XElement(_mrf + "SteeringPumpTechnology", x)),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem, new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, pneumaticSystemData.Technology.Single())),
				new XElement(_mrf + XMLNames.BusAux_ElectricSystem, new XElement(_mrf + "LEDHeadLights", electricSystemData.Technology.Contains("Standard technology - LED headlights, all"))));
		}
	}


	internal class MRFHEV_LorryAuxiliariesType : MRFConventionalLorryAuxiliariesType
	{
		public MRFHEV_LorryAuxiliariesType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

	}

	internal class MRFFCHV_LorryAuxiliariesType : AbstractMrfXmlType, IMRFLorryAuxiliariesType
	{
		public MRFFCHV_LorryAuxiliariesType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetXmlType(IAuxiliariesDeclarationInputData auxData)
		{
			var steeringPumpData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.SteeringPump);
			var pneumaticSystemData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.PneumaticSystem);

			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				steeringPumpData.Technology.Select(x => new XElement(_mrf + "SteeringPumpTechnology", x)),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem, new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, pneumaticSystemData.Technology.Single())));
		}
	}

	internal class MRFPEV_LorryAuxiliariesType : AbstractMrfXmlType, IMRFLorryAuxiliariesType
	{
		public MRFPEV_LorryAuxiliariesType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFLorryAuxiliariesType

		public XElement GetXmlType(IAuxiliariesDeclarationInputData auxData)
		{
			
			var steeringPumpData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.SteeringPump);
			var electricSystemData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.ElectricSystem);
			var pneumaticSystemData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.PneumaticSystem);

			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				steeringPumpData.Technology.Select(x => new XElement(_mrf + "SteeringPumpTechnology", x)),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem, new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, pneumaticSystemData.Technology.Single())),
				new XElement(_mrf + XMLNames.BusAux_ElectricSystem, new XElement(_mrf + "LEDHeadLights", electricSystemData.Technology.Contains("Standard technology - LED headlights, all"))));
		}

		#endregion
	}
}