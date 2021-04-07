using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Castle.Core.Internal;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
    class DeclarationInterimStageBusVehicleViewModel_v2_8 : ViewModelBase, IVehicleViewModel
	{
		public static readonly string INPUTPROVIDERTYPE = typeof(XMLDeclarationInterimStageBusDataProviderV28).ToString();

		public string Name
		{
			get { return "Vehicle"; }
		}

		public bool IsPresent
		{
			get { return true; }
		}

		public DataSource DataSource
		{
			get { throw new NotImplementedException(); }
		}

		public bool SavedInDeclarationMode
		{
			get { throw new NotImplementedException(); }
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


		public bool MeasurementsGroupEditingEnabled
		{
			get { return _measurementsGroupEditingEnabled; }
			set { SetProperty(ref _measurementsGroupEditingEnabled, value); }
		}


		public Meter Height
		{
			get { return _height; }
			set { SetProperty(ref _height, value); }
		}


		public Meter Length
		{
			get { return _length; }
			set { SetProperty(ref _length, value); }
		}

		public Meter Width
		{
			get { return _width; }
			set { SetProperty(ref _width, value); }
		}


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
			get =>_grossVehicleMassRating;
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

		public bool? AirdragModifiedMultistage
		{
			get => _airdragModifiedMultistage;
			set => SetProperty(ref _airdragModifiedMultistage, value);
		}

		public RegistrationClass? RegisteredClass
		{
			get => _registeredClass;
			set => SetProperty(ref _registeredClass , value);
		}

		public VehicleCode? VehicleCode
		{
			get => _vehicleCode;
			set => SetProperty(ref _vehicleCode, value);
		}

		public bool? LowEntry
		{
			get => _lowEntry;
			set => SetProperty(ref _lowEntry, value);
		}


		public Meter EntranceHeight
		{
			get => _entranceHeight;
			set => SetProperty(ref _entranceHeight, value);
		}

		public VehicleDeclarationType VehicleDeclarationType
		{
			get => _vehicleDeclarationType;
			set => SetProperty(ref _vehicleDeclarationType, value);
		}

		#endregion

		private IList<IComponentViewModel> _componentViewModels;
		private IVehicleDeclarationInputData _inputData;

		private Dictionary<string, object> _accumulatedProperties = new Dictionary<string, object>();
		private IVehicleViewModel _prevStageVehicleViewModel;
		private IVehicleDeclarationInputData _consolidatedVehicleData;
		private VehicleDeclarationType _vehicleDeclarationType;

		public ObservableCollection<IComponentViewModel> ComponentViewModels
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public IVehicleViewModel PreviousStageVehicleViewModel
		{
			get => _prevStageVehicleViewModel;
		}


		public DeclarationInterimStageBusVehicleViewModel_v2_8()
		{

		}


		public DeclarationInterimStageBusVehicleViewModel_v2_8(IVehicleDeclarationInputData consolidatedVehicleData, IMultiStageViewModelFactory vmFactory)
		{
			ConsolidatedVehicleData = consolidatedVehicleData;
		}

		public IVehicleDeclarationInputData ConsolidatedVehicleData
		{
			get { return _consolidatedVehicleData; }
			set { SetProperty(ref _consolidatedVehicleData, value); }
		}


		#region implementation of IVehicleDeclarationInputData;
		public DateTime Date
		{
			get { throw new NotImplementedException(); }
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
			get { throw new NotImplementedException(); }
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





		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS
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
			get { throw new NotImplementedException(); }
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

		public IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { throw new NotImplementedException(); }
		}

		public Dictionary<string, object> AccumulatedProperties
		{
			get => _accumulatedProperties;
			set => throw new NotImplementedException();
		}

		#endregion;
	}
}
