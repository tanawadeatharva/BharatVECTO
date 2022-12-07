using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.PrimaryBusRunDataFactory
{
	public abstract class DeclarationModePrimaryBusRunDataFactory
	{
		public abstract class PrimaryBusBase : AbstractDeclarationVectoRunDataFactory
		{
			#region Implementation of IVectoRunDataFactory

			public IPrimaryBusDeclarationDataAdapter DataAdapter { get; }
			public IDeclarationInputDataProvider DataProvider { get; }

			public IDeclarationReport Report { get; }
       
            protected PrimaryBusBase(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, false)
            {
				DataAdapter = declarationDataAdapter;
				DataProvider = dataProvider;
				Report = report;
			}


			protected abstract IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary();
			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				if (InputDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle)
				{
					//if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle)
					//{
					//	yield return CreateVectoRunData(InputDataProvider.JobInputData.Vehicle, 0, null,
					//		new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
					//}
					//else
					//{
						foreach (var vectoRunData in VectoRunDataHeavyBusPrimary())
						{
							yield return vectoRunData;
						}
					//}
				}

				foreach (var entry in new List<VectoRunData>())
				{
					yield return entry;
				}
			}

			protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				return _segment.Missions.Select(
						mission => CreateVectoRunData(
							vehicle, mission, mission.Loadings.First(), 0))
					.FirstOrDefault(x => x != null);
			}

			protected Segment GetSegment(IVehicleDeclarationInputData vehicle)
			{
				if (vehicle.VehicleCategory != VehicleCategory.HeavyBusPrimaryVehicle)
				{
					throw new VectoException(
						"Invalid vehicle category for bus factory! {0}", vehicle.VehicleCategory.GetCategoryName());
				}

				var segment = DeclarationData.PrimaryBusSegments.Lookup(
					vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.Articulated);
				if (!segment.Found)
				{
					throw new VectoException(
						"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, primary",
						vehicle.VehicleCategory, vehicle.AxleConfiguration,
						vehicle.Articulated);
				}

				return segment;
			}

			protected override void Initialize()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				if (vehicle.ExemptedVehicle)
				{
					return;
				}

				_segment = GetSegment(vehicle);
				_driverdata = DataAdapter.CreateDriverData(_segment);
				var tempVehicle = DataAdapter.CreateVehicleData(vehicle, _segment, _segment.Missions.First(),
														_segment.Missions.First().Loadings.First(), _allowVocational);
				_airdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData,
													_segment.Missions.First(), _segment);
				if (InputDataProvider.JobInputData.Vehicle.AxleConfiguration.AxlegearIncludedInGearbox())
				{
					_axlegearData = DataAdapter.CreateDummyAxleGearData(InputDataProvider.JobInputData.Vehicle.Components.GearboxInputData);
				}
				else
				{
					_axlegearData = DataAdapter.CreateAxleGearData(InputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData);
				}
				_angledriveData = DataAdapter.CreateAngledriveData(InputDataProvider.JobInputData.Vehicle.Components.AngledriveInputData);

				_retarderData = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData);

			}

			#endregion
			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle,
				Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle)
				{
					return new VectoRunData
					{
						InputData = DataProvider,
						Exempted = true,
						Report = Report,
						Mission = new Mission { MissionType = MissionType.ExemptedMission },
						VehicleData = DataAdapter.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, new Segment(),
							null,
							new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad,
								Tuple.Create<Kilogram, double?>(0.SI<Kilogram>(), null)), _allowVocational),
						InputDataHash = InputDataProvider.XMLHash
					};
				}

				var engine = vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				var engineMode = engineModes[modeIdx.Value];

				var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

				var simulationRunData = new VectoRunData
				{
					InputData = DataProvider,
					Loading = loading.Key,
					VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational),
					VehicleDesignSpeed = _segment.DesignSpeed,
					AirdragData = DataAdapter.CreateAirdragData(null, mission, new Segment()),
					EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission),
					ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
					GearboxData = _gearboxData,
					AxleGearData = _axlegearData,
					AngledriveData = _angledriveData,
					Aux = DataAdapter.CreateAuxiliaryData(
						vehicle.Components.AuxiliaryInputData,
						vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
						vehicle.Length ?? mission.BusParameter.VehicleLength,
						vehicle.Components.AxleWheels.NumSteeredAxles, vehicle.VehicleType),
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					Retarder = _retarderData,
					DriverData = _driverdata,
					ExecutionMode = ExecutionMode.Declaration,
					JobName = InputDataProvider.JobInputData.JobName,
					ModFileSuffix = $"{(engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "")}" +
									$"_{mission.BusParameter.BusGroup.GetClassNumber()}_{loading.Key}",
					Report = Report,
					Mission = mission,
					InputDataHash = InputDataProvider.XMLHash,
					SimulationType = SimulationType.DistanceCycle,
					GearshiftParameters = _gearshiftData,
					JobType = vehicle.VehicleType,
				};
				simulationRunData.EngineData.FuelMode = modeIdx.Value;
				simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
				simulationRunData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
					mission, InputDataProvider.JobInputData.Vehicle, simulationRunData);
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
		}

		public class Conventional : PrimaryBusBase
		{
			public Conventional(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase

			protected override IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
			{
				var vehicle = DataProvider.JobInputData.Vehicle;
				var engine = vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++)
				{
					foreach (var mission in _segment.Missions)
					{
						foreach (var loading in mission.Loadings)
						{
							var simulationRunData = CreateVectoRunData(vehicle, mission, loading, modeIdx);
							if (simulationRunData == null)
							{
								continue;
							}
							yield return simulationRunData;
						}
					}
				}
			}

			#endregion

		}

		public abstract class Hybrid : PrimaryBusBase
		{
			protected Hybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
			
			protected override IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
			{
				var vehicle = DataProvider.JobInputData.Vehicle;
				var engine = vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++)
				{
					foreach (var mission in _segment.Missions)
					{
						foreach (var loading in mission.Loadings)
						{
							var simulationRunData = CreateVectoRunData(vehicle, mission, loading, modeIdx);
							if (simulationRunData == null)
							{
								continue;
							}
							yield return simulationRunData;
						}
					}
				}
			}
		}

		public abstract class SerialHybrid : Hybrid
		{
			protected SerialHybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report,
				declarationDataAdapter)
			{

			}

		}


		public class HEV_S2 : SerialHybrid
		{
			public HEV_S2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			//#region Overrides of PrimaryBusBase

			//protected override IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
			//{
			//	throw new NotImplementedException();
			//}

			//#endregion

			#region Overrides of PrimaryBusBase

		

			#endregion

			#region Overrides of PrimaryBusBase


			#endregion
		}

		public class HEV_S3 : SerialHybrid
		{
			public HEV_S3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase


			#endregion
		}

		public class HEV_S4 : SerialHybrid
		{
			public HEV_S4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase


			#endregion
		}

		public class HEV_S_IEPC : SerialHybrid
		{
			public HEV_S_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase


			#endregion
		}

		public abstract class ParallelHybrid : PrimaryBusBase
		{
			protected ParallelHybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase

			protected override IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public class HEV_P1 : ParallelHybrid
		{
			public HEV_P1(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase


			#endregion
		}

		public class HEV_P2 : ParallelHybrid
		{
			public HEV_P2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase


			#endregion
		}

		public class HEV_P2_5 : ParallelHybrid
		{
			public HEV_P2_5(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase


			#endregion
		}

		public class HEV_P3 : ParallelHybrid
		{
			public HEV_P3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase


			#endregion
		}

		public class HEV_P4 : ParallelHybrid
		{
			public HEV_P4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase

			protected override IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public abstract class BatteryElectric : PrimaryBusBase
		{
			public BatteryElectric(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase

			protected override IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
			{
				throw new NotImplementedException();
			}

			#endregion
		}


		public class PEV_E2 : BatteryElectric
		{
			public PEV_E2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase

			protected override IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public class PEV_E3 : BatteryElectric
		{
			public PEV_E3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase

			protected override IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public class PEV_E4 : BatteryElectric
		{
			public PEV_E4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase

			protected override IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public class PEV_E_IEPC : BatteryElectric
		{
			public PEV_E_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase

			protected override IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public class Exempted : PrimaryBusBase
		{
			public Exempted(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase

			protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				return CreateVectoRunData(vehicle, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(), 0);
			}

			protected override IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
			{
				yield return new VectoRunData {
					Exempted = true,
					Report = Report,
					Mission = new Mission { MissionType = MissionType.ExemptedMission },
					VehicleData = DataAdapter.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, new Segment(),
						null,
						new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad,
							Tuple.Create<Kilogram, double?>(0.SI<Kilogram>(), null)), _allowVocational),
					InputDataHash = InputDataProvider.XMLHash
				};
			}

			#endregion
		}
	}
}