using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.MediumLorryRunDataFactory
{
    internal abstract class DeclarationModeMediumLorryRunDataFactory
    {
		public abstract class MediumLorryBase : IVectoRunDataFactory
		{
			#region Implementation of IVectoRunDataFactory

			public IEnumerable<VectoRunData> NextRun()
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public class Conventional : MediumLorryBase
		{

		}

		public class HEV_S2 : MediumLorryBase
		{

		}

		public class HEV_S3 : MediumLorryBase
		{

		}


		public class HEV_S4 : MediumLorryBase
		{

		}




    }
}
