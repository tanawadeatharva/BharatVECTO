using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using VECTO3GUI2020.Model.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;

namespace VECTO3GUI2020.ViewModel.Interfaces
{
    public interface IJobListViewModel : IMainViewModel
    {
		ObservableCollection<IDocumentViewModel> Jobs { get; }
		ICommand NewManufacturingStageFile { get; }
		Task<IDocumentViewModel> AddJobAsync(string fileName);
	}
}
