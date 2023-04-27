using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Generic
{
    public abstract class DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration
	{
		public abstract class CompletedBusDeclarationBase : IGenericCompletedBusDeclarationDataAdapter
		{
			private static readonly GearboxType[] SupportedGearboxTypes =
				{ GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };

            #region ComponentDataAdapter
			private readonly IDriverDataAdapter _driverDataAdapter = new CompletedBusGenericDriverDataAdapter();
			protected readonly IVehicleDataAdapter _vehicleDataAdapter = new CompletedBusGenericVehicleDataAdapter();
			private readonly IAxleGearDataAdapter _axleGearDataAdapter = new GenericCompletedBusAxleGearDataAdapter();
			private readonly IRetarderDataAdapter _retarderDataAdapter = new GenericRetarderDataAdapter();
			private readonly IAirdragDataAdapter _airdragDataAdapter = new AirdragDataAdapter();
			private readonly IAngledriveDataAdapter _angledriveDataAdapter = new GenericAngledriveDataAdapter();
            #endregion

			protected abstract IEngineDataAdapter EngineDataAdapter { get; }

			protected abstract IGearboxDataAdapter GearboxDataAdapter { get; }

			protected virtual IElectricMachinesDataAdapter ElectricMachinesDataAdapter => throw new NotImplementedException();

			protected abstract IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; }

			protected virtual ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; } = new GenericCompletedBusAuxiliaryDataAdapter();

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
				return EngineDataAdapter.CreateEngineData(primaryVehicle, modeIdx, mission);
			}

			public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles,
				VectoSimulationJobType jobType)
			{
				return AuxDataAdapter.CreateAuxiliaryData(auxData, busAuxData, missionType, vehicleClass, vehicleLength,
					numSteeredAxles, jobType);
			}

			public virtual AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
			{
				return _axleGearDataAdapter.CreateAxleGearData(axlegearData);
			}

			public virtual AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData)
			{
				return _angledriveDataAdapter.CreateAngledriveData(angledriveData);
			}

			public virtual GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return GearboxDataAdapter.CreateGearboxData(inputData, runData, shiftPolygonCalc, supportedGearboxTypes:SupportedGearboxTypes);
			}

			public virtual ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
			{
				return GearboxDataAdapter.CreateGearshiftData(axleRatio, engineIdlingSpeed, gbx.Type, gbx.Gears.Count);
			}

			public RetarderData CreateRetarderData(IRetarderInputData retarderData, PowertrainPosition position = PowertrainPosition.HybridPositionNotSet)
			{
				return _retarderDataAdapter.CreateRetarderData(retarderData, position);
			}

			public virtual IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(IElectricMachinesDeclarationInputData electricMachines, IDictionary<PowertrainPosition, IList<Tuple<Volt, TableData>>> torqueLimits,
				Volt averageVoltage, GearList gears = null)
			{
				return ElectricMachinesDataAdapter.CreateElectricMachines(electricMachines, torqueLimits, averageVoltage, gears);
			}

            public virtual List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(IIEPCDeclarationInputData iepc, Volt averageVoltage)
			{
				return ElectricMachinesDataAdapter.CreateIEPCElectricMachines(iepc, averageVoltage);
            }

			public abstract void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData,
				Action<SuperCapData> setSuperCapData);

			// serial hybrids
			public virtual HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData, 
				SuperCapData runDataSuperCapData,
				Kilogram vehicleMass, 
				VectoRunData.OvcHevMode ovcMode, 
				LoadingType loading, 
				VehicleClass vehicleClass, 
				MissionType missionType)
			{
				return HybridStrategyDataAdapter.CreateHybridStrategyParameters(runDataBatteryData,
					runDataSuperCapData, vehicleMass, ovcMode);
            }

			// parallel hybrids
			public virtual HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData, SuperCapData runDataSuperCapData,
				Kilogram vehicleMass, VectoRunData.OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType,
				TableData boostingLimitations, GearboxData gearboxData, CombustionEngineData engineData,
				ArchitectureID architectureId)
			{
				return HybridStrategyDataAdapter.CreateHybridStrategyParameters(
					batterySystemData: runDataBatteryData,
					superCap: runDataSuperCapData,
					ovcMode: ovcMode,
					loading: loading,
					vehicleClass: vehicleClass,
					missionType: missionType, architectureId, engineData, gearboxData, boostingLimitations);
            }


			public DriverData CreateDriverData(Segment segment)
			{
				return _driverDataAdapter.CreateDriverData(segment);
			}

			public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle,
                VectoRunData runData)
			{
				return AuxDataAdapter.CreateBusAuxiliariesData(mission, primaryVehicle, null, runData);
			}
			#endregion
		}

		public class Conventional : CompletedBusDeclarationBase
		{
			#region Overrides of CompletedBusDeclarationBase

			protected override IEngineDataAdapter EngineDataAdapter { get; } = new GenericCombustionEngineComponentDataAdapter();
			
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GenericCompletedBusGearboxDataAdapter(new GenericCompletedBusTorqueConverterDataAdapter());
			
			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter =>
				throw new NotImplementedException();
			
			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}

			#endregion
        }

		public abstract class Hybrid : CompletedBusDeclarationBase
		{
			private IElectricStorageAdapter _eletricStorageAdapter = new ElectricStorageAdapter();

            #region Overrides of CompletedBusDeclarationBase

            protected override IEngineDataAdapter EngineDataAdapter { get; } = new GenericCombustionEngineComponentDataAdapter();

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				var batteryData = _eletricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc);
				var superCapData = _eletricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

				if (batteryData != null) {
					setBatteryData(batteryData);
				}
				if (superCapData != null) {
					setSuperCapData(superCapData);
				}

				if (batteryData != null && superCapData != null) {
					throw new VectoException("Either battery or super cap must be provided");
				}
			}

			#endregion
		}

		public abstract class SerialHybrid : Hybrid
		{
			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; } = new
				SerialHybridStrategyParameterDataAdapter();
        }

		public class HEV_S2 : SerialHybrid
		{
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GenericCompletedBusGearboxDataAdapter(new GenericCompletedBusTorqueConverterDataAdapter());

        }

        public class HEV_S3 : SerialHybrid { }
		
		public class HEV_S4 : SerialHybrid { }
		
		public class HEV_S_IEPC : SerialHybrid { }

		public abstract class ParallelHybrid : Hybrid
		{
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GenericCompletedBusGearboxDataAdapter(new GenericCompletedBusTorqueConverterDataAdapter());

            protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; } = new
				ParallelHybridStrategyParameterDataAdapter();
        }
		
		public class HEV_P1 : ParallelHybrid { }
		
		public class HEV_P2 : ParallelHybrid { }
		
		public class HEV_P2_5 : ParallelHybrid { }
		
		public class HEV_P3 : ParallelHybrid { }
		
		public class HEV_P4 : ParallelHybrid { }

		public abstract class BatteryElectric : CompletedBusDeclarationBase
		{
			private readonly IElectricStorageAdapter _electricStorageAdapter = new GenericElectricStorageDataAdapter();

            protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();

			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } =
				new GenericElectricMachinesDataAdapter();

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				var batteryData = _electricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc);
				var superCapData = _electricStorageAdapter.CreateSuperCapData(componentsElectricStorage);


				if (batteryData == null) {
					throw new VectoException("Could not create BatterySystem for PEV");
				}
				setBatteryData(batteryData);


				if (superCapData != null) {
					throw new VectoException("Supercaps are not allowed for PEVs");
				}
			}
        }

		public class PEV_E2 : BatteryElectric
		{
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GenericCompletedBusGearboxDataAdapter(new GenericCompletedBusTorqueConverterDataAdapter());

        }

        public class PEV_E3 : BatteryElectric { }
		
		public class PEV_E4 : BatteryElectric { }
		
		public class PEV_E_IEPC : BatteryElectric { }


		
		public class Exempted : CompletedBusDeclarationBase
		{
			#region Overrides of CompletedBusBase

			protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();
			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();
            protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();
            protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter => throw new NotImplementedException();

            public override VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				throw new NotImplementedException();
            }

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}
            #endregion
        }
	}
}