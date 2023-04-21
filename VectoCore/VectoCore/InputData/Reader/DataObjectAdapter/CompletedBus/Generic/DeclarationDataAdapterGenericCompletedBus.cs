using System;
using System.Collections.Generic;
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

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Generic
{
    public abstract class DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration
	{
		public abstract class CompletedBusDeclarationBase : IGenericCompletedBusDeclarationDataAdapter
		{
			private static readonly GearboxType[] SupportedGearboxTypes =
				{ GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };

			#region ComponentDataAdapter
			protected readonly IVehicleDataAdapter _vehicleDataAdapter = new CompletedBusGenericVehicleDataAdapter();
			private readonly IRetarderDataAdapter _retarderDataAdapter = new GenericRetarderDataAdapter();
			private readonly IEngineDataAdapter _engineDataAdapter = new GenericCombustionEngineComponentDataAdapter();
			private readonly IAirdragDataAdapter _airdragDataAdapter = new AirdragDataAdapter();
			private readonly IDriverDataAdapter _driverDataAdapter = new CompletedBusGenericDriverDataAdapter();
			private readonly IAxleGearDataAdapter _axleGearDataAdapter = new GenericCompletedBusAxleGearDataAdapter();
			private readonly IAngledriveDataAdapter _angledriveDataAdapter = new GenericAngledriveDataAdapter();
			private readonly IGearboxDataAdapter _gearboxDataAdapter = new GenericCompletedBusGearboxDataAdapter(new GenericCompletedBusTorqueConverterDataAdapter());
			private readonly IPrimaryBusAuxiliaryDataAdapter _auxDataAdapter = new PrimaryBusAuxiliaryDataAdapter();
			#endregion

			#region Implementation of IGenericCompletedBusDeclarationDataAdapter
			public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return _vehicleDataAdapter.CreateVehicleData(vehicle, segment, mission, loading.Value.Item1,
					loading.Value.Item2, allowVocational);
			}

			public AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment)
			{
				return _airdragDataAdapter.CreateAirdragData(airdragData, mission, segment);
			}

			public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx, Mission mission)
			{
				return _engineDataAdapter.CreateEngineData(primaryVehicle, modeIdx, mission);
			}

			public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles,
				VectoSimulationJobType jobType)
			{
				return _auxDataAdapter.CreateAuxiliaryData(auxData, busAuxData, missionType, vehicleClass, vehicleLength,
					numSteeredAxles, jobType);
			}

			public AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
			{
				return _axleGearDataAdapter.CreateAxleGearData(axlegearData);
			}

			public AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData)
			{
				return _angledriveDataAdapter.CreateAngledriveData(angledriveData);
			}

			public GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return _gearboxDataAdapter.CreateGearboxData(inputData, runData, shiftPolygonCalc, supportedGearboxTypes:SupportedGearboxTypes);
			}

			public ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
			{
                //throw new NotImplementedException();
				return _gearboxDataAdapter.CreateGearshiftData(axleRatio, engineIdlingSpeed, gbx.Type, gbx.Gears.Count);
				//gbx, axleRatio, engineIdlingSpeed, gbx.Type, gbx.Gears.Count);
			}

			public RetarderData CreateRetarderData(IRetarderInputData retarderData, PowertrainPosition position = PowertrainPosition.HybridPositionNotSet)
			{
				return _retarderDataAdapter.CreateRetarderData(retarderData, position);
			}

			public List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(IIEPCDeclarationInputData iepc, Volt averageVoltage)
			{
				throw new NotImplementedException();
			}

			public DriverData CreateDriverData(Segment segment)
			{
				return _driverDataAdapter.CreateDriverData(segment);
			}

			public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData vehicleData,
				VectoRunData runData)
			{
				return _auxDataAdapter.CreateBusAuxiliariesData(mission, vehicleData, runData);
			}
			#endregion
		}

		public class Conventional : CompletedBusDeclarationBase { }
		public class HEV_S2 : CompletedBusDeclarationBase { }
		public class HEV_S3 : CompletedBusDeclarationBase { }
		public class HEV_S4 : CompletedBusDeclarationBase { }
		public class HEV_S_IEPC : CompletedBusDeclarationBase { }
		public class HEV_P1 : CompletedBusDeclarationBase { }
		public class HEV_P2 : CompletedBusDeclarationBase { }
		public class HEV_P2_5 : CompletedBusDeclarationBase { }
		public class HEV_P3 : CompletedBusDeclarationBase { }
		public class HEV_P4 : CompletedBusDeclarationBase { }
		public class PEV_E2 : CompletedBusDeclarationBase { }
		public class PEV_E3 : CompletedBusDeclarationBase { }
		public class PEV_E4 : CompletedBusDeclarationBase { }
		public class PEV_E_IEPC : CompletedBusDeclarationBase { }

		public class Exempted : CompletedBusDeclarationBase
		{
			#region Overrides of CompletedBusBase

			public override VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return _vehicleDataAdapter.CreateExemptedVehicleData(vehicle);
			}

			#endregion
		}
	}
}