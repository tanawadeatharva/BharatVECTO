using System.IO;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Tests.Utils
{

	public class TestDeclarationCycleFactory : IDeclarationCycleFactory
	{
		private const string BASE_PATH = "Resources/Missions/";

		public TestDeclarationCycleFactory()
		{

		}

		public string Variant { get; set; } = "Short_10";

		public virtual DrivingCycleData GetDeclarationCycle(Mission mission)
		{
			return ReadDeclarationCycle(mission.MissionType);
		}

		protected virtual DrivingCycleData ReadDeclarationCycle(MissionType missionType)
		{
			var cycleFile = Path.Combine(BASE_PATH, Variant,
				missionType.ToString().Replace("EMS", "") + ".vdri");
			if (File.Exists(cycleFile)) {
				var cycle = File.OpenRead(cycleFile);
				return DrivingCycleDataReader.ReadFromStream(cycle, CycleType.DistanceBased, "", false);
			}

			throw new VectoException($"Cycle data for mission type {missionType} not found!");
		}
	}
}