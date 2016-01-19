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