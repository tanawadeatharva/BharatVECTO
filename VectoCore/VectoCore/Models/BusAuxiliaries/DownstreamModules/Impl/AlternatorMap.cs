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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{
	public class AlternatorMap : IAlternatorMap
	{
		private readonly string filePath;

		private List<MapPoint> _map = new List<MapPoint>();
		private List<Ampere> _yRange;
		private List<PerSecond> _xRange;
		private PerSecond _minX;
		private Ampere _minY;
		private PerSecond _maxX;
		private Ampere _maxY;

		// Required Action Test or Interpolation Type
		public bool OnBoundaryYInterpolatedX(PerSecond x, Ampere y)
		{
			return _yRange.Contains(y) && !_xRange.Contains(x);
		}

		public bool OnBoundaryXInterpolatedY(PerSecond x, Ampere y)
		{
			return !_yRange.Contains(y) && _xRange.Contains(x);
		}

		public bool ONBoundaryXY(PerSecond x, Ampere y)
		{
			return (from sector in _map
					where sector.Y == y && sector.X == x
					select sector).Count() == 1;
		}

		// Determine Value Methods
		private double GetOnBoundaryXY(PerSecond x, Ampere y)
		{
			return (from sector in _map
					where sector.Y == y && sector.X == x
					select sector).First().V;
		}

		private double GetOnBoundaryYInterpolatedX(PerSecond x, Ampere y)
		{
			//double x0, x1, v0, v1, slope, dx;

			var x0 = (from p in _xRange
				  orderby p
				  where p < x
				  select p).Last();
			var x1 = (from p in _xRange
				  orderby p
				  where p > x
				  select p).First();
			var dx = x1 - x0;

			var v0 = GetOnBoundaryXY(x0, y);
			var v1 = GetOnBoundaryXY(x1, y);

			return v0 + (x - x0) * (v1 - v0) / (x1 - x0);
		}

		private double GetOnBoundaryXInterpolatedY(PerSecond x, Ampere y)
		{
			//double y0, y1, v0, v1, dy, v, slope;

			var y0 = (from p in _yRange
				  orderby p
				  where p < y
				  select p).Last();
			var y1 = (from p in _yRange
				  orderby p
				  where p > y
				  select p).First();
			var dy = y1 - y0;

			var v0 = GetOnBoundaryXY(x, y0);
			var v1 = GetOnBoundaryXY(x, y1);

			var v = v0 + (y - y0) * (v1 - v0) / (y1 - y0);

			return v;
		}

		private double GetBiLinearInterpolatedValue(PerSecond x, Ampere y)
		{
			//double q11, q12, q21, q22, x1, x2, y1, y2, r1, r2, p;

			var y1 = (from mapSector in _map
				  where mapSector.Y < y
				  select mapSector).Last().Y;
			var y2 = (from mapSector in _map
				  where mapSector.Y > y
				  select mapSector).First().Y;

			var x1 = (from mapSector in _map
				  where mapSector.X < x
				  select mapSector).Last().X;
			var x2 = (from mapSector in _map
				  where mapSector.X > x
				  select mapSector).First().X;

			var q11 = GetOnBoundaryXY(x1, y1);
			var q12 = GetOnBoundaryXY(x1, y2);

			var q21 = GetOnBoundaryXY(x2, y1);
			var q22 = GetOnBoundaryXY(x2, y2);

			var r1 = ((x2 - x) / (x2 - x1)) * q11 + ((x - x1) / (x2 - x1)).Value() * q21;

			var r2 = ((x2 - x) / (x2 - x1)) * q12 + ((x - x1) / (x2 - x1)).Value() * q22;


			var p = ((y2 - y) / (y2 - y1)).Value() * r1 + ((y - y1) / (y2 - y1)).Value() * r2;


			return p;
		}

		private void getMapRanges()
		{
			;/* 
Input: 

			_yRange = (From coords As MapPoint In _map Order By coords.Y Select coords.Y Distinct).ToList()
 */
			;/* 
Input: 
			_xRange = (From coords As MapPoint In _map Order By coords.x Select coords.x Distinct).ToList()
 */
			_yRange = _map.Select(x => x.Y).Distinct().OrderBy(x => x).ToList();
			_xRange = _map.Select(x => x.X).Distinct().OrderBy(x => x).ToList();

			_minX = _xRange.First();
			_maxX = _xRange.Last();
			_minY = _yRange.First();
			_maxY = _yRange.Last();
		}

		// Single entry point to determine Value on map
		public double GetValue(PerSecond x, Ampere y)
		{
			if (x < _minX || x > _maxX || y < _minY || y > _maxY) {

				// OnAuxiliaryEvent(String.Format("Alternator Map Limiting : RPM{0}, AMPS{1}",x,y),AdvancedAuxiliaryMessageType.Warning)


				// Limiting
				if (x < _minX) {
					x = _minX;
				}
				if (x > _maxX) {
					x = _maxX;
				}
				if (y < _minY) {
					y = _minY;
				}
				if (y > _maxY) {
					y = _maxY;
				}
			}


			// Satisfies both data points - non interpolated value
			if (ONBoundaryXY(x, y)) {
				return GetOnBoundaryXY(x, y);
			}

			// Satisfies only x or y - single interpolation value
			if (OnBoundaryXInterpolatedY(x, y)) {
				return GetOnBoundaryXInterpolatedY(x, y);
			}
			if (OnBoundaryYInterpolatedX(x, y)) {
				return GetOnBoundaryYInterpolatedX(x, y);
			}

			// satisfies no data points - Bi-Linear interpolation
			return GetBiLinearInterpolatedValue(x, y);
		}

		public string ReturnDefaultMapValueTests()
		{
			var sb = new StringBuilder();

			// All Sector Values
			sb.AppendLine("All Values From Map");
			sb.AppendLine("-------------------");
			foreach (var xr in _xRange) {
				foreach (var yr in _yRange)
					sb.AppendLine(string.Format("X:{0}, Y:{1}, V:{2}", xr, yr, GetValue(xr, yr)));
			}

			sb.AppendLine("");
			sb.AppendLine("Four Corners with interpolated other");
			sb.AppendLine("-------------------");
			var x = 1500.0.RPMtoRad();
			var y = 18.5.SI<Ampere>();
			sb.AppendLine(string.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)));
			x = 7000.RPMtoRad();
			y = 96.5.SI<Ampere>();
			sb.AppendLine(string.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)));
			x = 1750.RPMtoRad();
			y = 10.SI<Ampere>();
			sb.AppendLine(string.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)));
			x = 6500.RPMtoRad();
			y = 10.SI<Ampere>();
			sb.AppendLine(string.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)));

			sb.AppendLine("");
			sb.AppendLine("Interpolated both");
			sb.AppendLine("-------------------");

			//double mx, my;
			int x2, y2;
			for (x2 = 0; x2 <= _xRange.Count - 2; x2++) {
				for (y2 = 0; y2 <= _yRange.Count - 2; y2++) {
					var mx = _xRange[x2] + (_xRange[x2 + 1] - _xRange[x2]) / 2;
					var my = _yRange[y2] + (_yRange[y2 + 1] - _yRange[y2]) / 2;

					sb.AppendLine(string.Format("X:{0}, Y:{1}, V:{2}", mx.AsRPM, my.Value(), GetValue(mx, my)));
				}
			}

			sb.AppendLine("");
			sb.AppendLine("MIKE -> 40 & 1000");
			sb.AppendLine("-------------------");
			x = 1000.RPMtoRad();
			y = 40.SI<Ampere>();
			sb.AppendLine(string.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)));


			return sb.ToString();
		}

		// Constructors
		public AlternatorMap(string filepath)
		{
			this.filePath = filepath;

			Initialise();

			getMapRanges();
		}

		private class MapPoint
		{
			public Ampere Y;
			public PerSecond X;
			public double V;

			public MapPoint(Ampere y, PerSecond x, double v)
			{
				Y = y;
				X = x;
				V = v;
			}
		}

		// Get Alternator Efficiency
		public AlternatorMapValues GetEfficiency(PerSecond rpm, Ampere amps)
		{
			return new AlternatorMapValues(GetValue(rpm, amps));
		}

		// Initialises the map.
		public bool Initialise()
		{
			if (File.Exists(filePath)) {
				using (var sr = new StreamReader(filePath)) {
					// get array og lines fron csv
					var lines = sr.ReadToEnd().Split(new [] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

					// Must have at least 2 entries in map to make it usable [dont forget the header row]
					if (lines.Count() < 3)
						throw new ArgumentException("Insufficient rows in csv to build a usable map");

					_map = new List<MapPoint>();
					var firstline = true;

					foreach (var line in lines) {
						if (!firstline) {

							// Advanced Alternator Source Check.
							if (line.Contains("[MODELSOURCE"))
								break;

							// split the line
							var elements = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							// 3 entries per line required
							if ((elements.Length != 3))
								throw new ArgumentException("Incorrect number of values in csv file");
							// add values to map

							// Create AlternatorKey
							var newPoint = new MapPoint(elements[0].ToDouble().SI<Ampere>(), elements[1].ToDouble().RPMtoRad(), elements[2].ToDouble());
							_map.Add(newPoint);
						} 
						firstline = false;
					}
				}
				return true;
			} 
			throw new ArgumentException("Supplied input file does not exist");
		}


		// Public Events
		public event AuxiliaryEventEventHandler AuxiliaryEvent;

		//public delegate void AuxiliaryEventEventHandler(ref object sender, string message, AdvancedAuxiliaryMessageType messageType);

		protected void OnAuxiliaryEvent(string message, AdvancedAuxiliaryMessageType messageType)
		{
			object alternatorMap = this;
			AuxiliaryEvent?.Invoke(ref alternatorMap, message, messageType);
		}
	}
}
