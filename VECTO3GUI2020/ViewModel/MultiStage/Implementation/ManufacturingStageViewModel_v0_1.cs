using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.GenericModelData;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
    public class ManufacturingStageViewModel_v0_1 : ViewModelBase, IManufacturingStageViewModel
	{
		public static readonly string INPUTPROVIDERTYPE = typeof(XMLDeclarationMultistageTypeInputDataV01).ToString();
		public static readonly string QualifiedXSD = XMLDeclarationMultistageTypeInputDataV01.QUALIFIED_XSD_TYPE;
		private Dictionary<string, IViewModelBase> Components = new Dictionary<string, IViewModelBase>(StringComparer.CurrentCultureIgnoreCase);

		public DigestData HashPreviousStage => throw new NotImplementedException();

		public int StageCount => throw new NotImplementedException();

		public IVehicleDeclarationInputData Vehicle => _vehicleViewModel;

		public IViewModelBase CurrentView
		{
			get => _currentview;
			set => SetProperty(ref _currentview, value);
		}

		private IApplicationInformation _applicationInformation;
		private IVehicleViewModel _vehicleViewModel;
		private IMultiStageViewModelFactory _viewModelFactory;
		private IViewModelBase _currentview;

		public IVehicleViewModel VehicleViewModel
		{
			get => _vehicleViewModel;
			set => SetProperty(ref _vehicleViewModel, value);
		}

		public IApplicationInformation ApplicationInformation
		{
			get => _applicationInformation;

		} 

		public DigestData Signature => throw new NotImplementedException();


		public ManufacturingStageViewModel_v0_1(IManufacturingStageInputData prevStageInputData, IMultiStageViewModelFactory viewModelFactory)
		{
			_viewModelFactory = viewModelFactory;

			VehicleViewModel =
				_viewModelFactory.CreateInterimStageVehicleViewModel(prevStageInputData.Vehicle.GetType().ToString());
			

			CurrentView = VehicleViewModel as IViewModelBase;



			Components.Add(VehicleViewModel.Name, VehicleViewModel as IViewModelBase);

			var airDragEditViewModel = viewModelFactory.createMultistageAirdragViewModel();
			Components.Add("Airdrag", airDragEditViewModel as IViewModelBase);
		}


		private ICommand _switchComponentViewCommand;
		public ICommand SwitchComponentViewCommand
		{
			get {
				return _switchComponentViewCommand ?? new RelayCommand<string>(SwitchViewExecute, (string s) => true);

			}
		}

		private void SwitchViewExecute(string viewToShow)
		{
			IViewModelBase newView;
			var success= Components.TryGetValue(viewToShow, out newView);
			if (success) {
				CurrentView = newView;
			}
		}
	}

	public interface IManufacturingStageViewModel : IManufacturingStageInputData { }
}
