using System.ComponentModel;
using System.Windows;

namespace VECTO3GUI2020.ViewModel.Interfaces.Common
{
	public interface IViewModelBase : INotifyPropertyChanged
	{
		string Title { get; set; }
		double? Width { get; set; }
		double? Height { get; set; }
		SizeToContent SizeToContent { get; set; }
	}
}