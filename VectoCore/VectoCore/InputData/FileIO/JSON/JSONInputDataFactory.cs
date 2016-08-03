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

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	// ReSharper disable once InconsistentNaming
	public class JSONInputDataFactory
	{
		protected internal static JObject ReadFile(string fileName)
		{
			if (!File.Exists(fileName)) {
				throw new FileNotFoundException("failed to load file: " + fileName, fileName);
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
				case 3:
					return new JSONInputDataV3(json, filename);
				default:
					throw new VectoException("Job-File: Unsupported FileVersion. Got: {0} ", version);
			}
		}

		public static IVehicleEngineeringInputData ReadJsonVehicle(string filename)
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

		public static IGearboxEngineeringInputData ReadGearbox(string filename)
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

		public static IEngineEngineeringInputData ReadEngine(string filename)
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