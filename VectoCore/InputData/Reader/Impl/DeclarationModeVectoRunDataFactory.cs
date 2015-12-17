using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdaper;
using TUGraz.VectoCore.Models;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData.PDF;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DeclarationModeVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		protected static Dictionary<MissionType, DrivingCycleData> CyclesCache =
			new Dictionary<MissionType, DrivingCycleData>();

		protected IInputDataProvider InputDataProvider;

		protected DeclarationReport Report;

		internal DeclarationModeVectoRunDataFactory(IInputDataProvider dataProvider, DeclarationReport report)
		{
			InputDataProvider = dataProvider;
			Report = report;
		}

		public IEnumerable<VectoRunData> NextRun()
		{
			var dao = new DeclarationDataAdapter();
			var segment = GetVehicleClassification(InputDataProvider.VehicleInputData.VehicleCategory,
				InputDataProvider.VehicleInputData.AxleConfiguration,
				InputDataProvider.VehicleInputData.GrossVehicleMassRating, InputDataProvider.VehicleInputData.CurbWeight);
			var driverdata = dao.CreateDriverData(InputDataProvider.DriverInputData);
			driverdata.AccelerationCurve = AccelerationCurveData.ReadFromStream(segment.AccelerationFile);

			var engineData = dao.CreateEngineData(InputDataProvider.EngineInputData);

			var gearboxData = dao.CreateGearboxData(InputDataProvider.GearboxInputData, engineData);

			ConfigureReport(engineData, gearboxData, segment);

			foreach (var mission in segment.Missions) {
				DrivingCycleData cycle;
				if (CyclesCache.ContainsKey(mission.MissionType)) {
					cycle = CyclesCache[mission.MissionType];
				} else {
					cycle = DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased);
					CyclesCache.Add(mission.MissionType, cycle);
				}
				foreach (var loading in mission.Loadings) {
					var simulationRunData = new VectoRunData {
						Loading = loading.Key,
						VehicleData = dao.CreateVehicleData(InputDataProvider.VehicleInputData, mission, loading.Value),
						EngineData = engineData,
						GearboxData = dao.CreateGearboxData(InputDataProvider.GearboxInputData, engineData),
						AxleGearData = dao.CreateAxleGearData(InputDataProvider.AxleGearInputData),
						Aux =
							dao.CreateAuxiliaryData(InputDataProvider.AuxiliaryInputData(), mission.MissionType,
								segment.VehicleClass),
						Cycle = cycle,
						Retarder = dao.CreateRetarderData(InputDataProvider.RetarderInputData),
						DriverData = driverdata,
						IsEngineOnly = false, // InputDataProvider.JobInputData().EngineOnlyMode,
						JobName = InputDataProvider.JobInputData().JobName,
						ModFileSuffix = loading.Key.ToString(),
						Report = Report,
						Mission = mission,
					};
					simulationRunData.EngineData.WHTCCorrectionFactor = DeclarationData.WHTCCorrection.Lookup(mission.MissionType,
						engineData.WHTCRural.Value(), engineData.WHTCUrban.Value(), engineData.WHTCMotorway.Value());
					simulationRunData.Cycle.Name = mission.MissionType.ToString();
					simulationRunData.VehicleData.VehicleClass = segment.VehicleClass;
					yield return simulationRunData;
				}
			}
		}

		private void ConfigureReport(CombustionEngineData engineData, GearboxData gearboxData, Segment segment)
		{
			if (Report == null) {
				return;
			}
			Report.EngineModel = engineData.ModelName;
			Report.EngineStr = string.Format("{0} l, {1} kW",
				engineData.Displacement.ConvertTo().Cubic.Dezi.Meter.ToOutputFormat(1),
				engineData.FullLoadCurve.MaxPower.ConvertTo().Kilo.Watt.ToOutputFormat(0));
			Report.Flc = engineData.FullLoadCurve;
			Report.GearboxModel = gearboxData.ModelName;
			Report.GearboxStr = string.Format("{0}-Speed {1}", gearboxData.Gears.Count, gearboxData.Type);
			Report.Segment = segment;
			Report.ResultCount = segment.Missions.Sum(m => m.Loadings.Count);
		}

		internal Segment GetVehicleClassification(VehicleCategory category, AxleConfiguration axles, Kilogram grossMassRating,
			Kilogram curbWeight)
		{
			return DeclarationData.Segments.Lookup(category, axles, grossMassRating, curbWeight);
		}
	}
}