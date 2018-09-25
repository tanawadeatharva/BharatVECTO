using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class MaxGradabilityLookup
	{
		protected internal Dictionary<uint, Tuple<Radian, Radian>> _data;

		public MaxGradabilityLookup()
		{
		}

		public Dictionary<uint, Tuple<Radian, Radian>> Data
		{
			set { _data = value; }
		}
	}
}