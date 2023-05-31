using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.ViewModel.Interfaces.Document
{
    public interface IDocumentViewModelFactory : INewDocumentViewModelFactory
    {
		/*
	public enum XmlDocumentType
	{
		DeclarationJobData = 1 << 1,
		PrimaryVehicleBusOutputData = 1 << 2,
		DeclarationComponentData = 1 << 3,
		EngineeringJobData = 1 << 4,
		EngineeringComponentData = 1 << 5,
		ManufacturerReport = 1 << 6,
		CustomerReport = 1 << 7,
		MonitoringReport = 1 << 8,
		VTPReport = 1 << 9,
		DeclarationTrailerJobData = 1 << 10,
	}

		*/
		IDocumentViewModel CreateDocumentViewModel(IInputDataProvider declarationInput);
	}


	public interface IMultiStepInputViewModelFactory
	{
		IDocumentViewModel CreateMultistepViewModel(IMultistepBusInputDataProvider inputData);
	}

	public interface ICreateVifViewModelFactory
	{
		IDocumentViewModel CreateVifViewModel(IMultistageVIFInputData multistep);
	}

	public interface IDeclarationInputViewModelFactory
	{
		IDocumentViewModel CreateDeclarationViewModel(IDeclarationInputDataProvider inputData);
	}

	public interface IPrimaryAndStageInputViewModelFactory
	{
		IDocumentViewModel CreateDeclarationViewModel(IMultistagePrimaryAndStageInputDataProvider inputData);
	}



	public interface INewDocumentViewModelFactory
	{
		IDocumentViewModel GetCreateNewStepInputViewModel(bool exemptedVehicle);
		IDocumentViewModel GetCreateNewVifViewModel(bool completed);
		//IDocumentViewModel GetCreateNewVifViewModel();
		

    }
}
