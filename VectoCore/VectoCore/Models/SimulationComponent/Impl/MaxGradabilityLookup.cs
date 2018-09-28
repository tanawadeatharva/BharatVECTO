using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class MaxGradabilityLookup
	{
		protected internal Dictionary<uint, Tuple<Radian, Radian>> _data;

		public Dictionary<uint, Tuple<Radian, Radian>> Data
		{
			set { _data = value; }
		}

		public Radian GradabilityMaxTorque(uint gear)
		{
			return _data[gear].Item1;
		}

		public Radian GradabilityLimitedTorque(uint gear)
		{
			return _data[gear].Item2;
		}
	}
}