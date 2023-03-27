using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Implementation.Document;
using VECTO3GUI2020.ViewModel.Interfaces;
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

		private IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider
			_multistageViewModelFactoryFirstParameterAsNameInstanceProvider;

		private readonly IDocumentViewModelFactory _documentViewModelFactory;


		public MultiStageViewModelFactory(
			IMultiStageViewModelFactoryDefaultInstanceProvider multiStageVmFactoryDefaultInstanceProvider,
			IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider multistageViewModelFactoryFirstParameterAsNameInstanceProvider,
			IDocumentViewModelFactory documentViewModelFactory)
		{
			_multiStageVmFactoryDefaultInstanceProvider = multiStageVmFactoryDefaultInstanceProvider;
			_multistageViewModelFactoryFirstParameterAsNameInstanceProvider = multistageViewModelFactoryFirstParameterAsNameInstanceProvider;

			_documentViewModelFactory = documentViewModelFactory;
		}

		#region Implementation of IMultiStageViewModelFactoryDefaultInstanceProvider

		public IViewModelBase GetNewMultistageJobViewModel()
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetNewMultistageJobViewModel();
		}

		public IMultiStageJobViewModel GetMultiStageJobViewModel(IMultistepBusInputDataProvider inputData)
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

		//public ICreateVifViewModel GetCreateNewVifViewModel(bool completed)
		//{
		//	return _multiStageVmFactoryDefaultInstanceProvider.GetCreateNewVifViewModel(completed);
		//}

		//public ICreateVifViewModel GetCreateNewVifViewModel()
		//{
		//	return _multiStageVmFactoryDefaultInstanceProvider.GetCreateNewVifViewModel();
		//}

		#endregion

		



		#region Implementation of IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider

		public IVehicleViewModel CreateStageInputVehicleViewModel(string inputProviderType)
		{
			return _multistageViewModelFactoryFirstParameterAsNameInstanceProvider.CreateStageInputVehicleViewModel(inputProviderType);
		}

		#endregion

		#region Implementation of IDocumentViewModelFactory

		public IDocumentViewModel CreateDocumentViewModel(IInputDataProvider declarationInput)
		{
			return _documentViewModelFactory.CreateDocumentViewModel(declarationInput);
		}

		public IDocumentViewModel GetCreateNewStepInputViewModel(bool exemptedVehicle)
		{
			return _documentViewModelFactory.GetCreateNewStepInputViewModel(exemptedVehicle);
		}

		public IDocumentViewModel GetCreateNewVifViewModel(bool completed)
		{
			return _documentViewModelFactory.GetCreateNewVifViewModel(completed);
		}

		#endregion

	}
}