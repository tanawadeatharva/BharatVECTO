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
using System.Diagnostics;

namespace TUGraz.VectoCore.Utils
{
	public static class IntExtensionMethods
	{
		/// <summary>
		/// Converts the value from rounds per minute to the SI Unit PerSecond
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		[DebuggerHidden]
		public static PerSecond RPMtoRad(this int d)
		{
			return (d * 2.0 * Math.PI / 60.0).SI<PerSecond>();
			//d.SI().Rounds.Per.Minute.ConvertTo().Radian.Per.Second.Cast<PerSecond>();
		}

		[DebuggerHidden]
		public static MeterPerSecond KMPHtoMeterPerSecond(this int d)
		{
			return (d / 3.6).SI<MeterPerSecond>();
			//d.SI().Kilo.Meter.Per.Hour.Cast<MeterPerSecond>();
		}


		/// <summary>
		/// Gets the unit-less SI representation of the number.
		/// </summary>
		[DebuggerHidden]
		public static SI SI(this int value)
		{
			return new SI(value);
		}

		/// <summary>
		/// Gets the special SI class of the number.
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		[DebuggerHidden]
		public static T SI<T>(this int d) where T : SIBase<T>
		{
			return SIBase<T>.Create(d);
		}

		public static double ToRadian(this int self)
		{
			return self * Math.PI / 180.0;
		}

		/// <summary>
		/// Modulo functions which also works on negative Numbers (not like the built-in %-operator which just returns the remainder).
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int Mod(this int a, int b)
		{
			return (a %= b) < 0 ? a + b : a;
		}
	}
}