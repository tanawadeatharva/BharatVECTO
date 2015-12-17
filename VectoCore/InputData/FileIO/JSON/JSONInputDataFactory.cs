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
			return json.GetEx(JsonKeys.JsonHeader).GetEx(JsonKeys.JsonHeader_FileVersion).Value<int>();
		}
	}
}