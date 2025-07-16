using System.Xml.Linq;
using NLog.LayoutRenderers;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.InterimComponents
{
	public abstract class InterimVIFAuxiliaryType : IReportMultistepCompletedBusTypeWriter
	{
		protected XNamespace _vif = XMLDefinitions.VEHICLE_INTERIM_FILE_TARGET_VERSION;
		protected XNamespace _v27 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.7";
		protected XNamespace _xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		protected readonly IVIFReportInterimFactory _vifFactory;

		protected InterimVIFAuxiliaryType(IVIFReportInterimFactory vifFactory)
		{
			_vifFactory = vifFactory;
		}

		#region Implementation of IReportMultistepCompletedBusTypeWriter

		public abstract XElement GetElement(IMultistageVIFInputData inputData);

		#endregion
	}

	public class ConventionalInterimAuxiliaryType : InterimVIFAuxiliaryType
	{
		public ConventionalInterimAuxiliaryType(IVIFReportInterimFactory vifFactory) : base(vifFactory) { }

		protected virtual string XMLType => "AUX_Conventional_CompletedBusType";

		#region Overrides of InterimVIFAuxiliaryType

		public override XElement GetElement(IMultistageVIFInputData inputData)
		{
			var busAux = inputData.VehicleInputData.Components?.BusAuxiliaries;
			if (busAux == null)
				return null;

			var electricSystemEntry = GetElectricSystem(busAux.ElectricConsumers);
			var hvacEntry = GetHVAC(busAux.HVACAux);

			if (electricSystemEntry == null && hvacEntry == null)
				return null;

			return new XElement(_v27 + XMLNames.Component_Auxiliaries,
				new XElement(_v27 + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.Attr_Type, XMLType),
					electricSystemEntry != null
						? GetElectricSystem(busAux.ElectricConsumers) : null,
					hvacEntry != null
						? GetHVAC(busAux.HVACAux) : null
				));
		}

		#endregion

		protected virtual XElement GetElectricSystem(IElectricConsumersDeclarationData electricConsumer)
		{
			if (electricConsumer == null)
				return null;

			if (electricConsumer.InteriorLightsLED == null && electricConsumer.DayrunninglightsLED == null &&
				electricConsumer.PositionlightsLED == null && electricConsumer.BrakelightsLED == null &&
				electricConsumer.HeadlightsLED == null)
				return null;

			return new XElement(_v27 + XMLNames.BusAux_ElectricSystem,
				new XElement(_v27 + XMLNames.BusAux_LEDLights,
					electricConsumer.InteriorLightsLED != null
						? new XElement(_v27 + XMLNames.Bus_Interiorlights, electricConsumer.InteriorLightsLED) : null,
					electricConsumer.DayrunninglightsLED != null
						? new XElement(_v27 + XMLNames.Bus_Dayrunninglights, electricConsumer.DayrunninglightsLED) : null,
					electricConsumer.PositionlightsLED != null
						? new XElement(_v27 + XMLNames.Bus_Positionlights, electricConsumer.PositionlightsLED) : null,
					electricConsumer.BrakelightsLED != null
						? new XElement(_v27 + XMLNames.Bus_Brakelights, electricConsumer.BrakelightsLED) : null,
					electricConsumer.HeadlightsLED != null
						? new XElement(_v27 + XMLNames.Bus_Headlights, electricConsumer.HeadlightsLED) : null
				));
		}

		protected virtual XElement GetHVAC(IHVACBusAuxiliariesDeclarationData hvac)
		{
			if (hvac == null)
				return null;

			if (hvac.SystemConfiguration == null &&
				hvac.HeatPumpTypeCoolingDriverCompartment == null && hvac.HeatPumpTypeHeatingDriverCompartment == null &&
				hvac.HeatPumpTypeCoolingPassengerCompartment == null && hvac.HeatPumpTypeHeatingPassengerCompartment == null &&
				hvac.AuxHeaterPower == null && hvac.DoubleGlazing == null &&
				hvac.AdjustableAuxiliaryHeater == null && hvac.SeparateAirDistributionDucts == null &&
				hvac.WaterElectricHeater == null && hvac.AirElectricHeater == null && hvac.OtherHeatingTechnology == null)
				return null;

			return new XElement(_v27 + XMLNames.BusAux_HVAC,
				hvac.SystemConfiguration != null
					? new XElement(_v27 + XMLNames.Bus_SystemConfiguration, hvac.SystemConfiguration.ToXmlFormat()) : null,
				hvac.HeatPumpTypeCoolingDriverCompartment != null && hvac.HeatPumpTypeHeatingDriverCompartment != null
					? new XElement(_v27 + XMLNames.Bus_HeatPumpTypeDriver,
						new XElement(_v27 + XMLNames.BusHVACHeatPumpCooling, hvac.HeatPumpTypeCoolingDriverCompartment.GetLabel()),
						new XElement(_v27 + XMLNames.BusHVACHeatPumpHeating, hvac.HeatPumpTypeHeatingDriverCompartment.GetLabel())) : null,
				hvac.HeatPumpTypeCoolingPassengerCompartment != null && hvac.HeatPumpTypeHeatingPassengerCompartment != null
					? new XElement(_v27 + XMLNames.Bus_HeatPumpTypePassenger,
						new XElement(_v27 + XMLNames.BusHVACHeatPumpCooling, hvac.HeatPumpTypeCoolingPassengerCompartment.GetLabel()),
						new XElement(_v27 + XMLNames.BusHVACHeatPumpHeating, hvac.HeatPumpTypeHeatingPassengerCompartment.GetLabel())) : null,
				hvac.AuxHeaterPower != null
					? new XElement(_v27 + XMLNames.Bus_AuxiliaryHeaterPower, hvac.AuxHeaterPower.ToXMLFormat(0)) : null,
				hvac.DoubleGlazing != null
					? new XElement(_v27 + XMLNames.Bus_DoubleGlazing, hvac.DoubleGlazing) : null,
				hvac.AdjustableAuxiliaryHeater != null
					? new XElement(_v27 + XMLNames.Bus_AdjustableAuxiliaryHeater, hvac.AdjustableAuxiliaryHeater) : null,
				hvac.SeparateAirDistributionDucts != null
					? new XElement(_v27 + XMLNames.Bus_SeparateAirDistributionDucts, hvac.SeparateAirDistributionDucts) : null
			);
		}

	}

	public class XEVInterimAuxiliaryType : ConventionalInterimAuxiliaryType
	{
		public XEVInterimAuxiliaryType(IVIFReportInterimFactory vifFactory) : base(vifFactory) { }

		protected override string XMLType => "AUX_xEV_CompletedBusType";

		protected override XElement GetHVAC(IHVACBusAuxiliariesDeclarationData hvac)
		{
			var retVal = base.GetHVAC(hvac);
			retVal.Add(GetxEVHVAC(hvac));

			return retVal;
			
		}

		protected virtual XElement[] GetxEVHVAC(IHVACBusAuxiliariesDeclarationData hvac)
		{
			return new[] {
				hvac.WaterElectricHeater.HasValue ?
					new XElement(_v27 + XMLNames.Bus_WaterElectricHeater, hvac.WaterElectricHeater) :null,
				hvac.AirElectricHeater.HasValue ?
					new XElement(_v27 + XMLNames.Bus_AirElectricHeater, hvac.AirElectricHeater) : null,
				hvac.OtherHeatingTechnology.HasValue ?
					new XElement(_v27 + XMLNames.Bus_OtherHeatingTechnology, hvac.OtherHeatingTechnology) : null
			};
		}
	}
}