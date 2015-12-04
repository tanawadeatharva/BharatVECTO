/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.FileIO.Reader
{
	public interface ISimulationDataReader
	{
		void SetJobFile(string fileName);

		IEnumerable<VectoRunData> NextRun();

		bool IsEngineOnly { get; }
		//	void SetJobJson(string jsonData, string basePath);
	}

	//public interface IDataFileReader
	//{
	//	VectoRunData ReadVectoJobFile(string fileName);

	//	VehicleData ReadVehicleDataFile(string fileName);

	//	VehicleData ReadVehicleDataJson(string jsonData, string basePath);

	//	void ReadEngineFile(string fileName);

	//	void ReadEngineJson(string jsonData, string basePath);

	//	//void AddCycle(string fileName);
	//}
}