using System;
using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Models.Declaration.PostMortemAnalysisStrategy
{

	public interface IPostMortemAnalyzeStrategy
	{
		bool AbortSimulation(IVehicleContainer container, Exception exception);
	}

	public class DefaultPostMortemAnalyseStrategy : IPostMortemAnalyzeStrategy
	{
		#region Implementation of IPostMortemAnalyzeStrategy

		public bool AbortSimulation(IVehicleContainer container, Exception exception)
		{
			return true;
		}

		#endregion
	}

	public class PrimaryBusPostMortemStrategy : IPostMortemAnalyzeStrategy
	{
		#region Implementation of IPostMortemAnalyzeStrategy

		public bool AbortSimulation(IVehicleContainer container, Exception exception)
		{
			return false;
		}

		#endregion
	}

}