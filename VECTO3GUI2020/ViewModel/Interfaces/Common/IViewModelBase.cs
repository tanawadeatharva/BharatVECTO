using System.ComponentModel;

namespace VECTO3GUI2020.ViewModel.Interfaces.Common
{
	public interface IViewModelBase : INotifyPropertyChanged
	{
		string Title { get; set; }
	}
}