using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ShiftStrategy {
	public class ShareTorque99lLookup
	{
		private KeyValuePair<MeterPerSecond, double>[] _entries;

		public ShareTorque99lLookup(KeyValuePair<MeterPerSecond, double>[] entries)
		{
			_entries = entries;
		}

		public double Lookup(MeterPerSecond velocity)
		{
			return _entries.Interpolate(x => x.Key.Value(), y => y.Value, velocity.Value());
		}
	}
}