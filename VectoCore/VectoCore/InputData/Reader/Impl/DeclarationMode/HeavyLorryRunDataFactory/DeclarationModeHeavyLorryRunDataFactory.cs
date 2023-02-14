using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory
{
	public abstract partial class DeclarationModeHeavyLorryRunDataFactory
	{
		public abstract class LorryBase : AbstractDeclarationVectoRunDataFactory
		{
			public ILorryDeclarationDataAdapter DataAdapter { get; }
			public IDeclarationInputDataProvider InputDataProvider { get; }
			public IDeclarationReport Report { get; }

			protected DriverData _driverdata;
			protected AirdragData _airdragData;
			protected AxleGearData _axlegearData;
			protected AngledriveData _angledriveData;
			protected GearboxData _gearboxData;
			protected RetarderData _retarderData;
			protected PTOData _ptoTransmissionData;
			protected PTOData _municipalPtoTransmissionData;
			protected ShiftStrategyParameters _gearshiftData;
			private bool _allowVocational;


			protected LorryBase(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, false)
			{
				DataAdapter = declarationDataAdapter;
				InputDataProvider = dataProvider;
				Report = report;
			}

			/// <summary>
			/// Sets <see cref="VectoRunData.Loading"/>
			///  ,<see cref="VectoRunData.JobName"/>
			///  ,<see cref="VectoRunData.JobType"/>
			///  ,<see cref="VectoRunData.Mission"/>
			///  ,<see cref="VectoRunData.Report"/>
			///  ,<see cref="VectoRunData.ExecutionMode"/>
			///  ,<see cref="VectoRunData.Cycle"/>
			///  ,<see cref="VectoRunData.SimulationType"/>
			///  ,<see cref="VectoRunData.InputData"/>
			///  ,<see cref="VectoRunData.ModFileSuffix"/>
			///  ,<see cref="VectoRunData.VehicleDesignSpeed"/>
			///  ,<see cref="VectoRunData.InputDataHash"/>
			/// </summary>
			/// <param name="vehicle"></param>
			/// <param name="mission"></param>
			/// <param name="loading"></param>
			/// <param name="segment"></param>
			/// <param name="engineModes"></param>
			/// <param name="modeIdx"></param>
			/// <returns></returns>
			protected VectoRunData CreateCommonRunData(IVehicleDeclarationInputData vehicle,
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				Segment segment,
				IList<IEngineModeDeclarationInputData> engineModes = null, int modeIdx = 0)
			{
				var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType,
					_ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

				CheckSuperCap(vehicle);

				var simulationRunData = new VectoRunData
				{
					Loading = loading.Key,
					JobName = InputDataProvider.JobInputData.JobName,
					JobType = vehicle.VehicleType,
					Mission = mission,
					Report = Report,
					ExecutionMode = ExecutionMode.Declaration,
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					SimulationType = SimulationType.DistanceCycle,
					InputData = InputDataProvider,
					ModFileSuffix = (engineModes?.Count > 1 ? $"_EngineMode{modeIdx}_" : "") + loading.Key,
					VehicleDesignSpeed = segment.DesignSpeed,
					InputDataHash = InputDataProvider.XMLHash,
					MaxChargingPower = InputDataProvider.JobInputData.Vehicle.MaxChargingPower,
				};
				return simulationRunData;
			}

			/// <summary>
			/// Super caps are not allowed for ovc hevs or pevs
			/// </summary>
			protected void CheckSuperCap(IVehicleDeclarationInputData vehicle)
			{
				if (vehicle.VehicleType == VectoSimulationJobType.BatteryElectricVehicle || vehicle.OvcHev) {
					if (vehicle.Components.ElectricStorage.ElectricStorageElements.Any(e =>
							e.REESSPack.StorageType == REESSType.SuperCap)) {
						throw new VectoException("Super caps are not allowed for OVC-HEVs or PEVs");
					}
				}

				if (vehicle.Components.ElectricStorage?.ElectricStorageElements == null) {
					return;
				}

				var hasSuperCap = vehicle.Components.ElectricStorage.ElectricStorageElements.Any(e =>
					e.REESSPack.StorageType == REESSType.SuperCap);
				var hasBattery = vehicle.Components.ElectricStorage.ElectricStorageElements.Any(e =>
						e.REESSPack.StorageType == REESSType.Battery);

				if (hasSuperCap && hasBattery) {
					//Already handled by XML Schema
					throw new VectoException("Super caps AND batteries are not supported");
				}
			}
			protected override void Initialize()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				if (vehicle.ExemptedVehicle)
				{
					return;
				}
				if (!vehicle.LegislativeClass.IsOneOf(LegislativeClass.M3,
						LegislativeClass.N2, LegislativeClass.N3)) {
					throw new VectoException("Unsupported Legislative class '{0}'",
						InputDataProvider.JobInputData.Vehicle.LegislativeClass.ToString());
				}

				_segment = GetSegment(vehicle);
				_driverdata = DataAdapter.CreateDriverData(_segment);

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

				_ptoTransmissionData = DataAdapter.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData, vehicle.Components.GearboxInputData);

				_municipalPtoTransmissionData = DataAdapter.CreatePTOCycleData(vehicle.Components.GearboxInputData, vehicle.Components.PTOTransmissionInputData);


			}

			protected abstract void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle,
				VectoRunData runData);

			#region Implementation of IVectoRunDataFactory

			public override IEnumerable<VectoRunData> NextRun()
			{
				Initialize();
				if (Report != null)
				{
					InitializeReport();
				}

				return GetNextRun();
			}

			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				return GetNextRun().First(x => x != null);
				//return _segment.Missions.Select(
				//		mission => CreateVectoRunData(
				//			vehicle, mission, mission.Loadings.First(), 0))
				//	.FirstOrDefault(x => x != null);
			}

			#endregion

			protected Segment GetSegment(IVehicleDeclarationInputData vehicle, bool batteryElectric = false)
			{
				_allowVocational = true;
				var ng = vehicle.Components.EngineInputData?.EngineModes.Any(e =>
					e.Fuels.Any(f => f.FuelType.IsOneOf(FuelType.LPGPI, FuelType.NGCI, FuelType.NGPI))) ?? false;
				var ovcHev = vehicle.OvcHev;
				Segment segment;
				try
				{
					segment = DeclarationData.TruckSegments.Lookup(
						vehicle.VehicleCategory, batteryElectric, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating,
						vehicle.CurbMassChassis,
						vehicle.VocationalVehicle, ng, ovcHev);
				}
				catch (VectoException)
				{
					_allowVocational = false;
					segment = DeclarationData.TruckSegments.Lookup(
						vehicle.VehicleCategory, batteryElectric, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating,
						vehicle.CurbMassChassis,
						false, ng, ovcHev);
				}

				if (!segment.Found)
				{
					throw new VectoException(
						"no segment found for vehicle configuration: vehicle category: {0}, axle configuration: {1}, GVMR: {2}",
						vehicle.VehicleCategory, vehicle.AxleConfiguration,
						vehicle.GrossVehicleMassRating);
				}
				return segment;
			}


			#endregion

			protected abstract bool AxleGearRequired();

		}

		public sealed class Conventional : LorryBase
		{
			public Conventional(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			protected override void Initialize()
			{
				base.Initialize();
			}

			#region Overrides of LorryBase
			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;

				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					foreach (var mission in _segment.Missions) {
						if (mission.MissionType.IsEMS() &&
							engine.RatedPowerDeclared.IsSmaller(DeclarationData.MinEnginePowerForEMS)) {
							continue;
						}

						foreach (var loading in mission.Loadings) {
							var simulationRunData = CreateVectoRunData(vehicle, mission, loading, modeIdx);
							
							yield return simulationRunData;
						}
					}
				}
			}


			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle,
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				if (!modeIdx.HasValue) {
					throw new VectoException("Engine mode has to be specified for conventional vehicle");
				}
				var engineMode = engineModes[modeIdx.Value];

				var simulationRunData = CreateCommonRunData(vehicle, mission, loading, _segment, engineModes, modeIdx.Value);



				simulationRunData.VehicleData =
				DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational);

				simulationRunData.AirdragData =
					DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData, mission, _segment);

				simulationRunData.EngineData =
					DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode,
						mission); // _engineData.Copy(), // a copy is necessary because every run has a different correction factor!

				simulationRunData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
		

				CreateGearboxAndGearshiftData(vehicle, simulationRunData);


				simulationRunData.AngledriveData = _angledriveData;
				simulationRunData.Aux = DataAdapter.CreateAuxiliaryData(
					vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType,
					_segment.VehicleClass, vehicle.Length,
					vehicle.Components.AxleWheels.NumSteeredAxles, vehicle.VehicleType);

				simulationRunData.Retarder = _retarderData;
				simulationRunData.DriverData = _driverdata;
				simulationRunData.PTO = mission.MissionType == MissionType.MunicipalUtility
					? _municipalPtoTransmissionData
					: _ptoTransmissionData;
				
				
			
				simulationRunData.EngineData.FuelMode = modeIdx.Value;
				simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
				simulationRunData.VehicleData.InputData = vehicle;
				return simulationRunData;
			}

			
			#endregion


			#region Overrides of LorryBase

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						runData.AxleGearData?.AxleGear.Ratio ?? 1.0,
						vehicle.EngineIdleSpeed,
						vehicle.Components.GearboxInputData.Type,
						vehicle.Components.GearboxInputData.Gears.Count
					);


				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(vehicle.Components.GearboxInputData.Type,
						vehicle.VehicleType);
				runData.AxleGearData = _axlegearData;
				runData.GearboxData = DataAdapter.CreateGearboxData(vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));
			}

			protected override bool AxleGearRequired()
			{
				return true;
			}

			

			#endregion
		}



		public abstract class BatteryElectric : LorryBase
		{
			#region Overrides of LorryBase

			protected override bool AxleGearRequired()
			{
				return InputDataProvider.JobInputData.Vehicle.ArchitectureID != ArchitectureID.E4;
			}

			#endregion

			public BatteryElectric(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report,
				declarationDataAdapter)
			{

			}
			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				foreach (var mission in _segment.Missions) {
					if (mission.MissionType.IsEMS() &&
						DeclarationData.GetReferencePropulsionPower(vehicle)
							.IsSmaller(DeclarationData.MinEnginePowerForEMS_PEV))
                    {
                        continue;
                    }

                    foreach (var loading in mission.Loadings)
					{
						var simulationRunData = CreateVectoRunData(vehicle, mission, loading);
						simulationRunData.BatteryData.Batteries.ForEach(t => t.Item2.ChargeSustainingBattery = true);
						yield return simulationRunData;
					}
				}
			}
			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle,
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null,
				VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				_segment = GetSegment(vehicle);
				var result = CreateCommonRunData(vehicle, mission, loading, _segment);
				result.AirdragData =
					DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData, mission, _segment);
				result.DriverData = DataAdapter.CreateDriverData(_segment);


				DataAdapter.CreateREESSData(
					componentsElectricStorage: vehicle.Components.ElectricStorage,
					vehicle.VehicleType,
					true,
					(bs) => result.BatteryData = bs,
					(sc) => result.SuperCapData = sc);
				// result.BatteryData = DataAdapter.CreateBatteryData(componentsElectricStorage: vehicle.Components.ElectricStorage, vehicle.VehicleType, true);
				// result.SuperCapData = DataAdapter.CreateSuperCapData(componentsElectricStorage: vehicle.Components.ElectricStorage);
				
				

				result.ElectricMachinesData = DataAdapter.CreateElectricMachines(vehicle.Components.ElectricMachines, vehicle.ElectricMotorTorqueLimits, result.BatteryData.CalculateAverageVoltage(), null);
				if (vehicle.VehicleType == VectoSimulationJobType.IEPC_E)
				{
					
					result.ElectricMachinesData = DataAdapter.CreateIEPCElectricMachines(vehicle.Components.IEPC,
						result.BatteryData.CalculateAverageVoltage());
					
				}




				result.AngledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);
				if (AxleGearRequired() || vehicle.Components.AxleGearInputData != null) {
					result.AxleGearData = DataAdapter.CreateAxleGearData(vehicle.Components.AxleGearInputData);
				}
				
				
				result.VehicleData =
					DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational);

				if (vehicle.Components.RetarderInputData != null) {
					result.Retarder = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData,
						result.ElectricMachinesData.First(e => e.Item1 != PowertrainPosition.GEN).Item1);
				}


				CreateGearboxAndGearshiftData(vehicle, result);


				result.Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData, null,
					mission.MissionType, _segment.VehicleClass, vehicle.Length,
					vehicle.Components.AxleWheels.NumSteeredAxles, vehicle.VehicleType);


				var ptoTransmissionData = DataAdapter.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData, vehicle.Components.GearboxInputData);

				var municipalPtoTransmissionData = DataAdapter.CreatePTOCycleData(vehicle.Components.GearboxInputData, vehicle.Components.PTOTransmissionInputData);

				result.PTO = mission.MissionType == MissionType.MunicipalUtility
					? municipalPtoTransmissionData
					: ptoTransmissionData;







				return result;
			}

			#region Overrides of LorryBase

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				if (vehicle.ArchitectureID == ArchitectureID.E2) {
					throw new ArgumentException();
				}
				runData.GearshiftParameters = new ShiftStrategyParameters()
				{
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				};
			}

			#endregion

			protected override void Initialize()
			{
				if (!InputDataProvider.JobInputData.Vehicle.LegislativeClass.IsOneOf(LegislativeClass.M3,
						LegislativeClass.N2, LegislativeClass.N3)) {
					throw new VectoException("Unsupported Legislative class '{0}'",
						InputDataProvider.JobInputData.Vehicle.LegislativeClass.ToString());
				}
				_segment = GetSegment(InputDataProvider.JobInputData.Vehicle, true);

			}

			#endregion
		}

		public class PEV_E2 : BatteryElectric
		{
			public PEV_E2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report,
				declarationDataAdapter)
			{

			}


			#region Overrides of BatteryElectric

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				if (vehicle.ArchitectureID != ArchitectureID.E2) {
					throw new ArgumentException(nameof(vehicle));
				}

				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						runData.AxleGearData?.AxleGear.Ratio ?? 1.0,
						null,
						vehicle.Components.GearboxInputData.Type,
						vehicle.Components.GearboxInputData.Gears.Count
					);


				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(vehicle.Components.GearboxInputData.Type,
						vehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));

			}

			#endregion
		}

		public class PEV_E3 : BatteryElectric
		{
			public PEV_E3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class PEV_E4 : BatteryElectric
		{
			public PEV_E4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class PEV_E_IEPC : BatteryElectric
		{
			public PEV_E_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report,
				declarationDataAdapter)
			{

			}

			protected override bool AxleGearRequired()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				var iepcInput = vehicle.Components.IEPC;
				var axleGearRequired = !iepcInput.DifferentialIncluded && !iepcInput.DesignTypeWheelMotor;
				if (axleGearRequired && vehicle.Components.AxleGearInputData == null)
				{
					throw new VectoException(
						$"Axlegear reqhired for selected type of IEPC! DifferentialIncluded: {iepcInput.DifferentialIncluded}, DesignTypeWheelMotor: {iepcInput.DesignTypeWheelMotor}");
				}

				var numGearsPowermap =
					iepcInput.VoltageLevels.Select(x => Tuple.Create(x.VoltageLevel, x.PowerMap.Count)).ToArray();
				var gearCount = iepcInput.Gears.Count;
				var numGearsDrag = iepcInput.DragCurves.Count;

				if (numGearsPowermap.Any(x => x.Item2 != gearCount))
				{
					throw new VectoException(
						$"Number of gears for voltage levels does not match! PowerMaps: {numGearsPowermap.Select(x => $"{x.Item1}: {x.Item2}").Join()}; Gear count: {gearCount}");
				}

				if (numGearsDrag > 1 && numGearsDrag != gearCount)
				{
					throw new VectoException(
						$"Number of gears drag curve does not match gear count! DragCurve {numGearsDrag}; Gear count: {gearCount}");
				}

				return axleGearRequired || vehicle.Components.AxleGearInputData != null;

			}

			#region Overrides of BatteryElectric

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						runData.AxleGearData?.AxleGear.Ratio ?? 1.0,
						null,
						GearboxType.APTN,
						vehicle.Components.IEPC.Gears.Count
					);
				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(GearboxType.APTN,
						vehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));

			}

			#endregion
		}

		public class Exempted : LorryBase
		{
			public Exempted(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of AbstractDeclarationVectoRunDataFactory


			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;


				var simulationRunData = CreateVectoRunData(vehicle,
					null, 
					new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(), 
					0);

				yield return simulationRunData;
			}

		#region Overrides of LorryBase

			protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				return CreateVectoRunData(vehicle, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(), 0);
			}

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				throw new NotImplementedException();
			}

			#endregion

			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle,
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				var runData = new VectoRunData
				{
					InputData = InputDataProvider,
					Exempted = true,
					Report = Report,
					Mission = new Mission() { MissionType = MissionType.ExemptedMission },
					VehicleData = DataAdapter.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, new Segment(), null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad, Tuple.Create<Kilogram, double?>(0.SI<Kilogram>(), null)), _allowVocational),
					InputDataHash = InputDataProvider.XMLHash
				};
				runData.VehicleData.InputData = vehicle;
				return runData;
			}

			#endregion

			#region Overrides of LorryBase

			protected override bool AxleGearRequired()
			{
				throw new NotImplementedException();
			}

			#endregion
		}
		
	}
}