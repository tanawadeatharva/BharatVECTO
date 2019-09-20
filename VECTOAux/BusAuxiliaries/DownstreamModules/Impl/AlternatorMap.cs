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
using TUGraz.VectoCore.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace Electrics
{
	public class AlternatorMap : IAlternatorMap
	{
		private readonly string filePath;

		private List<MapPoint> _map = new List<MapPoint>();
		private List<double> _yRange;
		private List<double> _xRange;
		private double _minX, _minY, _maxX, _maxY;

		// Required Action Test or Interpolation Type
		public bool OnBoundaryYInterpolatedX(double x, double y)
		{
			return _yRange.Contains(y) && !_xRange.Contains(x);
		}

		public bool OnBoundaryXInterpolatedY(double x, double y)
		{
			return !_yRange.Contains(y) && _xRange.Contains(x);
		}

		public bool ONBoundaryXY(double x, double y)
		{
			return (from sector in _map
					where sector.Y == y && sector.x == x
					select sector).Count() == 1;
		}

		// Determine Value Methods
		private double GetOnBoundaryXY(double x, double y)
		{
			return (from sector in _map
					where sector.Y == y && sector.x == x
					select sector).First().v;
		}

		private double GetOnBoundaryYInterpolatedX(double x, double y)
		{
			double x0, x1, v0, v1, slope, dx;

			x0 = (from p in _xRange
				  orderby p
				  where p < x
				  select p).Last();
			x1 = (from p in _xRange
				  orderby p
				  where p > x
				  select p).First();
			dx = x1 - x0;

			v0 = GetOnBoundaryXY(x0, y);
			v1 = GetOnBoundaryXY(x1, y);

			slope = (v1 - v0) / (x1 - x0);

			return v0 + ((x - x0) * slope);
		}

		private double GetOnBoundaryXInterpolatedY(double x, double y)
		{
			double y0, y1, v0, v1, dy, v, slope;

			y0 = (from p in _yRange
				  orderby p
				  where p < y
				  select p).Last();
			y1 = (from p in _yRange
				  orderby p
				  where p > y
				  select p).First();
			dy = y1 - y0;

			v0 = GetOnBoundaryXY(x, y0);
			v1 = GetOnBoundaryXY(x, y1);

			slope = (v1 - v0) / (y1 - y0);

			v = v0 + ((y - y0) * slope);

			return v;
		}

		private double GetBiLinearInterpolatedValue(double x, double y)
		{
			double q11, q12, q21, q22, x1, x2, y1, y2, r1, r2, p;

			y1 = (from mapSector in _map
				  where mapSector.Y < y
				  select mapSector).Last().Y;
			y2 = (from mapSector in _map
				  where mapSector.Y > y
				  select mapSector).First().Y;

			x1 = (from mapSector in _map
				  where mapSector.x < x
				  select mapSector).Last().x;
			x2 = (from mapSector in _map
				  where mapSector.x > x
				  select mapSector).First().x;

			q11 = GetOnBoundaryXY(x1, y1);
			q12 = GetOnBoundaryXY(x1, y2);

			q21 = GetOnBoundaryXY(x2, y1);
			q22 = GetOnBoundaryXY(x2, y2);

			r1 = ((x2 - x) / (x2 - x1)) * q11 + ((x - x1) / (x2 - x1)) * q21;

			r2 = ((x2 - x) / (x2 - x1)) * q12 + ((x - x1) / (x2 - x1)) * q22;


			p = ((y2 - y) / (y2 - y1)) * r1 + ((y - y1) / (y2 - y1)) * r2;


			return p;
		}

		// Utilities
		private void fillMapWithDefaults()
		{
			_map.Add(new MapPoint(10, 1500, 0.615));
			_map.Add(new MapPoint(27, 1500, 0.7));
			_map.Add(new MapPoint(53, 1500, 0.1947));
			_map.Add(new MapPoint(63, 1500, 0.0));
			_map.Add(new MapPoint(68, 1500, 0.0));
			_map.Add(new MapPoint(125, 1500, 0.0));
			_map.Add(new MapPoint(136, 1500, 0.0));
			_map.Add(new MapPoint(10, 2000, 0.62));
			_map.Add(new MapPoint(27, 2000, 0.7));
			_map.Add(new MapPoint(53, 2000, 0.3));
			_map.Add(new MapPoint(63, 2000, 0.1462));
			_map.Add(new MapPoint(68, 2000, 0.692));
			_map.Add(new MapPoint(125, 2000, 0.0));
			_map.Add(new MapPoint(136, 2000, 0.0));
			_map.Add(new MapPoint(10, 4000, 0.64));
			_map.Add(new MapPoint(27, 4000, 0.6721));
			_map.Add(new MapPoint(53, 4000, 0.7211));
			_map.Add(new MapPoint(63, 4000, 0.74));
			_map.Add(new MapPoint(68, 4000, 0.7352));
			_map.Add(new MapPoint(125, 4000, 0.68));
			_map.Add(new MapPoint(136, 4000, 0.6694));
			_map.Add(new MapPoint(10, 6000, 0.53));
			_map.Add(new MapPoint(27, 6000, 0.5798));
			_map.Add(new MapPoint(53, 6000, 0.656));
			_map.Add(new MapPoint(63, 6000, 0.6853));
			_map.Add(new MapPoint(68, 6000, 0.7));
			_map.Add(new MapPoint(125, 6000, 0.6329));
			_map.Add(new MapPoint(136, 6000, 0.62));
			_map.Add(new MapPoint(10, 7000, 0.475));
			_map.Add(new MapPoint(27, 7000, 0.5337));
			_map.Add(new MapPoint(53, 7000, 0.6235));
			_map.Add(new MapPoint(63, 7000, 0.658));
			_map.Add(new MapPoint(68, 7000, 0.6824));
			_map.Add(new MapPoint(125, 7000, 0.6094));
			_map.Add(new MapPoint(136, 7000, 0.5953));
		}

		private void getMapRanges()
		{
			;/* Cannot convert AssignmentStatementSyntax, System.NotImplementedException: Conversion for query clause with kind 'DistinctClause' not implemented
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.<>c__DisplayClass99_0.<ConvertQueryBodyClause>b__0(QueryClauseSyntax _)
   at ICSharpCode.CodeConverter.Util.ObjectExtensions.TypeSwitch[TBaseType,TDerivedType1,TDerivedType2,TDerivedType3,TDerivedType4,TResult](TBaseType obj, Func`2 matchFunc1, Func`2 matchFunc2, Func`2 matchFunc3, Func`2 matchFunc4, Func`2 defaultFunc)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.ConvertQueryBodyClause(QueryClauseSyntax node)
   at System.Linq.Enumerable.WhereSelectEnumerableIterator`2.MoveNext()
   at Microsoft.CodeAnalysis.SyntaxList`1.CreateNode(IEnumerable`1 nodes)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitQueryExpression(QueryExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitQueryExpression(QueryExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ParenthesizedExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ParenthesizedExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitAssignmentStatement(AssignmentStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.AssignmentStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 

			_yRange = (From coords As MapPoint In _map Order By coords.Y Select coords.Y Distinct).ToList()

 */
			;/* Cannot convert AssignmentStatementSyntax, System.NotImplementedException: Conversion for query clause with kind 'DistinctClause' not implemented
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.<>c__DisplayClass99_0.<ConvertQueryBodyClause>b__0(QueryClauseSyntax _)
   at ICSharpCode.CodeConverter.Util.ObjectExtensions.TypeSwitch[TBaseType,TDerivedType1,TDerivedType2,TDerivedType3,TDerivedType4,TResult](TBaseType obj, Func`2 matchFunc1, Func`2 matchFunc2, Func`2 matchFunc3, Func`2 matchFunc4, Func`2 defaultFunc)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.ConvertQueryBodyClause(QueryClauseSyntax node)
   at System.Linq.Enumerable.WhereSelectEnumerableIterator`2.MoveNext()
   at Microsoft.CodeAnalysis.SyntaxList`1.CreateNode(IEnumerable`1 nodes)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitQueryExpression(QueryExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitQueryExpression(QueryExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ParenthesizedExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ParenthesizedExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitAssignmentStatement(AssignmentStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.AssignmentStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
			_xRange = (From coords As MapPoint In _map Order By coords.x Select coords.x Distinct).ToList()

 */
			_minX = _xRange.First();
			_maxX = _xRange.Last();
			_minY = _yRange.First();
			_maxY = _yRange.Last();
		}

		// Single entry point to determine Value on map
		public double GetValue(double x, double y)
		{
			if (x < _minX || x > _maxX || y < _minY || y > _maxY) {

				// OnAuxiliaryEvent(String.Format("Alternator Map Limiting : RPM{0}, AMPS{1}",x,y),AdvancedAuxiliaryMessageType.Warning)


				// Limiting
				if (x < _minX)
					x = _minX;
				if (x > _maxX)
					x = _maxX;
				if (y < _minY)
					y = _minY;
				if (y > _maxY)
					y = _maxY;
			}


			// Satisfies both data points - non interpolated value
			if (ONBoundaryXY(x, y))
				return GetOnBoundaryXY(x, y);

			// Satisfies only x or y - single interpolation value
			if (OnBoundaryXInterpolatedY(x, y))
				return GetOnBoundaryXInterpolatedY(x, y);
			if (OnBoundaryYInterpolatedX(x, y))
				return GetOnBoundaryYInterpolatedX(x, y);

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
			var x = 1500.0;
			var y = 18.5;
			sb.AppendLine(string.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)));
			x = 7000;
			y = 96.5;
			sb.AppendLine(string.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)));
			x = 1750;
			y = 10;
			sb.AppendLine(string.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)));
			x = 6500;
			y = 10;
			sb.AppendLine(string.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)));

			sb.AppendLine("");
			sb.AppendLine("Interpolated both");
			sb.AppendLine("-------------------");

			double mx, my;
			int x2, y2;
			for (x2 = 0; x2 <= _xRange.Count - 2; x2++) {
				for (y2 = 0; y2 <= _yRange.Count - 2; y2++) {
					mx = _xRange[x2] + (_xRange[x2 + 1] - _xRange[x2]) / 2;
					my = _yRange[y2] + (_yRange[y2 + 1] - _yRange[y2]) / 2;

					sb.AppendLine(string.Format("X:{0}, Y:{1}, V:{2}", mx, my, GetValue(mx, my)));
				}
			}

			sb.AppendLine("");
			sb.AppendLine("MIKE -> 40 & 1000");
			sb.AppendLine("-------------------");
			x = 1000;
			y = 40;
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
			public double Y;
			public double x;
			public double v;

			public MapPoint(double y, double x, double v)
			{
				this.Y = y;
				this.x = x;
				this.v = v;
			}
		}

		// Get Alternator Efficiency
		public AlternatorMapValues GetEfficiency(double rpm, Ampere amps)
		{
			return new AlternatorMapValues(GetValue(rpm, amps.Value()));
		}

		// Initialises the map.
		public bool Initialise()
		{
			if (File.Exists(filePath)) {
				using (StreamReader sr = new StreamReader(filePath)) {
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
							string[] elements = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							// 3 entries per line required
							if ((elements.Length != 3))
								throw new ArgumentException("Incorrect number of values in csv file");
							// add values to map

							// Create AlternatorKey
							var newPoint = new MapPoint(float.Parse(elements[0], CultureInfo.InvariantCulture), float.Parse(elements[1], CultureInfo.InvariantCulture), float.Parse(elements[2], CultureInfo.InvariantCulture));
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
