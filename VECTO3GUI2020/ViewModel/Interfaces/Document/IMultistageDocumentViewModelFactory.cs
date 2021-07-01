using TUGraz.VectoCommon.InputData;

namespace VECTO3GUI2020.ViewModel.Interfaces.Document
{
	public interface IMultistageDocumentViewModelFactory
	{
		IDocumentViewModel CreateDocumentViewModel(IDeclarationInputDataProvider inputData);
	}
}