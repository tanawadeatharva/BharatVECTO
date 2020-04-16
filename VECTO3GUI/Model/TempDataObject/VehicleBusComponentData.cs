using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.Model.TempDataObject
{
	public class VehicleBusComponentData : ICompleteVehicleBus
	{
		#region Properties
		
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


		public VehicleBusComponentData(ICompleteVehicleBusViewModel vehicleBus)
		{
			SetCurrentValues(vehicleBus);
		}

		public void UpdateCurrentValues(ICompleteVehicleBusViewModel vehicleBus)
		{
			SetCurrentValues(vehicleBus);
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
