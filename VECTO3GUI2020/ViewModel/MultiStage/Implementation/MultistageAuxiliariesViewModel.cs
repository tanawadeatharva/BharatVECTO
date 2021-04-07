using System;
using System.Collections.Generic;
using System.Xml;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.Common;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	
	public class MultistageAuxiliariesViewModel : ViewModelBase, IBusAuxiliariesDeclarationData,
		IElectricSupplyDeclarationData,
		IPneumaticSupplyDeclarationData,
		IElectricConsumersDeclarationData,
		IPneumaticConsumersDeclarationData,
		IHVACBusAuxiliariesDeclarationData //implements all used interfaces similar to 
	{
		#region Implementation of interfaces;
		private XmlNode _xmlSource;
		private string _fanTechnology;
		private IList<string> _steeringPumpTechnology;
		private IElectricSupplyDeclarationData _electricSupply;
		private IElectricConsumersDeclarationData _electricConsumers;
		private IPneumaticSupplyDeclarationData _pneumaticSupply;
		private IPneumaticConsumersDeclarationData _pneumaticConsumers;
		private IHVACBusAuxiliariesDeclarationData _hvacAux;
		private IList<IAlternatorDeclarationInputData> _alternators;
		private bool _smartElectrics;
		private Watt _maxAlternatorPower;
		private WattSecond _electricStorageCapacity;
		private string _clutch;
		private double _ratio;
		private string _compressorSize;
		private bool _smartAirCompression;
		private bool _smartRegeneration;
		private bool? _interiorLightsLed;
		private bool? _dayrunninglightsLed;
		private bool? _positionlightsLed;
		private bool? _headlightsLed;
		private bool? _brakelightsLed;
		private ConsumerTechnology _airsuspensionControl;
		private ConsumerTechnology _adBlueDosing;
		private BusHVACSystemConfiguration? _systemConfiguration;
		private HeatPumpType? _heatPumpTypeDriverCompartment;
		private HeatPumpMode? _heatPumpModeDriverCompartment;
		private HeatPumpType? _heatPumpTypePassengerCompartment;
		private HeatPumpMode? _heatPumpModePassengerCompartment;
		private Watt _auxHeaterPower;
		private bool? _doubleGlazing;
		private bool? _adjustableAuxiliaryHeater;
		private bool? _separateAirDistributionDucts;
		private bool? _waterElectricHeater;
		private bool? _airElectricHeater;
		private bool? _otherHeatingTechnology;
		private bool? _adjustableCoolantThermostat;
		private bool _engineWasteGasHeatExchanger;

		public XmlNode XMLSource
		{
			get => _xmlSource;
			set => _xmlSource = value;
		}

		public string FanTechnology
		{
			get => _fanTechnology;
			set => _fanTechnology = value;
		}

		public IList<string> SteeringPumpTechnology
		{
			get => _steeringPumpTechnology;
			set => _steeringPumpTechnology = value;
		}

		public IElectricSupplyDeclarationData ElectricSupply
		{
			get => this;
			set => throw new NotImplementedException();
		}

		public IElectricConsumersDeclarationData ElectricConsumers
		{
			get => this;
			set => throw new NotImplementedException();
		}

		public IPneumaticSupplyDeclarationData PneumaticSupply
		{
			get => this;
			set => throw new NotImplementedException();
		}

		public IPneumaticConsumersDeclarationData PneumaticConsumers
		{
			get => this;
			set => throw new NotImplementedException();
		}

		public IHVACBusAuxiliariesDeclarationData HVACAux
		{
			get => this;
			set => throw new NotImplementedException();
		}

		public IList<IAlternatorDeclarationInputData> Alternators
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public bool SmartElectrics
		{
			get => _smartElectrics;
			set => _smartElectrics = value;
		}

		public Watt MaxAlternatorPower
		{
			get => _maxAlternatorPower;
			set => _maxAlternatorPower = value;
		}

		public WattSecond ElectricStorageCapacity
		{
			get => _electricStorageCapacity;
			set => _electricStorageCapacity = value;
		}

		public string Clutch
		{
			get => _clutch;
			set => _clutch = value;
		}

		public double Ratio
		{
			get => _ratio;
			set => _ratio = value;
		}

		public string CompressorSize
		{
			get => _compressorSize;
			set => _compressorSize = value;
		}

		public bool SmartAirCompression
		{
			get => _smartAirCompression;
			set => _smartAirCompression = value;
		}

		public bool SmartRegeneration
		{
			get => _smartRegeneration;
			set => _smartRegeneration = value;
		}

		public bool? InteriorLightsLED
		{
			get => _interiorLightsLed;
			set => _interiorLightsLed = value;
		}

		public bool? DayrunninglightsLED
		{
			get => _dayrunninglightsLed;
			set => _dayrunninglightsLed = value;
		}

		public bool? PositionlightsLED
		{
			get => _positionlightsLed;
			set => _positionlightsLed = value;
		}

		public bool? HeadlightsLED
		{
			get => _headlightsLed;
			set => _headlightsLed = value;
		}

		public bool? BrakelightsLED
		{
			get => _brakelightsLed;
			set => _brakelightsLed = value;
		}

		public ConsumerTechnology AirsuspensionControl
		{
			get => _airsuspensionControl;
			set => _airsuspensionControl = value;
		}

		public ConsumerTechnology AdBlueDosing
		{
			get => _adBlueDosing;
			set => _adBlueDosing = value;
		}

		public BusHVACSystemConfiguration? SystemConfiguration
		{
			get => _systemConfiguration;
			set => _systemConfiguration = value;
		}

		public HeatPumpType? HeatPumpTypeDriverCompartment
		{
			get => _heatPumpTypeDriverCompartment;
			set => _heatPumpTypeDriverCompartment = value;
		}

		public HeatPumpMode? HeatPumpModeDriverCompartment
		{
			get => _heatPumpModeDriverCompartment;
			set => _heatPumpModeDriverCompartment = value;
		}

		public HeatPumpType? HeatPumpTypePassengerCompartment
		{
			get => _heatPumpTypePassengerCompartment;
			set => _heatPumpTypePassengerCompartment = value;
		}

		public HeatPumpMode? HeatPumpModePassengerCompartment
		{
			get => _heatPumpModePassengerCompartment;
			set => _heatPumpModePassengerCompartment = value;
		}

		public Watt AuxHeaterPower
		{
			get => _auxHeaterPower;
			set => _auxHeaterPower = value;
		}

		public bool? DoubleGlazing
		{
			get => _doubleGlazing;
			set => _doubleGlazing = value;
		}

		public bool? AdjustableAuxiliaryHeater
		{
			get => _adjustableAuxiliaryHeater;
			set => _adjustableAuxiliaryHeater = value;
		}

		public bool? SeparateAirDistributionDucts
		{
			get => _separateAirDistributionDucts;
			set => _separateAirDistributionDucts = value;
		}

		public bool? WaterElectricHeater
		{
			get => _waterElectricHeater;
			set => _waterElectricHeater = value;
		}

		public bool? AirElectricHeater
		{
			get => _airElectricHeater;
			set => _airElectricHeater = value;
		}

		public bool? OtherHeatingTechnology
		{
			get => _otherHeatingTechnology;
			set => _otherHeatingTechnology = value;
		}

		public bool? AdjustableCoolantThermostat
		{
			get => _adjustableCoolantThermostat;
			set => _adjustableCoolantThermostat = value;
		}

		public bool EngineWasteGasHeatExchanger
		{
			get => _engineWasteGasHeatExchanger;
			set => _engineWasteGasHeatExchanger = value;
		}
		#endregion
	}
}