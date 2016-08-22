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

using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockSimulationDataFactory
	{
		/// <summary>
		/// Create gearboxdata instance directly from a file
		/// </summary>
		/// <param name="gearBoxFile"></param>
		/// <param name="engineFile"></param>
		/// <param name="declarationMode"></param>
		/// <returns>GearboxData instance</returns>
		public static GearboxData CreateGearboxDataFromFile(string gearBoxFile, string engineFile, bool declarationMode = true)
		{
			var gearboxInput = JSONInputDataFactory.ReadGearbox(gearBoxFile);
			var engineInput = JSONInputDataFactory.ReadEngine(engineFile);
			if (declarationMode) {
				var dao = new DeclarationDataAdapter();
				var engineData = dao.CreateEngineData(engineInput, gearboxInput.Type);
				return dao.CreateGearboxData(gearboxInput, engineData, ((IAxleGearInputData)gearboxInput).Ratio, 0.5.SI<Meter>(),
					false);
			} else {
				var dao = new EngineeringDataAdapter();
				var engineData = dao.CreateEngineData(engineInput, gearboxInput);
				return dao.CreateGearboxData(gearboxInput, engineData, ((IAxleGearInputData)gearboxInput).Ratio, 0.5.SI<Meter>(),
					true);
			}
		}

		public static AxleGearData CreateAxleGearDataFromFile(string axleGearFile)
		{
			var dao = new DeclarationDataAdapter();
			var axleGearInput = JSONInputDataFactory.ReadGearbox(axleGearFile);
			return dao.CreateAxleGearData((IAxleGearInputData)axleGearInput, false);
		}

		public static CombustionEngineData CreateEngineDataFromFile(string engineFile)
		{
			var dao = new EngineeringDataAdapter();
			var engineInput = JSONInputDataFactory.ReadEngine(engineFile);
			return dao.CreateEngineData(engineInput, null);
		}

		public static VehicleData CreateVehicleDataFromFile(string vehicleDataFile)
		{
			var dao = new EngineeringDataAdapter();
			var vehicleInput = JSONInputDataFactory.ReadJsonVehicle(vehicleDataFile);
			return dao.CreateVehicleData(vehicleInput);
		}

		public static DriverData CreateDriverDataFromFile(string driverDataFile)
		{
			var jobInput = JSONInputDataFactory.ReadJsonJob(driverDataFile);
			var engineeringJob = jobInput as IEngineeringInputDataProvider;
			if (engineeringJob == null) {
				throw new VectoException("Failed to cas to Engineering InputDataProvider");
			}
			var dao = new EngineeringDataAdapter();
			return dao.CreateDriverData(engineeringJob.DriverInputData);
		}
	}
}