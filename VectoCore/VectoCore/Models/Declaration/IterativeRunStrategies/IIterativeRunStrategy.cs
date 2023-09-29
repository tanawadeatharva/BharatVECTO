using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies
{
	public interface IIterativeRunStrategy
	{
		/// <summary>
		/// Determines if the run should be executed again
		/// </summary>
		/// <param name="iteration"></param>
		/// <param name="modData"></param>
		/// <param name="runData"></param>
		/// <returns></returns>
		bool RunAgain(int iteration, IModalDataContainer modData, VectoRunData runData);


		/// <summary>
		/// Updates runData according to the strategy and stores the needed results from the finished run
		/// </summary>
		/// <param name="iteration">The finished iteration</param>
		/// <param name="modData">The results of the finished run</param>
		/// <param name="runData">modifies runData</param>
		void UpdateRunData(int iteration, IModalDataContainer modData, VectoRunData runData);


		/// <summary>
		/// Enables or disables the iterative run strategy
		/// </summary>
		bool Enabled { get; set; }



		/// <summary>
		/// Get the PreRunOptions for the current iteration 
		/// </summary>
		PreRunOptions GetPreRunOptions(int iteration);
	}

	public interface IIterativeRunResult
	{
		//IIterativeRunResult CreateIterativeResult(IModalDataContainer modData);
	}



}