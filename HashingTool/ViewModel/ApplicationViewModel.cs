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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using HashingTool.Views;

namespace HashingTool.ViewModel
{
	public class ApplicationViewModel : ObservableObject
	{
		private ICommand _changeViewCommand;
		public static ICommand HomeView;

		private IMainView _currentView;
		public static List<IMainView> AvailableViews;
		private string _hashingLib;
		private string _myVersion;

		public ApplicationViewModel()
		{
			var homeView = new HomeViewModel();
			AvailableViews = new List<IMainView> {
				new HashComponentDataViewModel(),
				new VerifyComponentInputDataViewModel(),
				new VerifyJobInputDataViewModel(),
				new VerifyResultDataViewModel(),
			};

			CurrentViewModel = homeView;

			HomeView = new RelayCommand(() => CurrentViewModel = homeView);

			try {
				_hashingLib = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VectoHashing.dll"))
					.GetName().Version.ToString();
			} catch (Exception) {
				_hashingLib = "NOT FOUND";
			}
			try {
				_myVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			} catch (Exception) {
				_myVersion = "NOT FOUND";
			}
		}

		public List<IMainView> MainViewModels
		{
			get { return AvailableViews ?? (AvailableViews = new List<IMainView>()); }
		}

		public IMainView CurrentViewModel
		{
			get { return _currentView; }
			set {
				if (_currentView == value) {
					return;
				}
				_currentView = value;
				RaisePropertyChanged("CurrentViewModel");
			}
		}

		public ICommand ChangeViewCommand
		{
			get { return _changeViewCommand ?? (_changeViewCommand = new RelayCommand<IMainView>(ChangeViewModel)); }
		}

		public ICommand ShowHomeViewCommand
		{
			get { return HomeView; }
		}


		private void ChangeViewModel(IMainView mainView)
		{
			if (!MainViewModels.Contains(mainView)) {
				return;
			}

			CurrentViewModel = MainViewModels.FirstOrDefault(mv => mv == mainView);
		}

		public string VersionInformation
		{
			get { return string.Format("Vecto Hashing Tool {0} / Hashing Library {1}", _myVersion, _hashingLib); }
		}
	}
}
