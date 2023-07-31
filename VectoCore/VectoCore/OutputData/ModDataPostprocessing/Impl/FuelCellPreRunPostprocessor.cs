using System;
using System.Data;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
	public class FuelCellPreRunPostprocessor
	{

		protected ModDataWindowIterator WindowIterator;


		public FuelCellPreRunPostprocessor(IModalDataContainer modData)
		{
			WindowIterator = new ModDataWindowIterator((modData as ModalDataContainer).Data, 300.SI<Meter>());
		}


	}

	public class ModDataWindowIterator
	{
		public ModalResults ModData { get; set; }

		protected DataColumn DistanceColumn { get; }

		//protected ModDataAsRingBuffer RingBuffer { get; }

		protected Meter WindowDistance { get; }

		protected int NumRows;
		protected int PreStart;
		protected int Start;
		protected int End;
		protected int PostEnd;

        public ModDataWindowIterator(ModalResults results, Meter windowDistance)
		{
			ModData = results;
			NumRows = ModData.Rows.Count;
			
			WindowDistance = windowDistance;

			DistanceColumn = results.Columns[ModalResultField.dist.GetCaption()];

			Start = 0;
			var endDistance = (Meter)ModData.Rows[Start][DistanceColumn] + windowDistance;
			End = MoveITeratorForward(Start, r => (Meter)(ModData.Rows[r][DistanceColumn]) <= endDistance);
		} 

		protected int MoveITeratorForward(int startIdx, Func<int, bool> action)
		{
			var retVal = startIdx;
			var initialValue = action(startIdx);
			do {
				retVal = GetNextRow(retVal);
				if (action(retVal) != initialValue) {
					return retVal;
				}

				if (retVal == startIdx) {
					throw new VectoException("Failed to move iterator forward - iterated all elements");
                }
			} while (true);


			//while (!action(retVal)) {
			//	retVal = GetNextRow(retVal);
			//	if (retVal == startIdx) {
			//		throw new VectoException("Failed to move iterator forward - iterated all elements");
			//	}
			//}
			//return retVal;
		}

		protected int GetNextRow(int rowIdx)
		{
			return (rowIdx + 1) % NumRows;

		}

		protected int GetPreviousRow(int rowIdx)
		{
			return  (rowIdx - 1) % NumRows;
		}

    }

	public class ModDataAsRingBuffer
	{
		public ModalResults ModData { get; }

		protected int NumRows;
		
		public ModDataAsRingBuffer(ModalResults modData)
		{
			ModData = modData;
			NumRows = modData.Rows.Count;
		}

		
	}
}