/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DeclarationModeVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		private static readonly object CyclesCacheLock = new object();

		private static readonly Dictionary<MissionType, DrivingCycleData> CyclesCache =
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
			driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);

			var tempVehicle = dao.CreateVehicleData(InputDataProvider.VehicleInputData, segment.Missions.First(),
				segment.Missions.First().Loadings.First().Value);
			var engineData = dao.CreateEngineData(InputDataProvider.EngineInputData, InputDataProvider.GearboxInputData.Type);
			var axlegearData = dao.CreateAxleGearData(InputDataProvider.AxleGearInputData, false);
			var angularGearData = dao.CreateAngularGearData(InputDataProvider.AngularGearInputData, false);
			var gearboxData = dao.CreateGearboxData(InputDataProvider.GearboxInputData, engineData, axlegearData.AxleGear.Ratio,
				tempVehicle.DynamicTyreRadius, false);
			var retarderData = dao.CreateRetarderData(InputDataProvider.RetarderInputData);

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
				lock (CyclesCacheLock) {
					if (CyclesCache.ContainsKey(mission.MissionType)) {
						cycle = CyclesCache[mission.MissionType];
					} else {
						cycle = DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false);
						CyclesCache.Add(mission.MissionType, cycle);
					}
				}
				foreach (var loading in mission.Loadings) {
					var simulationRunData = new VectoRunData {
						Loading = loading.Key,
						VehicleData = dao.CreateVehicleData(InputDataProvider.VehicleInputData, mission, loading.Value),
						EngineData = engineData.Copy(),
						GearboxData = gearboxData,
						AxleGearData = axlegearData,
						AngularGearData = angularGearData,
						Aux = dao.CreateAuxiliaryData(InputDataProvider.AuxiliaryInputData(), mission.MissionType,
							segment.VehicleClass),
						Cycle = cycle,
						Retarder = retarderData,
						DriverData = driverdata,
						ExecutionMode = ExecutionMode.Declaration,
						JobName = InputDataProvider.JobInputData().JobName,
						ModFileSuffix = loading.Key.ToString(),
						Report = Report,
						Mission = mission,
					};
					simulationRunData.EngineData.WHTCCorrectionFactor = DeclarationData.WHTCCorrection.Lookup(mission.MissionType,
						engineData.WHTCRural, engineData.WHTCUrban, engineData.WHTCMotorway);
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