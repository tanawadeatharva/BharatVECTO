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

namespace TUGraz.VectoCommon.Utils
{
	public static class IntExtensionMethods
	{
		/// <summary>
		/// Gets the unit-less SI representation of the number.
		/// </summary>
		//[DebuggerHidden]
		public static SI SI(this int value)
		{
			return SIBase<Scalar>.Create(value);
		}
		
        public static SI SI(this int value, UnitInstance si)
        {
            return new SI(si,value);
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