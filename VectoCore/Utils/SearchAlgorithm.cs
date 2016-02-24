using System;
using System.Collections.Generic;
using NLog;
using TUGraz.VectoCore.Exceptions;

namespace TUGraz.VectoCore.Utils
{
	public static class SearchAlgorithm
	{
		public static T Search<T>(T x, SI y, T interval, Func<object, SI> getYValue,
			Func<T, object> evaluateFunction, Func<object, bool> criterion) where T : SIBase<T>
		{
#if DEBUG
			var res = InterpolateLinear(x, y, interval, getYValue, evaluateFunction, criterion);
#else
			var res = SearchBinary(x, y, interval, getYValue, evaluateFunction, criterion);
#endif
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

		public static T InterpolateLinear<T>(T x1, SI y1, T interval, Func<object, SI> getYValue,
			Func<T, object> evaluateFunction, Func<object, bool> criterion) where T : SIBase<T>
		{
			var debug = new List<dynamic> { new { x = x1, y = y1 } };

			var x2 = x1 + interval;
			var result = evaluateFunction(x2);
			if (criterion(result)) {
				LogManager.GetLogger(typeof(SearchAlgorithm).FullName)
					.Debug("InterpolateLinear found an operating point after 1 function call.");
				return x2;
			}

			for (var iterationCount = 2; iterationCount < 10; iterationCount++) {
				var y2 = getYValue(result);
				debug.Add(new { x = x2, y = y2 });
				var k = (y2 - y1) / (x2 - x1);
				var d = y2 - k * x2;

				x1 = x2;
				x2 = (-d / k).Cast<T>();

				result = evaluateFunction(x2);
				if (criterion(result)) {
					LogManager.GetLogger(typeof(SearchAlgorithm).FullName)
						.Debug("InterpolateLinear found an operating point after {0} function calls.", iterationCount);
					return x2;
				}

				y1 = y2;
			}

			LogManager.GetLogger(typeof(SearchAlgorithm).FullName).Debug("InterpolateLinear found no operating point");
			throw new VectoException("Operating point not found.");
		}
	}
}