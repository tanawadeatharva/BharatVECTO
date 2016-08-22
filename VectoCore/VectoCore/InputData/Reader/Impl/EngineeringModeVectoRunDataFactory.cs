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
		protected DriverData Driver;

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
			var engineData = dao.CreateEngineData(InputDataProvider.EngineInputData, InputDataProvider.GearboxInputData);

			var tempVehicle = dao.CreateVehicleData(InputDataProvider.VehicleInputData);

			var vehicleInputData = InputDataProvider.VehicleInputData;
			var axlegearData = dao.CreateAxleGearData(InputDataProvider.AxleGearInputData, useEfficiencyFallback: true);
			var gearboxData = dao.CreateGearboxData(InputDataProvider.GearboxInputData, engineData, axlegearData.AxleGear.Ratio,
				tempVehicle.DynamicTyreRadius, useEfficiencyFallback: true);
			var crossWindRequired = vehicleInputData.CrossWindCorrectionMode == CrossWindCorrectionMode.VAirBetaLookupTable;
			var angularGearData = dao.CreateAngularGearData(InputDataProvider.AngularGearInputData, useEfficiencyFallback: true);
			var ptoTransmissionData = dao.CreatePTOTransmissionData(InputDataProvider.PTOTransmissionInputData);

			return InputDataProvider.JobInputData().Cycles.Select(cycle => new VectoRunData {
				JobName = InputDataProvider.JobInputData().JobName,
				EngineData = engineData,
				GearboxData = gearboxData,
				AxleGearData = axlegearData,
				AngularGearData = angularGearData,
				VehicleData = dao.CreateVehicleData(vehicleInputData),
				DriverData = driver,
				Aux = dao.CreateAuxiliaryData(InputDataProvider.AuxiliaryInputData()),
				AdvancedAux = dao.CreateAdvancedAuxData(InputDataProvider.AuxiliaryInputData()),
				Retarder = dao.CreateRetarderData(InputDataProvider.RetarderInputData, InputDataProvider.VehicleInputData),
				PTOTransmission = ptoTransmissionData,
				Cycle = DrivingCycleDataReader.ReadFromDataTable(cycle.CycleData, cycle.Name, crossWindRequired),
				ExecutionMode = ExecutionMode.Engineering
			});
		}
	}
}