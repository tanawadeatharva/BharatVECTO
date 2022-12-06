using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory
{
	public abstract partial class DeclarationModeHeavyLorryRunDataFactory
	{
		public abstract class Hybrid : LorryBase
		{
			public Hybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report,
				declarationDataAdapter)
			{

			}

			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;

				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++)
				{
					foreach (var mission in _segment.Missions)
					{
						if (mission.MissionType.IsEMS() &&
							engine.RatedPowerDeclared.IsSmaller(DeclarationData.MinEnginePowerForEMS))
						{
							continue;
						}

						foreach (var loading in mission.Loadings)
						{
							if (vehicle.OvcHev) {
								yield return CreateVectoRunData(vehicle, mission, loading, modeIdx, VectoRunData.OvcHevMode.ChargeDepleting);
								yield return CreateVectoRunData(vehicle, mission, loading, modeIdx, VectoRunData.OvcHevMode.ChargeSustaining);
							} else {
								yield return CreateVectoRunData(vehicle, mission, loading, modeIdx);
							}
						}
					}
				}
			}
			
			#endregion
		}

		public class SerialHybrid : Hybrid
		{

			public SerialHybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report,
				declarationDataAdapter) { }

			#region Overrides of Hybrid

			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle,
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				_segment = GetSegment(InputDataProvider.JobInputData.Vehicle, false);

				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				var engineMode = engineModes[modeIdx.Value];
				var runData = CreateCommonRunData(vehicle, mission, loading, _segment, engineModes, modeIdx.Value);


				runData.DriverData = DataAdapter.CreateDriverData(_segment);
				runData.AirdragData =
					DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData, mission, _segment);
				runData.VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational);


				runData.EngineData = DataAdapter.CreateEngineData(vehicle, engineMode, mission);
				DataAdapter.CreateREESSData(vehicle.Components.ElectricStorage, vehicle.VehicleType, vehicle.OvcHev,
					((batteryData) => runData.BatteryData = batteryData),
					((sCdata => runData.SuperCapData = sCdata)));
				runData.ElectricMachinesData = DataAdapter.CreateElectricMachines(
					vehicle.Components.ElectricMachines, vehicle.ElectricMotorTorqueLimits,
					runData.BatteryData.CalculateAverageVoltage());
				if (vehicle.Components.AxleGearInputData != null) {
					runData.AxleGearData = DataAdapter.CreateAxleGearData(vehicle.Components.AxleGearInputData);
				}
				
				runData.Retarder = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData);
				runData.Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData, null, mission.MissionType,
					_segment.VehicleClass, vehicle.Length, vehicle.Components.AxleWheels.NumSteeredAxles,
					VectoSimulationJobType.SerialHybridVehicle);

				if (vehicle.ArchitectureID == ArchitectureID.S2) {
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
				} else {
					runData.GearshiftParameters = new ShiftStrategyParameters()
					{
						StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
						StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
					};
				}
				runData.HybridStrategyParameters =
					DataAdapter.CreateHybridStrategy(runData.BatteryData, runData.SuperCapData, runData.VehicleData.TotalVehicleMass, ovcMode);

				if (ovcMode != VectoRunData.OvcHevMode.NotApplicable) {
					runData.BatteryData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
				}




				var ptoTransmissionData = DataAdapter.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData, vehicle.Components.GearboxInputData);

				var municipalPtoTransmissionData = DataAdapter.CreatePTOCycleData(vehicle.Components.GearboxInputData, vehicle.Components.PTOTransmissionInputData);

				runData.PTO = mission.MissionType == MissionType.MunicipalUtility
					? municipalPtoTransmissionData
					: ptoTransmissionData;




				return runData;
			}

			#endregion

			#region Overrides of LorryBase

			protected override void SetGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, AxleGearData axleGearData,
				AngledriveData angledriveData)
			{
				throw new NotImplementedException();
			}
			protected override void Initialize()
			{
				_segment = GetSegment(InputDataProvider.JobInputData.Vehicle, false);
			}

			#endregion
		}

		public class ParallelHybrid : Hybrid
		{
			public ParallelHybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report,
				declarationDataAdapter) { }

			#region Overrides of LorryBase

			protected override void SetGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, AxleGearData axleGearData,
				AngledriveData angledriveData)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				_segment = GetSegment(InputDataProvider.JobInputData.Vehicle, false);
				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				var engineMode = engineModes[modeIdx.Value];
				var runData = CreateCommonRunData(vehicle, mission, loading, _segment, engineModes, modeIdx.Value);





				return runData;
			}

			#region Overrides of LorryBase

			protected override void Initialize()
			{
				
			}

			#endregion

			#endregion
		}

		public class HEV_S2 : SerialHybrid
		{
			public HEV_S2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report,
				declarationDataAdapter)
			{ }
		}

		public class HEV_S3 : SerialHybrid
		{
			public HEV_S3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_S4 : SerialHybrid
		{
			public HEV_S4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_S_IEPC : SerialHybrid
		{
			public HEV_S_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P1 : ParallelHybrid
		{
			public HEV_P1(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P2 : ParallelHybrid
		{
			public HEV_P2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P2_5 : ParallelHybrid
		{
			public HEV_P2_5(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P3 : ParallelHybrid
		{
			public HEV_P3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P4 : ParallelHybrid
		{
			public HEV_P4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}
	}
}