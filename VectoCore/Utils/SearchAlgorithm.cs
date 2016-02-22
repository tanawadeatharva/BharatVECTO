using System;
using System.Collections.Generic;
using System.Diagnostics;
using NLog;
using TUGraz.VectoCore.Exceptions;

namespace TUGraz.VectoCore.Utils
{
	public static class SearchAlgorithm
	{
		public static T Search<T>(T x, SI y, T interval, Func<object, SI> getYValue,
			Func<T, object> evaluateFunction, Func<object, bool> criterion) where T : SIBase<T>
		{
			var res = SearchBinary(x, y, interval, getYValue, evaluateFunction, criterion);
			return res;
		}

		/// <summary>
		/// Line Search Algorithm. 
		/// Phase 1: Linear Bracketing
		/// Phase 2: Binary Sectioning
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="interval"></param>
		/// <param name="getYValue"></param>
		/// <param name="evaluateFunction"></param>
		/// <param name="criterion"></param>
		/// <returns></returns>
		public static T SearchBinary<T>(T x, SI y, T interval, Func<object, SI> getYValue,
			Func<T, object> evaluateFunction, Func<object, bool> criterion) where T : SIBase<T>
		{
			var intervalFactor = 1.0;
			var origY = y;
			var debug = new List<dynamic> { new { x, y } };

			for (var iterationCount = 1; iterationCount < 100; iterationCount++) {
				debug.Add(new { x, y });

				if (origY.Sign() != y.Sign()) {
					intervalFactor = 0.5;
				}

				interval *= intervalFactor;
				x += interval * -y.Sign();

				var result = evaluateFunction(x);
				if (criterion(result)) {
					LogManager.GetLogger(typeof(SearchAlgorithm).FullName)
						.Debug("InterpolateLinear found an operating point after {0} function calls.", iterationCount);
					return x;
				}
				y = getYValue(result);
			}

			LogManager.GetLogger(typeof(SearchAlgorithm).FullName).Debug("InterpolateLinear found no operating point");
			throw new VectoException("Operating point not found.");
		}
	}
}