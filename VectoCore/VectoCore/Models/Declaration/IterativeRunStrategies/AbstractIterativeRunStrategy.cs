using System.Collections.Generic;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies
{
	public abstract class AbstractIterativeRunStrategy<T> : IIterativeRunStrategy where T : IIterativeRunResult, new()
	{

		protected Dictionary<int, T> _results = new Dictionary<int, T>(3);
		#region Implementation of IIterativeRunStrategy

		protected AbstractIterativeRunStrategy()
		{

		}

		public abstract bool RunAgain(int iteration, IModalDataContainer modData, VectoRunData runData);

		public abstract void UpdateRunData(int iteration, IModalDataContainer modData, VectoRunData runData);
		public bool Enabled { get; set; } = true;

		#endregion
	}
}