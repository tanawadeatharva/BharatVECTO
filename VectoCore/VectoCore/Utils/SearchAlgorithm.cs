/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	public static class SearchAlgorithm
	{
		/// <summary>
		/// Applies a numerical search over the evaluateFunction until the criterion reaches approximately 0.
		/// <code>
		/// SearchAlgorithm.Search(firstAcceleration, firstDelta, secondAccelerationInterval,
		///		getYValue: result => ((ResponseDryRun)result).Delta,
		///		evaluateFunction: x => NextComponent.Request(absTime, dt, x, gradient, true),
		///		criterion: result => ((ResponseDryRun)result).Delta);
		/// </code>
		/// </summary>
		public static T Search<T>(T x, SI y, T interval, Func<object, SI> getYValue, Func<T, object> evaluateFunction,
			Func<object, double> criterion) where T : SIBase<T>
		{
			var iterationCount = 0;
			return Search(x, y, interval, getYValue, evaluateFunction, criterion, ref iterationCount);
		}

		/// <summary>
		/// Applies a numerical search over the evaluateFunction until the criterion reaches approximately 0.
		/// <code>
		/// SearchAlgorithm.Search(firstAcceleration, firstDelta, secondAccelerationInterval,
		///		getYValue: result => ((ResponseDryRun)result).Delta,
		///		evaluateFunction: x => NextComponent.Request(absTime, dt, x, gradient, true),
		///		criterion: result => ((ResponseDryRun)result).Delta);
		/// </code>
		/// </summary>
		public static T Search<T>(T x, SI y, T interval, Func<object, SI> getYValue,
			Func<T, object> evaluateFunction, Func<object, double> criterion, ref int iterationCount) where T : SIBase<T>
		{
			T result;
			try {
				result = InterpolateLinear(x, y, interval, getYValue, evaluateFunction, criterion, ref iterationCount);
			} catch (VectoException ex) {
				var log = LogManager.GetLogger(typeof(SearchAlgorithm).FullName);
				log.Warn("Falling back to LineSearch. InterpolationSearch failed: " + ex.Message);
				result = LineSearch(x, y, interval, getYValue, evaluateFunction, criterion, ref iterationCount);
			}
			return result;
		}


		/// <summary>
		/// Line Search Algorithm. 
		/// Phase 1: Linear Bracketing: Search iterative for the area of interest (with fixed step size).
		/// Phase 2: Binary Sectioning: Binary search in the area of interest.
		/// </summary>
		private static T LineSearch<T>(T x, SI y, T interval, Func<object, SI> getYValue,
			Func<T, object> evaluateFunction, Func<object, double> criterion, ref int iterationCount) where T : SIBase<T>
		{
			var log = LogManager.GetLogger(typeof(SearchAlgorithm).FullName);

			var intervalFactor = 1.0;
			var origY = y;
			var debug = new List<dynamic> { new { x, y } };
			log.Debug("Log Disabled during Search LineSearch.");
			LogManager.DisableLogging();
			try {
				for (var count = 1; count < 100; count++) {
					debug.Add(new { x, y });

					if (origY.Sign() != y.Sign()) {
						intervalFactor = 0.5;
					}

					interval *= intervalFactor;
					x += interval * -y.Sign();

					var result = evaluateFunction(x);
					if (criterion(result).IsEqual(0, Constants.SimulationSettings.LineSearchTolerance)) {
						LogManager.EnableLogging();
						log.Debug("LineSearch found an operating point after {0} function calls.", count);
						iterationCount += count;
						LogManager.DisableLogging();
						return x;
					}
					y = getYValue(result);
				}
			} finally {
				LogManager.EnableLogging();
			}

			iterationCount += 100;
			log.Debug("LineSearch could not find an operating point.");
			log.Error("Exceeded max iterations when searching for operating point!");
			log.Error("debug: {0} ... {1}", ", ".Join(debug.Take(5)), ", ".Join(debug.Slice(-6)));
			throw new VectoSearchFailedException("Failed to find operating point! points: {0} ... {1}", ", ".Join(debug.Take(5)),
				", ".Join(debug.Slice(-6)));
		}

		/// <summary>
		/// Interpolating Search algorithm.
		/// Calculates linear equation of 2 points and jumps directly to root-point.
		/// </summary>
		private static T InterpolateLinear<T>(T x1, SI y1, T interval, Func<object, SI> getYValue,
			Func<T, object> evaluateFunction, Func<object, double> criterion, ref int iterationCount) where T : SIBase<T>
		{
			var log = LogManager.GetLogger(typeof(SearchAlgorithm).FullName);
			var debug = new List<dynamic> { new { x = x1, y = y1 } };
			log.Debug("Log Disabled during Search InterpolateLinear.");
			LogManager.DisableLogging();
			try {
				var x2 = x1 + interval;
				var result = evaluateFunction(x2);
				if (criterion(result).IsEqual(0, Constants.SimulationSettings.InterpolateSearchTolerance)) {
					LogManager.EnableLogging();
					log.Debug("InterpolateLinear found an operating point after 1 function call.");
					LogManager.DisableLogging();
					iterationCount++;
					return x2;
				}

				for (var count = 2; count < 30; count++) {
					var y2 = getYValue(result);
					debug.Add(new { x = x2, y = y2 });

					try {
						var k = (y2 - y1) / (x2 - x1);
						var d = y2 - k * x2;
						x1 = x2;
						x2 = (-d / k).Cast<T>();
					} catch (VectoException ex) {
						if (!(ex.InnerException is DivideByZeroException)) {
							throw;
						}
						debug.Add(new { x = x2, y = getYValue(result) });
						LogManager.EnableLogging();
						log.Debug("InterpolateLinear could not get more exact. Aborting after {0} function calls.", count);
						LogManager.DisableLogging();
						iterationCount += count;
						return x2;
					}

					result = evaluateFunction(x2);
					if (criterion(result).IsEqual(0, Constants.SimulationSettings.InterpolateSearchTolerance)) {
						debug.Add(new { x = x2, y = getYValue(result) });
						LogManager.EnableLogging();
						log.Debug("InterpolateLinear found an operating point after {0} function calls.", count);
						LogManager.DisableLogging();
						iterationCount += count;
						return x2;
					}

					y1 = y2;
				}
			} finally {
				LogManager.EnableLogging();
			}

			iterationCount += 30;
			log.Debug("InterpolateLinear could not find an operating point.");
			log.Error("Exceeded max iterations when searching for operating point!");
			log.Error("debug: {0} ... {1}", ", ".Join(debug.Take(5)), ", ".Join(debug.Slice(-6)));
			throw new VectoSearchFailedException("Failed to find operating point! points: {0} ... {1}", ", ".Join(debug.Take(5)),
				", ".Join(debug.Slice(-6)));
		}
	}
}