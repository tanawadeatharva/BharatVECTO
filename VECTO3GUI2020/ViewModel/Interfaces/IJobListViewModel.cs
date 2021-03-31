using System.Collections.ObjectModel;
using System.Windows.Input;
using VECTO3GUI2020.Model.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;

namespace VECTO3GUI2020.ViewModel.Interfaces
{
    public interface IJobListViewModel : IMainViewModel
    {
        ICommand AddJob { get; }
        ICommand EditJob { get; }
        ObservableCollection<IDocumentViewModel> Jobs { get; }
		ICommand NewManufacturingStageFile { get; }
	}
}
