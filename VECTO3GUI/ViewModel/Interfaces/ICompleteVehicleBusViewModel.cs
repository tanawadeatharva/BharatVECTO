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
		string RegisteredClass { get; set; }
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

		string DoorDriveTechnology { get; set; }

		#endregion

		#region MyRegion

		AllowedEntry<LegislativeClass>[] AllowedLegislativeClass { get; set; }
		AllowedEntry<VehicleCode>[] AllowedVehicleCode{ get; set; }
		AllowedEntry<FloorType>[] AllowedFloorType { get; set; }
		//public AllowedEntry<AxleConfiguration>[] AllowedAxleConfigurations { get; set; }
		//public AllowedEntry<RetarderType>[] AllowedRetarderTypes { get; set; }
		//public AllowedEntry<AngledriveType>[] AllowedAngledriveTypes { get; set; }

		#endregion



		#endregion


	}
}
