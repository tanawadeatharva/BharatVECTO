using System.Collections.ObjectModel;
using System.Windows.Input;
using VECTO3.ViewModel.Impl;

namespace VECTO3.ViewModel.Interfaces
{
	public interface IJoblistViewModel : IMainView
	{
		ObservableCollection<JobEntry> Jobs { get; }
		ICommand AddJob { get; }
		ICommand RemoveJob { get; }
		ICommand MoveJobUp { get; }
		ICommand MoveJobDown { get; }
		ICommand StartSimulation { get; }
		ICommand EditJob { get; }
		ICommand JobEntrySetActive { get; }
	}
}
