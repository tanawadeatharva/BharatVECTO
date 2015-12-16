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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Utils
{
	/// <summary>
	/// Provides helper methods for mathematical functions.
	/// </summary>
	public class VectoMath
	{
		/// <summary>
		/// Linearly interpolates a value between two points.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="x1">First Value on the X-Axis.</param>
		/// <param name="x2">Second Value on the X-Axis.</param>
		/// <param name="y1">First Value on the Y-Axis.</param>
		/// <param name="y2">Second Value on the Y-Axis.</param>
		/// <param name="xint">Value on the X-Axis, for which the Y-Value should be interpolated.</param>
		/// <returns></returns>
		public static TResult Interpolate<T, TResult>(T x1, T x2, TResult y1, TResult y2, T xint) where T : SI
			where TResult : SIBase<TResult>
		{
			return ((xint - x1) * (y2 - y1) / (x2 - x1) + y1).Cast<TResult>();
		}


		public static double Interpolate<T>(T x1, T x2, double y1, double y2, T xint)
			where T : SI
		{
			return (((xint - x1) * (y2 - y1) / (x2 - x1)).Cast<Scalar>() + y1).Value();
		}

		public static TResult Interpolate<TResult>(double x1, double x2, TResult y1, TResult y2, double xint)
			where TResult : SIBase<TResult>
		{
			return ((xint - x1) * (y2 - y1) / (x2 - x1) + y1).Cast<TResult>();
		}

		/// <summary>
		/// Linearly interpolates a value between two points.
		/// </summary>
		public static double Interpolate(double x1, double x2, double y1, double y2, double xint)
		{
			return ((xint - x1) * (y2 - y1) / (x2 - x1) + y1);
		}


		/// <summary>
		/// Returns the absolute value.
		/// </summary>
		public static SI Abs(SI si)
		{
			return si.Abs();
		}

		/// <summary>
		/// Returns the minimum of two values.
		/// </summary>
		public static T Min<T>(T c1, T c2) where T : IComparable
		{
			return c1.CompareTo(c2) <= 0 ? c1 : c2;
		}

		/// <summary>
		/// Returns the maximum of two values.
		/// </summary>
		public static T Max<T>(T c1, T c2) where T : IComparable
		{
			return c1.CompareTo(c2) >= 0 ? c1 : c2;
		}

		public static T Limit<T>(T value, T lowerBound, T upperBound) where T : IComparable
		{
			if (lowerBound.CompareTo(upperBound) > 0) {
				throw new VectoException("VectoMath.Limit: lowerBound must not be greater than upperBound");
			}

			if (value.CompareTo(upperBound) > 0) {
				return upperBound;
			}
			if (value.CompareTo(lowerBound) < 0) {
				return lowerBound;
			}
			return value;
		}

		public static T Sqrt<T>(SI si) where T : SIBase<T>
		{
			return si.Sqrt().Cast<T>();
		}

		/// <summary>
		///		converts the given inclination in percent (0-1+) into Radians
		/// </summary>
		/// <param name="inclinationPercent"></param>
		/// <returns></returns>
		public static Radian InclinationToAngle(double inclinationPercent)
		{
			return Math.Atan(inclinationPercent).SI<Radian>();
		}

		public static List<double> QuadraticEquationSolver(double a, double b, double c)
		{
			var retVal = new List<double>();
			var D = b * b - 4 * a * c;

			if (D < 0) {
				return retVal;
			} else if (D > 0) {
				// two solutions possible
				retVal.Add((-b + Math.Sqrt(D)) / (2 * a));
				retVal.Add((-b - Math.Sqrt(D)) / (2 * a));
			} else {
				// only one solution possible
				retVal.Add((-b / (2 * a)));
			}
			return retVal;
		}
	}

	[DebuggerDisplay("(X:{X}, Y:{Y}, Z:{Z})")]
	public class Point
	{
		public readonly double X;
		public readonly double Y;
		public readonly double Z;

		public Point(double x, double y, double z = 0)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static Point operator +(Point p1, Point p2)
		{
			return new Point(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
		}

		public static Point operator -(Point p1, Point p2)
		{
			return new Point(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
		}

		public static Point operator -(Point p1)
		{
			return new Point(-p1.X, -p1.Y);
		}

		public static Point operator *(Point p1, double scalar)
		{
			return new Point(p1.X * scalar, p1.Y * scalar, p1.Z * scalar);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="scalar"></param>
		/// <param name="p1"></param>
		/// <returns></returns>
		public static Point operator *(double scalar, Point p1)
		{
			return p1 * scalar;
		}

		/// <summary>
		/// Calculates cross product between two 3d-vectors.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Point Cross(Point other)
		{
			return new Point(Y * other.Z - Z * other.Y, Z * other.X - X * other.Z, X * other.Y - Y * other.X);
		}

		/// <summary>
		/// Returns perpendicular vector for xy-components of this point. P = (-Y, X)
		/// </summary>
		/// <returns></returns>
		public Point CrossXY()
		{
			return new Point(-Y, X);
		}

		/// <summary>
		/// Returns dot product between two 3d-vectors.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public double Dot(Point other)
		{
			return X * other.X + Y * other.Y + Z * other.Z;
		}

		#region Equality members

		private bool Equals(Point other)
		{
			//Contract.Requires(other != null);

			return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			return obj.GetType() == GetType() && Equals((Point)obj);
		}

		public override int GetHashCode()
		{
			return unchecked((((X.GetHashCode() * 397) ^ Y.GetHashCode()) * 397) ^ Z.GetHashCode());
		}

		#endregion
	}

	[DebuggerDisplay("Plane({X}, {Y}, {Z}, {W})")]
	public class Plane
	{
		public double X;
		public double Y;
		public double Z;
		public double W;

		public Plane(double x, double y, double z, double w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Plane(Triangle tr)
		{
			var ab = tr.P2 - tr.P1;
			var ac = tr.P3 - tr.P1;

			var cross = ab.Cross(ac);

			X = cross.X;
			Y = cross.Y;
			Z = cross.Z;
			W = tr.P1.X * cross.X + tr.P1.Y * cross.Y + tr.P1.Z * cross.Z;
		}
	}

	[DebuggerDisplay("Triangle(({P1.X}, {P1.Y}, {P1.Z}), ({P2.X}, {P2.Y}, {P2.Z}), ({P3.X}, {P3.Y}, {P3.Z}))")]
	public class Triangle
	{
		public Point P1;
		public Point P2;
		public Point P3;

		public Triangle(Point p1, Point p2, Point p3)
		{
			P1 = p1;
			P2 = p2;
			P3 = p3;
		}

		/// <summary>
		/// Barycentric Technique: http://www.blackpawn.com/texts/pointinpoly/default.html
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="exact"></param>
		/// <returns></returns>
		public bool IsInside(double x, double y, bool exact)
		{
			var v0X = P3.X - P1.X;
			var v0Y = P3.Y - P1.Y;
			var v1X = P2.X - P1.X;
			var v1Y = P2.Y - P1.Y;
			var v2X = x - P1.X;
			var v2Y = y - P1.Y;

			var dot00 = v0X * v0X + v0Y * v0Y;
			var dot01 = v0X * v1X + v0Y * v1Y;
			var dot02 = v0X * v2X + v0Y * v2Y;
			var dot11 = v1X * v1X + v1Y * v1Y;
			var dot12 = v1X * v2X + v1Y * v2Y;

			var invDenom = 1.0 / (dot00 * dot11 - dot01 * dot01);
			var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
			var v = (dot00 * dot12 - dot01 * dot02) * invDenom;

			if (exact) {
				return u >= 0 && v >= 0 && u + v <= 1;
			}

			return u.IsPositive() && v.IsPositive() && (u + v).IsSmallerOrEqual(1);
		}

		public bool ContainsInCircumcircle(Point p)
		{
			var p0X = P1.X - p.X;
			var p0Y = P1.Y - p.Y;
			var p1X = P2.X - p.X;
			var p1Y = P2.Y - p.Y;
			var p2X = P3.X - p.X;
			var p2Y = P3.Y - p.Y;

			var p0square = p0X * p0X + p0Y * p0Y;
			var p1square = p1X * p1X + p1Y * p1Y;
			var p2square = p2X * p2X + p2Y * p2Y;

			var det01 = p0X * p1Y - p1X * p0Y;
			var det12 = p1X * p2Y - p2X * p1Y;
			var det20 = p2X * p0Y - p0X * p2Y;

			var result = p0square * det12 + p1square * det20 + p2square * det01;

			return result > 0;
		}

		public bool Contains(Point p)
		{
			return p.Equals(P1) || p.Equals(P2) || p.Equals(P3);
		}

		public bool SharesVertexWith(Triangle t)
		{
			Contract.Requires(t != null);
			return Contains(t.P1) || Contains(t.P2) || Contains(t.P3);
		}

		public Edge[] GetEdges()
		{
			return new[] { new Edge(P1, P2), new Edge(P2, P3), new Edge(P3, P1) };
		}

		#region Equality members

		protected bool Equals(Triangle other)
		{
			Contract.Requires(other != null);
			return Equals(P1, other.P1) && Equals(P2, other.P2) && Equals(P3, other.P3);
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
			return Equals((Triangle)obj);
		}

		public override int GetHashCode()
		{
			unchecked {
				var hashCode = P1.GetHashCode();
				hashCode = (hashCode * 397) ^ P2.GetHashCode();
				hashCode = (hashCode * 397) ^ P3.GetHashCode();
				return hashCode;
			}
		}

		#endregion
	}

	[DebuggerDisplay("Edge(({P1.X}, {P1.Y},{P1.Z}), ({P2.X}, {P2.Y},{P2.Z}))")]
	public class Edge
	{
		public readonly Point P1;
		public readonly Point P2;
		private Point _vector;

		public Edge(Point p1, Point p2)
		{
			P1 = p1;
			P2 = p2;
		}

		public Point Vector
		{
			get { return _vector ?? (_vector = P2 - P1); }
		}

		#region Equality members

		protected bool Equals(Edge other)
		{
			Contract.Requires(other != null);
			return Equals(P1, other.P1) && Equals(P2, other.P2)
					|| Equals(P1, other.P2) && Equals(P1, other.P2);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			return obj.GetType() == GetType() && Equals((Edge)obj);
		}

		public override int GetHashCode()
		{
			return (P1.GetHashCode()) ^ (P2.GetHashCode());
		}

		#endregion
	}
}