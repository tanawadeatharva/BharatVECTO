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

namespace TUGraz.VectoCore.BusAuxiliaries.Legacy {
	public class cDelaunayMap
	{
		public int ptDim;

		public List<dPoint> ptList;
		private List<dTriangle> lDT;
		private List<double[]> planes;

		public bool DualMode;
		private List<dPoint> ptListXZ;
		private List<double[]> planesXZ;
		private List<dTriangle> lDTXZ;

		public bool ExtrapolError;


		public cDelaunayMap()
		{
			ptList = new List<dPoint>();
			ptListXZ = new List<dPoint>();
			DualMode = false;
		}

		public void AddPoints(double X, double Y, double Z)
		{
			ptList.Add(new dPoint(X, Y, Z));
			if (DualMode)
				ptListXZ.Add(new dPoint(X, Z, Y));
		}

		public bool Triangulate()
		{
			//dTriangle tr;
			dTriangulation DT;

			ptDim = ptList.Count - 1;

			// XY-triangulation
			try {
				DT = new dTriangulation();
				lDT = DT.Triangulate(ptList);
			} catch (Exception ) {
				return false;
			}

			planes = new List<double[]>();

			foreach (var tr in lDT)
				planes.Add(GetPlane(tr));


			// #If DEBUG Then
			// Dim i As Int16
			// Debug.Print("#,x1,y1,z1,x2,y2,z2")
			// i = -1
			// For Each tr In lDT
			// i += 1
			// Debug.Print(i & "," & tr.P1.X & "," & tr.P1.Y & "," & tr.P1.Z & "," & tr.P2.X & "," & tr.P2.Y & "," & tr.P2.Z)
			// Debug.Print(i & "," & tr.P3.X & "," & tr.P3.Y & "," & tr.P3.Z & "," & tr.P2.X & "," & tr.P2.Y & "," & tr.P2.Z)
			// Debug.Print(i & "," & tr.P1.X & "," & tr.P1.Y & "," & tr.P1.Z & "," & tr.P3.X & "," & tr.P3.Y & "," & tr.P3.Z)
			// Next
			// #End If


			// XZ-triangulation
			if (DualMode) {
				if (ptDim != ptListXZ.Count - 1)
					return false;

				try {
					DT = new dTriangulation();
					lDTXZ = DT.Triangulate(ptListXZ);
				} catch (Exception ) {
					return false;
				}

				planesXZ = new List<double[]>();

				foreach (var tr in lDTXZ)
					planesXZ.Add(GetPlane(tr));
			}

			return true;
		}

		// XY => Z Interpolation
		public double Intpol(double x, double y)
		{
			int j;
			double[] l0;
			//dTriangle tr;

			ExtrapolError = false;

			// Try exact solution for IsInside()
			j = -1;
			foreach (var tr in lDT) {
				j += 1;
				if (IsInside(tr, x, y, true)) {
					l0 = planes[j];
					return (l0[3] - x * l0[0] - y * l0[1]) / l0[2];
				}
			}

			// Try approx. solution (fixes rounding errors when points lies exactly on an edge of a triangle)
			j = -1;
			foreach (var tr in lDT) {
				j += 1;
				if (IsInside(tr, x, y, false)) {
					l0 = planes[j];
					return (l0[3] - x * l0[0] - y * l0[1]) / l0[2];
				}
			}


			// ERROR: Extrapolation
			ExtrapolError = true;

			return default(Double);
		}

		// XZ => Y Interpolation
		public double IntpolXZ(double x, double z)
		{
			int j;
			double[] l0;
			//dTriangle tr;

			ExtrapolError = false;

			if (DualMode) {
				j = -1;

				// Try exact solution for IsInside()
				foreach (var tr in lDTXZ) {
					j += 1;
					if (IsInside( tr, x, z, true)) {
						l0 = planesXZ[j];
						return (l0[3] - x * l0[0] - z * l0[1]) / l0[2];
					}
				}

				// Try approx. solution (fixes rounding errors when points lies exactly on an edge of a triangle)
				j = -1;
				foreach (var tr in lDTXZ) {
					j += 1;
					if (IsInside( tr, x, z, false)) {
						l0 = planesXZ[j];
						return (l0[3] - x * l0[0] - z * l0[1]) / l0[2];
					}
				}

				// ERROR: Extrapolation
				ExtrapolError = true;
				return default(Double);
			} else {

				// ERROR: Extrapolation
				ExtrapolError = true;
				return default(Double);
			}
		}

		private double[] GetPlane(dTriangle tr)
		{
			dPoint AB;
			dPoint AC;
			dPoint cross;
			var l = new double[4];
			dPoint pt1;
			dPoint pt2;
			dPoint pt3;

			pt1 = tr.P1;
			pt2 = tr.P2;
			pt3 = tr.P3;

			AB = new dPoint(pt2.X - pt1.X, pt2.Y - pt1.Y, pt2.Z - pt1.Z);
			AC = new dPoint(pt3.X - pt1.X, pt3.Y - pt1.Y, pt3.Z - pt1.Z);

			cross = new dPoint(AB.Y * AC.Z - AB.Z * AC.Y, AB.Z * AC.X - AB.X * AC.Z, AB.X * AC.Y - AB.Y * AC.X);

			l[0] = cross.X;
			l[1] = cross.Y;
			l[2] = cross.Z;

			l[3] = pt1.X * cross.X + pt1.Y * cross.Y + pt1.Z * cross.Z;

			return l;
		}

		private bool IsInside(dTriangle tr, double xges, double yges, bool Exact)
		{
			var v0 = new double[2];
			var v1 = new double[2];
			var v2 = new double[2];
			double dot00;
			double dot01;
			double dot02;
			double dot11;
			double dot12;
			double invDenom;
			double u;
			double v;
			dPoint pt1;
			dPoint pt2;
			dPoint pt3;

			pt1 = tr.P1;
			pt2 = tr.P2;
			pt3 = tr.P3;

			// Quelle: http://www.blackpawn.com/texts/pointinpoly/default.html  (Barycentric Technique)

			// Compute vectors        
			v0[0] = pt3.X - pt1.X;
			v0[1] = pt3.Y - pt1.Y;

			v1[0] = pt2.X - pt1.X;
			v1[1] = pt2.Y - pt1.Y;

			v2[0] = xges - pt1.X;
			v2[1] = yges - pt1.Y;

			// Compute dot products
			dot00 = v0[0] * v0[0] + v0[1] * v0[1];
			dot01 = v0[0] * v1[0] + v0[1] * v1[1];
			dot02 = v0[0] * v2[0] + v0[1] * v2[1];
			dot11 = v1[0] * v1[0] + v1[1] * v1[1];
			dot12 = v1[0] * v2[0] + v1[1] * v2[1];

			// Compute barycentric coordinates
			invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
			u = (dot11 * dot02 - dot01 * dot12) * invDenom;
			v = (dot00 * dot12 - dot01 * dot02) * invDenom;

			// Debug.Print(u & ", " & v & ", " & u + v)

			// Check if point is in triangle
			if (Exact)
				return (u >= 0) & (v >= 0) & (u + v <= 1);
			else
				return (u >= -0.001) & (v >= -0.001) & (u + v <= 1.001);
		}

		public struct dPoint
		{
			public double X;
			public double Y;
			public double Z;

			public dPoint(double xd, double yd, double zd)
			{
				X = xd;
				Y = yd;
				Z = zd;
			}

			public static bool operator ==(dPoint left, dPoint right)
			{

				// If DirectCast(left, Object) = DirectCast(right, Object) Then
				// Return True
				// End If

				// If (DirectCast(left, Object) Is Nothing) OrElse (DirectCast(right, Object) Is Nothing) Then
				// Return False
				// End If

				// Just compare x and y here...
				if (left.X != right.X)
					return false;

				if (left.Y != right.Y)
					return false;

				return true;
			}

			public static bool operator !=(dPoint left, dPoint right)
			{
				return !(left == right);
			}
		}

		public class dTriangle
		{
			public dPoint P1;
			public dPoint P2;
			public dPoint P3;

			public dTriangle( dPoint pp1,  dPoint pp2,  dPoint pp3)
			{
				P1 = pp1;
				P2 = pp2;
				P3 = pp3;
			}

			public double ContainsInCircumcircle(dPoint pt)
			{
				var ax = this.P1.X - pt.X;
				var ay = this.P1.Y - pt.Y;
				var bx = this.P2.X - pt.X;
				var by = this.P2.Y - pt.Y;
				var cx = this.P3.X - pt.X;
				var cy = this.P3.Y - pt.Y;
				var det_ab = ax * by - bx * ay;
				var det_bc = bx * cy - cx * by;
				var det_ca = cx * ay - ax * cy;
				var a_squared = ax * ax + ay * ay;
				var b_squared = bx * bx + by * by;
				var c_squared = cx * cx + cy * cy;

				return a_squared * det_bc + b_squared * det_ca + c_squared * det_ab;
			}

			public bool SharesVertexWith(dTriangle triangle)
			{
				if (this.P1.X == triangle.P1.X && this.P1.Y == triangle.P1.Y)
					return true;
				if (this.P1.X == triangle.P2.X && this.P1.Y == triangle.P2.Y)
					return true;
				if (this.P1.X == triangle.P3.X && this.P1.Y == triangle.P3.Y)
					return true;

				if (this.P2.X == triangle.P1.X && this.P2.Y == triangle.P1.Y)
					return true;
				if (this.P2.X == triangle.P2.X && this.P2.Y == triangle.P2.Y)
					return true;
				if (this.P2.X == triangle.P3.X && this.P2.Y == triangle.P3.Y)
					return true;

				if (this.P3.X == triangle.P1.X && this.P3.Y == triangle.P1.Y)
					return true;
				if (this.P3.X == triangle.P2.X && this.P3.Y == triangle.P2.Y)
					return true;
				if (this.P3.X == triangle.P3.X && this.P3.Y == triangle.P3.Y)
					return true;

				return false;
			}
		}

		public class dEdge
		{
			public dPoint StartPoint;
			public dPoint EndPoint;

			public dEdge(dPoint p1, dPoint p2)
			{
				StartPoint = p1;
				EndPoint = p2;
			}

			public static bool operator ==(dEdge left, dEdge right)
			{
				// If DirectCast(left, Object) = DirectCast(right, Object) Then
				// Return True
				// End If

				// If (DirectCast(left, Object) Is Nothing) Or (DirectCast(right, Object) Is Nothing) Then
				// Return False
				// End If

				return ((left.StartPoint == right.StartPoint && left.EndPoint == right.EndPoint) || (left.StartPoint == right.EndPoint && left.EndPoint == right.StartPoint));
			}

			public static bool operator !=(dEdge left, dEdge right)
			{
				return !(left == right);
			}
		}

		public class dTriangulation
		{
			public List<dTriangle> Triangulate(List<dPoint> triangulationPoints)
			{
				if (triangulationPoints.Count < 3)
					throw new ArgumentException("Can not triangulate less than three vertices!");

				// The triangle list
				var triangles = new List<dTriangle>();


				// The "supertriangle" which encompasses all triangulation points.
				// This triangle initializes the algorithm and will be removed later.
				var superTriangle = this.SuperTriangle(triangulationPoints);
				triangles.Add(superTriangle);

				// Include each point one at a time into the existing triangulation
				for (var i = 0; i <= triangulationPoints.Count - 1; i++) {
					// Initialize the edge buffer.
					var EdgeBuffer = new List<dEdge>();

					// If the actual vertex lies inside the circumcircle, then the three edges of the 
					// triangle are added to the edge buffer and the triangle is removed from list.                             
					for (var j = triangles.Count - 1; j >= 0; j += -1) {
						var t = triangles[j];
						if (t.ContainsInCircumcircle(triangulationPoints[i]) > 0) {
							EdgeBuffer.Add(new dEdge(t.P1,  t.P2));
							EdgeBuffer.Add(new dEdge(t.P2,  t.P3));
							EdgeBuffer.Add(new dEdge(t.P3,  t.P1));
							triangles.RemoveAt(j);
						}
					}

					// Remove duplicate edges. This leaves the convex hull of the edges.
					// The edges in this convex hull are oriented counterclockwise!
					for (var j = EdgeBuffer.Count - 2; j >= 0; j += -1) {
						for (var k = EdgeBuffer.Count - 1; k >= j + 1; k += -1) {
							if (EdgeBuffer[j] == EdgeBuffer[k]) {
								EdgeBuffer.RemoveAt(k);
								EdgeBuffer.RemoveAt(j);
								k -= 1;
								continue;
							}
						}
					}

					// Generate new counterclockwise oriented triangles filling the "hole" in
					// the existing triangulation. These triangles all share the actual vertex.
					for (var j = 0; j <= EdgeBuffer.Count - 1; j++)
						triangles.Add(new dTriangle(EdgeBuffer[j].StartPoint, EdgeBuffer[j].EndPoint, triangulationPoints[i]));
				}

				// We don't want the supertriangle in the triangulation, so
				// remove all triangles sharing a vertex with the supertriangle.
				for (var i = triangles.Count - 1; i >= 0; i += -1) {
					if (triangles[i].SharesVertexWith(superTriangle))
						triangles.RemoveAt(i);
				}

				// Return the triangles
				return triangles;
			}


			private dTriangle SuperTriangle(List<dPoint> triangulationPoints)
			{
				var M = triangulationPoints[0].X;

				// get the extremal x and y coordinates
				for (var i = 1; i <= triangulationPoints.Count - 1; i++) {
					var xAbs = Math.Abs(triangulationPoints[i].X);
					var yAbs = Math.Abs(triangulationPoints[i].Y);
					if (xAbs > M)
						M = xAbs;
					if (yAbs > M)
						M = yAbs;
				}

				// make a triangle
				var sp1 = new dPoint(10 * M, 0, 0);
				var sp2 = new dPoint(0, 10 * M, 0);
				var sp3 = new dPoint(-10 * M, -10 * M, 0);

				return new dTriangle( sp1,  sp2,  sp3);
			}
		}
	}
}
