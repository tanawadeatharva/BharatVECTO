/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models;

namespace TUGraz.VectoCore.Utils
{
	[JsonObject(MemberSerialization.Fields)]
	public class DelauneyMap : LoggingObject
	{
		private readonly List<Point> _points = new List<Point>();
		private List<Triangle> _triangles = new List<Triangle>();

		public void AddPoint(double x, double y, double z)
		{
			_points.Add(new Point(x, y, z));
		}

		public void Triangulate()
		{
			if (_points.Count < 3) {
				throw new ArgumentException(string.Format("Triangulation needs at least 3 Points. Got {0} Points.", _points.Count));
			}

			// The "supertriangle" encompasses all triangulation points.
			// This is just a helper triangle which initializes the algorithm and will be removed later.
			const int superTriangleScalingFactor = 10;
			var max = _points.Max(point => Math.Max(Math.Abs(point.X), Math.Abs(point.Y))) * superTriangleScalingFactor;
			var superTriangle = new Triangle(new Point(max, 0), new Point(0, max), new Point(-max, -max));
			var triangles = new List<Triangle> { superTriangle };

			// iteratively add each point into the correct triangle and split up the triangle
			foreach (var point in _points) {
				// If the vertex lies inside a triangle, the edges of the triangle are 
				// added to the edge buffer and the triangle is removed from list.
				var containerTriangles = triangles.FindAll(t => t.ContainsInCircumcircle(point));
				triangles.RemoveAll(t => t.ContainsInCircumcircle(point));

				// Remove duplicate edges. This leaves the convex hull of the edges.
				// The edges in this convex hull are oriented counterclockwise!
				var convexHullEdges = containerTriangles.
					SelectMany(t => t.GetEdges()).
					GroupBy(edge => edge).
					Where(group => group.Count() == 1).
					SelectMany(group => group);

				var newTriangles = convexHullEdges.Select(edge => new Triangle(edge.P1, edge.P2, point));

				triangles.AddRange(newTriangles);
			}

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

			// todo: extrapolate
			Extrapolated = true;


			return y;
		}

		public bool Extrapolated { get; set; }

		public DelauneyMap CreateInvertedMap()
		{
			var reverted = new DelauneyMap();
			reverted._points.AddRange(_points.Select(p => new Point(p.X, p.Z, p.Y)));
			reverted.Triangulate();
			return reverted;
		}

		#region Equality members

		protected bool Equals(DelauneyMap other)
		{
			return _points.SequenceEqual(other._points) && _triangles.SequenceEqual(other._triangles);
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
				return ((_points != null ? _points.GetHashCode() : 0) * 397) ^
						(_triangles != null ? _triangles.GetHashCode() : 0);
			}
		}

		#endregion
	}
}