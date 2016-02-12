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

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCore.Exceptions;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONInputDataFactory
	{
		protected static JObject ReadFile(string fileName)
		{
			if (!File.Exists(fileName)) {
				throw new FileNotFoundException("failed to load file", fileName);
			}
			using (var reader = File.OpenText(fileName)) {
				return (JObject)JToken.ReadFrom(new JsonTextReader(reader));
			}
		}

		public static IInputDataProvider ReadJsonJob(string filename)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version) {
				case 2:
					return new JSONInputDataV2(json, filename);
				default:
					throw new VectoException("Job-File: Unsupported FileVersion. Got: {0} ", version);
			}
		}

		public static IVehicleInputData ReadJsonVehicle(string filename)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version) {
				case 7:
					return new JSONVehicleDataV7(json, filename);
				default:
					throw new VectoException("Vehicle-File: Unsupported FileVersion. Got {0}", version);
			}
		}

		public static IGearboxInputData ReadGearbox(string filename)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version) {
				case 5:
					return new JSONGearboxDataV5(json, filename);
				default:
					throw new VectoException("Gearbox-File: Unsupported FileVersion. Got {0}", version);
			}
		}

		public static IEngineInputData ReadEngine(string filename)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version) {
				case 3:
					return new JSONEngineDataV3(json, filename);
				default:
					throw new VectoException("Engine-File: Unsupported FileVersion. Got {0}", version);
			}
		}

		private static int ReadVersion(JObject json)
		{
			var value = json.GetEx(JsonKeys.JsonHeader).GetEx<string>(JsonKeys.JsonHeader_FileVersion);
			return (int)double.Parse(value.Trim('"'));
		}
	}
}