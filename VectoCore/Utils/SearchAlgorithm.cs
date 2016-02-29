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
using NLog;
using TUGraz.VectoCore.Exceptions;

namespace TUGraz.VectoCore.Utils
{
	public static class SearchAlgorithm
	{
		public static T Search<T>(T x, SI y, T interval, Func<object, SI> getYValue,
			Func<T, object> evaluateFunction, Func<object, bool> criterion) where T : SIBase<T>
		{
//#if DEBUG
			var res = InterpolateLinear(x, y, interval, getYValue, evaluateFunction, criterion);
//#else
			//var res = SearchBinary(x, y, interval, getYValue, evaluateFunction, criterion);
//#endif
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
			var log = LogManager.GetLogger(typeof(SearchAlgorithm).FullName);
			LogManager.DisableLogging();
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
					log.Debug("InterpolateLinear found an operating point after {0} function calls.", iterationCount);
					return x;
				}
				y = getYValue(result);
			}

			LogManager.EnableLogging();

			log.Debug("SearchBinary could not find an operating point.");
			log.Error("Exceeded max iterations when searching for operating point!");
			log.Error("debug: {0} ... {1}", ", ".Join(debug.Take(5)), ", ".Join(debug.Slice(-6)));
			throw new VectoSearchFailedException("Failed to find operating point! points: {0} ... {1}", ", ".Join(debug.Take(5)),
				", ".Join(debug.Slice(-6)));
		}

		public static T InterpolateLinear<T>(T x1, SI y1, T interval, Func<object, SI> getYValue,
			Func<T, object> evaluateFunction, Func<object, bool> criterion) where T : SIBase<T>
		{
			var log = LogManager.GetLogger(typeof(SearchAlgorithm).FullName);
			LogManager.DisableLogging();
			var debug = new List<dynamic> { new { x = x1, y = y1 } };

			var x2 = x1 + interval;
			var result = evaluateFunction(x2);
			if (criterion(result)) {
				log.Debug("InterpolateLinear found an operating point after 1 function call.");
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
					log.Debug("InterpolateLinear found an operating point after {0} function calls.", iterationCount);
					return x2;
				}

				y1 = y2;
			}

			LogManager.EnableLogging();
			log.Debug("InterpolateLinear could not find an operating point.");
			log.Error("Exceeded max iterations when searching for operating point!");
			log.Error("debug: {0} ... {1}", ", ".Join(debug.Take(5)), ", ".Join(debug.Slice(-6)));
			throw new VectoSearchFailedException("Failed to find operating point! points: {0} ... {1}", ", ".Join(debug.Take(5)),
				", ".Join(debug.Slice(-6)));
		}
	}
}