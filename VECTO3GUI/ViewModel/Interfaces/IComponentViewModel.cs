using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.ViewModel.Interfaces {
	public interface IComponentViewModel
	{
		IJobEditViewModel JobViewModel { set; }

		bool DeclarationMode { get; }
		

		ObservableCollection<Component> Components { get; }
		IComponentViewModel ParentViewModel { get; set; }

		IComponentViewModel GetComponentViewModel(Component component);

		bool AnyDataChanges();
	}
}