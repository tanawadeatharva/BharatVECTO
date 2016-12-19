using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DrivingCycleProxy : IDrivingCycleData
	{
		public DrivingCycleProxy(DrivingCycleData cycle, string name)
		{
			Name = name;
			CycleType = cycle.CycleType;
			Entries = cycle.Entries;
		}

		public List<DrivingCycleData.DrivingCycleEntry> Entries { get; private set; }

		public string Name { get; private set; }

		public CycleType CycleType { get; private set; }

		public void Finish()
		{
			Entries = new List<DrivingCycleData.DrivingCycleEntry>() {
				Entries.First(),
				Entries.Last()
			};
		}
	}
}