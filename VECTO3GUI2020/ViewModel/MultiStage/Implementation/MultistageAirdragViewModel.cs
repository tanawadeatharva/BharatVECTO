using System;
using System.Windows;
using System.Windows.Input;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public class MultistageAirdragViewModel : ViewModelBase, IMultistageAirdragViewModel
	{
		private IDialogHelper _dialogHelper;
		private IXMLInputDataReader _inputDataReader;
		private IComponentViewModelFactory _componentViewModelFactory;
		private IAirDragViewModel _airdragViewModel;
		private bool _airdragModified;


		public IAirDragViewModel AirDragViewModel
		{
			get => _airdragViewModel;
			set => SetProperty(ref _airdragViewModel, value);
		}

		public bool AirdragModified
		{
			get => _airdragModified;
			set => SetProperty(ref _airdragModified, value);
		}


		#region Commands

		private ICommand _loadAirdragFileCommand;

		public ICommand LoadAirdragFileCommand
		{
			get => _loadAirdragFileCommand ?? new RelayCommand(LoadAirdragFileCommandExecute, () => true);
		}

		public void LoadAirdragFileCommandExecute()
		{
			var fileName = _dialogHelper.OpenXMLFileDialog(Settings.Default.DefaultFilePath);

			try {
				IAirdragDeclarationInputData airdragInputData = _inputDataReader.Create(fileName) as IAirdragDeclarationInputData;
				AirDragViewModel = (AirDragViewModel)_componentViewModelFactory.CreateComponentViewModel(airdragInputData);
				AirdragModified = true;


			}
			catch (Exception e) {
				_dialogHelper.ShowMessageBox(e.Message, "Invalid File", MessageBoxButton.OK,
					MessageBoxImage.Error);
			}
		}

		#endregion




		public MultistageAirdragViewModel(IDialogHelper dialogHelper, IXMLInputDataReader inputDataReader, IComponentViewModelFactory componentViewModelFactory)
		{
			_dialogHelper = dialogHelper;
			_inputDataReader = inputDataReader;
			_componentViewModelFactory = componentViewModelFactory;
		}
	}
}