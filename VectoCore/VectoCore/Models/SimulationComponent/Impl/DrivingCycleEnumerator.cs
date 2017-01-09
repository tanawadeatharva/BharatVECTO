using System.Collections.Generic;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public sealed class DrivingCycleEnumerator : IEnumerator<DrivingCycleData.DrivingCycleEntry>
	{
		private int _currentCycleIndex;
		private readonly IDrivingCycleData _data;

		public DrivingCycleEnumerator(IDrivingCycleData data)
		{
			_currentCycleIndex = 0;
			_data = data;
			LastEntry = false;
		}

		public DrivingCycleEnumerator Clone()
		{
			return new DrivingCycleEnumerator(_data) {
				_currentCycleIndex = _currentCycleIndex,
				LastEntry = LastEntry
			};
		}

		public DrivingCycleData.DrivingCycleEntry Current
		{
			get { return LeftSample; }
		}

		public DrivingCycleData.DrivingCycleEntry Next
		{
			get { return RightSample; }
		}

		public DrivingCycleData.DrivingCycleEntry LeftSample
		{
			get { return _data.Entries[_currentCycleIndex]; }
		}

		public DrivingCycleData.DrivingCycleEntry RightSample
		{
			get { return _currentCycleIndex + 1 >= _data.Entries.Count ? null : _data.Entries[_currentCycleIndex + 1]; }
		}

		public bool LastEntry { get; private set; }

		object System.Collections.IEnumerator.Current
		{
			get { return LeftSample; }
		}

		public bool MoveNext()
		{
			// cycleIndex has to be max. next to last (so that rightSample is still valid.
			if (_currentCycleIndex >= _data.Entries.Count - 2) {
				LastEntry = true;
				return false;
			}
			_currentCycleIndex++;
			if (_currentCycleIndex == _data.Entries.Count - 2) {
				LastEntry = true;
			}

			return true;
		}

		public void Reset()
		{
			_currentCycleIndex = 0;
		}

		public void Dispose() {}
	}
}