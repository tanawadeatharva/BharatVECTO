/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdaper;
using TUGraz.VectoCore.Models;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.PDF;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DeclarationModeVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		protected static Dictionary<MissionType, DrivingCycleData> CyclesCache =
			new Dictionary<MissionType, DrivingCycleData>();

		protected IDeclarationInputDataProvider InputDataProvider;

		protected DeclarationReport Report;

		internal DeclarationModeVectoRunDataFactory(IDeclarationInputDataProvider dataProvider, DeclarationReport report)
		{
			InputDataProvider = dataProvider;
			Report = report;
		}

		public IEnumerable<VectoRunData> NextRun()
		{
			var dao = new DeclarationDataAdapter();
			var segment = GetVehicleClassification(InputDataProvider.VehicleInputData.VehicleCategory,
				InputDataProvider.VehicleInputData.AxleConfiguration,
				InputDataProvider.VehicleInputData.GrossVehicleMassRating, InputDataProvider.VehicleInputData.CurbWeightChassis);
			var driverdata = dao.CreateDriverData(InputDataProvider.DriverInputData);
			driverdata.AccelerationCurve = AccelerationCurveData.ReadFromStream(segment.AccelerationFile);

			var engineData = dao.CreateEngineData(InputDataProvider.EngineInputData);

			var gearboxData = dao.CreateGearboxData(InputDataProvider.GearboxInputData, engineData);
			var axlegearData = dao.CreateAxleGearData(InputDataProvider.AxleGearInputData);
			var retarderData = dao.CreateRetarderData(InputDataProvider.RetarderInputData, InputDataProvider.VehicleInputData);

			if (Report != null) {
				var powertrainConfig = new VectoRunData() {
					VehicleData = dao.CreateVehicleData(InputDataProvider.VehicleInputData, segment.Missions.First(),
						segment.Missions.First().Loadings.First().Value),
					EngineData = engineData,
					GearboxData = gearboxData,
					AxleGearData = axlegearData,
					Retarder = retarderData,
					Aux = dao.CreateAuxiliaryData(InputDataProvider.AuxiliaryInputData(), segment.Missions.First().MissionType,
						segment.VehicleClass)
				};
				Report.InitializeReport(powertrainConfig, segment);
			}

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
						GearboxData = gearboxData,
						AxleGearData = axlegearData,
						Aux = dao.CreateAuxiliaryData(InputDataProvider.AuxiliaryInputData(), mission.MissionType,
							segment.VehicleClass),
						Cycle = cycle,
						Retarder = retarderData,
						DriverData = driverdata,
						IsEngineOnly = false,
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

		internal Segment GetVehicleClassification(VehicleCategory category, AxleConfiguration axles, Kilogram grossMassRating,
			Kilogram curbWeight)
		{
			return DeclarationData.Segments.Lookup(category, axles, grossMassRating, curbWeight);
		}
	}
}