using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.ViewModel.Impl;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.Model.TempDataObject
{
	public class VehicleBusComponentData : ICompleteVehicleBus, ITempDataObject<ICompleteVehicleBusViewModel>
	{
		#region ICompleteVehicleBus Interface

		public string Manufacturer { get; set; }
		public string ManufacturerAddress { get; set; }
		public string Model { get; set; }
		public string VIN { get; set; }
		public DateTime Date { get; set; }
		public LegislativeClass LegislativeClass { get; set; }
		public RegistrationClass RegisteredClass { get; set; }
		public VehicleCode VehicleCode { get; set; }
		public Kilogram CurbMassChassis { get; set; }
		public Kilogram TechnicalPermissibleMaximumLadenMass { get; set; }
		public int NumberOfPassengersLowerDeck { get; set; }
		public int NumberOfPassengersUpperDeck { get; set; }
		public FloorType FloorType { get; set; }
		public Meter HeightIntegratedBody { get; set; }
		public Meter VehicleLength { get; set; }
		public Meter VehicleWidth { get; set; }
		public Meter EntranceHeight { get; set; }
		public ConsumerTechnology DoorDriveTechnology { get; set; }

		#endregion

		public VehicleBusComponentData(ICompleteVehicleBusViewModel viewModel, bool defaultValues)
		{
			if(defaultValues)
				ClearValues(viewModel);
		}

		public VehicleBusComponentData(ICompleteVehicleBusViewModel viewModel)
		{
			SetCurrentValues(viewModel);
		}

		public void UpdateCurrentValues(ICompleteVehicleBusViewModel viewModel)
		{
			SetCurrentValues(viewModel);
		}

		public void ResetToComponentValues(ICompleteVehicleBusViewModel viewModel)
		{
			viewModel.Manufacturer = Manufacturer;
			viewModel.ManufacturerAddress = ManufacturerAddress;
			viewModel.Model = Model;
			viewModel.VIN = VIN;
			viewModel.Date = Date;
			viewModel.LegislativeClass = LegislativeClass;
			viewModel.RegisteredClass = RegisteredClass;
			viewModel.VehicleCode = VehicleCode;
			viewModel.CurbMassChassis = CurbMassChassis;
			viewModel.TechnicalPermissibleMaximumLadenMass = TechnicalPermissibleMaximumLadenMass;
			viewModel.NumberOfPassengersLowerDeck = NumberOfPassengersLowerDeck;
			viewModel.NumberOfPassengersUpperDeck = NumberOfPassengersUpperDeck;
			viewModel.FloorType = FloorType;
			viewModel.HeightIntegratedBody = HeightIntegratedBody;
			viewModel.VehicleLength = VehicleLength;
			viewModel.VehicleWidth = VehicleWidth;
			viewModel.EntranceHeight = EntranceHeight;
			viewModel.DoorDriveTechnology = DoorDriveTechnology;
		}

		public void ClearValues(ICompleteVehicleBusViewModel viewModel)
		{
			viewModel.Manufacturer = default(string);
			viewModel.ManufacturerAddress = default(string);
			viewModel.Model = default(string);
			viewModel.VIN = default(string);
			viewModel.Date = default(DateTime);
			viewModel.LegislativeClass = default(LegislativeClass);
			viewModel.RegisteredClass = default(RegistrationClass);
			viewModel.VehicleCode = default(VehicleCode);
			viewModel.CurbMassChassis = default(Kilogram);
			viewModel.TechnicalPermissibleMaximumLadenMass = default(Kilogram);
			viewModel.NumberOfPassengersLowerDeck = default(int);
			viewModel.NumberOfPassengersUpperDeck = default(int);
			viewModel.FloorType = default(FloorType);
			viewModel.HeightIntegratedBody = default(Meter);
			viewModel.VehicleLength = default(Meter);
			viewModel.VehicleWidth = default(Meter);
			viewModel.EntranceHeight = default(Meter);
			viewModel.DoorDriveTechnology = default(ConsumerTechnology);
		}

		
		private void SetCurrentValues(ICompleteVehicleBusViewModel vehicleBus)
		{
			Manufacturer = vehicleBus.Manufacturer;
			ManufacturerAddress = vehicleBus.ManufacturerAddress;
			Model = vehicleBus.Model;
			VIN = vehicleBus.VIN;
			Date = vehicleBus.Date;
			LegislativeClass = vehicleBus.LegislativeClass;
			RegisteredClass = vehicleBus.RegisteredClass;
			VehicleCode = vehicleBus.VehicleCode;
			CurbMassChassis = vehicleBus.CurbMassChassis;
			TechnicalPermissibleMaximumLadenMass = vehicleBus.TechnicalPermissibleMaximumLadenMass;
			NumberOfPassengersLowerDeck = vehicleBus.NumberOfPassengersLowerDeck;
			NumberOfPassengersUpperDeck = vehicleBus.NumberOfPassengersUpperDeck;
			FloorType = vehicleBus.FloorType;
			HeightIntegratedBody = vehicleBus.HeightIntegratedBody;
			VehicleLength = vehicleBus.VehicleLength;
			VehicleWidth = vehicleBus.VehicleWidth;
			EntranceHeight = vehicleBus.EntranceHeight;
			DoorDriveTechnology = vehicleBus.DoorDriveTechnology;
		}

	}
}
