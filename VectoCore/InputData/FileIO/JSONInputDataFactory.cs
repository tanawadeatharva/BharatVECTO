using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.InputData.FileIO.JSON;

namespace TUGraz.VectoCore.InputData.FileIO
{
	public class JSONInputDataFactory
	{
		protected static JObject ReadFile(string fileName)
		{
			using (StreamReader reader = File.OpenText(fileName)) {
				return (JObject)JToken.ReadFrom(new JsonTextReader(reader));
			}
		}

		public static IInputDataProvider ReadJsonJob(string filename)
		{
			var json = ReadFile(filename);
			var version = json[JsonKeys.JsonHeader][JsonKeys.JsonHeader_FileVersion].Value<int>();
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
			var version = json[JsonKeys.JsonHeader][JsonKeys.JsonHeader_FileVersion].Value<int>();
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
			var version = json[JsonKeys.JsonHeader][JsonKeys.JsonHeader_FileVersion].Value<int>();
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
			var version = json[JsonKeys.JsonHeader][JsonKeys.JsonHeader_FileVersion].Value<int>();
			switch (version) {
				case 3:
					return new JSONEngineDataV3(json, filename);
				default:
					throw new VectoException("Engine-File: Unsupported FileVersion. Got {0}", version);
			}
		}
	}
}