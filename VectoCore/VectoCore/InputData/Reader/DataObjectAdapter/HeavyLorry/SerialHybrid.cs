using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry
{
	public partial class DeclarationDataAdapterHeavyLorry
	{
		public abstract class SerialHybrid : LorryBase { }

		public class HEV_S2 : SerialHybrid
		{
		}


		public class HEV_S_IEPC : SerialHybrid
		{
		}
    }
}