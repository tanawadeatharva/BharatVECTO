/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

[assembly: InternalsVisibleTo("VectoCoreTest")]

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class EngineeringModeVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		private static readonly Dictionary<string, DrivingCycleData> CyclesCache = new Dictionary<string, DrivingCycleData>();

		protected readonly IEngineeringInputDataProvider InputDataProvider;

		internal EngineeringModeVectoRunDataFactory(IEngineeringInputDataProvider dataProvider)
		{
			InputDataProvider = dataProvider;
		}

		/// <summary>
		/// Iterate over all cycles defined in the JobFile and create a container with all data required for creating a simulation run
		/// </summary>
		/// <returns>VectoRunData instance for initializing the powertrain.</returns>
		public virtual IEnumerable<VectoRunData> NextRun()
		{

			switch(InputDataProvider.JobInputData.JobType) {
				case VectoSimulationJobType.ConventionalVehicle:
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.EngineOnlySimulation:
					return GetConventionalVehicleRunData();
				case VectoSimulationJobType.BatteryElectricVehicle:
					return GetBatteryElectricVehicleRunData();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private IEnumerable<VectoRunData> GetBatteryElectricVehicleRunData()
		{
            foreach (var cycle in InputDataProvider.JobInputData.Cycles)
            {
                var dao = new EngineeringDataAdapter();
                var driver = dao.CreateDriverData(InputDataProvider.DriverInputData);
				var vehicle = InputDataProvider.JobInputData.Vehicle;

				var axlegearData = vehicle.Components.AxleGearInputData != null
					? dao.CreateAxleGearData(vehicle.Components.AxleGearInputData)
					: null;

				var electricMachinesData = dao.CreateElectricMachines(vehicle.Components.ElectricMachines, vehicle.ElectricMotorTorqueLimits);

				GearboxData gearboxData = null;
				ShiftStrategyParameters gearshiftParams = null;
				AngledriveData angledriveData = null;
				if (electricMachinesData.Any(x => x.Item1 == PowertrainPosition.BatteryElectricE2)) {
					// gearbox required!
					var tmpRunData = new VectoRunData() {
						JobType = VectoSimulationJobType.BatteryElectricVehicle,
						ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy,
						GearboxData = new GearboxData() {
							Type = vehicle.Components.GearboxInputData.Type,
						},
						//ElectricMachinesData = electricMachinesData,
						//VehicleData = dao.CreateVehicleData(vehicle)
					};
					var tempVehicle = dao.CreateVehicleData(vehicle);
					var tmpStrategy = PowertrainBuilder.GetShiftStrategy(new SimplePowertrainContainer(tmpRunData));
					gearboxData = dao.CreateGearboxData(
						InputDataProvider, new VectoRunData() {
							JobType = VectoSimulationJobType.BatteryElectricVehicle,
							VehicleData = tempVehicle,
							AxleGearData = axlegearData,
							ElectricMachinesData = electricMachinesData
						}, tmpStrategy);
                    angledriveData = dao.CreateAngledriveData(vehicle.Components.AngledriveInputData);
                    gearshiftParams = dao.CreateGearshiftData(
						gearboxData.Type, InputDataProvider.DriverInputData.GearshiftInputData,
						axlegearData.AxleGear.Ratio * (angledriveData?.Angledrive.Ratio ?? 1.0),null);
				}

				if (gearshiftParams == null) {
					gearshiftParams = new ShiftStrategyParameters() {
						StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
						StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
					};
				}

				var crossWindRequired = vehicle.Components.AirdragInputData.CrossWindCorrectionMode ==
										CrossWindCorrectionMode.VAirBetaLookupTable;
                //var ptoTransmissionData = dao.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

                var drivingCycle = CyclesCache.ContainsKey(cycle.CycleData.Source)
                    ? CyclesCache[cycle.CycleData.Source]
                    : DrivingCycleDataReader.ReadFromDataTable(cycle.CycleData, cycle.Name, crossWindRequired);

				yield return new VectoRunData
                {
                    JobName = InputDataProvider.JobInputData.JobName,
                    JobType = VectoSimulationJobType.BatteryElectricVehicle,
                    GearboxData = gearboxData,
                    AxleGearData = axlegearData,
                    AngledriveData = angledriveData,
                    VehicleData = dao.CreateVehicleData(vehicle),
                    AirdragData = dao.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
                    DriverData = driver,
                    Aux = dao.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
                    //BusAuxiliaries = dao.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData),
                    Retarder = dao.CreateRetarderData(vehicle.Components.RetarderInputData),
                    //PTO = ptoTransmissionData,
                    Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
                    ExecutionMode = ExecutionMode.Engineering,
                    ElectricMachinesData = electricMachinesData,
                    //HybridStrategyParameters = dao.CreateHybridStrategyParameters(InputDataProvider.JobInputData.HybridStrategyParameters),
                    BatteryData = dao.CreateBatteryData(vehicle.Components.ElectricStorage, vehicle.InitialSOC),
					SuperCapData = dao.CreateSuperCapData(vehicle.Components.ElectricStorage, vehicle.InitialSOC),
                    SimulationType = SimulationType.DistanceCycle | SimulationType.MeasuredSpeedCycle | SimulationType.PWheel,
                    GearshiftParameters = gearshiftParams,
                    ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy,
					ElectricAuxDemand = InputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries.ElectricPowerDemand,
                };
            }
        }

		protected virtual IEnumerable<VectoRunData> GetConventionalVehicleRunData()
		{
			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;

			//foreach (var engineMode in engineModes) {
			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				var engineMode = engineModes[modeIdx];
				foreach (var cycle in InputDataProvider.JobInputData.Cycles) {
					var dao = new EngineeringDataAdapter();
					var driver = dao.CreateDriverData(InputDataProvider.DriverInputData);
					var vehicle = InputDataProvider.JobInputData.Vehicle;
					var engineData = dao.CreateEngineData(vehicle, engineMode);
					engineData.FuelMode = modeIdx;
					var tempVehicle = dao.CreateVehicleData(vehicle);

					var axlegearData = dao.CreateAxleGearData(vehicle.Components.AxleGearInputData);
					var tmpRunData = new VectoRunData() {
						ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy,
						GearboxData = new GearboxData() {
							Type = vehicle.Components.GearboxInputData.Type,
						}
					};
					var tmpStrategy = PowertrainBuilder.GetShiftStrategy(new SimplePowertrainContainer(tmpRunData));
					var gearboxData = dao.CreateGearboxData(
						InputDataProvider, new VectoRunData() {
							EngineData = engineData,
							VehicleData = tempVehicle,
							AxleGearData = axlegearData
						}, tmpStrategy);

					var crossWindRequired = vehicle.Components.AirdragInputData.CrossWindCorrectionMode ==
											CrossWindCorrectionMode.VAirBetaLookupTable;
					var angledriveData = dao.CreateAngledriveData(vehicle.Components.AngledriveInputData);
					var ptoTransmissionData = dao.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

					var drivingCycle = CyclesCache.ContainsKey(cycle.CycleData.Source)
						? CyclesCache[cycle.CycleData.Source]
						: DrivingCycleDataReader.ReadFromDataTable(cycle.CycleData, cycle.Name, crossWindRequired);

					var electricMachines = dao.CreateElectricMachines(vehicle.Components.ElectricMachines, vehicle.ElectricMotorTorqueLimits) ?? new List<Tuple<PowertrainPosition, ElectricMotorData>>();
					var battery = dao.CreateBatteryData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
					var superCap = dao.CreateSuperCapData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);

					var jobType = electricMachines.Count > 0 && (battery != null || superCap != null)
						? VectoSimulationJobType.ParallelHybridVehicle
						: VectoSimulationJobType.ConventionalVehicle;

					var vehicleData = dao.CreateVehicleData(vehicle);
					
					var hybridParameters = jobType == VectoSimulationJobType.ParallelHybridVehicle
						? dao.CreateHybridStrategyParameters(InputDataProvider.JobInputData.HybridStrategyParameters, vehicle.MaxPropulsionTorque, engineData)
						: null;
					yield return new VectoRunData {
						JobName = InputDataProvider.JobInputData.JobName,
						JobType = jobType,
						EngineData = engineData,
						GearboxData = gearboxData,
						AxleGearData = axlegearData,
						AngledriveData = angledriveData,
						VehicleData = vehicleData,
						AirdragData = dao.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
						DriverData = driver,
						Aux = dao.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
						BusAuxiliaries = dao.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, vehicleData),
						Retarder = dao.CreateRetarderData(vehicle.Components.RetarderInputData),
						PTO = ptoTransmissionData,
						Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
						ExecutionMode = ExecutionMode.Engineering,
						ElectricMachinesData = electricMachines,
						HybridStrategyParameters = hybridParameters,
						BatteryData = battery,
						SuperCapData = superCap,
						SimulationType = SimulationType.DistanceCycle | SimulationType.MeasuredSpeedCycle | SimulationType.PWheel,
						GearshiftParameters = dao.CreateGearshiftData(
							gearboxData.Type, InputDataProvider.DriverInputData.GearshiftInputData,
							axlegearData.AxleGear.Ratio * (angledriveData?.Angledrive.Ratio ?? 1.0), engineData.IdleSpeed),
						ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy,
						ElectricAuxDemand = InputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries.ElectricPowerDemand,
					};
				}
			}
		}
	}
}
