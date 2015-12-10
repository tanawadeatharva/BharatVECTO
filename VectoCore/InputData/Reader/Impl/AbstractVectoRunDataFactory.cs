using System.Collections.Generic;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public abstract class AbstractVectoRunDataFactory : IVectoRunDataFactory
	{
		public abstract IEnumerable<VectoRunData> NextRun();
	}
}