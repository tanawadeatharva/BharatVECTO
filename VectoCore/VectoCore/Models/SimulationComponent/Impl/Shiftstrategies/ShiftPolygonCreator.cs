using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies.ShiftPolygonCalc;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies
{
	public abstract class ShiftPolygonCalculator
	{
		public IShiftPolygonCalculator Create(string name, ShiftStrategyParameters shiftStrategyParameters)
		{
			if (name == AMTShiftStrategyOptimized.Name) {
				return new AMTShiftStrategyOptimizedPolygonCalculator();
			}

			if (name == AMTShiftStrategy.Name) {
				return new AMTShiftStrategyPolygonCalculator();
			}

			if (name == PEVAMTShiftStrategy.Name) {
				if (shiftStrategyParameters == null) {
					throw new ArgumentException($"{nameof(shiftStrategyParameters)} required for {name}");
				}
				return new PEVAMTShiftStrategyPolygonCreator(shiftStrategyParameters);
			}

			if (name == MTShiftStrategy.Name) {
				return new AMTShiftStrategyPolygonCalculator();
			}

			if (name == ATShiftStrategyOptimized.Name) {
				return new ATShiftStrategyOptimizedPolygonCalculator();
			}

			if (name == APTNShiftStrategy.Name) {
				if (shiftStrategyParameters == null)
				{
					throw new ArgumentException($"{nameof(shiftStrategyParameters)} required for {name}");
				}
				return new PEVAMTShiftStrategyPolygonCreator(shiftStrategyParameters);
			}
			//if (name == ATShiftStrategy.Name) {
			//	return new ATShiftStrategyPolygonCalculator();
			//}


			throw new ArgumentException($"Could not create ShiftPolygonCalculator for {name}");


		}
	}
}