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
using System.Diagnostics;

namespace TUGraz.VectoCore.Utils
{
	/// <summary>
	/// Extension Methods for Creating a Switch-Case Construct on Types.
	/// </summary>
	/// <remarks>
	/// Adapted for VECTO. Created by Virtlink. Original source code on GitHub: <see href="https://gist.github.com/Virtlink/8722649"/>.
	/// </remarks>
	public static class SwitchExtension
	{
		/// <summary>
		/// Switches on the type.
		/// With ".Case" you can define single alternatives (only the first suitable case will be executed).
		/// With ".If" you can define multiple type-conditionals (every suitable if will be executed)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self">The self.</param>
		/// <returns></returns>
		[DebuggerHidden]
		public static Switch<T> Switch<T>(this T self)
		{
			return new Switch<T>(self);
		}
	}

	public class Switch<T>
	{
		private readonly T _value;
		private bool _handled;

		[DebuggerHidden]
		internal Switch(T value)
		{
			_value = value;
			_handled = false;
		}

		[DebuggerHidden]
		public Switch<T> Case<TFilter>(Action action) where TFilter : T
		{
			return Case<TFilter>(_ => action());
		}

		[DebuggerHidden]
		public Switch<T> Case<TFilter>() where TFilter : T
		{
			return Case<TFilter>(() => {});
		}


		[DebuggerHidden]
		public Switch<T> Case<TFilter>(Action<TFilter> action) where TFilter : T
		{
			if (!_handled && _value.GetType() == typeof(TFilter)) {
				action((TFilter)_value);
				_handled = true;
			}
			return this;
		}

		/// <summary>
		/// Does the action if the type is fullfilled and continues the evaluation.
		/// </summary>
		/// <typeparam name="TFilter">The type of the filter.</typeparam>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		[DebuggerHidden]
		public Switch<T> If<TFilter>(Action<TFilter> action) where TFilter : class
		{
			if (_value is TFilter) {
				action(_value as TFilter);
			}
			return this;
		}

		[DebuggerHidden]
		public void Default(Action action)
		{
			Default(_ => action());
		}

		[DebuggerHidden]
		public void Default(Action<T> action)
		{
			if (!_handled) {
				action(_value);
			}
		}
	}
}