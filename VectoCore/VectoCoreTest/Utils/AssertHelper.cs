/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using System.Diagnostics;
using System.Globalization;
using TUGraz.VectoCommon.Utils;
using NUnit.Framework;

namespace TUGraz.VectoCore.Tests.Utils
{
	public static class AssertHelper
	{
		/// <summary>
		/// Assert an expected Exception.
		/// </summary>
		[DebuggerHidden]
		public static void Exception<T>(this Action func, string message = null) where T : Exception
		{
			try {
				func();
				Assert.Fail("Expected Exception {0}, but no exception occured.", typeof(T));
			} catch (T ex) {
				if (message != null) {
					Assert.AreEqual(message, ex.Message);
				}
			}
		}

		[DebuggerHidden]
		public static void AreRelativeEqual(SI expected, SI actual,
			double toleranceFactor = DoubleExtensionMethods.ToleranceFactor, string message = null)
		{
			Assert.IsTrue(actual.HasEqualUnit(expected),
				string.Format("Wrong SI Units: expected: {0}, actual: {1}", expected.ToBasicUnits(), actual.ToBasicUnits()));
			AreRelativeEqual(expected.Value(), actual.Value(), toleranceFactor: toleranceFactor, message: message);
		}

		[DebuggerHidden]
		public static void AreRelativeEqual(Scalar expected, Scalar actual,
			double toleranceFactor = DoubleExtensionMethods.ToleranceFactor)
		{
			AreRelativeEqual(expected.Value(), actual.Value(), toleranceFactor: toleranceFactor);
		}

		[DebuggerHidden]
		public static void AreRelativeEqual(double? expected, SI actual,
			double toleranceFactor = DoubleExtensionMethods.ToleranceFactor)
		{
			if (expected.HasValue) {
				AreRelativeEqual(expected.Value, actual.Value(), toleranceFactor: toleranceFactor);
			} else {
				Assert.IsNull(actual, "Both Values have to be null or not null.");
			}
		}

		[DebuggerHidden]
		public static void AreRelativeEqual(double? expected, double? actual, string message = null,
			double toleranceFactor = DoubleExtensionMethods.ToleranceFactor)
		{
			if (!string.IsNullOrWhiteSpace(message)) {
				message = "\n" + message;
			} else {
				message = "";
			}

			Assert.IsFalse(expected.HasValue ^ actual.HasValue, "Both Values have to be null or not null.");

			if (double.IsNaN(expected.Value)) {
				Assert.IsTrue(double.IsNaN(actual.Value),
					string.Format("Actual value is not NaN. Expected: {0}, Actual: {1}{2}", expected, actual, message));
				return;
			}

			var ratio = expected == 0 ? Math.Abs(actual.Value) : Math.Abs(actual.Value / expected.Value - 1);
			Assert.IsTrue(ratio < toleranceFactor, string.Format(CultureInfo.InvariantCulture,
				"Given values are not equal. Expected: {0}, Actual: {1}, Difference: {3} (Tolerance Factor: {2}){4}",
				expected, actual, toleranceFactor, expected - actual, message));
		}
	}
}
