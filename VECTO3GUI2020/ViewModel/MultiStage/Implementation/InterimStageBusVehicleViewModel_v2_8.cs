using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Castle.Core.Internal;
using Microsoft.Build.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;
using Convert = System.Convert;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public enum AIRDRAGMODIFIED
	{
		[GuiLabel("Unknown")]
		UNKNOWN = 0,
		[GuiLabel("True")]
		TRUE = 1,
		[GuiLabel("False")]
		FALSE = 2,

	}

	public static class AirdragModifiedEnumHelper
	{
		public static AIRDRAGMODIFIED toAirdragModifiedEnum(this bool? nullableBool)
		{
			if (nullableBool.HasValue) {
				return nullableBool.Value == true ? AIRDRAGMODIFIED.TRUE : AIRDRAGMODIFIED.FALSE;

			}
			return AIRDRAGMODIFIED.UNKNOWN;
		}


		public static bool? toNullableBool(this AIRDRAGMODIFIED airdragModified)
		{
			switch (airdragModified) {
				case AIRDRAGMODIFIED.TRUE:
					return true;
				case AIRDRAGMODIFIED.FALSE:
					return false;
				default:
					return null;
			}
		}
	}

	public interface IMultistageVehicleViewModel : IVehicleViewModel
	{
		bool HasErrors { get; }
		Dictionary<string, string> Errors { get; }
		IMultistageAirdragViewModel MultistageAirdragViewModel { get; set; }
		IMultistageAuxiliariesViewModel MultistageAuxiliariesViewModel { get; set; }
		void SetAirdragData(IAirdragDeclarationInputData airdragData);
		void SetVehicleInputData(IVehicleDeclarationInputData vehicleInputData);
	}


	public class DeclarationInterimStageBusVehicleViewModel_v2_8 : ViewModelBase, IMultistageVehicleViewModel,
		IVehicleComponentsDeclaration, IAdvancedDriverAssistantSystemDeclarationInputData, IDataErrorInfo
	{

		private readonly IMultiStageViewModelFactory _multiStageViewModelFactory;

		#region Subcomponents
		private IMultistageAirdragViewModel _multistageAirdragViewModel;
		private IMultistageAuxiliariesViewModel _multistageAuxiliariesViewModel;
		public IMultistageAirdragViewModel MultistageAirdragViewModel
		{
			get => _multistageAirdragViewModel;
			set => SetProperty(ref _multistageAirdragViewModel, value);
		}

		public IMultistageAuxiliariesViewModel MultistageAuxiliariesViewModel
		{
			get => _multistageAuxiliariesViewModel;
			set => SetProperty(ref _multistageAuxiliariesViewModel, value);
		}

		#endregion

		public static readonly string INPUTPROVIDERTYPE =
			typeof(XMLDeclarationInterimStageBusDataProviderV28).ToString();

		public string Name => "Vehicle";

		public bool IsPresent => true;

		public DataSource DataSource => throw new NotImplementedException();

		public bool SavedInDeclarationMode => true;

		public ObservableCollection<IComponentViewModel> ComponentViewModels
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}


		public DeclarationInterimStageBusVehicleViewModel_v2_8(IVehicleDeclarationInputData consolidatedVehicleData,
			IMultiStageViewModelFactory multistageViewModelFactory)
		{
			ConsolidatedVehicleData = consolidatedVehicleData;
			_multiStageViewModelFactory = multistageViewModelFactory;

			MultistageAirdragViewModel = _multiStageViewModelFactory.GetMultistageAirdragViewModel(consolidatedVehicleData?.Components?.AirdragInputData);
			MultistageAuxiliariesViewModel =
				_multiStageViewModelFactory.GetAuxiliariesViewModel(consolidatedVehicleData?.Components?
					.BusAuxiliaries);
			AirdragModifiedMultistageEditingEnabled = false;
		}

		public IVehicleDeclarationInputData ConsolidatedVehicleData
		{
			get { return _consolidatedVehicleData; }
			set { SetProperty(ref _consolidatedVehicleData, value); }
		}

		public void SetAirdragData(IAirdragDeclarationInputData airdragData)
		{
			throw new NotImplementedException();
			MultistageAirdragViewModel.SetAirdragInputData(airdragInputData: airdragData);
		}

		public void SetBusAuxiliaries(IBusAuxiliariesDeclarationData busAuxData)
		{
			throw new NotImplementedException();
		}

		public void SetVehicleInputData(IVehicleDeclarationInputData vehicleInputData)
		{
			Manufacturer = vehicleInputData.Manufacturer;
			Identifier = vehicleInputData.Identifier;
			ManufacturerAddress = vehicleInputData.ManufacturerAddress;
			VIN = vehicleInputData.VIN;
			Model = vehicleInputData.Model;
			LegislativeClass = vehicleInputData.LegislativeClass;
			CurbMassChassis = vehicleInputData.CurbMassChassis;
			GrossVehicleMassRating = vehicleInputData.GrossVehicleMassRating;
			AirdragModifiedMultistage = vehicleInputData.AirdragModifiedMultistage;
			TankSystem = vehicleInputData.TankSystem;
			RegisteredClass = vehicleInputData.RegisteredClass;
			NumberOfPassengersUpperDeck = vehicleInputData.NumberOfPassengersUpperDeck;
			NumberOfPassengersLowerDeck = vehicleInputData.NumberOfPassengersLowerDeck;
			VehicleCode = vehicleInputData.VehicleCode;
			LowEntry = vehicleInputData.LowEntry;
			Height = vehicleInputData.Height;
			Width = vehicleInputData.Width;
			Length = vehicleInputData.Length;
			EntranceHeight = vehicleInputData.EntranceHeight;
			DoorDriveTechnology = vehicleInputData.DoorDriveTechnology;
			VehicleDeclarationType = vehicleInputData.VehicleDeclarationType;
			AdasEditingEnabled = vehicleInputData.ADAS != null;
			EngineStopStartNullable = vehicleInputData.ADAS?.EngineStopStart;
			EcoRollTypeNullable = vehicleInputData.ADAS?.EcoRoll;
			PredictiveCruiseControlNullable = vehicleInputData.ADAS?.PredictiveCruiseControl;
			ATEcoRollReleaseLockupClutch = vehicleInputData.ADAS?.ATEcoRollReleaseLockupClutch;
			AirdragModifiedMultistage = vehicleInputData.AirdragModifiedMultistage;
			AirdragModifiedMultistageEditingEnabled = false;
			OnPropertyChanged(String.Empty);
			

		}




		#region Implementation used fields in IVehicleInputData

		private string _manufacturer;
		private string _model;
		private string _vin;
		private string _manufacturerAddress;

		private bool _measurementsGroupEditingEnabled = false;
		private bool _numberOfPassengersEditingEnabled = false;
		private int? _numberOfPassengersUpperDeck;
		private int? _numberOfPassengersLowerDeck;
		private Kilogram _grossVehicleMassRating;
		private Meter _entranceHeight;
		private bool? _lowEntry;
		private VehicleCode? _vehicleCode;
		private RegistrationClass? _registeredClass;
		private bool? _airdragModifiedMultistage;
		private bool _airdragModifiedEditingEnabled = false;
		private LegislativeClass? _legislativeClass;
		private ConsumerTechnology? _doorDriveTechnology;
		private TankSystem? _tankSystem;
		private Kilogram _curbMassChassis;
		private Meter _length;
		private Meter _height;
		private Meter _width;


		public string Manufacturer
		{
			get { return _manufacturer; }
			set
			{
				SetProperty(ref _manufacturer, value);
			}
		}

		public string Model
		{
			get { return _model; }
			set { SetProperty(ref _model, value); }
		}

		public string VIN
		{
			get { return _vin; }
			set { SetProperty(ref _vin, value); }
		}

		public string ManufacturerAddress
		{
			get { return String.IsNullOrEmpty(_manufacturerAddress) ? null : _manufacturerAddress; }
			set { SetProperty(ref _manufacturerAddress, value); }
		}

		#region Measurements
		public bool MeasurementsGroupEditingEnabled
		{
			get { return _measurementsGroupEditingEnabled; }
			set { SetProperty(ref _measurementsGroupEditingEnabled, value); }
		}

		public ConvertedSI HeightInMm
		{
			get { return Height?.ConvertToMilliMeter(); }
			set { Height = value?.ConvertToMeter(); }
		}

		public ConvertedSI ConsolidatedHeightInMm
		{
			get { return ConsolidatedVehicleData?.Height?.ConvertToMilliMeter(); }
			set { throw new NotImplementedException(); }
		}

		public Meter Height
		{
			get { return _height; }
			set
			{
				SetProperty(ref _height, value);
				//OnPropertyChanged(nameof(HeightInMm));
			}
		}

		public ConvertedSI LengthInMm
		{
			get { return Length?.ConvertToMilliMeter(); }
			set { Length = value?.ConvertToMeter(); }
		}

		public ConvertedSI ConsolidatedLengthInMm
		{
			get { return ConsolidatedVehicleData?.Length?.ConvertToMilliMeter(); }
			set { throw new NotImplementedException(); }
		}


		public Meter Length
		{
			get { return _length; }
			set
			{
				SetProperty(ref _length, value);
				//OnPropertyChanged(nameof(LengthInMm));
			}
		}

		public ConvertedSI WidthInMm
		{
			get { return Width?.ConvertToMilliMeter(); }
			set { Width = value?.ConvertToMeter(); }
		}

		public ConvertedSI ConsolidatedWidthInMm
		{
			get { return ConsolidatedVehicleData?.Width?.ConvertToMilliMeter(); }
			set { throw new NotImplementedException(); }
		}


		public Meter Width
		{
			get { return _width; }
			set
			{
				SetProperty(ref _width, value);
				//OnPropertyChanged(nameof(WidthInMm));
			}
		}

		public ConvertedSI ConsolidatedEntranceHeightInMm
		{
			get { return ConsolidatedVehicleData?.EntranceHeight?.ConvertToMilliMeter(); }
			set { throw new NotImplementedException(); }
		}

		public ConvertedSI EntranceHeightInMm
		{
			get { return EntranceHeight?.ConvertToMilliMeter(); }
			set
			{
				EntranceHeight = value?.ConvertToMeter(); 
				
			}
		}


		public Meter EntranceHeight
		{
			get => _entranceHeight;
			set
			{
				SetProperty(ref _entranceHeight, value);
				//OnPropertyChanged(nameof(EntranceHeightInMm));
			}
		}

		#endregion
		public Kilogram CurbMassChassis //Corrected Actual Mass
		{
			get { return _curbMassChassis; }
			set { SetProperty(ref _curbMassChassis, value); }
		}

		public bool NumberOfPassengersEditingEnabled
		{
			get { return _numberOfPassengersEditingEnabled; }
			set { SetProperty(ref _numberOfPassengersEditingEnabled, value); }
		}


		public int? NumberOfPassengersUpperDeck
		{
			get { return _numberOfPassengersUpperDeck; }
			set { SetProperty(ref _numberOfPassengersUpperDeck, value); }
		}

		public int? NumberOfPassengersLowerDeck
		{
			get { return _numberOfPassengersLowerDeck; }
			set { SetProperty(ref _numberOfPassengersLowerDeck, value); }
		}


		public TankSystem? TankSystem
		{
			get { return _tankSystem; }
			set { SetProperty(ref _tankSystem, value); }
		}

		public Kilogram GrossVehicleMassRating //Technical Permissible Maximum Laden Mass
		{
			get => _grossVehicleMassRating;
			set => SetProperty(ref _grossVehicleMassRating, value);
		}

		public ConsumerTechnology? DoorDriveTechnology
		{
			get => _doorDriveTechnology;
			set => SetProperty(ref _doorDriveTechnology, value);
		}

		public LegislativeClass? LegislativeClass
		{
			get => _legislativeClass;
			set => SetProperty(ref _legislativeClass, value);
		}

        #region AirdragModified
		

        public AIRDRAGMODIFIED? AirdragModifiedEnum
		{
			get
			{
				if (AirdragModifiedMultistageEditingEnabled) {
					return _airdragModifiedMultistage.toAirdragModifiedEnum();
				} else {
					return null;
				}
			}
			set {
				AirdragModifiedMultistage = value?.toNullableBool();
			}
		}

		public AIRDRAGMODIFIED? ConsolidatedAirdragmodified
		{
			get
			{
				if (_consolidatedVehicleData?.AirdragModifiedMultistage != null) {
					return _consolidatedVehicleData.AirdragModifiedMultistage.toAirdragModifiedEnum();
				} else {
					return null;
				}
			}
			set => throw new NotImplementedException();
		}

		public bool? AirdragModifiedMultistage
		{
			get => _airdragModifiedMultistage;
			set => SetProperty(ref _airdragModifiedMultistage, value);
		}

		
		public bool AirdragModifiedMultistageEditingEnabled
		{
			get
			{
				//IF MODIFIED ONCE IT HAS TO BE SET
				if (_consolidatedVehicleData?.AirdragModifiedMultistage != null) {
					_airdragModifiedEditingEnabled = true;
				}
				return _airdragModifiedEditingEnabled;
			}
			set{
				SetProperty(ref _airdragModifiedEditingEnabled, value);
			}
				
		}
		#endregion;

		public RegistrationClass? RegisteredClass
		{
			get => _registeredClass;
			set => SetProperty(ref _registeredClass, value);
		}

		public VehicleCode? VehicleCode
		{
			get => _vehicleCode;
			set => SetProperty(ref _vehicleCode, value);
		}

		public ObservableCollection<Enum> VehicleCodeAllowedValues { get; } = 
			new ObservableCollection<Enum>(Enum.GetValues(typeof(VehicleCode)).Cast<Enum>().ToList().Where(
				(e => (VehicleCode)e != TUGraz.VectoCommon.Models.VehicleCode.NOT_APPLICABLE)));


        public bool? LowEntry
		{
			get => _lowEntry;
			set => SetProperty(ref _lowEntry, value);
		}

		



		public VehicleDeclarationType VehicleDeclarationType
		{
			get => _vehicleDeclarationType;
			set => SetProperty(ref _vehicleDeclarationType, value);
		}

		#endregion


		private string _identifier;
		private IVehicleDeclarationInputData _consolidatedVehicleData;
		private VehicleDeclarationType _vehicleDeclarationType;


		#region implementation of IVehicleComponentsDeclaration

		public IAirdragDeclarationInputData AirdragInputData
		{
			get => MultistageAirdragViewModel.AirDragViewModel;
		}

		public IBusAuxiliariesDeclarationData BusAuxiliaries
		{
			get => MultistageAuxiliariesViewModel.HasValues ? MultistageAuxiliariesViewModel : null;
		}

		#region not implemented

		public string LegislativeCategory
		{
			get => throw new NotImplementedException();
		}
		public IGearboxDeclarationInputData GearboxInputData => throw new NotImplementedException();

		public ITorqueConverterDeclarationInputData TorqueConverterInputData => throw new NotImplementedException();

		public IAxleGearInputData AxleGearInputData => throw new NotImplementedException();

		public IAngledriveInputData AngledriveInputData => throw new NotImplementedException();

		public IEngineDeclarationInputData EngineInputData => throw new NotImplementedException();

		public IAuxiliariesDeclarationInputData AuxiliaryInputData => throw new NotImplementedException();

		public IRetarderInputData RetarderInputData => throw new NotImplementedException();

		public IPTOTransmissionInputData PTOTransmissionInputData => throw new NotImplementedException();

		public IAxlesDeclarationInputData AxleWheels => throw new NotImplementedException();


		public IElectricStorageDeclarationInputData ElectricStorage => throw new NotImplementedException();

		public IElectricMachinesDeclarationInputData ElectricMachines => throw new NotImplementedException();

		#endregion

		#endregion



		#region implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		private PredictiveCruiseControlType _predictiveCruiseControl;
		private bool? _atEcoRollReleaseLockupClutch;
		private EcoRollType _ecoRoll;
		private bool _engineStopStart;
		private bool _adasEditingEnabled;
		private bool? _engineStopStartNullable;
		private EcoRollType? _ecoRollTypeNullable;
		private PredictiveCruiseControlType? _predictiveCruiseControlNullable;



		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get
			{
				if (EngineStopStartNullable.HasValue 
					|| EcoRollTypeNullable.HasValue
					|| PredictiveCruiseControlNullable.HasValue
					|| ATEcoRollReleaseLockupClutch.HasValue) {
					return this;
				} else {
					return null;
				}
			;
		}

				
			
		}

		public bool AdasEditingEnabled
		{
			get => _adasEditingEnabled;
			set
			{
				SetProperty(ref _adasEditingEnabled, value);
			}
		}

		public bool? EngineStopStartNullable
		{
			get => _engineStopStartNullable;
			set
			{
				SetProperty(ref _engineStopStartNullable, value);
			}
		}

		public bool EngineStopStart
		{
			get
			{
				return _engineStopStartNullable.HasValue ? _engineStopStartNullable.Value : false;
			}
		}

		public EcoRollType? EcoRollTypeNullable
		{
			get => _ecoRollTypeNullable;
			set
			{
				SetProperty(ref _ecoRollTypeNullable, value);
			}
		}

		public EcoRollType EcoRoll
		{
			get
			{
				return _ecoRollTypeNullable.HasValue ? _ecoRollTypeNullable.Value : EcoRollType.None;
			}
		}

		public PredictiveCruiseControlType? PredictiveCruiseControlNullable
		{
			get => _predictiveCruiseControlNullable;
			set
			{
				SetProperty(ref _predictiveCruiseControlNullable, value);
			}
		}

		public PredictiveCruiseControlType PredictiveCruiseControl
		{
			get
			{
				return _predictiveCruiseControlNullable.HasValue
					? _predictiveCruiseControlNullable.Value
					: PredictiveCruiseControlType.None;
			}
		}

		public bool? ATEcoRollReleaseLockupClutch
		{
			get => _atEcoRollReleaseLockupClutch;
			set
			{
				SetProperty(ref _atEcoRollReleaseLockupClutch, value);
			}
		}

		#endregion


		#region implementation of IVehicleDeclarationInputData;

		public DateTime Date
		{
			get => DateTime.Today;
		}

		public string AppVersion
		{
			get { throw new NotImplementedException(); }
		}

		public CertificationMethod CertificationMethod
		{
			get { throw new NotImplementedException(); }
		}

		public string CertificationNumber
		{
			get { throw new NotImplementedException(); }
		}

		public DigestData DigestValue
		{
			get { throw new NotImplementedException(); }
		}

		public string Identifier
		{
			get
			{
				return _identifier;
			}
			private set
			{
				SetProperty(ref _identifier, value);
			}
		}

		public bool ExemptedVehicle
		{
			get { throw new NotImplementedException(); }
		}



		public VehicleCategory VehicleCategory
		{
			get { throw new NotImplementedException(); }
		}

		public AxleConfiguration AxleConfiguration
		{
			get { throw new NotImplementedException(); }
		}




		public IList<ITorqueLimitInputData> TorqueLimits
		{
			get { throw new NotImplementedException(); }
		}


		public PerSecond EngineIdleSpeed
		{
			get { throw new NotImplementedException(); }
		}

		public bool VocationalVehicle
		{
			get { throw new NotImplementedException(); }
		}

		public bool SleeperCab
		{
			get { throw new NotImplementedException(); }
		}







		public bool ZeroEmissionVehicle
		{
			get { throw new NotImplementedException(); }
		}

		public bool HybridElectricHDV
		{
			get { throw new NotImplementedException(); }
		}

		public bool DualFuelVehicle
		{
			get { throw new NotImplementedException(); }
		}

		public Watt MaxNetPower1
		{
			get { throw new NotImplementedException(); }
		}

		public Watt MaxNetPower2
		{
			get { throw new NotImplementedException(); }
		}



		public CubicMeter CargoVolume
		{
			get { throw new NotImplementedException(); }
		}


		public bool Articulated
		{
			get { throw new NotImplementedException(); }
		}





		public IVehicleComponentsDeclaration Components
		{
			get
			{
				if (AirdragInputData != null || BusAuxiliaries != null) {
					return this;
				} else {
					return null;
				}
			}
		}

		
		public XmlNode XMLSource
		{
			get { throw new NotImplementedException(); }
		}

		public IPTOViewModel PTOViewModel
		{
			get { throw new NotImplementedException(); }
		}

		public RetarderType RetarderType
		{
			get { throw new NotImplementedException(); }
		}

		public double RetarderRatio
		{
			get { throw new NotImplementedException(); }
		}

		public AngledriveType AngledriveType
		{
			get { throw new NotImplementedException(); }
		}

		#endregion;



		#region Implementation of IDataErrorInfo

		public Dictionary<string, string> Errors { get; private set; } = new Dictionary<string, string>();

		public string this[string propertyName]
		{
			get
			{
				string result = null;
				switch (propertyName) {
					case nameof(Manufacturer):
						if (string.IsNullOrWhiteSpace(Manufacturer)) {
							result = "Manufacturer cannot be empty";
						}
						
						break;
					case nameof(ManufacturerAddress):
						if (string.IsNullOrWhiteSpace(ManufacturerAddress))
						{
							result = "Manufacturer address cannot be empty";
						}
						break;
					case nameof(VIN):
						if (string.IsNullOrEmpty(VIN)) {
							result = "VIN cannot be empty";
						}
						break;
					case nameof(AirdragModifiedEnum):
						if (AirdragModifiedMultistageEditingEnabled && (AirdragModifiedEnum == AIRDRAGMODIFIED.UNKNOWN)) {
							result = "Air drag modified has to be set";
						}
						break;
				}
				//https://www.youtube.com/watch?v=5KF0GGObuAQ

				if (result == null) {
					if(Errors.ContainsKey(propertyName))
					Errors.Remove(propertyName);
				} else {
					Errors[propertyName] = result;
				}
				

				return result;
			}
		}

		public string Error { get => String.Join(",", Errors.Values); }
		public bool HasErrors
		{
			get
			{
				return !Error.IsNullOrEmpty();
			}
		}
		#endregion
	}
}