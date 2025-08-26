using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	internal class DeclarationFuelCellIterativeStrategy
	{
		public static FCHEVIterativeRunStrategy SetUpFCHEVIterativeRunStrategy()
		{
			return new FCHEVIterativeRunStrategy(
					new[]
					{
							// Pre-run, iteration 0.
							new PreRunOptions()
							{
#if TRACE_FC
								WriteModAndSumData = true,
#else
								WriteModAndSumData = false
#endif
							},

							// Real run, iteration 1.
							new PreRunOptions()
							{
								WriteModAndSumData = true
							}
					});
		}

	}
}
