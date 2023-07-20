using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.Fluent;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies
{
    internal class FCHEVIterativeRunStrategy : AbstractIterativeRunStrategy<FCHEVIterativeRunStrategy.FCHEVIterativeRunResult>
	{
		

		private bool firstRunPerformed = false;
		public class FCHEVIterativeRunResult : IIterativeRunResult
		{

		}


		#region Overrides of AbstractIterativeRunStrategy<FCHEVIterativeRunResult>

		public override bool RunAgain(int iteration, IModalDataContainer modData, VectoRunData runData)
		{
			if (!firstRunPerformed) {
				firstRunPerformed = true;
				
				return true;
			}

			return false;

		}

		public override void UpdateRunData(int iteration, IModalDataContainer modData, VectoRunData runData)
		{
			


			//Perform necessary changes to the rundata for the next run
		}

		#endregion
	}
}
