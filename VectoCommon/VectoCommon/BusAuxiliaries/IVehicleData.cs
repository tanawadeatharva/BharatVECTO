using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries {
	public interface IVehicleData
	{
		string ModelName { get; }
		double PassengerCount { get; }

		Kilogram TotalVehicleMass { get; }
		FloorType FloorType { get; }
		bool DoubleDecker { get; }
		Meter Length { get; }
		Meter Width { get; }
		Meter Height { get; }
	}
}