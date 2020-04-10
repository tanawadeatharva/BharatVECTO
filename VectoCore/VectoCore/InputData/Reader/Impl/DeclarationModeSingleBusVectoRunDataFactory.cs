using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl {
	internal class DeclarationModeSingleBusVectoRunDataFactory : DeclarationModePrimaryBusVectoRunDataFactory
	{
		protected new DeclarationDataAdapterSingleBus _dao = new DeclarationDataAdapterSingleBus();
		private ISingleBusInputDataProvider _singleBusInputData;

		public DeclarationModeSingleBusVectoRunDataFactory(ISingleBusInputDataProvider singleBusInputData, IDeclarationReport report) : base(singleBusInputData, report)
		{
			_singleBusInputData = singleBusInputData;
			_dao.SingleBusInputData = singleBusInputData;
		}

		#region Implementation of IVectoRunDataFactory

		#region Overrides of DeclarationModePrimaryBusVectoRunDataFactory

		protected override Segment GetSegment(IVehicleDeclarationInputData vehicle)
		{
			//if (vehicle.VehicleCategory != VehicleCategory.HeavyBusCompletedVehicle) {
			//	throw new VectoException(
			//		"Invalid vehicle category for bus factory! {0}", vehicle.VehicleCategory.GetCategoryName());
			//}

			var completedVehicle = _singleBusInputData.CompletedVehicle;

			var segment = DeclarationData.CompletedBusSegments.Lookup(
				_singleBusInputData.PrimaryVehicle.AxleConfiguration.NumAxles(), completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.NumberOfPassengersLowerDeck,
				completedVehicle.Height, completedVehicle.FloorType == FloorType.LowFloor);
			if (!segment.Found) {
				throw new VectoException(
					"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, primary",
					vehicle.VehicleCategory, _singleBusInputData.PrimaryVehicle.AxleConfiguration,
					vehicle.Articulated);
			}
			foreach (var mission in segment.Missions) {
				mission.VehicleHeight = completedVehicle.Height + mission.BusParameter.DeltaHeight;
				mission.BusParameter.VehicleLength = completedVehicle.Length;
			}

			return segment;
		}

		#endregion

		protected override IDeclarationDataAdapter DataAdapter { get { return _dao; } }


		protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			var doubleDecker = _singleBusInputData.CompletedVehicle.NumberOfPassengersUpperDeck > 0;
			if (mission.BusParameter.DoubleDecker != doubleDecker) {
				return null;
			}

			var engine = vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;
			var engineMode = engineModes[modeIdx];
			DrivingCycleData cycle;
			lock (CyclesCacheLock) {
				if (CyclesCache.ContainsKey(mission.MissionType)) {
					cycle = CyclesCache[mission.MissionType];
				} else {
					cycle = DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false);
					CyclesCache.Add(mission.MissionType, cycle);
				}
			}
			var simulationRunData = new VectoRunData {
				Loading = loading.Key,
				VehicleData = DataAdapter.CreateVehicleData(vehicle, mission, loading),
				AirdragData = _dao.CreateAirdragData(_singleBusInputData.CompletedVehicle, mission),
				EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission),
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData,
													vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass, vehicle.Length ?? mission.BusParameter.VehicleLength),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverdata,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = InputDataProvider.JobInputData.JobName,
				ModFileSuffix = (engineModes.Count > 1 ? string.Format("_EngineMode{0}_", modeIdx) : "") + "_" + mission.BusParameter.BusGroup.GetClassNumber() + "-Single_" + loading.Key.ToString(),
				Report = Report,
				Mission = mission,
				InputDataHash = InputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				GearshiftParameters = _gearshiftData,
				VehicleDesignSpeed = _segment.DesignSpeed,
				//ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy
			};
			simulationRunData.EngineData.FuelMode = modeIdx;
			simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
			simulationRunData.BusAuxiliaries = _dao.CreateBusAuxiliariesData(mission, _singleBusInputData.PrimaryVehicle, _singleBusInputData.CompletedVehicle, simulationRunData);
			return simulationRunData;
		}
	}

	#endregion
}
