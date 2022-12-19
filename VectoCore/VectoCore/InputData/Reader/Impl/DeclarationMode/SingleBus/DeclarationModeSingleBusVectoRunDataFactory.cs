using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.SingleBus
{

	public abstract class DeclarationModeSingleBusRunDataFactory
	{
		public abstract class SingleBusBase : IVectoRunDataFactory
		{
			private Segment _segment;
			private DriverData _driverdata;
			private bool _allowVocational = true;
			private AirdragData _airdragData;
			private AxleGearData _axlegearData;
			private AngledriveData _angledriveData;
			private GearboxData _gearboxData;
			private ShiftStrategyParameters _gearshiftData;
			private RetarderData _retarderData;
			private PTOData _ptoTransmissionData;
			protected ISingleBusDeclarationDataAdapter DataAdapter { get; }
			protected IDeclarationReport Report { get; }

			public ISingleBusInputDataProvider DataProvider { get; }

			protected SingleBusBase(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				ISingleBusDeclarationDataAdapter dataAdapter)
			{
				DataAdapter = dataAdapter;
				Report = report;
				DataProvider = dataProvider;
			}




			#region Overrides of AbstractDeclarationVectoRunDataFactory

			public IEnumerable<VectoRunData> NextRun()
			{

				Initialize();
				if (Report != null)
				{
					InitializeReport();
				}

				return GetNextRun();
			}

			private IEnumerable<VectoRunData> GetNextRun()
			{
				
				var vehicle = DataProvider.JobInputData.Vehicle;
				if (vehicle.ExemptedVehicle) {
					throw new NotImplementedException("Implement in derived class for exempted single buses");
				}
				var engine = vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++)
				{
					foreach (var mission in _segment.Missions)
					{
						foreach (var loading in mission.Loadings)
						{
							var simulationRunData = CreateVectoRunData(DataProvider, modeIdx, mission, loading);
							if (simulationRunData == null)
							{
								continue;
							}
							yield return simulationRunData;
						}
					}
				}
			}


			protected void InitializeReport()
			{
				//var powertrainConfig = CreateVectoRunData(DataProvider, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
				//var fuels = new List<List<FuelData.Entry>>();
				//Report.InitializeReport(powertrainConfig, fuels);

				VectoRunData powertrainConfig;
				List<List<FuelData.Entry>> fuels;
				var vehicle = DataProvider.JobInputData.Vehicle;
				if (vehicle.ExemptedVehicle) {
					powertrainConfig = CreateVectoRunData(DataProvider, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
					fuels = new List<List<FuelData.Entry>>();
				} else {
					powertrainConfig = _segment.Missions.Select(
							mission => CreateVectoRunData(
								DataProvider, 0, mission, mission.Loadings.First()))
						.FirstOrDefault(x => x != null);
					fuels = vehicle.Components.EngineInputData.EngineModes.Select(x =>
							x.Fuels.Select(f =>
									DeclarationData.FuelData.Lookup(f.FuelType,
										DataProvider.CompletedVehicle.TankSystem))
								.ToList())
						.ToList();
					// set vehicle category to completed for single bus simulations to instantiate correct reports (MRF/CIF)
					//powertrainConfig.VehicleData.InputData = DataProvider.CompletedVehicle;
				}
				Report.InitializeReport(powertrainConfig);
			}

			protected void Initialize()
			{
				var vehicle = DataProvider.JobInputData.Vehicle;
				if (vehicle.ExemptedVehicle)
				{
					return;
				}

				_segment = GetSegment(DataProvider);
				_driverdata = DataAdapter.CreateDriverData(_segment); //PrimaryBus
				//_driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segment.AccelerationFile);
				var tempVehicle = DataAdapter.CreateVehicleData(DataProvider, _segment, _segment.Missions.First(),
														_segment.Missions.First().Loadings.First(), _allowVocational);
				if (vehicle.AxleConfiguration.AxlegearIncludedInGearbox())
				{
					_axlegearData = DataAdapter.CreateDummyAxleGearData(vehicle.Components.GearboxInputData);
				}
				else
				{
					_axlegearData = DataAdapter.CreateAxleGearData(vehicle.Components.AxleGearInputData);
				}
				_angledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);
				//var tmpRunData = new VectoRunData()
				//{
				//	GearboxData = new GearboxData()
				//	{
				//		Type = vehicle.Components.GearboxInputData.Type,
				//	}
				//};
				//var tmpStrategy = PowertrainBuilder.GetShiftStrategy(new SimplePowertrainContainer(tmpRunData));
				//var tmpEngine = DataAdapter.CreateEngineData(
				//	vehicle, vehicle.Components.EngineInputData.EngineModes[0], _segment.Missions.First());
				//_gearboxData = DataAdapter.CreateGearboxData(
				//	vehicle, new VectoRunData() { EngineData = tmpEngine, AxleGearData = _axlegearData, VehicleData = tempVehicle },
				//	tmpStrategy);

				_retarderData = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData);

				//_gearshiftData = DataAdapter.CreateGearshiftData(
				//	_gearboxData, _axlegearData.AxleGear.Ratio * (_angledriveData?.Angledrive.Ratio ?? 1.0), tmpEngine.IdleSpeed);
			}

			protected VectoRunData CreateVectoRunData(ISingleBusInputDataProvider singleBus, int modeIdx, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
			{
				var vehicle = singleBus.JobInputData.Vehicle;
				var completedVehicle = singleBus.CompletedVehicle;
				var primaryVehicle = singleBus.PrimaryVehicle;
				var doubleDecker = completedVehicle.NumberPassengerSeatsUpperDeck > 0;
				if (mission.BusParameter.DoubleDecker != doubleDecker)
				{
					return null;
				}

				var engine = vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				var engineMode = engineModes[modeIdx];

				var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

				var simulationRunData = new VectoRunData
				{
					InputData = DataProvider,
					Loading = loading.Key,
					VehicleData = DataAdapter.CreateVehicleData(singleBus, _segment, mission, loading, _allowVocational), //Primary
					AirdragData = DataAdapter.CreateAirdragData(completedVehicle, mission), //Single
					EngineData = DataAdapter.CreateEngineData(vehicle, engineMode, mission), //Primary
					ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
					GearboxData = _gearboxData,
					AxleGearData = _axlegearData,
					AngledriveData = _angledriveData,
					Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData,
														vehicle.Components.BusAuxiliaries, mission.MissionType,
														_segment.VehicleClass, vehicle.Length ?? mission.BusParameter.VehicleLength,
														vehicle.Components.AxleWheels.NumSteeredAxles, vehicle.VehicleType),
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					Retarder = _retarderData,
					DriverData = _driverdata,
					ExecutionMode = ExecutionMode.Declaration,
					JobName = DataProvider.JobInputData.JobName,
					ModFileSuffix = $"{(engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "")}" +
									$"_{mission.BusParameter.BusGroup.GetClassNumber()}-Single_{loading.Key}",
					Report = Report,
					Mission = mission,
					InputDataHash = DataProvider.XMLHash,
					SimulationType = SimulationType.DistanceCycle,
					GearshiftParameters = _gearshiftData,
					VehicleDesignSpeed = _segment.DesignSpeed,
					//ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy
				};
				simulationRunData.EngineData.FuelMode = modeIdx;
				simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
				simulationRunData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(mission, primaryVehicle, completedVehicle, simulationRunData);
				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(vehicle.Components.GearboxInputData.Type,
						vehicle.VehicleType);
				simulationRunData.GearboxData = DataAdapter.CreateGearboxData(vehicle, simulationRunData,
					ShiftPolygonCalculator.Create(shiftStrategyName, simulationRunData.GearshiftParameters));
				simulationRunData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						simulationRunData.GearboxData,
						(simulationRunData.AxleGearData?.AxleGear.Ratio ?? 1.0) * (simulationRunData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						vehicle.EngineIdleSpeed
					);
				return simulationRunData;
			}

			#endregion
			protected Segment GetSegment(ISingleBusInputDataProvider singleBus)
			{
				var vehicle = singleBus.JobInputData.Vehicle;
				var completedVehicle = singleBus.CompletedVehicle;
				var primaryVehicle = singleBus.PrimaryVehicle;

				var segment = DeclarationData.CompletedBusSegments.Lookup(
					primaryVehicle.AxleConfiguration.NumAxles(), completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.NumberPassengerSeatsLowerDeck,
					completedVehicle.Height, completedVehicle.LowEntry);
				if (!segment.Found)
				{
					throw new VectoException(
						"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, vehicle code: {3}, registered class: {4}, passengersLowerDeck: {5}, height: {6}, lowfloor: {7}. completed",
						vehicle.VehicleCategory, primaryVehicle.AxleConfiguration,
						vehicle.Articulated, completedVehicle.VehicleCode, completedVehicle.RegisteredClass.GetLabel(), completedVehicle.NumberPassengerSeatsLowerDeck,
						completedVehicle.Height, completedVehicle.LowEntry);
				}
				foreach (var mission in segment.Missions)
				{
					mission.VehicleHeight = completedVehicle.Height + mission.BusParameter.DeltaHeight;
					mission.BusParameter.VehicleLength = completedVehicle.Length;
				}
				return segment;
			}
		}

		

		public class Conventional : SingleBusBase
		{
			public Conventional(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter)
			{

			}

		}

		public abstract class Hybrid : SingleBusBase
		{
			protected Hybrid(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class SerialHybrid : Hybrid
		{
			protected SerialHybrid(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}
		public  class HEV_S2 : SerialHybrid
		{
			protected HEV_S2(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}
        
		public class HEV_S3 : SerialHybrid
		{
			protected HEV_S3(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class HEV_S4 : SerialHybrid
		{
			protected HEV_S4(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}


		public class HEV_S_IEPC : SerialHybrid
		{
			protected HEV_S_IEPC(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

        public class ParallelHybrid : Hybrid
		{
			public ParallelHybrid(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class HEV_P1 : ParallelHybrid
		{
			public HEV_P1(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class HEV_P2 : ParallelHybrid
		{
			public HEV_P2(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class HEV_P2_5 : ParallelHybrid
		{
			public HEV_P2_5(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class HEV_P3 : ParallelHybrid
		{
			public HEV_P3(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class HEV_P4 : ParallelHybrid
		{
			public HEV_P4(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class BatteryElectric : SingleBusBase
		{
			public BatteryElectric(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class PEV_E2 : BatteryElectric
		{
			public PEV_E2(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class PEV_E3 : BatteryElectric
		{
			public PEV_E3(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class PEV_E4 : BatteryElectric
		{
			public PEV_E4(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class PEV_E_IEPC : BatteryElectric
		{
			public PEV_E_IEPC(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}







	}

    //internal class DeclarationModeSingleBusVectoRunDataFactory : DeclarationModePrimaryBusVectoRunDataFactory
    //{
    //    protected new DeclarationDataAdapterSingleBus _dao = new DeclarationDataAdapterSingleBus();
    //    private ISingleBusInputDataProvider _singleBusInputData;

    //    public DeclarationModeSingleBusVectoRunDataFactory(ISingleBusInputDataProvider singleBusInputData, IDeclarationReport report) : base(singleBusInputData, report)
    //    {
    //        _singleBusInputData = singleBusInputData;
    //        _dao.SingleBusInputData = singleBusInputData;
    //    }


    //    protected override Segment GetSegment(IVehicleDeclarationInputData vehicle)
    //    {
    //        //if (vehicle.VehicleCategory != VehicleCategory.HeavyBusCompletedVehicle) {
    //        //	throw new VectoException(
    //        //		"Invalid vehicle category for bus factory! {0}", vehicle.VehicleCategory.GetCategoryName());
    //        //}

    //        var completedVehicle = _singleBusInputData.CompletedVehicle;

    //        var segment = DeclarationData.CompletedBusSegments.Lookup(
    //            _singleBusInputData.PrimaryVehicle.AxleConfiguration.NumAxles(), completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.NumberPassengerSeatsLowerDeck,
    //            completedVehicle.Height, completedVehicle.LowEntry);
    //        if (!segment.Found)
    //        {
    //            throw new VectoException(
    //                "no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, vehicle code: {3}, registered class: {4}, passengersLowerDeck: {5}, height: {6}, lowfloor: {7}. completed",
    //                vehicle.VehicleCategory, _singleBusInputData.PrimaryVehicle.AxleConfiguration,
    //                vehicle.Articulated, completedVehicle.VehicleCode, completedVehicle.RegisteredClass.GetLabel(), completedVehicle.NumberPassengerSeatsLowerDeck,
    //                completedVehicle.Height, completedVehicle.LowEntry);
    //        }
    //        foreach (var mission in segment.Missions)
    //        {
    //            mission.VehicleHeight = completedVehicle.Height + mission.BusParameter.DeltaHeight;
    //            mission.BusParameter.VehicleLength = completedVehicle.Length;
    //        }

    //        return segment;
    //    }


    //    protected override ISingleBusDeclarationDataAdapter DataAdapter => _dao;


    //    protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
    //    {
    //        var doubleDecker = _singleBusInputData.CompletedVehicle.NumberPassengerSeatsUpperDeck > 0;
    //        if (mission.BusParameter.DoubleDecker != doubleDecker)
    //        {
    //            return null;
    //        }

    //        var engine = vehicle.Components.EngineInputData;
    //        var engineModes = engine.EngineModes;
    //        var engineMode = engineModes[modeIdx];

    //        var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

    //        var simulationRunData = new VectoRunData
    //        {
    //            Loading = loading.Key,
    //            VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational),
    //            AirdragData = _dao.CreateAirdragData(_singleBusInputData.CompletedVehicle, mission),
    //            EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission),
    //            ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
    //            GearboxData = _gearboxData,
    //            AxleGearData = _axlegearData,
    //            AngledriveData = _angledriveData,
    //            Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData,
    //                                                vehicle.Components.BusAuxiliaries, mission.MissionType,
    //                                                _segment.VehicleClass, vehicle.Length ?? mission.BusParameter.VehicleLength,
    //                                                vehicle.Components.AxleWheels.NumSteeredAxles),
    //            Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
    //            Retarder = _retarderData,
    //            DriverData = _driverdata,
    //            ExecutionMode = ExecutionMode.Declaration,
    //            JobName = InputDataProvider.JobInputData.JobName,
    //            ModFileSuffix = $"{(engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "")}" +
    //                            $"_{mission.BusParameter.BusGroup.GetClassNumber()}-Single_{loading.Key}",
    //            Report = Report,
    //            Mission = mission,
    //            InputDataHash = InputDataProvider.XMLHash,
    //            SimulationType = SimulationType.DistanceCycle,
    //            GearshiftParameters = _gearshiftData,
    //            VehicleDesignSpeed = _segment.DesignSpeed,
    //            //ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy
    //        };
    //        simulationRunData.EngineData.FuelMode = modeIdx;
    //        simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
    //        simulationRunData.BusAuxiliaries = _dao.CreateBusAuxiliariesData(mission, _singleBusInputData.PrimaryVehicle, _singleBusInputData.CompletedVehicle, simulationRunData);
    //        return simulationRunData;
    //    }

    //    protected override void InitializeReport()
    //    {
    //        VectoRunData powertrainConfig;
    //        List<List<FuelData.Entry>> fuels;
    //        var vehicle = InputDataProvider.JobInputData.Vehicle;
    //        if (vehicle.ExemptedVehicle)
    //        {
    //            powertrainConfig = CreateVectoRunData(vehicle, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
    //            fuels = new List<List<FuelData.Entry>>();
    //        }
    //        else
    //        {
    //            powertrainConfig = _segment.Missions.Select(
    //                    mission => CreateVectoRunData(
    //                        vehicle, 0, mission, mission.Loadings.First()))
    //                .FirstOrDefault(x => x != null);
    //            fuels = vehicle.Components.EngineInputData.EngineModes.Select(x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, _singleBusInputData.CompletedVehicle.TankSystem)).ToList())
    //                .ToList();
    //        }
    //        Report.InitializeReport(powertrainConfig, fuels);
    //    }
    //}


}
