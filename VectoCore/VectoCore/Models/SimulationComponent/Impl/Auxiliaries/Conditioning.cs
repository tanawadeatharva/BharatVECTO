using System;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Auxiliaries
{
	public class Conditioning
	{


		public Conditioning(VectoRunData.AuxData auxData)
		{
			if (!auxData.ID.Contains(Constants.Auxiliaries.IDs.Cond)) {
				throw new ArgumentException();
			}












		}
	}
}