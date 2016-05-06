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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using Newtonsoft.Json;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	[JsonObject(MemberSerialization.Fields)]
	public class DelaunayMap : LoggingObject
	{
		internal readonly ICollection<Point> Points = new HashSet<Point>();
		private List<Triangle> _triangles = new List<Triangle>();
		private Edge[] _convexHull;

		private readonly ThreadLocal<bool> _extrapolated = new ThreadLocal<bool>();

		private readonly string _mapName;

		public DelaunayMap(string name)
		{
			_mapName = name;
		}

		public bool Extrapolated
		{
			get { return _extrapolated.Value; }
			set { _extrapolated.Value = value; }
		}

		public void AddPoint(double x, double y, double z)
		{
			Points.Add(new Point(x, y, z));
		}

		/// <summary>
		/// Triangulate the points.
		/// </summary>
		/// <remarks>
		/// Triangulation with the Bowyer-Watson algorithm (iteratively insert points into a super triangle).
		/// https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
		/// </remarks>
		public void Triangulate()
		{
			if (Points.Count < 3) {
				throw new ArgumentException(string.Format("{0}: Triangulation needs at least 3 Points. Got {1} Points.", _mapName,
					Points.Count));
			}

			SanitycheckInputPoints();

			// The "supertriangle" encompasses all triangulation points.
			// This is just a helper triangle which initializes the algorithm and will be removed later.
			const int superTriangleScalingFactor = 10;
			var max = Points.Max(point => Math.Max(Math.Abs(point.X), Math.Abs(point.Y))) * superTriangleScalingFactor;
			var superTriangle = new Triangle(new Point(max, 0), new Point(0, max), new Point(-max, -max));
			var triangles = new List<Triangle> { superTriangle };

			var pointCount = 0;

			var points = Points.ToArray();

			var xmin = points.Min(p => p.X);
			var xmax = points.Max(p => p.X);
			var ymin = points.Min(p => p.Y);
			var ymax = points.Max(p => p.Y);

			// iteratively add each point into the correct triangle and split up the triangle
			foreach (var point in points) {
				// If the vertex lies inside the circumcircle of a triangle, the edges of this triangle are 
				// added to the edge buffer and the triangle is removed from list.
				var point1 = point;
				var containerTriangles = triangles.FindAll(t => t.ContainsInCircumcircle(point1));
				triangles = triangles.Except(containerTriangles).ToList();

				// Remove duplicate edges. This leaves the convex hull of the edges.
				// The edges in this convex hull are oriented counterclockwise!
				var allEdges = containerTriangles.SelectMany(t => t.GetEdges()).ToList();
				var groupedEdges = allEdges.GroupBy(edge => edge).ToList();
				var convexHullEdges = groupedEdges.Where(group => group.Count() == 1).Select(group => group.Key).ToList();

				var newTriangles = convexHullEdges.Select(edge => new Triangle(edge.P1, edge.P2, point)).ToList();

				triangles.AddRange(newTriangles);
				//DrawGraph(pointCount, triangles, superTriangle, xmin, xmax, ymin, ymax, point);
				pointCount++;

				// check invariant: m = 2n-2-k
				// m...triangle count
				// n...point count (pointCount +3 points on the supertriangle)
				// k...points on convex hull (exactly 3 --> supertriangle)
				if (triangles.Count != 2 * (pointCount + 3) - 2 - 3) {
					throw new VectoException(
						"Delaunay-Triangulation invariant violated! Triangle count and point count doesn't fit together.");
				}
			}

			//DrawGraph(pointCount, triangles, superTriangle, xmin, xmax, ymin, ymax);

			_convexHull = triangles.FindAll(t => t.SharesVertexWith(superTriangle)).
				SelectMany(t => t.GetEdges()).
				Where(e => !(superTriangle.Contains(e.P1) || superTriangle.Contains(e.P2))).ToArray();

			_triangles = triangles.FindAll(t => !t.SharesVertexWith(superTriangle));
		}

		private void SanitycheckInputPoints()
		{
			var duplicates = Points.GroupBy(pt => new { pt.X, pt.Y }, x => x).Where(g => g.Count() > 1).ToList();

			foreach (var duplicate in duplicates) {
				Log.Error("{0}: Input Point appears twice: x: {1}, y: {2}", duplicate.Key.X, duplicate.Key.Y);
			}
			if (duplicates.Any()) {
				throw new VectoException("{0}: Input Data for Delaunay map contains duplicates! \n{1}", _mapName,
					string.Join("\n", duplicates.Select(pt => string.Format("{0} / {1}", pt.Key.X, pt.Key.Y))));
			}
		}


		/// <summary>
		/// Draws the delaunay map (except supertriangle).
		/// </summary>
		[Conditional("TRACE")]
		private static void DrawGraph(int i, List<Triangle> triangles, Triangle superTriangle, double xmin, double xmax, double ymin,
			double ymax, Point lastPoint = null)
		{
			using (var chart = new Chart { Width = 1000, Height = 1000 }) {
				chart.ChartAreas.Add(new ChartArea("main") {
					AxisX = new Axis { Minimum = Math.Min(xmin, xmin), Maximum = Math.Max(xmax, xmax) },
					AxisY = new Axis { Minimum = Math.Min(ymin, ymin), Maximum = Math.Max(ymax, ymax) }
				});

				foreach (var tr in triangles) {
					if (tr.SharesVertexWith(superTriangle)) {
						continue;
					}

					var series = new Series {
						ChartType = SeriesChartType.FastLine,
						Color = lastPoint != null && tr.Contains(lastPoint) ? Color.Red : Color.Blue
					};
					series.Points.AddXY(tr.P1.X, tr.P1.Y);
					series.Points.AddXY(tr.P2.X, tr.P2.Y);
					series.Points.AddXY(tr.P3.X, tr.P3.Y);
					series.Points.AddXY(tr.P1.X, tr.P1.Y);
					chart.Series.Add(series);
				}

				if (lastPoint != null) {
					var series = new Series {
						ChartType = SeriesChartType.Point,
						Color = Color.Red,
						MarkerSize = 5,
						MarkerStyle = MarkerStyle.Circle
					};
					series.Points.AddXY(lastPoint.X, lastPoint.Y);
					chart.Series.Add(series);
				}

				var frame = new StackFrame(2);
				var method = frame.GetMethod();
				var type = string.Join("", method.DeclaringType.Name.Split(Path.GetInvalidFileNameChars()));
				var methodName = string.Join("", method.Name.Split(Path.GetInvalidFileNameChars()));
				Directory.CreateDirectory("delaunay");
				chart.SaveImage(string.Format("delaunay\\{0}_{1}_{2}_{3}.png", type, methodName, superTriangle.GetHashCode(), i),
					ChartImageFormat.Png);
			}
		}

		public double Interpolate(double x, double y, bool allowExtrapolation = false)
		{
			var tr = _triangles.Find(triangle => triangle.IsInside(x, y, exact: true)) ??
					_triangles.Find(triangle => triangle.IsInside(x, y, exact: false));

			if (tr != null) {
				Extrapolated = false;
				var plane = new Plane(tr);
				return (plane.W - plane.X * x - plane.Y * y) / plane.Z;
			}

			if (!allowExtrapolation) {
				throw new VectoException("{2}: Interpolation failed. x: {0}, y: {1}", x, y, _mapName);
			}

			Extrapolated = true;
			var point = new Point(x, y);

			// get nearest point on convex hull
			var nearestPoint = _convexHull.Select(e => e.P1).MinBy(p => Math.Pow(p.X - x, 2) + Math.Pow(p.Y - y, 2));

			// test if point is on left side of the perpendicular vector (to x,y coordinates) of edge1 in the nearest point
			//                            ^
			//                 (point)    |
			//                            |
			// (p1)--edge1-->(nearestPoint)
			var edge1 = _convexHull.First(e => e.P2.Equals(nearestPoint));
			if (point.IsLeftOf(new Edge(nearestPoint, edge1.Vector.Perpendicular() + nearestPoint))) {
				return Extrapolate(x, y, edge1);
			}

			// test if point is on right side of the perpendicular vector of edge2 in the nearest point
			// ^
			// |   (point)
			// |        
			// (nearestPoint)--edge2-->(p2)
			var edge2 = _convexHull.First(e => e.P1.Equals(nearestPoint));
			if (!point.IsLeftOf(new Edge(nearestPoint, edge2.Vector.Perpendicular() + nearestPoint))) {
				return Extrapolate(x, y, edge2);
			}

			// if point is right of perpendicular vector of edge1 and left of perpendicular vector of edge2: take the nearest point z-value
			return nearestPoint.Z;
		}

		/// <summary>
		/// Constant z-axis-extrapolation of a point from a line
		/// </summary>
		/// <remarks>
		/// https://en.wikibooks.org/wiki/Linear_Algebra/Orthogonal_Projection_Onto_a_Line
		/// </remarks>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="edge"></param>
		/// <returns></returns>
		private static double Extrapolate(double x, double y, Edge edge)
		{
			// shortcut if edge end points have same Z values
			if (edge.P1.Z.IsEqual(edge.P2.Z)) {
				return edge.P1.Z;
			}

			// 2d vector of the edge:  A--->B
			var AB = new Point(edge.Vector.X, edge.Vector.Y);

			// 2d vector of the point: A---->P
			var AP = new Point(x - edge.P1.X, y - edge.P1.Y);

			// projection of point (x,y) onto the edge
			var z = edge.P1.Z + edge.Vector.Z * (AP.Dot(AB) / AB.Dot(AB));
			return z;
		}

		#region Equality members

		protected bool Equals(DelaunayMap other)
		{
			return Points.SequenceEqual(other.Points) && _triangles.SequenceEqual(other._triangles);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((DelaunayMap)obj);
		}

		public override int GetHashCode()
		{
			unchecked {
				return ((Points != null ? Points.GetHashCode() : 0) * 397) ^
						(_triangles != null ? _triangles.GetHashCode() : 0);
			}
		}

		#endregion
	}
}