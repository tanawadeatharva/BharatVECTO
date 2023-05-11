using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Tests.Utils
{

	public class TestDeclarationCycleFactoryVariant : IDeclarationCycleFactory
	{
		private const string BASE_PATH = "Resources/Missions/";

		public TestDeclarationCycleFactoryVariant() { }

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


	public class TestDeclarationCycleFactoryStartPoint : DeclarationCycleFactory
	{
		private readonly Dictionary<MissionType, (Meter start, Meter distance)> _startPointDict = new Dictionary<MissionType, (Meter start, Meter distance)>();
		#region Implementation of IDeclarationCycleFactory
		/// <summary>
		/// Sets start point for specific mission
		/// </summary>
		/// <param name="missionType"></param>
		/// <param name="startPoint"></param>
		/// <param name="distance"></param>
		public void SetStartPoint(MissionType missionType, Meter startPoint, Meter distance = null)
		{
			_startPointDict[missionType] = (start: startPoint, distance: distance);
		}
		/// <summary>
		/// Sets start point for all missions
		/// </summary>
		/// <param name="startPoint"></param>
		/// <param name="distance"></param>
		public void SetStartPoint(Meter startPoint, Meter distance = null)
		{
			foreach (var missionType in Enum.GetValues<MissionType>()) {
				_startPointDict[missionType] = (start: startPoint, distance: distance);
            }
		}

		public override DrivingCycleData GetDeclarationCycle(Mission mission)
		{
			var cycle = base.GetDeclarationCycle(mission);
			if (_startPointDict.TryGetValue(mission.MissionType, out var entry)) {
				RestrictCycle(cycle, entry);
			}
			return cycle;
		}

		private void RestrictCycle(DrivingCycleData cycle, (Meter start, Meter distance) startPoint)
		{
	
			var entries = cycle.Entries;
			DrivingCycleData.DrivingCycleEntry startEntry = entries.First();
			DrivingCycleData.DrivingCycleEntry endEntry = entries.Last();

			startEntry = entries
				.Where(e => e.Distance.IsSmallerOrEqual(startPoint.start) &&
							e.VehicleTargetSpeed.IsEqual(0)) //Select all points before startpoint
				.MinBy(e => startPoint.start - e.Distance) ?? startEntry; //select the nearest one;
		
			//remove entries before selected start point
			//var startIdx = entries.FindIndex(e => e.Distance.IsEqual(startEntry.Distance));
			//entries.RemoveRange(0, startIdx);

			if (startPoint.distance != null) {



				var endIdx = entries.FindIndex(e =>
					(e.Distance - startEntry.Distance).IsGreaterOrEqual(startPoint.distance) &&
					e.VehicleTargetSpeed.IsEqual(0));
				if (endIdx != -1) {
					endEntry = entries[endIdx];
				}
				
			}

			entries.RemoveAll(e =>
				e.Distance.IsSmaller(startEntry.Distance) || e.Distance.IsGreater(endEntry.Distance));
		}
		#endregion
	}
}