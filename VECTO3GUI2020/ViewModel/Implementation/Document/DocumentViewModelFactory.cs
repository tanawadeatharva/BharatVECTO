using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.Views.Multistage;

namespace VECTO3GUI2020.ViewModel.Implementation.Document
{
	public class DocumentViewModelFactory : IDocumentViewModelFactory
	{
		private readonly IMultiStepInputViewModelFactory _multistepInputFactory;
		private readonly IDeclarationInputViewModelFactory _declarationInputViewModelFactory;
		private readonly INewDocumentViewModelFactory _newDocumentViewModelFactory;
		private readonly IPrimaryAndStageInputViewModelFactory _primaryAndStageViewModelFactory;
		private readonly ICreateVifViewModelFactory _createVifViewModelFactory;


		public DocumentViewModelFactory(
			IMultiStepInputViewModelFactory multiStepInputFactory, 
			IDeclarationInputViewModelFactory declarationInputViewModelFactory, 
			INewDocumentViewModelFactory newDocumentViewModelFactory,
			IPrimaryAndStageInputViewModelFactory primaryAndStageFactory,
			ICreateVifViewModelFactory createVifViewModelFactory)
		{
			_multistepInputFactory = multiStepInputFactory;
			_declarationInputViewModelFactory = declarationInputViewModelFactory;
			_newDocumentViewModelFactory = newDocumentViewModelFactory;
			_primaryAndStageViewModelFactory = primaryAndStageFactory;
			_createVifViewModelFactory = createVifViewModelFactory;
		}


		public IDocumentViewModel CreateDocumentViewModel(IInputDataProvider inputData)
		{
			switch (inputData)
			{
				case IMultistepBusInputDataProvider multiStep:
					return CreateMultistageViewModel(multiStep);
				case IMultistageVIFInputData createVifInput:
					return CreateVifViewModel(createVifInput);
				case IMultistagePrimaryAndStageInputDataProvider primaryAndStep:
					return CreatePrimaryAndStageDocumentViewModel(primaryAndStep);
				case IDeclarationInputDataProvider declarationInputDataProvider:
					return CreateDeclarationInputDocumentViewModel(declarationInputDataProvider);
			}
			throw new Exception($"{inputData.DataSource.SourceFile} could not be opened");

		}
		/// <summary>
		/// Attempts the creation of a viewmodel for the provided input data, in case no matching binding is found, a <see cref="SimulationOnlyDeclarationJob"/> is created, to allow simulation but no editing
		/// </summary>
		/// <param name="declarationInputDataProvider"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		private IDocumentViewModel CreateDeclarationInputDocumentViewModel(IDeclarationInputDataProvider declarationInputDataProvider)
		{
			return _declarationInputViewModelFactory.CreateDeclarationViewModel(declarationInputDataProvider);
		}

		private IDocumentViewModel CreatePrimaryAndStageDocumentViewModel(IMultistagePrimaryAndStageInputDataProvider primaryAndStep)
		{
			return _primaryAndStageViewModelFactory.CreateDeclarationViewModel(primaryAndStep);
		}

		private IDocumentViewModel CreateMultistageViewModel(IMultistepBusInputDataProvider inputData)
		{
			return _multistepInputFactory.CreateMultistepViewModel(inputData);
		}



        /// <summary>
        /// Viewmodel to create VIF out of VIF + step input
        /// </summary>
        /// <returns></returns>
        private IDocumentViewModel CreateVifViewModel(IMultistageVIFInputData createVif)
		{
			return _createVifViewModelFactory.CreateVifViewModel(createVif);
		}

		public IDocumentViewModel GetCreateNewStepInputViewModel(bool exemptedVehicle)
		{
			return _newDocumentViewModelFactory.GetCreateNewStepInputViewModel(exemptedVehicle);
		}

		public IDocumentViewModel GetCreateNewVifViewModel(bool completed)
		{
			return _newDocumentViewModelFactory.GetCreateNewVifViewModel(completed);
		}

		//public IDocumentViewModel GetCreateNewVifViewModel()
		//{
		//	return _newDocumentViewModelFactory.GetCreateNewVifViewModel();
		//}
	}
}