/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

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
			var dao = new EngineeringDataAdapter();
			var driver = dao.CreateDriverData(InputDataProvider.DriverInputData);
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			var engineData = dao.CreateEngineData(vehicle.EngineInputData, vehicle.GearboxInputData,
				vehicle.TorqueLimits, vehicle.TankSystem);

			var tempVehicle = dao.CreateVehicleData(vehicle);

			var axlegearData = dao.CreateAxleGearData(vehicle.AxleGearInputData);
			var gearboxData = dao.CreateGearboxData(vehicle.GearboxInputData, engineData, axlegearData.AxleGear.Ratio,
				tempVehicle.DynamicTyreRadius,tempVehicle.VehicleCategory);
			var crossWindRequired = vehicle.AirdragInputData.CrossWindCorrectionMode ==
									CrossWindCorrectionMode.VAirBetaLookupTable;
			var angledriveData = dao.CreateAngledriveData(vehicle.AngledriveInputData);
			var ptoTransmissionData = dao.CreatePTOTransmissionData(vehicle.PTOTransmissionInputData);

			return InputDataProvider.JobInputData.Cycles.Select(cycle => {
				var drivingCycle = CyclesCache.ContainsKey(cycle.CycleData.Source)
					? CyclesCache[cycle.CycleData.Source]
					: DrivingCycleDataReader.ReadFromDataTable(cycle.CycleData, cycle.Name, crossWindRequired);

				return new VectoRunData {
					JobName = InputDataProvider.JobInputData.JobName,
					EngineData = engineData,
					GearboxData = gearboxData,
					AxleGearData = axlegearData,
					AngledriveData = angledriveData,
					VehicleData = dao.CreateVehicleData(vehicle),
					AirdragData = dao.CreateAirdragData(vehicle.AirdragInputData, vehicle),
					DriverData = driver,
					Aux = dao.CreateAuxiliaryData(vehicle.AuxiliaryInputData()),
					AdvancedAux = dao.CreateAdvancedAuxData(vehicle.AuxiliaryInputData()),
					Retarder = dao.CreateRetarderData(vehicle.RetarderInputData),
					PTO = ptoTransmissionData,
					Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
					ExecutionMode = ExecutionMode.Engineering,
					SimulationType = SimulationType.DistanceCycle | SimulationType.MeasuredSpeedCycle | SimulationType.PWheel
				};
			});
		}
	}
}
