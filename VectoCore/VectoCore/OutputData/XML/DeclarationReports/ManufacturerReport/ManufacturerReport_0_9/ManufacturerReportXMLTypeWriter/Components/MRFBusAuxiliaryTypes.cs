using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{


	public interface IMRFBusAuxiliariesType
	{
		XElement GetXmlType(IBusAuxiliariesDeclarationData auxData);
	}




	internal class MRFPrimaryBusAuxType_Conventional : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusAuxType_Conventional(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }


		#region Implementation of MRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			var steeringPumpData = auxData.SteeringPumpTechnology;

			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				new XElement(_mrf + "CoolingFanTechnology",
					auxData.FanTechnology),
				new XElement(_mrf + "SteeringPumpTechnology", string.Join("\n", steeringPumpData)),


				_mrfFactory.GetPrimaryBusElectricSystemType_Conventional_HEV().GetXmlType(auxData),
				_mrfFactory.GetPrimaryBusPneumaticSystemType_Conventional_HEV_Px().GetXmlType(auxData),
				_mrfFactory.GetPrimaryBusHVACSystemType_Conventional_HEV().GetXmlType(auxData)
			);
		}

		#endregion
	}

	internal class MRFPrimaryBusAuxType_HEV_P : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusAuxType_HEV_P(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			var steeringPumpData = auxData.SteeringPumpTechnology;

			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				new XElement(_mrf + "CoolingFanTechnology",
					auxData.FanTechnology),
				new XElement(_mrf + "SteeringPumpTechnology", string.Join("\n", steeringPumpData)),
				_mrfFactory.GetPrimaryBusElectricSystemType_Conventional_HEV().GetXmlType(auxData),
				_mrfFactory.GetPrimaryBusPneumaticSystemType_Conventional_HEV_Px().GetXmlType(auxData),
				_mrfFactory.GetPrimaryBusHVACSystemType_Conventional_HEV().GetXmlType(auxData)
			);
		}

		#endregion
	}
	internal class MRFPrimaryBusAuxType_HEV_S : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusAuxType_HEV_S(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			var steeringPumpData = auxData.SteeringPumpTechnology;

			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				new XElement(_mrf + "CoolingFanTechnology",
					auxData.FanTechnology),
				new XElement(_mrf + "SteeringPumpTechnology", string.Join("\n", steeringPumpData)),
				_mrfFactory.GetPrimaryBusElectricSystemType_Conventional_HEV().GetXmlType(auxData),

				_mrfFactory.GetPrimaryBusPneumaticSystemType_HEV_S().GetXmlType(auxData),
				_mrfFactory.GetPrimaryBusHVACSystemType_Conventional_HEV().GetXmlType(auxData)
			);
		}

		#endregion
	}

	internal class MRFPrimaryBusAuxType_PEV : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusAuxType_PEV(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			var steeringPumpData = auxData.SteeringPumpTechnology;

			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				new XElement(_mrf + "SteeringPumpTechnology", string.Join("\n", steeringPumpData)),
				_mrfFactory.GetPrimaryBusElectricSystemType_PEV().GetXmlType(auxData),
				_mrfFactory.GetPrimaryBusPneumaticSystemType_PEV_IEPC().GetXmlType(auxData),
				_mrfFactory.GetPrimaryBusHVACSystemType_PEV().GetXmlType(auxData)
			);
		}

		#endregion
	}



	public class MRFPrimaryBusHVACSystemType_Conventional_HEV : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusHVACSystemType_Conventional_HEV(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	public class MRFPrimaryBusHVACSystemType_PEV : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusHVACSystemType_PEV(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			var hvac = auxData.HVACAux;
			return new XElement(_mrf + "HVACSystem",
				new XElement(_mrf + XMLNames.Bus_AdjustableCoolantThermostat, hvac.AdjustableCoolantThermostat));
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

	internal class MRFPrimaryBusElectricSystemType_Conventional_HEV : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusElectricSystemType_Conventional_HEV(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }


		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			var maxAlternatorPower = auxData.ElectricSupply.Alternators?.Select(alt => alt.RatedCurrent * alt.RatedVoltage)?
				.Max();
			var electricStorageCapacity =
				auxData.ElectricSupply.ElectricStorage?.Sum(electricStorage => electricStorage.ElectricStorageCapacity);
			return new XElement(_mrf + XMLNames.BusAux_ElectricSystem,
				new XElement(_mrf + XMLNames.BusAux_ElectricSystem_AlternatorTechnology,
					auxData.ElectricSupply.AlternatorTechnology.ToXMLFormat()),
				(auxData.ElectricSupply.AlternatorTechnology == AlternatorType.None || maxAlternatorPower == null)
					? null
					: new XElement(_mrf + "MaxAlternatorPower", maxAlternatorPower.ToXMLFormat(0)),

			auxData.ElectricSupply.ElectricStorage == null || electricStorageCapacity == null ? null : new XElement(_mrf + "ElectricStorageCapacity", electricStorageCapacity.ToXMLFormat()));
		}

		#endregion
	}

	internal class MRFPrimaryBusElectricSystemType_PEV : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusElectricSystemType_PEV(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }


		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + XMLNames.BusAux_ElectricSystem,
				auxData.ElectricSupply.ElectricStorage != null ? "TODO" : null);
		}

		#endregion
	}


	internal class MRFPrimaryBusPneumaticSystemType_Conventional_Hev_Px : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusPneumaticSystemType_Conventional_Hev_Px(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + XMLNames.BusAux_PneumaticSystem,
				new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, auxData.PneumaticSupply.CompressorDrive.GetLabel()),
				new XElement(_mrf + XMLNames.Bus_CompressorRatio, auxData.PneumaticSupply.Ratio.ToXMLFormat(3)),

				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_SmartcompressionSystem, auxData.PneumaticSupply.SmartAirCompression),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem, auxData.PneumaticSupply.SmartRegeneration),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_AirsuspensionControl, auxData.PneumaticConsumers.AirsuspensionControl.ToXMLFormat()),
				new XElement(_mrf + "ReagentDosing", auxData.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically)
			);
		}

		#endregion
	}

	internal class MRFPrimaryBusPneumaticSystemType_HEV_S : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusPneumaticSystemType_HEV_S(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + XMLNames.BusAux_PneumaticSystem,
				new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, auxData.PneumaticSupply.CompressorDrive.GetLabel()),
				new XElement(_mrf + XMLNames.Bus_CompressorRatio, auxData.PneumaticSupply.Ratio.ToXMLFormat(3)),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem, auxData.PneumaticSupply.SmartRegeneration),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_AirsuspensionControl, auxData.PneumaticConsumers.AirsuspensionControl.ToXMLFormat()),
				new XElement(_mrf + "ReagentDosing", auxData.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically)
			);
		}

		#endregion
	}

	internal class MRFPrimaryBusPneumaticSystemType_PEV_IEPC : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusPneumaticSystemType_PEV_IEPC(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetXmlType(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + XMLNames.BusAux_PneumaticSystem,
				new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, auxData.PneumaticSupply.CompressorDrive.GetLabel()),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem, auxData.PneumaticSupply.SmartRegeneration),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_AirsuspensionControl, auxData.PneumaticConsumers.AirsuspensionControl.ToXMLFormat()),
				new XElement(_mrf + "ReagentDosing", auxData.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically)
			);
		}

		#endregion
	}
}
