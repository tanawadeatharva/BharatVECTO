using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{
	public class CompleteVehicleBusViewModel : AbstractViewModel, ICompleteVehicleBusViewModel
	{

		#region Members

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
			set { SetProperty(ref _manufacturer, value); }
		}
		public string ManufacturerAddress
		{
			get { return _manufacturerAddress; }
			set { SetProperty(ref _manufacturerAddress, value); }
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
		public DateTime Date
		{
			get { return _date; }
			set { SetProperty(ref _date, value); }
		}
		public LegislativeClass LegislativeClass
		{
			get { return _legislativeClass; }
			set { SetProperty(ref _legislativeClass, value); }
		}
		public RegistrationClass RegisteredClass
		{
			get { return _registeredClass; }
			set { SetProperty(ref _registeredClass, value); }
		}
		public VehicleCode VehicleCode
		{
			get { return _vehicleCode; }
			set { SetProperty(ref _vehicleCode, value); }
		}
		public Kilogram CurbMassChassis
		{
			get { return _curbMassChassis; }
			set { SetProperty(ref _curbMassChassis, value); }
		}
		public Kilogram TechnicalPermissibleMaximumLadenMass
		{
			get { return _technicalPermissibleMaximumLadenMass; }
			set { SetProperty(ref _technicalPermissibleMaximumLadenMass, value); }
		}
		public int NumberOfPassengersLowerDeck
		{
			get { return _numberOfPassengersLowerDeck; }
			set { SetProperty(ref _numberOfPassengersLowerDeck, value); }
		}
		public int NumberOfPassengersUpperDeck
		{
			get { return _numberOfPassengersUpperDeck; }
			set { SetProperty(ref _numberOfPassengersUpperDeck, value); }
		}
		public FloorType FloorType
		{
			get { return _floorType;}
			set { SetProperty(ref _floorType, value); }
		}
		public Meter HeightIntegratedBody
		{
			get { return _heightIntegratedBody;}
			set { SetProperty(ref _heightIntegratedBody, value); }
		}

		public Meter VehicleLength
		{
			get { return _vehicleLength;}
			set { SetProperty(ref _vehicleLength, value); }
		}
		public Meter VehicleWidth
		{
			get { return _vehicleWidth;}
			set { SetProperty(ref _vehicleWidth, value); }
		}
		public Meter EntranceHeight
		{
			get { return _entranceHeight;}
			set { SetProperty(ref _entranceHeight, value); }
		}
		public ConsumerTechnology DoorDriveTechnology
		{
			get { return _doorDriveTechnology;}
			set { SetProperty(ref _doorDriveTechnology, value); }
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
			SetVehicleData(inputData?.JobInputData.Vehicle);
			SetAllowedEntries();
		}

		private void SetVehicleData(IVehicleDeclarationInputData vehicle)
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

	}
}
