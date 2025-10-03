using System.Collections.Concurrent;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{

	public interface IDeclarationCycleFactory
	{
		DrivingCycleData GetDeclarationCycle(Mission missionType);
	}

	public class DeclarationCycleFactory : IDeclarationCycleFactory
	{
		public static readonly ConcurrentDictionary<MissionType, DrivingCycleData> CyclesCache =
			new ConcurrentDictionary<MissionType, DrivingCycleData>();

		public DeclarationCycleFactory()
		{

		}

		#region Implementation of IDeclarationCycleFactory

		public virtual DrivingCycleData GetDeclarationCycle(Mission mission)
		{
			return ReadDeclarationCycle(mission.MissionType);

			//return CyclesCache.GetOrAdd(mission.MissionType, ReadDeclarationCycle);

		}

		protected virtual DrivingCycleData ReadDeclarationCycle(MissionType missionType)
		{
			var cycle = RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix +
												".MissionCycles." +
												missionType.ToString().Replace("EMS", "") +
												Constants.FileExtensions.CycleFile);
			
			return DrivingCycleDataReader.ReadFromStream(cycle, CycleType.DistanceBased, "", false);
		}

		#endregion
	}

	//// ToDo: MQ 2023-05-09: REMOVE CLASS IN PRODUCTION!!!
	//public class DeclarationCycleFromFilesystemFactory : DeclarationCycleFactory
	//{
	//	protected override DrivingCycleData ReadDeclarationCycle(MissionType missionType)
	//	{
	//		// TODO: MQ 2021-11-30: REMOVE IN PRODUCTION
	//		var cycleFile = Path.Combine("DeclarationMissions",
	//			missionType.ToString().Replace("EMS", "") + ".vdri");
	//		if (File.Exists(cycleFile)) {
	//			var cycle = File.OpenRead(cycleFile);
	//			return DrivingCycleDataReader.ReadFromStream(cycle, CycleType.DistanceBased, "", false);
	//		}

	//		return base.ReadDeclarationCycle(missionType);
	//	}
	//}
}