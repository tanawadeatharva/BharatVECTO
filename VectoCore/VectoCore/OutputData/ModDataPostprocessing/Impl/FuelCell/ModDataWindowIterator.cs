using System;
using System.Data;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl.FuelCell
{
	public class ModDataWindowIterator
	{
		public ModalDataContainer ModData { get; set; }

		protected DataColumn DistanceColumn { get; }

		//protected ModDataAsRingBuffer RingBuffer { get; }

		protected Meter WindowDistance { get; }

		protected int NumRows;
		public int Position { get; protected set; }
		protected internal int Start { get; protected set; }
		protected internal int End { get; protected set; }
		public int Current { get; protected set; }

		protected bool CycleStart = true;

		public bool CycleEndReached { get; protected set; } = false;

		public bool WindowEndReached { get; protected set; } = false;

		public Meter CycleStartDistance { get; set; }

		public ModDataWindowIterator(ModalDataContainer results, Meter windowDistance)
		{
			ModData = results;
			NumRows = ModData.Data.Rows.Count;

			WindowDistance = windowDistance;

			DistanceColumn = results.Data.Columns[ModalResultField.dist.GetShortCaption()];
			CycleStartDistance = (Meter)results.Data.Rows[0][DistanceColumn] - (Meter)results.Data.Rows[0][ModalResultField.simulationDistance.GetName()];


			if (windowDistance.IsGreaterOrEqual(ModData.Distance))
			{
				throw new VectoException("Window size must be smaller than total cycle distance");
			}

			SetIteratorPositions(0);
		}


		public virtual void MoveNext()
		{
			if (CycleEndReached)
			{
				//return;
				throw new VectoException("Failed to move iterator forward - iterated all elements");

			}

			CycleStart = false;

			var newStart = GetNextRow(Position);
			SetIteratorPositions(newStart);
			//var nextWindow = GetNextRow(Position);
			if (Position == 0)
			{
				CycleEndReached = true;
			}
		}

		public virtual void NextEntry()
		{
			if (WindowEndReached)
			{
				return;
			}
			Current = GetNextRow(Current);
			if (Current == GetNextRow(End))
			{
				WindowEndReached = true;
			}
		}

		public override string ToString()
		{

			return
				$"ModDataWindow: wndSize: {WindowDistance}, Position: {Position} ({ModData.Data.Rows[Position][DistanceColumn]} " +
				$"Start: {Start} ({ModData.Data.Rows[Start][DistanceColumn]}) " +
				$"End: {End} ({ModData.Data.Rows[End][DistanceColumn]}) " +
				$"ActualWindowSize: {ActualWindowSize}";
		}

		public virtual Meter ActualWindowSize
		{
			get
			{
				var startDistance = (Meter)ModData.Data.Rows[Start][DistanceColumn];
				var endDistance = (Meter)ModData.Data.Rows[End][DistanceColumn];
				var diff = endDistance - startDistance;

				return diff < 0 ? diff + ModData.Distance : diff;
			}
		}

		protected void SetIteratorPositions(int position)
		{
			Position = position;

			var currentDistance = (Meter)ModData.Data.Rows[Position][DistanceColumn];
			var startDistance = GetDistanceInCycle(currentDistance - WindowDistance / 2.0);
			var endDistance = GetDistanceInCycle(currentDistance + WindowDistance / 2.0);

			if (CycleStart)
			{
				End = MoveIteratorForward(Position, r => (Meter)ModData.Data.Rows[r][DistanceColumn] <= endDistance);
				Start = MoveIteratorBackward(GetPreviousRow(Position), r => (Meter)ModData.Data.Rows[r][DistanceColumn] <= startDistance);
			}
			else
			{
				if (endDistance < (Meter)ModData.Data.Rows[End][DistanceColumn])
				{
					End = MoveIteratorForward(0, r => (Meter)ModData.Data.Rows[r][DistanceColumn] <= endDistance);
				}
				else
				{
					End = MoveIteratorForward(End, r =>
					{
						if (endDistance.IsEqual(ModData.Distance) && r == 0)
						{
							return false;
						}
						return (Meter)ModData.Data.Rows[r][DistanceColumn] <= endDistance;
					});
				}

				if (startDistance < (Meter)ModData.Data.Rows[Start][DistanceColumn])
				{
					Start = MoveIteratorForward(0, r => (Meter)ModData.Data.Rows[r][DistanceColumn] <= startDistance);
				}
				else
				{
					Start = GetNextRow(MoveIteratorForward(Start,
						r => (Meter)ModData.Data.Rows[r][DistanceColumn] <= startDistance));
				}
			}

			Current = Start;
			WindowEndReached = false;
		}

		protected virtual SIBase<Meter> GetDistanceInCycle(Meter relativeDistance)
		{
			var cycleDistance = ModData.Distance;

			while (relativeDistance.IsGreater(CycleStartDistance + cycleDistance))
			{
				relativeDistance -= cycleDistance;
			}

			while (relativeDistance.IsSmaller(CycleStartDistance))
			{
				relativeDistance += cycleDistance;
			}
			return relativeDistance;
		}

		protected virtual int MoveIteratorForward(int startIdx, Func<int, bool> action)
		{
			var retVal = startIdx;
			var initialValue = action(startIdx);
			do
			{
				retVal = GetNextRow(retVal);
				if (action(retVal) != initialValue)
				{
					return GetPreviousRow(retVal);
				}
			} while (retVal != startIdx);

			return retVal;
		}


		protected virtual int MoveIteratorBackward(int startIdx, Func<int, bool> action)
		{
			var retVal = startIdx;
			var initialValue = action(startIdx);
			do
			{
				retVal = GetPreviousRow(retVal);
				if (action(retVal) != initialValue)
				{
					return GetNextRow(retVal);
				}

				if (retVal == startIdx)
				{
					throw new VectoException("Failed to move iterator forward - iterated all elements");
				}
			} while (true);
		}

		protected virtual int GetNextRow(int rowIdx)
		{
			return (rowIdx + 1) % NumRows;

		}

		protected virtual int GetPreviousRow(int rowIdx)
		{
			var retVal = (rowIdx - 1) % NumRows;
			if (retVal < 0)
			{
				retVal += NumRows;
			}
			return retVal;
		}

	}
}