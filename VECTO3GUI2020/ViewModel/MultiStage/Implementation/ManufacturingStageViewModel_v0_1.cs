using System;
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.Impl;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public class ManufacturingStageViewModel_v0_1 : StageViewModelBase, IManufacturingStageViewModel
	{
		public DigestData HashPreviousStep
		{
			get => _hashPreviousStep;
			set => SetProperty(ref _hashPreviousStep, value);
		}

		public int StepCount
		{
			get => _stepCount;
			set => SetProperty(ref _stepCount, value);
		}

		public IVehicleDeclarationInputData Vehicle => _vehicleViewModel;

		private IApplicationInformation _applicationInformation = new ApplicationInformation {
			Date = DateTime.Today,
		};

		public IApplicationInformation ApplicationInformation
		{
			get => _applicationInformation;

		} 

		public DigestData Signature => throw new NotImplementedException();
		public void SetInputData(IVehicleDeclarationInputData vehicleInputData)
		{
			VehicleViewModel.SetVehicleInputData(vehicleInputData, true);

			OnPropertyChanged(nameof(CurrentView));

		}
		private ObservableCollection<CompletedBusArchitecture> _architectureItems =
			new ObservableCollection<CompletedBusArchitecture>(Enum.GetValues(typeof(CompletedBusArchitecture)).Cast<CompletedBusArchitecture>());

		public ObservableCollection<CompletedBusArchitecture> ArchitectureItems
		{
			get => _architectureItems;
			set => SetProperty(ref _architectureItems, value);
		}

        public CompletedBusArchitecture Architecture
		{
			get => _architecture;
			set
			{
				SetProperty(ref _architecture, value);
				_architectureItems = new ObservableCollection<CompletedBusArchitecture>() { Architecture };
			}
		}


		public ManufacturingStageViewModel_v0_1(IManufacturingStageInputData consolidatedManufacturingStageInputData, bool exempted,
			IMultiStageViewModelFactory viewModelFactory) : base(viewModelFactory)
		{
			Title = "Edit Manufacturing Stage";
			Architecture =
				consolidatedManufacturingStageInputData.Vehicle.VehicleType.GetCompletedBusArchitecture(
					consolidatedManufacturingStageInputData.Vehicle.ExemptedVehicle);
			
			_stepCount = consolidatedManufacturingStageInputData?.StepCount + 1 ?? 2;

			_consolidatedManufacturingStageInputData = consolidatedManufacturingStageInputData;


			VehicleViewModel = (IMultistageVehicleViewModel)_viewModelFactory.GetInterimStageVehicleViewModel(consolidatedManufacturingStageInputData?.Vehicle, exempted);
			CurrentView = VehicleViewModel as IViewModelBase;


			Components.Add(VehicleViewModel.Name, VehicleViewModel as IViewModelBase);
			Components.Add("Airdrag", VehicleViewModel.MultistageAirdragViewModel as IViewModelBase);
			Components.Add("Auxiliaries", VehicleViewModel.MultistageAuxiliariesViewModel as IViewModelBase);
		}


		
		private int _stepCount;
		private DigestData _hashPreviousStep;
		private IManufacturingStageInputData _consolidatedManufacturingStageInputData;
		private CompletedBusArchitecture _architecture;


		private class ApplicationInformationMultistage : IApplicationInformation
		{
			public string SimulationToolVersion => "VECTO3";

			public DateTime Date => DateTime.Today;
		}
	}


	public interface IManufacturingStageViewModel : IManufacturingStageInputData, IStageViewModelBase{
		void SetInputData(IVehicleDeclarationInputData vehicleInputData);
	}
}
