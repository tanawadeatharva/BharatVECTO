using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using VECTO3.Util;
using VECTO3.ViewModel.Impl;

namespace VECTO3.ViewModel.Interfaces {
	public interface IComponentViewModel
	{
		IJobEditViewModel JobViewModel { set; }

		bool DeclarationMode { get; }
		

		ObservableCollection<Component> Components { get; }
		IComponentViewModel ParentViewModel { get; set; }

		IComponentViewModel GetComponentViewModel(Component component);
	}
}