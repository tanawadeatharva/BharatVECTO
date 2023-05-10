using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SingleBus
{
    public abstract class DeclarationDataAdapterSingleBus
	{
		public abstract class SingleBusBase : ISingleBusDeclarationDataAdapter
		{
			public static readonly GearboxType[] SupportedGearboxTypes =
				{ GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };

			private IAirdragDataAdapter _airdragDataAdapter = new SingleBusAirdragDataAdapter();
			private IGearboxDataAdapter _gearboxDataAdapter = new GearboxDataAdapter(new TorqueConverterDataAdapter());

			private ICompletedBusAuxiliaryDataAdapter _busAuxiliaryDataAdapter =
				new SpecificCompletedBusAuxiliaryDataAdapter(new PrimaryBusAuxiliaryDataAdapter());

			private SingleBusVehicleDataAdapter _vehicleDataAdapter = new SingleBusVehicleDataAdapter();
			private IAxleGearDataAdapter _axleGearDataAdapter = new AxleGearDataAdapter();
			private IDriverDataAdapter _driverDataAdapter = new PrimaryBusDriverDataAdapter();
			private IAngledriveDataAdapter _angledriveDataAdapter = new AngledriveDataAdapter();
			private IEngineDataAdapter _engineDataAdapter = new CombustionEngineComponentDataAdapter();
			private IRetarderDataAdapter _retarderDataAdapter = new RetarderDataAdapter();

			#region Implementation of IDeclarationDataAdapter

			public VehicleData CreateVehicleData(ISingleBusInputDataProvider vehicle, Segment segment, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return _vehicleDataAdapter.CreateVehicleData(vehicle, segment, mission, loading
					, allowVocational);
			}


			#endregion

			#region Implementation of ISingleBusDeclarationDataAdapter

			public AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission)
			{
				return _airdragDataAdapter.CreateAirdragData(completedVehicle, mission);
			}

			public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
				IEngineModeDeclarationInputData engineMode, Mission mission)
			{
				return _engineDataAdapter.CreateEngineData(vehicle, engineMode, mission);
			}

			public DriverData CreateDriverData(Segment segment)
			{
				return _driverDataAdapter.CreateDriverData(segment);
			}

			public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gearboxInputData)
			{
				return _axleGearDataAdapter.CreateDummyAxleGearData(gearboxInputData);
			}

			public AxleGearData CreateAxleGearData(IAxleGearInputData axleGearInputData)
			{
				return _axleGearDataAdapter.CreateAxleGearData(axleGearInputData);
			}

			public AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData)
			{
				return _angledriveDataAdapter.CreateAngledriveData(angledriveData);
			}

			public GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return _gearboxDataAdapter.CreateGearboxData(inputData, runData, shiftPolygonCalc,
					SupportedGearboxTypes);
			}

			public RetarderData CreateRetarderData(IRetarderInputData retarderData)
			{
				return _retarderDataAdapter.CreateRetarderData(retarderData);
			}

			public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData)
			{
				throw new NotImplementedException();
			}

			public ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio,
				PerSecond engineIdlingSpeed)
			{
				return _gearboxDataAdapter.CreateGearshiftData(axleRatio, engineIdlingSpeed, gbx.Type, gbx.Gears.Count);
			}

			public IEnumerable<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData,
				IBusAuxiliariesDeclarationData busAuxInput, MissionType mission,
				VehicleClass segment, Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType)
			{
				return _busAuxiliaryDataAdapter.CreateAuxiliaryData(auxInputData, busAuxInput, mission, segment,
					vehicleLength, numSteeredAxles, jobType);
			}

			public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission,
				IVehicleDeclarationInputData primaryVehicle,
				IVehicleDeclarationInputData completedVehicle, VectoRunData simulationRunData)
			{
				return _busAuxiliaryDataAdapter.CreateBusAuxiliariesData(mission, primaryVehicle, completedVehicle,
					simulationRunData);
			}

			#endregion
		}

		public class Conventional : SingleBusBase { }

		public abstract class Hybrid : SingleBusBase { }

		public class SerialHybrid : Hybrid { }

		public class HEV_S2 : SerialHybrid { }

		public class HEV_S3 : SerialHybrid { }

		public class HEV_S4 : SerialHybrid { }

		public class HEV_S_IEPC : SerialHybrid { }

		public class ParallelHybrid : Hybrid { }

		public class HEV_P1 : ParallelHybrid { }

		public class HEV_P2 : ParallelHybrid { }

		public class HEV_P2_5 : ParallelHybrid { }

		public class HEV_P3 : ParallelHybrid { }

		public class HEV_P4 : ParallelHybrid { }

		public class BatteryElectric : SingleBusBase { }

		public class PEV_E2 : BatteryElectric { }

		public class PEV_E3 : BatteryElectric { }

		public class PEV_E4 : BatteryElectric { }

		public class PEV_E_IEPC : BatteryElectric { }

	}
}
