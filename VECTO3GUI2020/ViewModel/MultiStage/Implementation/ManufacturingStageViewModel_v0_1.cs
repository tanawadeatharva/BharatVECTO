using System;
using System.Collections;
using System.Linq;
using System.Net.Mime;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.GenericModelData;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public class ManufacturingStageViewModel_v0_1 : StageViewModelBase, IManufacturingStageViewModel
	{
		public DigestData HashPreviousStage
		{
			get => _hashPreviousStage;
			set => SetProperty(ref _hashPreviousStage, value);
		}

		public int StageCount
		{
			get => _stageCount;
			set => SetProperty(ref _stageCount, value);
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
			VehicleViewModel.SetVehicleInputData(vehicleInputData);

			OnPropertyChanged(nameof(CurrentView));

		}


		public ManufacturingStageViewModel_v0_1(IManufacturingStageInputData consolidatedManufacturingStageInputData, bool exempted,
			IMultiStageViewModelFactory viewModelFactory) : base(viewModelFactory)
		{
			Title = "Edit Manufacturing Stage";
			
			
			_stageCount = consolidatedManufacturingStageInputData?.StageCount + 1 ?? 2;

			_consolidatedManufacturingStageInputData = consolidatedManufacturingStageInputData;


			VehicleViewModel = (IMultistageVehicleViewModel)_viewModelFactory.GetInterimStageVehicleViewModel(consolidatedManufacturingStageInputData?.Vehicle, exempted);
			CurrentView = VehicleViewModel as IViewModelBase;


			Components.Add(VehicleViewModel.Name, VehicleViewModel as IViewModelBase);
			Components.Add("Airdrag", VehicleViewModel.MultistageAirdragViewModel as IViewModelBase);
			Components.Add("Auxiliaries", VehicleViewModel.MultistageAuxiliariesViewModel as IViewModelBase);
		}


		
		private int _stageCount;
		private DigestData _hashPreviousStage;
		private IManufacturingStageInputData _consolidatedManufacturingStageInputData;


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
