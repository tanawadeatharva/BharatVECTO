using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DeclarationDataAdapterCompletedBus
	{
		public DriverData CreateDriverData()
		{
			throw new System.NotImplementedException();
		}

		public AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragInputData, Mission mission, Segment segment)
		{
			throw new System.NotImplementedException();
		}

		public RetarderData CreateRetarderData(IRetarderInputData retarderInputData)
		{
			throw new System.NotImplementedException();
		}

		public ShiftStrategyParameters CreateGearshiftData(GearboxData gearboxData, double axleRatio, PerSecond idleSpeed)
		{
			throw new System.NotImplementedException();
		}

		public VehicleData CreateVehicleData(
			IVehicleDeclarationInputData vehicle, Mission mission, KeyValuePair<LoadingType, Kilogram> loading)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<VectoRunData.AuxData> CreateAuxiliaryData(
			IAuxiliariesDeclarationInputData auxiliaryInputData, IBusAuxiliariesDeclarationData mergedBusAux,
			MissionType mission, VehicleClass vehicleClass, Meter vehicleLength)
		{
			throw new System.NotImplementedException();
		}
	}
}
