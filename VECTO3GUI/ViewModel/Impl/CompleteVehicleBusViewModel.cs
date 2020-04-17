using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI.Model.TempDataObject;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{
	public class CompleteVehicleBusViewModel : AbstractComponentViewModel, ICompleteVehicleBusViewModel
	{
		#region Members

		private IVehicleDeclarationInputData _vehicle;
		private VehicleBusComponentData _componentData;

		private string _manufacturer;
		private string _manufacturerAddress;
		private string _model;
		private string _vin;
		private DateTime _date;
		private LegislativeClass _legislativeClass;
		private RegistrationClass _registeredClass;
		private VehicleCode _vehicleCode;
		private Kilogram _curbMassChassis;
		private Kilogram _technicalPermissibleMaximumLadenMass;
		private int _numberOfPassengersLowerDeck;
		private int _numberOfPassengersUpperDeck;
		private FloorType _floorType;
		private Meter _heightIntegratedBody;
		private Meter _vehicleLength;
		private Meter _vehicleWidth;
		private Meter _entranceHeight;
		private ConsumerTechnology _doorDriveTechnology;


		#endregion
		
		#region ICompleteVehicleBusViewModel

		public string Manufacturer
		{
			get { return _manufacturer; }
			set
			{
				if (SetProperty(ref _manufacturer, value)) {
					var changed = _vehicle != null
								? _vehicle.Manufacturer != value
								: value != default(string);
					SetChangedProperty(changed);
				}
			}
		}
		public string ManufacturerAddress
		{
			get { return _manufacturerAddress; }
			set
			{
				if (SetProperty(ref _manufacturerAddress, value)) {
					var changed = _vehicle != null
								? _vehicle.ManufacturerAddress != value
								: value != default(string);
					SetChangedProperty(changed);
				}
			}
		}
		public string Model
		{
			get { return _model; }
			set
			{
				if (SetProperty(ref _model, value)) {
					var changed = _vehicle != null
								? _vehicle.Model != value
								: value != default(string);
					SetChangedProperty(changed);
				}
			}
		}
		public string VIN
		{
			get { return _vin; }
			set
			{
				if (SetProperty(ref _vin, value)) {
					var changed = _vehicle != null
								? _vehicle.VIN != value
								: value != default(string);
					SetChangedProperty(changed);
				}
			}
		}
		public DateTime Date
		{
			get { return _date; }
			set
			{
				if (SetProperty(ref _date, value)) {
					var changed = _vehicle != null
								? _vehicle.Date != value
								: value != default(DateTime);
					SetChangedProperty(changed);
				}
			}
		}
		public LegislativeClass LegislativeClass
		{
			get { return _legislativeClass; }
			set
			{
				if (SetProperty(ref _legislativeClass, value)) {
					var changed = _vehicle != null
								? _vehicle.LegislativeClass != value
								: value != default(LegislativeClass);
					SetChangedProperty(changed);
				}
			}
		}
		public RegistrationClass RegisteredClass
		{
			get { return _registeredClass; }
			set
			{
				if (SetProperty(ref _registeredClass, value)) {
					var changed = _vehicle != null
								? _vehicle.RegisteredClass != value
								: value != default(RegistrationClass);
					SetChangedProperty(changed);
				}
			}
		}
		public VehicleCode VehicleCode
		{
			get { return _vehicleCode; }
			set {
				if (SetProperty(ref _vehicleCode, value)) {
					var changed = _vehicle != null
								? _vehicle.VehicleCode != value
								: value != default(VehicleCode);
					SetChangedProperty(changed);
				}
			}
		}
		public Kilogram CurbMassChassis
		{
			get { return _curbMassChassis; }
			set
			{
				if (SetProperty(ref _curbMassChassis, value)) {
					var changed = _vehicle != null
								? _vehicle.CurbMassChassis != value
								: value != default(Kilogram);
					SetChangedProperty(changed);
				}
			}
		}
		public Kilogram TechnicalPermissibleMaximumLadenMass
		{
			get { return _technicalPermissibleMaximumLadenMass; }
			set
			{
				if (SetProperty(ref _technicalPermissibleMaximumLadenMass, value)) {
					var changed = _vehicle != null
								? _vehicle.GrossVehicleMassRating != value
								: value != default(Kilogram);
					SetChangedProperty(changed);
				}
			}
		}
		public int NumberOfPassengersLowerDeck
		{
			get { return _numberOfPassengersLowerDeck; }
			set
			{
				if (SetProperty(ref _numberOfPassengersLowerDeck, value)) {
					var changed = _vehicle != null
								? _vehicle.NumberOfPassengersLowerDeck != value
								: value != default(int);
					SetChangedProperty(changed);
				}
			}
		}
		public int NumberOfPassengersUpperDeck
		{
			get { return _numberOfPassengersUpperDeck; }
			set
			{
				if (SetProperty(ref _numberOfPassengersUpperDeck, value)) {
					var changed = _vehicle != null
								? _vehicle.NuberOfPassengersUpperDeck != value
								: value != default(int);
					SetChangedProperty(changed);	
				}
			}
		}
		public FloorType FloorType
		{
			get { return _floorType; }
			set
			{
				if (SetProperty(ref _floorType, value)) {
					var changed = _vehicle != null
								? _vehicle.FloorType != value
								: value != default(FloorType);
					SetChangedProperty(changed);
				}
			}
		}
		public Meter HeightIntegratedBody
		{
			get { return _heightIntegratedBody; }
			set
			{
				if (SetProperty(ref _heightIntegratedBody, value)) {
					var changed = _vehicle != null
								? _vehicle.Height != value
								: value != default(Meter);
					SetChangedProperty(changed);
				}
			}
		}

		public Meter VehicleLength
		{
			get { return _vehicleLength; }
			set
			{
				if (SetProperty(ref _vehicleLength, value)) {
					var changed = _vehicle != null
								? _vehicle.Length != value
								: value != default(Meter);
					SetChangedProperty(changed);
				}
			}
		}
		public Meter VehicleWidth
		{
			get { return _vehicleWidth; }
			set
			{
				if (SetProperty(ref _vehicleWidth, value)) {
					var changed = _vehicle != null
								? _vehicle.Width != value
								: value != default(Meter);
					SetChangedProperty(changed);
				}
			}
		}
		public Meter EntranceHeight
		{
			get { return _entranceHeight; }
			set
			{
				if (SetProperty(ref _entranceHeight, value)) {
					var changed = _vehicle != null
								? _vehicle.EntranceHeight != value
								: value != default(Meter);
					SetChangedProperty(changed);
				}
			}
		}
		public ConsumerTechnology DoorDriveTechnology
		{
			get { return _doorDriveTechnology; }
			set
			{
				if (SetProperty(ref _doorDriveTechnology, value)) {
					var changed = _vehicle != null
						? ((XMLDeclarationCompletedBusDataProviderV26)_vehicle).DoorDriveTechnology != value
						: value != default(ConsumerTechnology);
					SetChangedProperty(changed);
				}
			}
		}

		public AllowedEntry<LegislativeClass>[] AllowedLegislativeClasses { get; private set; }
		public AllowedEntry<VehicleCode>[] AllowedVehicleCodes { get; private set; }
		public AllowedEntry<FloorType>[] AllowedFloorTypes { get; private set; }
		public AllowedEntry<ConsumerTechnology>[] AllowedConsumerTechnologies { get; private set; }

		#endregion


		#region Set XML Data

		protected override void InputDataChanged()
		{
			var inputData = JobViewModel.InputDataProvider as IDeclarationInputDataProvider;
			_vehicle = inputData?.JobInputData.Vehicle;
			_changedInput = new HashSet<string>();
			SetVehicleValues(_vehicle);
			SetAllowedEntries();
		}

		private void SetVehicleValues(IVehicleDeclarationInputData vehicle)
		{
			if (vehicle == null)
				return;

			Manufacturer = vehicle.Manufacturer;
			ManufacturerAddress = vehicle.ManufacturerAddress;
			Model = vehicle.Model;
			VIN = vehicle.VIN;
			Date = vehicle.Date;
			LegislativeClass = vehicle.LegislativeClass;
			RegisteredClass = vehicle.RegisteredClass;
			VehicleCode = vehicle.VehicleCode;
			CurbMassChassis = vehicle.CurbMassChassis;
			TechnicalPermissibleMaximumLadenMass = vehicle.GrossVehicleMassRating;
			NumberOfPassengersLowerDeck = vehicle.NumberOfPassengersLowerDeck;
			NumberOfPassengersUpperDeck = vehicle.NuberOfPassengersUpperDeck;
			FloorType = vehicle.FloorType;
			HeightIntegratedBody = vehicle.Height;
			VehicleLength = vehicle.Length;
			VehicleWidth = vehicle.Width;
			EntranceHeight = vehicle.EntranceHeight;
			DoorDriveTechnology = ((XMLDeclarationCompletedBusDataProviderV26)vehicle).DoorDriveTechnology;
		}

		private void SetAllowedEntries()
		{

			AllowedLegislativeClasses = Enum.GetValues(typeof(LegislativeClass)).Cast<LegislativeClass>()
				.Select(lc => AllowedEntry.Create(lc, lc.GetLabel())).ToArray();

			AllowedVehicleCodes = Enum.GetValues(typeof(VehicleCode)).Cast<VehicleCode>()
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();

			AllowedFloorTypes = Enum.GetValues(typeof(FloorType)).Cast<FloorType>()
				.Select(ft => AllowedEntry.Create(ft, ft.GetLabel())).ToArray();

			AllowedConsumerTechnologies = Enum.GetValues(typeof(ConsumerTechnology)).Cast<ConsumerTechnology>()
				.Select(sc => AllowedEntry.Create(sc, sc.GetLabel())).ToArray();
		}

		#endregion

		public override object SaveComponentData()
		{
			if(_componentData == null)
				_componentData = new VehicleBusComponentData(this);
			else
				_componentData.UpdateCurrentValues(this);

			return _componentData;
		}


		public override bool IsComponentDataChanged()
		{
			return _changedInput.Count > 0;
		}

		public override void ResetComponentData()
		{
			SetVehicleValues(_vehicle);
		}
	}

}
