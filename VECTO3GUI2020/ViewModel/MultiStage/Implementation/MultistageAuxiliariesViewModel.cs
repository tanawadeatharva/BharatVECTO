using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using Castle.Core.Internal;
using Microsoft.Toolkit.Mvvm.Input;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Properties;
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
		object PrimaryVehicleHybridElectric { get; set; }
		bool HasErrors { get; }
		Dictionary<string, string> Errors { get; }
	}


	public class MultistageAuxiliariesViewModel : ViewModelBase, IMultistageAuxiliariesViewModel, IDataErrorInfo
	{

		public MultistageAuxiliariesViewModel(IBusAuxiliariesDeclarationData consolidatedAuxiliariesInputData)
		{
			ConsolidatedInputData = consolidatedAuxiliariesInputData;
			if (ConsolidatedInputData?.HVACAux?.HeatPumpPassengerCompartments != null) {
				_consolidatedHeatPumpConfigurationsPassenger = new ObservableCollection<HeatPumpConfiguration>();
				foreach (var (heatPumpType, heatPumpMode) in ConsolidatedInputData?.HVACAux?.HeatPumpPassengerCompartments) {
					ConsolidatedHeatPumpConfigurationsPassenger.Add(new HeatPumpConfiguration()
					{
						HeatPumpMode = heatPumpMode,
						HeatPumpType = heatPumpType,
						IsReadOnly = true,
					});
				}
			}

			CreateParameterViewModels();
		}

		private void CreateParameterViewModels()
		{
			_parameterViewModels = new Dictionary<string, MultistageParameterViewModel>();
			var properties = this.GetType().GetProperties();
			var backedUpParameters = new HashSet<string>() {
				nameof(InteriorLightsLED),
				nameof(DayrunninglightsLED),
				nameof(PositionlightsLED),
				nameof(BrakelightsLED),
				nameof(HeadlightsLED),
			
				nameof(SystemConfiguration),
				nameof(HeatPumpModeDriverCompartment),
				nameof(HeatPumpTypeDriverCompartment),

				nameof(AuxHeaterPower),
				nameof(DoubleGlazing),
				nameof(AirElectricHeater),
				nameof(AdjustableAuxiliaryHeater),
				nameof(SeparateAirDistributionDucts),
				nameof(OtherHeatingTechnology),
				nameof(WaterElectricHeater)
			};

			foreach (var property in properties)
			{
				if (!backedUpParameters.Contains(property.Name))
				{
					continue;
				}


				object previousInputData = null;
				try
				{
					previousInputData = ConsolidatedInputData?.GetType().GetProperty(nameof(property.Name))?
						.GetValue(ConsolidatedInputData);
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
				}

				_parameterViewModels.Add(property.Name, new MultistageParameterViewModel(property.Name, previousInputData, this,
					resourceManagers: new ResourceManager[] { BusStrings.ResourceManager, Strings.ResourceManager }
				));
			}

			//Set consolidated Data
			_parameterViewModels[nameof(InteriorLightsLED)].PreviousContent =
				ConsolidatedInputData?.ElectricConsumers.InteriorLightsLED;
			_parameterViewModels[nameof(DayrunninglightsLED)].PreviousContent =
				ConsolidatedInputData?.ElectricConsumers.DayrunninglightsLED;
			_parameterViewModels[nameof(PositionlightsLED)].PreviousContent =
				ConsolidatedInputData?.ElectricConsumers.PositionlightsLED;
			_parameterViewModels[nameof(BrakelightsLED)].PreviousContent =
				ConsolidatedInputData?.ElectricConsumers.BrakelightsLED;
			_parameterViewModels[nameof(HeadlightsLED)].PreviousContent =
				ConsolidatedInputData?.ElectricConsumers.HeadlightsLED;
			_parameterViewModels[nameof(SystemConfiguration)].PreviousContent =
				ConsolidatedInputData?.HVACAux.SystemConfiguration;
			_parameterViewModels[nameof(HeatPumpModeDriverCompartment)].PreviousContent =
				ConsolidatedInputData?.HVACAux.HeatPumpModeDriverCompartment;
			//_parameterViewModels[nameof(HeatPumpTypePassengerCompartment)].PreviousContent =
			//	ConsolidatedInputData?.HVACAux.HeatPumpTypePassengerCompartment;
			_parameterViewModels[nameof(HeatPumpTypeDriverCompartment)].PreviousContent =
				ConsolidatedInputData?.HVACAux.HeatPumpTypeDriverCompartment;
			//_parameterViewModels[nameof(HeatPumpModePassengerCompartment)].PreviousContent =
			//	ConsolidatedInputData?.HVACAux.HeatPumpModePassengerCompartment;
			_parameterViewModels[nameof(AuxHeaterPower)].PreviousContent =
				ConsolidatedInputData?.HVACAux.AuxHeaterPower;
			_parameterViewModels[nameof(DoubleGlazing)].PreviousContent =
				ConsolidatedInputData?.HVACAux.DoubleGlazing;
			_parameterViewModels[nameof(AdjustableAuxiliaryHeater)].PreviousContent =
				ConsolidatedInputData?.HVACAux.AdjustableAuxiliaryHeater;
			_parameterViewModels[nameof(SeparateAirDistributionDucts)].PreviousContent =
				ConsolidatedInputData?.HVACAux.SeparateAirDistributionDucts;
			_parameterViewModels[nameof(AirElectricHeater)].PreviousContent =
				ConsolidatedInputData?.HVACAux.AirElectricHeater;
			_parameterViewModels[nameof(OtherHeatingTechnology)].PreviousContent =
				ConsolidatedInputData?.HVACAux.OtherHeatingTechnology;


			//Set editinggroups
			Action<MultistageParameterViewModel> HeatPumpGroupEditingEnabledCallback = model => {
				HeatPumpGroupEditingEnabled = model.EditingEnabled;
			};
			_parameterViewModels[nameof(SystemConfiguration)].EditingChangedCallback =
				HeatPumpGroupEditingEnabledCallback;
			_parameterViewModels[nameof(HeatPumpModeDriverCompartment)].EditingChangedCallback =
				HeatPumpGroupEditingEnabledCallback;
			_parameterViewModels[nameof(HeatPumpTypeDriverCompartment)].EditingChangedCallback =
				HeatPumpGroupEditingEnabledCallback;

			//Setup AllowedValues 
			HeatPumpModeDriverCompartmentAllowedValues =
				EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, HeatPumpMode>(HeatPumpMode.N_A);

			SystemConfigurationAllowedValues =
				EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, BusHVACSystemConfiguration>(
					BusHVACSystemConfiguration.Unknown);
		}

		protected override bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			var propertyChanged = base.SetProperty(ref field, value, propertyName);

			if (propertyChanged && _parameterViewModels != null && _parameterViewModels.ContainsKey(propertyName))
			{
				_parameterViewModels[propertyName].CurrentContent = value;
			}
			return propertyChanged;
		}

		public void SetAuxiliariesInputData(IBusAuxiliariesDeclarationData componentsAuxiliaryInputData)
		{
			if (componentsAuxiliaryInputData == null)
			{
				return;
			}

			HeatPumpGroupEditingEnabled = componentsAuxiliaryInputData.HVACAux != null;
			SystemConfiguration = componentsAuxiliaryInputData.HVACAux?.SystemConfiguration;
			HeatPumpTypeDriverCompartment = componentsAuxiliaryInputData.HVACAux?.HeatPumpTypeDriverCompartment;
			HeatPumpModeDriverCompartment = componentsAuxiliaryInputData.HVACAux?.HeatPumpModeDriverCompartment;
			HeatPumpConfigurationsPassenger.Clear();
			if (componentsAuxiliaryInputData.HVACAux?.HeatPumpPassengerCompartments != null)
			{
				foreach (var (heatPumpType, heatPumpMode) in componentsAuxiliaryInputData.HVACAux?.HeatPumpPassengerCompartments)
				{
					HeatPumpConfigurationsPassenger.Add(new HeatPumpConfiguration()
					{
						HeatPumpType = heatPumpType,
						HeatPumpMode = heatPumpMode,
					});
				}
			}



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

			foreach (var multistageParameterViewModel in _parameterViewModels.Values)
			{
				multistageParameterViewModel.UpdateEditingEnabled();
			}
			OnPropertyChanged(String.Empty);
		}



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

		private object _primaryVehicleHybridElectric = false;
		public object PrimaryVehicleHybridElectric
		{
			get => _primaryVehicleHybridElectric;
			set => SetProperty(ref _primaryVehicleHybridElectric, value);
		}

		private IndexedStorage<bool> _editingEnabledDictionary;
		public IndexedStorage<bool> EditingEnabledDictionary
		{
			get
			{
				return _editingEnabledDictionary;
			}
		}



		#region HVAC

		private bool _heatPumpGroupEditingEnabled;
		private BusHVACSystemConfiguration? _systemConfiguration;
		private HeatPumpType? _heatPumpTypeDriverCompartment;
		private HeatPumpMode? _heatPumpModeDriverCompartment;




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

		#region Heatpump

		private ICommand _addPassengerHeatpumpCommand;

		public ICommand AddPassengerHeatpumpCommand
		{
			get => _addPassengerHeatpumpCommand ??
					new RelayCommand(() => {
							HeatPumpGroupEditingEnabled = true;
							if (HeatPumpConfigurationsPassenger == null) {
								HeatPumpConfigurationsPassenger = new ObservableCollection<HeatPumpConfiguration>();
							}
							HeatPumpConfigurationsPassenger.Add(new HeatPumpConfiguration());
						},
						() => true);
		}

		public ICommand _removePasssengerHeatpumpCommand;

		public ICommand RemovePassengerHeatpumpCommand
		{
			get => _removePasssengerHeatpumpCommand ??
					new RelayCommand<HeatPumpConfiguration>(hp => HeatPumpConfigurationsPassenger?.Remove(hp), (hp) => true);
		}

		public IList<Tuple<HeatPumpType, HeatPumpMode>> HeatPumpPassengerCompartments{
			get
			{
				var list = new List<Tuple<HeatPumpType, HeatPumpMode>>();
				if (HeatPumpConfigurationsPassenger != null) {
					foreach (var heatPumpConfiguration in HeatPumpConfigurationsPassenger)
					{
						list.Add(new Tuple<HeatPumpType, HeatPumpMode>((HeatPumpType)heatPumpConfiguration.HeatPumpType, (HeatPumpMode)heatPumpConfiguration.HeatPumpMode));
					}
				}
				
				return list;
			}
		}

		public ObservableCollection<HeatPumpConfiguration> ConsolidatedHeatPumpConfigurationsPassenger
		{
			get => _consolidatedHeatPumpConfigurationsPassenger;
			private set => SetProperty(ref _consolidatedHeatPumpConfigurationsPassenger, value);
		}


		public ObservableCollection<HeatPumpConfiguration> HeatPumpConfigurationsPassenger
		{
			get => _heatPumpConfigurationsPassenger;
			private set => SetProperty(ref _heatPumpConfigurationsPassenger, value);
		}

		public class HeatPumpConfiguration : ViewModelBase, ITuple, IDataErrorInfo
		{
			public MultistageParameterViewModel HeatPumpTypeVM { get; }
			public MultistageParameterViewModel HeatPumpModeVM { get; }

			private HeatPumpType? _heatPumpType;
			private HeatPumpMode? _heatPumpMode;

			public HeatPumpType? HeatPumpType
			{
				get => _heatPumpType;
				set
				{
					if (SetProperty(ref _heatPumpType, value)) {
						
						if (value == TUGraz.VectoCommon.BusAuxiliaries.HeatPumpType.none)
						{
							HeatPumpModeVM.AllowedItems =
								EnumHelper.GetValuesAsObservableCollectionIncluding<Enum, HeatPumpMode>(
									items: TUGraz.VectoCommon.BusAuxiliaries.HeatPumpMode.N_A);
						}
						else
						{
							HeatPumpModeVM.AllowedItems =
								EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, HeatPumpMode>(
									items: TUGraz.VectoCommon.BusAuxiliaries.HeatPumpMode.N_A);
						}

						HeatPumpTypeVM.CurrentContent = value;
					}
				}
			}


			public HeatPumpMode? HeatPumpMode
			{
				get => _heatPumpMode;
				set
				{
					if (SetProperty(ref _heatPumpMode, value)) {
						HeatPumpModeVM.CurrentContent = value;
					}
				}
			}
			public HeatPumpConfiguration()
			{
				HeatPumpModeVM = new MultistageParameterViewModel(nameof(HeatPumpMode), null, this, ViewMode.COMBOBOX,
					mandatory: true, resourceManagers: new ResourceManager[] {
						BusStrings.ResourceManager,
						Strings.ResourceManager,
					}) {
					AllowedItems = EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, HeatPumpMode>(items: TUGraz.VectoCommon.BusAuxiliaries.HeatPumpMode.N_A)
				};
				HeatPumpTypeVM = new MultistageParameterViewModel(nameof(HeatPumpType), null, this, ViewMode.COMBOBOX,
					mandatory: true, resourceManagers: new ResourceManager[] {
						BusStrings.ResourceManager,
						Strings.ResourceManager,
					});
			}

			public bool IsReadOnly
			{
				get => HeatPumpModeVM.IsReadOnly && HeatPumpTypeVM.IsReadOnly;
				set
				{
					HeatPumpTypeVM.IsReadOnly = value;
					HeatPumpModeVM.IsReadOnly = value;
				}
			}

			#region Implementation of ITuple

			int ITuple.Length => 2;

			object ITuple.this[int index]
			{
				get
				{
					return null;
				}
			}

			#endregion

			#region Implementation of IDataErrorInfo

			string IDataErrorInfo.this[string columnName]
			{
				get
				{
					string result = null;
					switch (columnName) {
						case nameof(HeatPumpType):
							if (HeatPumpType == null) {
								result = $"{columnName} must not be empty";
							}
							break;
						case nameof(HeatPumpMode):
							if (HeatPumpMode == null) {
								result = $"{columnName} must not be empty";
							}
							break;
						default:
							result = null;
							break;
					}

					if (result == null)
					{
						if (Errors.ContainsKey(columnName))
							Errors.Remove(columnName);
					}
					else
					{
						Errors[columnName] = result;
					}



					return result;
				}
			}

			private Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();

			public bool HasErrors
			{
				get => !Errors.IsNullOrEmpty();

			}
			string IDataErrorInfo.Error => String.Join(",", Errors.Values);

			#endregion
		}

		public bool HeatPumpGroupEditingEnabled
		{
			get => _heatPumpGroupEditingEnabled;
			set
			{
				if (SetProperty(ref _heatPumpGroupEditingEnabled, value)) {
					_parameterViewModels[nameof(HeatPumpModeDriverCompartment)].EditingEnabled = value;
					_parameterViewModels[nameof(HeatPumpTypeDriverCompartment)].EditingEnabled = value;
					_parameterViewModels[nameof(SystemConfiguration)].EditingEnabled = value;
					if (value == false) {
						HeatPumpConfigurationsPassenger = null;
					}
				}
			}
		}

		private ObservableCollection<Enum> _systemConfigurationAllowedValues;
		public ObservableCollection<Enum> SystemConfigurationAllowedValues
		{
			get => _systemConfigurationAllowedValues;
			set
			{
				if (SetProperty(ref _systemConfigurationAllowedValues, value)) {
					_parameterViewModels[nameof(SystemConfiguration)].AllowedItems = value;
				}
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
				if (SetProperty(ref _heatPumpTypeDriverCompartment, value)) {
					if (value == HeatPumpType.none)
					{
						HeatPumpModeDriverCompartmentAllowedValues =
							EnumHelper.GetValuesAsObservableCollectionIncluding<Enum, HeatPumpMode>(items: HeatPumpMode.N_A);

					}
					else
					{
						HeatPumpModeDriverCompartmentAllowedValues =
							EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, HeatPumpMode>(
								items: HeatPumpMode.N_A);

					}

				}
			} 
		}

		public HeatPumpMode? HeatPumpModeDriverCompartment
		{
			get => _heatPumpModeDriverCompartment;
			set => SetProperty(ref _heatPumpModeDriverCompartment, value);
		}

		private ObservableCollection<Enum> _heatPumpModeDriverCompartmentAllowedValues;
	
		public ObservableCollection<Enum> HeatPumpModeDriverCompartmentAllowedValues
		{
			get
			{
				return _heatPumpModeDriverCompartmentAllowedValues;
			}
			private set
			{
				if (SetProperty(ref _heatPumpModeDriverCompartmentAllowedValues, value)) {
					_parameterViewModels[nameof(HeatPumpModeDriverCompartment)].AllowedItems = value;
				};
			}
		}

		#endregion

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
			set
			{
				SetProperty(ref _interiorLightsLed, value);
				if (value != null) {
					OnPropertyChanged(nameof(EditingEnabledDictionary));
				}
			}
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
		private Dictionary<string, MultistageParameterViewModel> _parameterViewModels;
		private ObservableCollection<HeatPumpConfiguration> _consolidatedHeatPumpConfigurationsPassenger;
		private ObservableCollection<HeatPumpConfiguration> _heatPumpConfigurationsPassenger;


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


		#region Implementation of IElectricSupplyDeclarationData

		public AlternatorType AlternatorTechnology => throw new NotImplementedException();
		public IList<IAlternatorDeclarationInputData> Alternators => throw new NotImplementedException();
		public IList<IBusAuxElectricStorageDeclarationInputData> ElectricStorage => throw new NotImplementedException();

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

		#region Implementation of IDataErrorInfo

		public Dictionary<string, string> Errors { get; private set; } = new Dictionary<string, string>();

		public string this[string propertyName]
		{
			get
			{
				string result = null;
				switch (propertyName)
				{
					case nameof(HeatPumpTypeDriverCompartment):
					case nameof(HeatPumpModeDriverCompartment):
					case nameof(SystemConfiguration):
						if (HeatPumpGroupEditingEnabled == true &&
							this.GetType().GetProperty(propertyName).GetValue(this) == null) {
							result = $"{NameResolver.ResolveName(propertyName, BusStrings.ResourceManager, Strings.ResourceManager)} has to be set if editing is enabled}}";
						}
						break;
					default:
						if (_parameterViewModels[propertyName].EditingEnabled == true && this.GetType().GetProperty(propertyName).GetValue(this) == null)
						{
							result = $"{NameResolver.ResolveName(propertyName, BusStrings.ResourceManager, Strings.ResourceManager)} has to be set if editing is enabled}}";
						}
						break;
				}
				//https://www.youtube.com/watch?v=5KF0GGObuAQ

				if (result == null)
				{
					if (Errors.ContainsKey(propertyName))
						Errors.Remove(propertyName);
				}
				else
				{
					Errors[propertyName] = result;
				}


				return result;
			}
		}

		public string Error
		{
			get
			{
				string result = string.Empty;
				var auxVmError = String.Join(",", Errors.Values);
				result = auxVmError;
				if (HeatPumpConfigurationsPassenger != null) {
					foreach (var heatPumpConfiguration in HeatPumpConfigurationsPassenger) {
						if (heatPumpConfiguration.HasErrors) {
							result += "," + ((IDataErrorInfo)heatPumpConfiguration).Error;
						}
					}
				}

				return result;
			}
		}

		public bool HasErrors
		{
			get
			{
				var auxVmHasErrors = !Error.IsNullOrEmpty();
				var passengerHeatPumpsHaveError = false;
				if (HeatPumpConfigurationsPassenger != null) {
					foreach (var heatPump in HeatPumpConfigurationsPassenger) {
						if (heatPump.HasErrors) {
							passengerHeatPumpsHaveError = true;
							break;
						}
					}
				}

				return auxVmHasErrors || passengerHeatPumpsHaveError;
			}
		}

		public Dictionary<string, MultistageParameterViewModel> ParameterViewModels
		{
			get
			{
				return _parameterViewModels;
			}
			set
			{
				SetProperty(ref _parameterViewModels, value);
			}
		}

		#endregion
	}
}