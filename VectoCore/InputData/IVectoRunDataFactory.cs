using System.Collections.Generic;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.InputData
{
	public interface IVectoRunDataFactory
	{
		IEnumerable<VectoRunData> NextRun();
	}
}