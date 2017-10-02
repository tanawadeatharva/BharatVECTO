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
using System.Windows.Input;

namespace HashingTool.ViewModel
{
	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other objects by invoking delegates. The default return value for the CanExecute method is 'true'.
	/// </summary>
	public class RelayCommand<T> : ICommand
	{
		#region Declarations

		readonly Predicate<T> _canExecute;
		readonly Action<T> _execute;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
		{
			if (execute == null) {
				throw new ArgumentNullException("execute");
			}
			_execute = execute;
			_canExecute = canExecute;
		}

		#endregion

		#region ICommand Members

		public event EventHandler CanExecuteChanged
		{
			add {
				if (_canExecute != null) {
					CommandManager.RequerySuggested += value;
				}
			}
			remove {
				if (_canExecute != null) {
					CommandManager.RequerySuggested -= value;
				}
			}
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}


		[DebuggerStepThrough]
		public Boolean CanExecute(Object parameter)
		{
			return _canExecute == null || _canExecute((T)parameter);
		}

		public void Execute(Object parameter)
		{
			_execute((T)parameter);
		}

		#endregion
	}

	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other objects by invoking delegates. The default return value for the CanExecute method is 'true'.
	/// </summary>
	public class RelayCommand : ICommand
	{
		#region Declarations

		readonly Func<Boolean> _canExecute;
		readonly Action _execute;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action execute, Func<Boolean> canExecute = null)
		{
			if (execute == null) {
				throw new ArgumentNullException("execute");
			}
			_execute = execute;
			_canExecute = canExecute;
		}

		#endregion

		#region ICommand Members

		public event EventHandler CanExecuteChanged
		{
			add {
				if (_canExecute != null) {
					CommandManager.RequerySuggested += value;
				}
			}
			remove {
				if (_canExecute != null) {
					CommandManager.RequerySuggested -= value;
				}
			}
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}

		[DebuggerStepThrough]
		public Boolean CanExecute(Object parameter)
		{
			return _canExecute == null || _canExecute();
		}

		public void Execute(Object parameter)
		{
			_execute();
		}

		#endregion
	}
}
