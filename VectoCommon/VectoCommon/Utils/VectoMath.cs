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
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCommon.Utils
{
	/// <summary>
	/// Provides helper methods for mathematical functions.
	/// </summary>
	public static class VectoMath
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
			return Interpolate(x1.Value(), x2.Value(), y1.Value(), y2.Value(), xint.Value()).SI<TResult>();
		}

		public static double Interpolate<T>(T x1, T x2, double y1, double y2, T xint) where T : SI
		{
			return Interpolate(x1.Value(), x2.Value(), y1, y2, xint.Value());
		}

		public static TResult Interpolate<TResult>(double x1, double x2, TResult y1, TResult y2, double xint)
			where TResult : SIBase<TResult>
		{
			return Interpolate(x1, x2, y1.Value(), y2.Value(), xint).SI<TResult>();
		}

		public static double Interpolate(Point p1, Point p2, double x)
		{
			return Interpolate(p1.X, p2.X, p1.Y, p2.Y, x);
		}

		/// <summary>
		/// Linearly interpolates a value between two points.
		/// </summary>
		public static double Interpolate(double x1, double x2, double y1, double y2, double xint)
		{
			return (xint - x1) * (y2 - y1) / (x2 - x1) + y1;
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

		public static T Max<T>(T c1, T c2, T c3) where T : IComparable
		{
			return Max(Max(c1, c2), c3);
		}

		public static T Limit<T>(this T value, T lowerBound, T upperBound) where T : IComparable
		{
			if (lowerBound.CompareTo(upperBound) > 0) {
				throw new VectoException(
					"VectoMath.Limit: lowerBound must not be greater than upperBound. lowerBound: {0}, upperBound: {1}", lowerBound,
					upperBound);
			}

			if (value.CompareTo(upperBound) > 0) {
				return upperBound;
			}
			if (value.CompareTo(lowerBound) < 0) {
				return lowerBound;
			}
			return value;
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
			var d = b * b - 4 * a * c;

			if (d < 0) {
				return retVal;
			} else if (d > 0) {
				// two solutions possible
				retVal.Add((-b + Math.Sqrt(d)) / (2 * a));
				retVal.Add((-b - Math.Sqrt(d)) / (2 * a));
			} else {
				// only one solution possible
				retVal.Add((-b / (2 * a)));
			}
			return retVal;
		}

		public static Point Intersect(Edge line1, Edge line2)
		{
			var s10X = line1.P2.X - line1.P1.X;
			var s10Y = line1.P2.Y - line1.P1.Y;
			var s32X = line2.P2.X - line2.P1.X;
			var s32Y = line2.P2.Y - line2.P1.Y;

			var denom = s10X * s32Y - s32X * s10Y;
			if (denom.IsEqual(0)) {
				return null;
			}

			var s02X = line1.P1.X - line2.P1.X;
			var s02Y = line1.P1.Y - line2.P1.Y;
			var sNumer = s10X * s02Y - s10Y * s02X;
			if ((sNumer < 0) == (denom > 0)) {
				return null;
			}
			var tNumer = s32X * s02Y - s32Y * s02X;
			if ((tNumer < 0) == (denom > 0)) {
				return null;
			}
			if (((sNumer > denom) == (denom > 0)) || ((tNumer > denom) == (denom > 0))) {
				return null;
			}
			var t = tNumer / denom;

			return new Point(line1.P1.X + (t * s10X), line1.P1.Y + t * s10Y);
		}

		/// <summary>
		/// Computes the time interval for driving the given distance ds with the vehicle's current speed and the given acceleration.
		/// If the distance ds can not be reached (i.e., the vehicle would halt before ds is reached) then the distance parameter is adjusted.
		/// Returns a new operating point (a, ds, dt)
		/// </summary>
		/// <param name="currentSpeed">vehicle's current speed at the beginning of the simulation interval</param>
		/// <param name="acceleration">vehicle's acceleration</param>
		/// <param name="distance">absolute distance at the beginning of the simulation interval (can be 0)</param>
		/// <param name="ds">distance to drive in the current simulation interval</param>
		/// <returns>Operating point (a, ds, dt)</returns>
		public static OperatingPoint ComputeTimeInterval(MeterPerSecond currentSpeed, MeterPerSquareSecond acceleration,
			Meter distance, Meter ds)
		{
			if (!(ds > 0)) {
				throw new VectoSimulationException("ds has to be greater than 0! ds: {0}", ds);
			}

			var retVal = new OperatingPoint() { Acceleration = acceleration, SimulationDistance = ds };
			if (acceleration.IsEqual(0)) {
				if (currentSpeed > 0) {
					retVal.SimulationInterval = ds / currentSpeed;
					return retVal;
				}
				//Log.Error("{2}: vehicle speed is {0}, acceleration is {1}", currentSpeed.Value(), acceleration.Value(),
				//	distance);
				throw new VectoSimulationException(
					"vehicle speed has to be > 0 if acceleration = 0!  v: {0}, a: {1}, distance: {2}", currentSpeed.Value(),
					acceleration.Value(), distance);
			}

			// we need to accelerate / decelerate. solve quadratic equation...
			// ds = acceleration / 2 * dt^2 + currentSpeed * dt   => solve for dt
			var solutions = VectoMath.QuadraticEquationSolver(acceleration.Value() / 2.0, currentSpeed.Value(),
				-ds.Value());

			if (solutions.Count == 0) {
				// no real-valued solutions: acceleration is so negative that vehicle stops already before the required distance can be reached.
				// adapt ds to the halting-point.
				// t = v / a
				var dt = currentSpeed / -acceleration;

				// s = a/2*t^2 + v*t
				var stopDistance = acceleration / 2 * dt * dt + currentSpeed * dt;

				if (stopDistance.IsGreater(ds)) {
					// just to cover everything - does not happen...
					//Log.Error(
					//	"Could not find solution for computing required time interval to drive distance ds: {0}. currentSpeed: {1}, acceleration: {2}, stopDistance: {3}, distance: {4}",
					//	ds, currentSpeed, acceleration, stopDistance,distance);
					throw new VectoSimulationException("Could not find solution for time-interval!  ds: {0}, stopDistance: {1}", ds,
						stopDistance);
				}

				//LoggingObject.Logger<>().Info(
				//	"Adjusted distance when computing time interval: currentSpeed: {0}, acceleration: {1}, distance: {2} -> {3}, timeInterval: {4}",
				//	currentSpeed, acceleration, stopDistance, stopDistance, dt);

				retVal.SimulationInterval = dt;
				retVal.SimulationDistance = stopDistance;
				return retVal;
			}
			// if there are 2 positive solutions (i.e. when decelerating), take the smaller time interval
			// (the second solution means that you reach negative speed)
			retVal.SimulationInterval = solutions.Where(x => x >= 0).Min().SI<Second>();
			return retVal;
		}

		public static T Ceiling<T>(T si) where T : SIBase<T>
		{
			return Math.Ceiling(si.Value()).SI<T>();
		}

		public static List<double> CubicEquationSolver(double A, double B, double C, double D)
		{
			//var a = B / A;
			//var b = C / A;
			//var d = D / A;

			//var p = b / a - a * a / 3;
			//var q = 2 * a * a / 27 - a * b / 3 + C;

			//var R = q * q / 4 + p * p * p / 27;
			//if (R > 0) {
			//	// one real and two complex solutions - we are only interested on the real solution

			//} else {
			//	// three real solutions (two may coincide)
			//}
			var solutions = new List<double>();
			if (A.IsEqual(0, 1e-12)) {
				return QuadraticEquationSolver(B, C, D);
			}
			var w = B / (3 * A);
			var p = Math.Pow(C / (3 * A) - w * w, 3);
			var q = -0.5 * (2 * (w * w * w) - (C * w - D) / A);
			var d = q * q + p; // discriminant
			if (d < 0.0) {
				// 3 real solutions
				var h = q / Math.Sqrt(-p);
				var phi = Math.Acos(Math.Max(-1.0, Math.Min(1.0, h)));
				p = 2 * Math.Pow(-p, 1.0 / 6.0);
				for (var i = 0; i < 3; i++) {
					solutions.Add(p * Math.Cos((phi + 2 * i * Math.PI) / 3.0) - w);
				}
			} else {
				// only one real solution
				d = Math.Sqrt(d);
				solutions.Add(Cbrt(q + d) + Cbrt(q - d) - w);
			}

			// 1 Newton iteration step in order to minimize round-off errors
			for (var i = 0; i < solutions.Count; i++) {
				var h = C + solutions[i] * (2 * B + 3 * solutions[i] * A);
				if (!h.IsEqual(0, 1e-12)) {
					solutions[i] -= (D + solutions[i] * (C + solutions[i] * (B + solutions[i] * A))) / h;
				}
			}
			solutions.Sort();
			return solutions;
		}

		private static double Cbrt(double x)
		{
			return x < 0 ? -Math.Pow(-x, 1.0 / 3.0) : Math.Pow(x, 1.0 / 3.0);
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
		public Point Perpendicular()
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
			return X.IsEqual(other.X) && Y.IsEqual(other.Y) && Z.IsEqual(other.Z);
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

		/// <summary>
		/// Test if point is on the left side of an edge.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public bool IsLeftOf(Edge e)
		{
			var abX = e.P2.X - e.P1.X;
			var abY = e.P2.Y - e.P1.Y;
			var acX = X - e.P1.X;
			var acY = Y - e.P1.Y;
			var z = abX * acY - abY * acX;
			return z.IsGreater(0);
		}
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
			var abX = tr.P2.X - tr.P1.X;
			var abY = tr.P2.Y - tr.P1.Y;
			var abZ = tr.P2.Z - tr.P1.Z;

			var acX = tr.P3.X - tr.P1.X;
			var acY = tr.P3.Y - tr.P1.Y;
			var acZ = tr.P3.Z - tr.P1.Z;

			X = abY * acZ - abZ * acY;
			Y = abZ * acX - abX * acZ;
			Z = abX * acY - abY * acX;
			W = tr.P1.X * X + tr.P1.Y * Y + tr.P1.Z * Z;
		}
	}

	[DebuggerDisplay("Triangle(({P1.X}, {P1.Y}, {P1.Z}), ({P2.X}, {P2.Y}, {P2.Z}), ({P3.X}, {P3.Y}, {P3.Z}))")]
	public class Triangle
	{
		public readonly Point P1;
		public readonly Point P2;
		public readonly Point P3;

		public Triangle(Point p1, Point p2, Point p3)
		{
			P1 = p1;
			P2 = p2;
			P3 = p3;

			if ((P1.X.IsEqual(P2.X) && P2.X.IsEqual(P3.X)) || (P1.Y.IsEqual(P2.Y) && P2.Y.IsEqual(P3.Y))) {
				throw new VectoException("triangle is not extrapolatable by a plane.");
			}
		}

		/// <summary>
		/// Barycentric Technique: http://www.blackpawn.com/texts/pointinpoly/default.html
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="exact"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsInside(double x, double y, bool exact)
		{
			var smallerY = y - DoubleExtensionMethods.Tolerance;
			var biggerY = y + DoubleExtensionMethods.Tolerance;
			var smallerX = x - DoubleExtensionMethods.Tolerance;
			var biggerX = x + DoubleExtensionMethods.Tolerance;

			if ((P1.Y < smallerY && P2.Y < smallerY && P3.Y < smallerY)
				|| (P1.X < smallerX && P2.X < smallerX && P3.X < smallerX)
				|| (P1.X > biggerX && P2.X > biggerX && P3.X > biggerX)
				|| (P1.Y > biggerY && P2.Y > biggerY && P3.Y > biggerY)) {
				return false;
			}

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

			var p0Square = p0X * p0X + p0Y * p0Y;
			var p1Square = p1X * p1X + p1Y * p1Y;
			var p2Square = p2X * p2X + p2Y * p2Y;

			var det01 = p0X * p1Y - p1X * p0Y;
			var det12 = p1X * p2Y - p2X * p1Y;
			var det20 = p2X * p0Y - p0X * p2Y;

			var result = p0Square * det12 + p1Square * det20 + p2Square * det01;
			return result > 0;

			//double[,] m = { { P1.X - p.X, P1.Y - p.Y, (P1.X * P1.X - p.X*p.X) + (P1.Y * P1.Y - p.Y*p.Y) }, 
			//				{ P2.X - p.X, P2.Y - p.Y, (P2.X * P2.X - p.X*p.X) + (P2.Y * P2.Y - p.Y*p.Y) }, 
			//				{ P3.X - p.X, P3.Y - p.Y, (P3.X * P3.X - p.X*p.X) + (P3.Y * P3.Y - p.Y*p.Y) } };
			//var det = m[0, 0] * m[1, 1] * m[2, 2]
			//		+ m[0, 1] * m[1, 2] * m[2, 0]
			//		+ m[0, 2] * m[1, 0] * m[2, 1]
			//		- m[0, 0] * m[1, 2] * m[2, 1]
			//		- m[0, 1] * m[1, 0] * m[2, 2]
			//		- m[0, 2] * m[1, 1] * m[2, 0];
			//return det > 0;
		}

		public bool Contains(Point p)
		{
			return p.Equals(P1) || p.Equals(P2) || p.Equals(P3);
		}

		public bool SharesVertexWith(Triangle t)
		{
			return Contains(t.P1) || Contains(t.P2) || Contains(t.P3);
		}

		public Edge[] GetEdges()
		{
			return new[] { new Edge(P1, P2), new Edge(P2, P3), new Edge(P3, P1) };
		}

		#region Equality members

		protected bool Equals(Triangle other)
		{
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

		public double SlopeXY
		{
			get { return Vector.Y / Vector.X; }
		}

		public double OffsetXY
		{
			get { return P2.Y - SlopeXY * P2.X; }
		}

		#region Equality members

		protected bool Equals(Edge other)
		{
			return (P1.Equals(other.P1) && Equals(P2, other.P2)) || (P1.Equals(other.P2) && P2.Equals(other.P1));
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
			return P1.GetHashCode() ^ P2.GetHashCode();
		}

		#endregion

		public static Edge Create(Point arg1, Point arg2)
		{
			return new Edge(arg1, arg2);
		}

		public bool ContainsXY(Point point)
		{
			return (SlopeXY * point.X + (P1.Y - SlopeXY * P1.X) - point.Y).IsEqual(0, 1E-9);
		}
	}
}