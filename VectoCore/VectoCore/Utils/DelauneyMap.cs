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
using Newtonsoft.Json;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models;

namespace TUGraz.VectoCore.Utils
{
	[JsonObject(MemberSerialization.Fields)]
	public class DelauneyMap : LoggingObject
	{
		internal readonly ICollection<Point> Points = new HashSet<Point>();
		private List<Triangle> _triangles = new List<Triangle>();
		private IEnumerable<Edge> _convexHull;

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
				throw new ArgumentException(string.Format("Triangulation needs at least 3 Points. Got {0} Points.", Points.Count));
			}

			// The "supertriangle" encompasses all triangulation points.
			// This is just a helper triangle which initializes the algorithm and will be removed later.
			const int superTriangleScalingFactor = 10;
			var max = Points.Max(point => Math.Max(Math.Abs(point.X), Math.Abs(point.Y))) * superTriangleScalingFactor;
			var superTriangle = new Triangle(new Point(max, 0), new Point(0, max), new Point(-max, -max));
			var triangles = new List<Triangle> { superTriangle };

			var pointCount = 0;
			// iteratively add each point into the correct triangle and split up the triangle
			foreach (var point in Points) {
				// If the vertex lies inside a triangle, the edges of the triangle are 
				// added to the edge buffer and the triangle is removed from list.
				var containerTriangles = triangles.FindAll(t => t.ContainsInCircumcircle(point));
				triangles = triangles.Except(containerTriangles).ToList();

				// Remove duplicate edges. This leaves the convex hull of the edges.
				// The edges in this convex hull are oriented counterclockwise!
				var allEdges = containerTriangles.SelectMany(t => t.GetEdges());
				var groupedEdges = allEdges.GroupBy(edge => edge);
				var convexHullEdges = groupedEdges.Where(group => group.Count() == 1).Select(group => group.Key);

				var newTriangles = convexHullEdges.Select(edge => new Triangle(edge.P1, edge.P2, point));

				triangles.AddRange(newTriangles);
				pointCount++;

				// check invariant: m = 2n-2-k
				// m...triangle count
				// n...point count (pointCount +3 points on the supertriangle)
				// k...points on convex hull (exactly 3 --> supertriangle)
				if (triangles.Count != 2 * (pointCount + 3) - 2 - 3) {
					throw new VectoException("Triangulation invariant violated! Triangle count and point count doesn't fit together.");
				}
			}

			_convexHull = triangles.FindAll(t => t.SharesVertexWith(superTriangle)).
				SelectMany(t => t.GetEdges()).
				Where(e => !(superTriangle.Contains(e.P1) || superTriangle.Contains(e.P2)));

			_triangles = triangles.FindAll(t => !t.SharesVertexWith(superTriangle));
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
				throw new VectoException("Interpolation failed. x: {0}, y: {1}", x, y);
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

		public bool Extrapolated { get; set; }

		#region Equality members

		protected bool Equals(DelauneyMap other)
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
			return Equals((DelauneyMap)obj);
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