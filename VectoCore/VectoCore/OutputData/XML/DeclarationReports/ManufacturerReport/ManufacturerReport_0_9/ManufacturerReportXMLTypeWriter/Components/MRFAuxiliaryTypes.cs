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
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
	public interface IMRFLorryAuxiliariesType
	{
		XElement GetXmlType(IAuxiliariesDeclarationInputData auxData);
	}

	public interface IMRFBusAuxiliariesType
	{
		XElement GetXmlType(IBusAuxiliariesDeclarationData auxData);
	}


	internal class MRFConventionalLorryAuxiliariesType : AbstractMrfXmlType, IMRFLorryAuxiliariesType
    {
		public MRFConventionalLorryAuxiliariesType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of MRFLorryAuxiliariesType

		public XElement GetXmlType(IAuxiliariesDeclarationInputData auxData)
		{
			var fanData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.Fan);
			var steeringPumpData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.SteeringPump);
			var electricSystemData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.ElectricSystem);
			var pneumaticSystemData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.PneumaticSystem);

			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				new XElement(_mrf + "CoolingFanTechnology",
					string.Join("\n", fanData.Technology)),
				new XElement(_mrf + "SteeringPumpTechnology", string.Join("\n", steeringPumpData.Technology)),
				new XElement(_mrf + XMLNames.BusAux_ElectricSystem, new XElement(_mrf + "LEDHeadLights", electricSystemData.Technology.Contains("Standard technology - LED headlights, all"))),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem, new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, string.Join("\n", pneumaticSystemData.Technology))));
		}
	}

	#endregion

	internal class MRFHEV_LorryAuxiliariesType : MRFConventionalLorryAuxiliariesType
	{
		public MRFHEV_LorryAuxiliariesType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

	}

	internal class MRFConventional_PrimaryBusAuxType : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFConventional_PrimaryBusAuxType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }


		#region Implementation of MRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			var steeringPumpData = auxData.SteeringPumpTechnology;

			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				new XElement(_mrf + "CoolingFanTechnology",
					auxData.FanTechnology),
				new XElement(_mrf + "SteeringPumpTechnology", string.Join("\n", steeringPumpData)),
				_mrfFactory.GetPrimaryBusElectricSystemType().GetXmlType(auxData),
				_mrfFactory.GetPrimaryBusPneumaticSystemType().GetXmlType(auxData),
				_mrfFactory.GetPrimaryBusHVACSystemType().GetXmlType(auxData)
			);
		}

		#endregion
	}

	internal class MRFHEV_PrimaryBusAuxType : MRFConventional_PrimaryBusAuxType
	{
		public MRFHEV_PrimaryBusAuxType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }
	}



	public class MRFPrimaryBusHVACSystemType : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusHVACSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			var hvac = auxData.HVACAux;
			return new XElement(_mrf + "HVACSystem",
				new XElement(_mrf + XMLNames.Bus_AdjustableCoolantThermostat, hvac.AdjustableCoolantThermostat),
				new XElement(_mrf + XMLNames.Bus_EngineWasteGasHeatExchanger, hvac.EngineWasteGasHeatExchanger));
		}

		#endregion
	}

	internal class MRFConventionalCompletedBusAuxType : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFConventionalCompletedBusAuxType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var input = inputData as IMultistageBusInputDataProvider;
			var auxData = input.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries;
			var result = new XElement(_mrf + XMLNames.Component_Auxiliaries,
				new XElement(_mrf + XMLNames.BusAux_ElectricSystem,
					new XElement(_mrf + "DayRunningLightsLED", auxData.ElectricConsumers.DayrunninglightsLED),
					new XElement(_mrf + "HeadLightsLED", auxData.ElectricConsumers.HeadlightsLED),
					new XElement(_mrf + "PositionLightsLED", auxData.ElectricConsumers.PositionlightsLED),
					new XElement(_mrf + "BrakeLightsLED", auxData.ElectricConsumers.BrakelightsLED),
					new XElement(_mrf + "InteriorLightsLED", auxData.ElectricConsumers.InteriorLightsLED))
			);






			return result;
		}

		#endregion

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}


	internal class MRFCompletedBusElectricSystemType : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFCompletedBusElectricSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + XMLNames.BusAux_ElectricSystem,
				new XElement(_mrf + "DayRunningLightsLED", auxData.ElectricConsumers.DayrunninglightsLED),
				new XElement(_mrf + "HeadLightsLED", auxData.ElectricConsumers.HeadlightsLED),
				new XElement(_mrf + "PositionLightsLED", auxData.ElectricConsumers.PositionlightsLED),
				new XElement(_mrf + "BrakeLightsLED", auxData.ElectricConsumers.BrakelightsLED),
				new XElement(_mrf + "InteriorLightsLED", auxData.ElectricConsumers.InteriorLightsLED));
		}

		#endregion
	}

	internal class MRFConventionalCompletedBus_HVACSystemType : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFConventionalCompletedBus_HVACSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + "HVACSystem",
				_mrfFactory.GetCompletedBus_HVACSystemGroup().GetElements(auxData));
		}

		#endregion
	}

	internal class MRFPrimaryBusElectricSystemType : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusElectricSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }


		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + XMLNames.BusAux_ElectricSystem,
				new XElement(_mrf + XMLNames.BusAux_ElectricSystem_AlternatorTechnology,
					auxData.ElectricSupply.AlternatorTechnology.ToXMLFormat()),
				auxData.ElectricSupply.AlternatorTechnology != AlternatorType.None ? "TODO" : null,
				auxData.ElectricSupply.ElectricStorage != null ? "TODO" : null);
		}

		#endregion
	}

	internal class MRFPrimaryBusPneumaticSystemType : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusPneumaticSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + XMLNames.BusAux_PneumaticSystem,
				new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, auxData.PneumaticSupply.CompressorSize),
				new XElement(_mrf + XMLNames.Bus_CompressorRatio, auxData.PneumaticSupply.Ratio.ToXMLFormat(3)),

				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_SmartcompressionSystem, auxData.PneumaticSupply.SmartAirCompression),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem, auxData.PneumaticSupply.SmartAirCompression),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_AirsuspensionControl, auxData.PneumaticConsumers.AirsuspensionControl),
				new XElement(_mrf + "ReagentDosing", auxData.PneumaticConsumers.AdBlueDosing)
			);
		}

		#endregion
	}
}
