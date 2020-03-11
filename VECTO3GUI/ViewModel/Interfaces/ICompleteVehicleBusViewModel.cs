using System;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface ICompleteVehicleBusViewModel : IComponentViewModel
	{
		#region CompleteVehicleBus

		string Manufacturer { get; set; }
		string ManufacturerAddress { get; set; }
		string Model { get; set; }
		string VIN { get; set; }
		DateTime Date { get; set; }
		LegislativeClass LegislativeClass { get; set; }
		RegistrationClass RegisteredClass { get; set; }
		VehicleCode VehicleCode { get; set; }
		Kilogram CurbMassChassis { get; set; }
		Kilogram TechnicalPermissibleMaximumLadenMass { get; set; }
		int NumberOfPassengersLowerDeck { get; set; }
		int NumberOfPassengersUpperDeck { get; set; }
		FloorType FloorType { get;set; }
		Meter HeightIntegratedBody { get; set; }
		Meter VehicleLength { get; set; }
		Meter VehicleWidth { get; set; }
		Meter EntranceHeight { get; set; }

		#region CompleteBusDataProviderV26

		ConsumerTechnology DoorDriveTechnology { get; set; }

		#endregion

		#region MyRegion

		AllowedEntry<LegislativeClass>[] AllowedLegislativeClasses { get;}
		AllowedEntry<VehicleCode>[] AllowedVehicleCodes{ get;}
		AllowedEntry<FloorType>[] AllowedFloorTypes { get; }
		AllowedEntry<ConsumerTechnology>[] AllowedConsumerTechnologies { get;}

		#endregion



		#endregion


	}
}
