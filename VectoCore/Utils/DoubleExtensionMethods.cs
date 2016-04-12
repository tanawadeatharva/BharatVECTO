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

namespace TUGraz.VectoCore.Utils
{
	/// <summary>
	/// Extension methods for double.
	/// </summary>
	public static class DoubleExtensionMethods
	{
		/// <summary>
		/// The tolerance.
		/// </summary>
		public const double Tolerance = 1e-6;

		/// <summary>
		/// The tolerancefactor for relative comparisons.
		/// </summary>
		public const double ToleranceFactor = 1e-6;


		/// <summary>
		/// Determines whether the specified other is equal within tolerance.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="other">The other.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerHidden]
		public static bool IsEqual(this double self, double other, double tolerance = Tolerance)
		{
			return Math.Abs(self - other) < tolerance;
		}

		[DebuggerHidden]
		public static bool IsRelativeEqual(this SI expected, SI actual, double toleranceFactor = ToleranceFactor)
		{
			return IsRelativeEqual(expected.Value(), actual.Value(), toleranceFactor: toleranceFactor);
		}

		[DebuggerHidden]
		public static bool IsRelativeEqual(this double expected, double actual,
			double toleranceFactor = DoubleExtensionMethods.ToleranceFactor)
		{
			if (double.IsNaN(expected)) {
				return double.IsNaN(actual);
			}

			var ratio = expected.IsEqual(0, toleranceFactor) ? Math.Abs(actual) : Math.Abs(actual / expected - 1);
			return ratio < toleranceFactor;
		}

		/// <summary>
		/// Determines whether the specified other is smaller within tolerance.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="other">The other.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerHidden]
		public static bool IsSmaller(this double self, double other, double tolerance = Tolerance)
		{
			return self < other - tolerance;
		}

		/// <summary>
		/// Determines whether the specified other is smaller or equal within tolerance.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="other">The other.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		public static bool IsSmallerOrEqual(this double self, double other, double tolerance = Tolerance)
		{
			return self <= other + tolerance;
		}

		/// <summary>
		/// Determines whether the specified other is greater within tolerance.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="other">The other.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		public static bool IsGreater(this double self, double other, double tolerance = Tolerance)
		{
			return self > other + tolerance;
		}

		/// <summary>
		/// Determines whether the specified other is greater or equal within tolerance.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="other">The other.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		public static bool IsGreaterOrEqual(this double self, double other, double tolerance = Tolerance)
		{
			return self >= other - tolerance;
		}

		/// <summary>
		/// Determines whether the specified tolerance is positive within tolerance.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		public static bool IsPositive(this double self, double tolerance = Tolerance)
		{
			return self.IsGreaterOrEqual(0.0, tolerance);
		}

		/// <summary>
		/// Converts the double-value from RPM (rounds per minute) to the SI Unit PerSecond.
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		public static PerSecond RPMtoRad(this double self)
		{
			return SI<PerSecond>(self * 2 * Math.PI / 60.0);
		}

		public static MeterPerSecond KMPHtoMeterPerSecond(this double self)
		{
			return SI<MeterPerSecond>(self / 3.6);
		}

		public static PerSecond RadToRPM(this double self)
		{
			return SI<PerSecond>(self * 60.0 / (2 * Math.PI));
		}

		public static double ToDegree(this double self)
		{
			return self * 180.0 / Math.PI;
		}


		/// <summary>
		/// Creates an SI object for the number (unit-less: [-]).
		/// </summary>
		public static SI SI(this double value)
		{
			return new SI(value);
		}

		/// <summary>
		/// Creates an templated SI object for the number.
		/// </summary>
		[DebuggerHidden]
		public static T SI<T>(this double value) where T : SIBase<T>
		{
			return SIBase<T>.Create(value);
		}

		public static IEnumerable<T> SI<T>(this IEnumerable<double> self) where T : SIBase<T>
		{
			return self.Select(x => x.SI<T>());
		}
	}

	public static class FloatExtensionMethods
	{
		[DebuggerHidden]
		public static T SI<T>(this float value) where T : SIBase<T>
		{
			return SIBase<T>.Create(value);
		}
	}
}