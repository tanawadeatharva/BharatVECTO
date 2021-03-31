using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
    class DeclarationInterimStageBusVehicleViewModel_v2_8 : ViewModelBase, IVehicleViewModel
	{
		public static String VERSION = typeof(XMLDeclarationInterimStageBusDataProviderV28).ToString();

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


		private string _manufacturer;
		private string _model;
		private string _vin;
		private string _manufacturerAddress;

		public string Manufacturer
		{
			get { return _manufacturer; }
			set { SetProperty(ref _manufacturer, value); }
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

		private bool _measurementsGroupEditingEnabled = false;
		public bool MeasurementsGroupEditingEnabled
		{
			get { return _measurementsGroupEditingEnabled; }
			set { SetProperty(ref _measurementsGroupEditingEnabled, value); }
		}

		private Meter _height;
		public Meter Height
		{
			get { return _height; }
			set { SetProperty(ref _height, value); }
		}

		private Meter _length;
		public Meter Length
		{
			get { return _length; }
			set { SetProperty(ref _length, value); }
		}

		private Meter _width;
		public Meter Width
		{
			get { return _width; }
			set { SetProperty(ref _width, value); }
		}

		private Kilogram _curbMassChassis;

		public Kilogram CurbMassChassis
		{
			get { return _curbMassChassis; }
			set { SetProperty(ref _curbMassChassis, value); }
		}

		private bool __numberOfPassengersEditingEnabled = false;
		private int? _numberOfPassengersUpperDeck;
		private int? _numberOfPassengersLowerDeck;
		public bool NumberOfPassengersEditingEnabled
		{
			get { return __numberOfPassengersEditingEnabled; }
			set { SetProperty(ref __numberOfPassengersEditingEnabled, value); }
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

		private TankSystem? _tankSystem;
		public TankSystem? TankSystem
		{
			get { return _tankSystem; }
			set { SetProperty(ref _tankSystem, value); }
		}


		private IList<IComponentViewModel> _componentViewModels;
		private IVehicleDeclarationInputData _inputData;




		public ObservableCollection<IComponentViewModel> ComponentViewModels
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}


		public DeclarationInterimStageBusVehicleViewModel_v2_8(IVehicleDeclarationInputData inputData)
		{
			_inputData = inputData;


			_manufacturer = inputData.Manufacturer;
			_manufacturerAddress = inputData.ManufacturerAddress;
			_vin = inputData.VIN;
			_width = inputData.Width;
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


		public LegislativeClass? LegislativeClass
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


		public Kilogram GrossVehicleMassRating
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

		public bool? AirdragModifiedMultistage
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

		public RegistrationClass? RegisteredClass
		{
			get { throw new NotImplementedException(); }
		}


		public CubicMeter CargoVolume
		{
			get { throw new NotImplementedException(); }
		}

		public VehicleCode? VehicleCode
		{
			get { throw new NotImplementedException(); }
		}

		public bool? LowEntry
		{
			get { throw new NotImplementedException(); }
		}

		public bool Articulated
		{
			get { throw new NotImplementedException(); }
		}

		public Meter EntranceHeight
		{
			get { throw new NotImplementedException(); }
		}

		public ConsumerTechnology? DoorDriveTechnology
		{
			get { throw new NotImplementedException(); }
		}

		public VehicleDeclarationType VehicleDeclarationType
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

		#endregion;
	}
}
