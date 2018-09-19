using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy
{
	public class PredictionDurationLookup : SimulationComponentData
	{
		private List<KeyValuePair<double, double>> _entries;

		protected internal PredictionDurationLookup(List<KeyValuePair<double, double>> entries)
		{
			_entries = entries;
		}

		public double Lookup(double speedRatio)
		{
			var index = FindIndex(speedRatio);

			return VectoMath.Interpolate(
				_entries[index - 1].Key, _entries[index].Key,
				_entries[index - 1].Value, _entries[index].Value, speedRatio);
		}

		private int FindIndex(double speedRatio)
		{
			var index = 1;
			if (speedRatio < _entries[0].Key) {
				Log.Error("requested speed ratio below minimum - extrapolating. speed ratio: {0}, min: {1}",
					speedRatio, _entries[0].Key);
			} else {
				index = _entries.FindIndex(x => x.Key > speedRatio);
				if (index <= 0) {
					index = speedRatio > _entries[0].Key ? _entries.Count - 1 : 1;
				}
			}
			return index;
		}

	}
}
