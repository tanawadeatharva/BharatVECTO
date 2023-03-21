using System;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies
{
	/// <summary>
	/// The default iterative run strategy wich doesn't perform any additional runs
	/// </summary>
	public class DefaultIterativeStrategy : AbstractIterativeRunStrategy<DefaultIterativeStrategy.DefaultIterativeResult>
	{
		public class DefaultIterativeResult : IIterativeRunResult {

		}

		public DefaultIterativeStrategy()
		{
			Enabled = false;
		}

		public override bool RunAgain(int iteration, IModalDataContainer modData, VectoRunData runData)
		{
			return false;
		}

		public override void UpdateRunData(int iteration, IModalDataContainer modData, VectoRunData runData)
		{
			throw new NotFiniteNumberException("We should not be here, ever!");
		}

	}
}