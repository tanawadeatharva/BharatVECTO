using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using EnumHelper = VECTO3GUI2020.Helper.EnumHelper;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{

	public interface IMultistageAuxiliariesViewModel : IBusAuxiliariesDeclarationData,
		IElectricSupplyDeclarationData,
		IPneumaticSupplyDeclarationData,
		IElectricConsumersDeclarationData,
		IPneumaticConsumersDeclarationData,
		IHVACBusAuxiliariesDeclarationData
	{
		void SetAuxiliariesInputData(IBusAuxiliariesDeclarationData componentsAuxiliaryInputData);
		bool HasValues { get; }
	}


	public class MultistageAuxiliariesViewModel : ViewModelBase, IMultistageAuxiliariesViewModel
	{

		private IBusAuxiliariesDeclarationData _consolidatedInputData;


		public IBusAuxiliariesDeclarationData ConsolidatedInputData
		{
			get => _consolidatedInputData;
			set => SetProperty(ref _consolidatedInputData, value);
		}

		public bool HasValues
		{
			get
			{
				var hasValues = false;


				//Check if one of the Implemented properties of is not null
				var HVACinterfaceProperties = typeof(IHVACBusAuxiliariesDeclarationData).GetProperties();

				var notImplemented = new List<string>(new string[] {
					nameof(IHVACBusAuxiliariesDeclarationData.AdjustableCoolantThermostat),
					nameof(IHVACBusAuxiliariesDeclarationData.EngineWasteGasHeatExchanger)
				});
				foreach (var propInfo in HVACinterfaceProperties) {
					if (notImplemented.Contains(propInfo.Name)) continue;
					hasValues = hasValues || propInfo.GetValue(this) != null;
				}


				var ElectricConsumersInterfaceProperties = typeof(IElectricConsumersDeclarationData).GetProperties();
				foreach (var propInfo in ElectricConsumersInterfaceProperties) {
					hasValues = hasValues || propInfo.GetValue(this) != null;
				}



				return hasValues;
			}
		}


		#region HVAC

		private bool _heatPumpGroupEditingEnabled;
		private BusHVACSystemConfiguration? _systemConfiguration;
		private HeatPumpType? _heatPumpTypeDriverCompartment;
		private HeatPumpMode? _heatPumpModeDriverCompartment;
		private HeatPumpType? _heatPumpTypePassengerCompartment;
		private HeatPumpMode? _heatPumpModePassengerCompartment;




		public IHVACBusAuxiliariesDeclarationData HVACAux
		{
			get => this;
			set => throw new NotImplementedException();
		}


		private Watt _auxHeaterPower;
		private bool? _doubleGlazing;
		private bool? _adjustableAuxiliaryHeater;
		private bool? _separateAirDistributionDucts;
		private bool? _waterElectricHeater;
		private bool? _airElectricHeater;
		private bool? _otherHeatingTechnology;


		public Watt AuxHeaterPower
		{
			get => _auxHeaterPower;
			set => SetProperty(ref _auxHeaterPower, value);
		}

		public bool? DoubleGlazing
		{
			get => _doubleGlazing;
			set => SetProperty(ref _doubleGlazing, value);
		}

		public bool? AdjustableAuxiliaryHeater
		{
			get => _adjustableAuxiliaryHeater;
			set => SetProperty(ref _adjustableAuxiliaryHeater, value);
		}

		public bool? SeparateAirDistributionDucts
		{
			get => _separateAirDistributionDucts;
			set => SetProperty(ref _separateAirDistributionDucts, value);
		}

		public bool? WaterElectricHeater
		{
			get => _waterElectricHeater;
			set => SetProperty(ref _waterElectricHeater, value);
		}

		public bool? AirElectricHeater
		{
			get => _airElectricHeater;
			set => SetProperty(ref _airElectricHeater, value);
		}

		public bool? OtherHeatingTechnology
		{
			get => _otherHeatingTechnology;
			set => SetProperty(ref _otherHeatingTechnology, value);
		}

		public bool? AdjustableCoolantThermostat => throw new NotImplementedException();

		public bool EngineWasteGasHeatExchanger => throw new NotImplementedException();

		public bool HeatPumpGroupEditingEnabled
		{
			get => _heatPumpGroupEditingEnabled;
			set
			{
				
				SetProperty(ref _heatPumpGroupEditingEnabled, value);
				//if (value == false)
				//{
				//	HeatPumpTypePassengerCompartment = null;
				//	HeatPumpModePassengerCompartment = null;
				//	HeatPumpModeDriverCompartment = null;
				//	HeatPumpTypeDriverCompartment = null;
				//}
			}
		}

		public BusHVACSystemConfiguration? SystemConfiguration
		{
			get => _systemConfiguration;
			set => SetProperty(ref _systemConfiguration, value);
		}

		public HeatPumpType? HeatPumpTypeDriverCompartment
		{
			get => _heatPumpTypeDriverCompartment;
			set
			{
				if (value == HeatPumpType.none) {
					HeatPumpModeDriverCompartmentAllowedValues =
						EnumHelper.GetValuesAsObservableCollectionIncluding<Enum, HeatPumpMode>(items:HeatPumpMode.N_A);
					//HeatPumpModeDriverCompartment = HeatPumpMode.N_A;
				} else {
					HeatPumpModeDriverCompartmentAllowedValues =
						EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, HeatPumpMode>(
							items: HeatPumpMode.N_A);
					//HeatPumpModeDriverCompartment = HeatPumpMode.cooling;
				}


				SetProperty(ref _heatPumpTypeDriverCompartment, value);
			} 
		}


		public HeatPumpMode? HeatPumpModeDriverCompartment
		{
			get => _heatPumpModeDriverCompartment;
			set
			{
				SetProperty(ref _heatPumpModeDriverCompartment, value);
			}
		}

		private ObservableCollection<Enum> _heatPumpModeDriverCompartmentAllowedValues =
			EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, HeatPumpMode>(HeatPumpMode.N_A);
		public ObservableCollection<Enum> HeatPumpModeDriverCompartmentAllowedValues
		{
			get
			{
				return _heatPumpModeDriverCompartmentAllowedValues;
			}
			private set
			{
				SetProperty(ref _heatPumpModeDriverCompartmentAllowedValues, value);
			}
		}




		public HeatPumpType? HeatPumpTypePassengerCompartment
		{
			get => _heatPumpTypePassengerCompartment;
			set
			{
				SetProperty(ref _heatPumpTypePassengerCompartment, value);
				if (value == HeatPumpType.none)
				{
					HeatPumpModePassengerCompartmentAllowedValues =
						EnumHelper.GetValuesAsObservableCollectionIncluding<Enum, HeatPumpMode>(items: HeatPumpMode.N_A);
					//HeatPumpModePassengerCompartment = HeatPumpMode.N_A;
				}
				else
				{
					HeatPumpModePassengerCompartmentAllowedValues =
						EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, HeatPumpMode>(
							items: HeatPumpMode.N_A);
					//HeatPumpModePassengerCompartment = HeatPumpMode.cooling;
				}
			}
		}

		public HeatPumpMode? HeatPumpModePassengerCompartment
		{
			get => _heatPumpModePassengerCompartment;
			set
			{
				SetProperty(ref _heatPumpModePassengerCompartment, value);
				
			}
		}

		private ObservableCollection<Enum> _heatPumpModePassengerCompartmentAllowedValues =
			EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, HeatPumpMode>(HeatPumpMode.N_A);
		public ObservableCollection<Enum> HeatPumpModePassengerCompartmentAllowedValues
		{
			get
			{
				return _heatPumpModePassengerCompartmentAllowedValues;
			}
			private set
			{
				SetProperty(ref _heatPumpModePassengerCompartmentAllowedValues, value);
			}
		}

		#endregion
		#region IElectricConsumersDeclaration

		//LED lights
		private bool? _interiorLightsLed;
		private bool? _dayrunninglightsLed;
		private bool? _positionlightsLed;
		private bool? _headlightsLed;
		private bool? _brakelightsLed;
	

		public bool? InteriorLightsLED
		{
			get => _interiorLightsLed;
			set => SetProperty(ref _interiorLightsLed, value);
		}

		public bool? DayrunninglightsLED
		{
			get => _dayrunninglightsLed;
			set => SetProperty(ref _dayrunninglightsLed, value);
		}

		public bool? PositionlightsLED
		{
			get => _positionlightsLed;
			set => SetProperty(ref _positionlightsLed, value);
		}

		public bool? HeadlightsLED
		{
			get => _headlightsLed;
			set => SetProperty(ref _headlightsLed, value);
		}

		public bool? BrakelightsLED
		{
			get => _brakelightsLed;
			set => SetProperty(ref _brakelightsLed, value);
		}



		#endregion
		#region IPneumaticSuppyDeclarationData
		public IPneumaticSupplyDeclarationData PneumaticSupply
		{
			get => this;
			set => throw new NotImplementedException();
		}

		private CompressorDrive _compressorDrive;


		public CompressorDrive CompressorDrive
		{
			get => _compressorDrive;
			set => SetProperty(ref _compressorDrive, value);
		}


		#endregion


		#region Implementation of interfaces (unused Properties);

		public XmlNode XMLSource => throw new NotImplementedException();

		public string FanTechnology => throw new NotImplementedException();

		public IList<string> SteeringPumpTechnology => throw new NotImplementedException();

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

		

		public IPneumaticConsumersDeclarationData PneumaticConsumers
		{
			get => this;
			set => throw new NotImplementedException();
		}

	



		#endregion
		public void SetAuxiliariesInputData(IBusAuxiliariesDeclarationData componentsAuxiliaryInputData)
		{
			if (componentsAuxiliaryInputData == null) {
				return;
			}

			HeatPumpGroupEditingEnabled = componentsAuxiliaryInputData.HVACAux != null;
			SystemConfiguration = componentsAuxiliaryInputData.HVACAux?.SystemConfiguration;
			HeatPumpTypeDriverCompartment = componentsAuxiliaryInputData.HVACAux?.HeatPumpTypeDriverCompartment;
			HeatPumpModeDriverCompartment = componentsAuxiliaryInputData.HVACAux?.HeatPumpModeDriverCompartment;
			HeatPumpTypePassengerCompartment = componentsAuxiliaryInputData.HVACAux?.HeatPumpTypePassengerCompartment;
			HeatPumpModePassengerCompartment = componentsAuxiliaryInputData.HVACAux?.HeatPumpModePassengerCompartment;

			AuxHeaterPower = componentsAuxiliaryInputData.HVACAux?.AuxHeaterPower;
			DoubleGlazing = componentsAuxiliaryInputData.HVACAux?.DoubleGlazing;
			AdjustableAuxiliaryHeater = componentsAuxiliaryInputData.HVACAux?.AdjustableAuxiliaryHeater;
			SeparateAirDistributionDucts = componentsAuxiliaryInputData.HVACAux?.SeparateAirDistributionDucts;
			WaterElectricHeater = componentsAuxiliaryInputData.HVACAux?.WaterElectricHeater;
			AirElectricHeater = componentsAuxiliaryInputData.HVACAux?.AirElectricHeater;
			OtherHeatingTechnology = componentsAuxiliaryInputData.HVACAux?.OtherHeatingTechnology;

			InteriorLightsLED = componentsAuxiliaryInputData.ElectricConsumers?.InteriorLightsLED;
			DayrunninglightsLED = componentsAuxiliaryInputData.ElectricConsumers?.DayrunninglightsLED;
			PositionlightsLED = componentsAuxiliaryInputData.ElectricConsumers?.PositionlightsLED;
			HeadlightsLED = componentsAuxiliaryInputData.ElectricConsumers?.HeadlightsLED;
			BrakelightsLED = componentsAuxiliaryInputData.ElectricConsumers?.BrakelightsLED;
		}

		private void ResetData()
		{
			HeatPumpGroupEditingEnabled = false;
			SystemConfiguration = null;
			HeatPumpTypeDriverCompartment = null;
			HeatPumpModeDriverCompartment = null;
			HeatPumpTypePassengerCompartment = null;
			HeatPumpModePassengerCompartment = null;

			AuxHeaterPower = null;
			DoubleGlazing = null;
			AdjustableAuxiliaryHeater = null;
			SeparateAirDistributionDucts = null;
			WaterElectricHeater = null;
			AirElectricHeater = null;
			OtherHeatingTechnology = null;

			InteriorLightsLED = null;
			DayrunninglightsLED = null;
			PositionlightsLED = null;
			HeadlightsLED = null;
			BrakelightsLED = null;
		}

		public MultistageAuxiliariesViewModel(IBusAuxiliariesDeclarationData consolidatedAuxiliariesInputData)
		{
			ConsolidatedInputData = consolidatedAuxiliariesInputData;
		}

		#region Implementation of IElectricSupplyDeclarationData

		public IList<IAlternatorDeclarationInputData> Alternators => throw new NotImplementedException();

		public bool SmartElectrics => throw new NotImplementedException();

		public Watt MaxAlternatorPower => throw new NotImplementedException();

		public WattSecond ElectricStorageCapacity => throw new NotImplementedException();

		#endregion

		#region Implementation of IPneumaticSupplyDeclarationData

		

		public string Clutch => throw new NotImplementedException();

		public double Ratio => throw new NotImplementedException();

		public string CompressorSize => throw new NotImplementedException();

		public bool SmartAirCompression => throw new NotImplementedException();

		public bool SmartRegeneration => throw new NotImplementedException();

		#endregion

		#region Implementation of IPneumaticConsumersDeclarationData

		public ConsumerTechnology AirsuspensionControl => throw new NotImplementedException();

		public ConsumerTechnology AdBlueDosing => throw new NotImplementedException();

		#endregion
	}
}