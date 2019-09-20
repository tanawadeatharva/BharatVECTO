// Copyright 2017 European Union.
// Licensed under the EUPL (the 'Licence');
// 
// * You may not use this work except in compliance with the Licence.
// * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
// * Unless required by applicable law or agreed to in writing,
// software distributed under the Licence is distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the LICENSE.txt for the specific language governing permissions and limitations.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using DownstreamModules.Electrics;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace Electrics
{
	public class ResultCard : IResultCard
	{
		private List<SmartResult> _results;

		// Constructor
		public ResultCard(List<SmartResult> results)
		{
			if (results == null)
				throw new ArgumentException("A list of smart results must be supplied.");

			_results = results;
		}


		// Public class outputs
		public List<SmartResult> Results
		{
			get { return _results; }
		}

		public Ampere GetSmartCurrentResult(Ampere Amps)
		{
			if (_results.Count < 2)
				return 10.SI<Ampere>();

			return GetOrInterpolate(Amps.Value()).SI<Ampere>();
		}


		// Helpers
		/// <summary>
		///         ''' Gets or interpolates value (A)
		///         ''' </summary>
		///         ''' <param name="amps"></param>
		///         ''' <returns></returns>
		///         ''' <remarks></remarks>
		private double GetOrInterpolate(double amps)
		{
			double pre;
			double post;
			double dAmps;
			double dSmartAmps;
			double smartAmpsSlope;
			double smartAmps;
			double maxKey;
			double minKey;

			maxKey = _results.Max().Amps;
			minKey = _results.Min().Amps;

			SmartResult compareKey = new SmartResult(amps, 0);

			// Is on boundary check
			if (_results.Contains(compareKey))
				return _results.OrderBy(x => x.Amps).First(x => x.Amps == compareKey.Amps).SmartAmps;

			// Is over map - Extrapolate
			if (amps > maxKey) {
				// get the entries before and after the supplied key
				pre = (from a in _results
						orderby a.Amps
						where a.Amps < maxKey
						select a).Last().Amps;
				post = maxKey;

				// get the delta values 
				dAmps = post - pre;
				dSmartAmps = (from da in _results
							orderby da.Amps
							where da.Amps == post
							select da).First().SmartAmps - (from da in _results
															orderby da.Amps
															where da.Amps == pre
															select da).First().SmartAmps;

				// calculate the slopes
				smartAmpsSlope = dSmartAmps / dAmps;

				// calculate the new values
				smartAmps = ((amps - post) * smartAmpsSlope) + (from da in _results
																orderby da.Amps
																where da.Amps == post
																select da).First().SmartAmps;

				return smartAmps;
			}

			// Is under map - Extrapolate
			if (amps < minKey) {
				// get the entries before and after the supplied key
				// Post is the first entry and pre is the penultimate to first entry
				post = minKey;
				pre = (from k in _results
						orderby k.Amps
						where k.Amps > minKey
						select k).First().Amps;

				// get the delta values 
				dAmps = post - pre;
				dSmartAmps = (from da in _results
							orderby da.Amps
							where da.Amps == post
							select da).First().SmartAmps - (from da in _results
															orderby da.Amps
															where da.Amps == pre
															select da).First().SmartAmps;

				// calculate the slopes
				smartAmpsSlope = dSmartAmps / dAmps;

				// calculate the new values
				smartAmps = ((amps - post) * smartAmpsSlope) + (from da in _results
																orderby da.Amps
																where da.Amps == post
																select da).First().SmartAmps;

				return smartAmps;
			}

			// Is Inside map - Interpolate

			// get the entries before and after the supplied rpm
			pre = (from m in _results
					orderby m.Amps
					where m.Amps < amps
					select m).Last().Amps;
			post = (from m in _results
					where m.Amps > amps
					select m).First().Amps;

			// get the delta values for rpm and the map values
			dAmps = post - pre;
			dSmartAmps = (from da in _results
						orderby da.Amps
						where da.Amps == post
						select da).First().SmartAmps - (from da in _results
														orderby da.Amps
														where da.Amps == pre
														select da).First().SmartAmps;

			// calculate the slopes
			smartAmpsSlope = dSmartAmps / dAmps;

			// calculate the new values
			smartAmps = ((amps - post) * smartAmpsSlope) + (from da in _results
															orderby da.Amps
															where da.Amps == post
															select da).First().SmartAmps;

			return smartAmps;
		}
	}
}
