using TUGraz.VectoCommon.InputData;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.Ninject
{
	public class MultiStageViewModelFactory : IMultiStageViewModelFactory	
	{
		private IMultiStageViewModelFactoryDefaultInstanceProvider _multiStageVmFactoryDefaultInstanceProvider;
		private IMultiStageViewModelFactoryTypeAsNameInstanceProvider _multiStageViewModelFactoryImplementation;


		public MultiStageViewModelFactory(IMultiStageViewModelFactoryDefaultInstanceProvider multiStageVmFactoryDefaultInstanceProvider, IMultiStageViewModelFactoryTypeAsNameInstanceProvider multiStageViewModelFactoryImplementation1)
		{
			_multiStageVmFactoryDefaultInstanceProvider = multiStageVmFactoryDefaultInstanceProvider;
			_multiStageViewModelFactoryImplementation = multiStageViewModelFactoryImplementation1;
		}

		#region Implementation of IMultiStageViewModelFactoryDefaultInstanceProvider

		public IViewModelBase GetNewMultistageJobViewModel()
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetNewMultistageJobViewModel();
		}

		public IMultiStageJobViewModel GetMultiStageJobViewModel(IMultistageBusInputDataProvider inputData)
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetMultiStageJobViewModel(inputData);
		}

		public IVehicleViewModel GetInterimStageVehicleViewModel()
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetInterimStageVehicleViewModel();
		}

		public IVehicleViewModel GetInterimStageVehicleViewModel(IVehicleDeclarationInputData consolidatedVehicleData, bool exempted)
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetInterimStageVehicleViewModel(consolidatedVehicleData, exempted);
		}

		public IManufacturingStageViewModel GetManufacturingStageViewModel(
			IManufacturingStageInputData consolidatedManufacturingStageInputData, bool exempted)
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetManufacturingStageViewModel(consolidatedManufacturingStageInputData, exempted);
		}

		public IMultistageAirdragViewModel GetMultistageAirdragViewModel()
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetMultistageAirdragViewModel();
		}

		public IMultistageAirdragViewModel GetMultistageAirdragViewModel(IAirdragDeclarationInputData consolidatedAirdragInputData)
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetMultistageAirdragViewModel(consolidatedAirdragInputData);
		}

		public IMultistageAuxiliariesViewModel GetAuxiliariesViewModel()
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetAuxiliariesViewModel();
		}

		public IMultistageAuxiliariesViewModel
			GetAuxiliariesViewModel(IBusAuxiliariesDeclarationData consolidatedAuxiliariesInputData)
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetAuxiliariesViewModel(consolidatedAuxiliariesInputData);
		}

		public ICreateVifViewModel GetCreateVifViewModel()
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetCreateVifViewModel();
		}

		#endregion

		#region Implementation of IMultiStageViewModelFactoryTypeAsNameInstanceProvider

		public IDocumentViewModel CreateDocumentViewModel(IDeclarationInputDataProvider inputData)
		{
			return _multiStageViewModelFactoryImplementation.CreateDocumentViewModel(inputData);
		}

		public IVehicleViewModel CreateStageInputVehicleViewModel(IVehicleDeclarationInputData inputData)
		{
			return _multiStageViewModelFactoryImplementation.CreateStageInputVehicleViewModel(inputData);
		}

		#endregion
	}
}