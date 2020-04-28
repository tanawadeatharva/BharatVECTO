using System.Collections.ObjectModel;
using System.Windows.Input;
using VECTO3GUI.Helper;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IJoblistViewModel : IMainView
	{
		ObservableCollectionEx<JobEntry> Jobs { get; }
		ObservableCollection<MessageEntry> Messages { get; }
		ICommand AddBusJob { get; }
		ICommand AddJob { get; }
		ICommand RemoveJob { get; }
		ICommand RemoveAllJobs { get; }
		ICommand MoveJobUp { get; }
		ICommand MoveJobDown { get; }
		ICommand StartSimulation { get; }
		ICommand EditJob { get; }
		ICommand EditCompletedFile { get; }
		ICommand JobEntrySetActive { get; }
		ICommand CreateNewJob { get; }
		ICommand OpenJob { get; }
		ICommand OpenSettings { get; }
		ICommand ExitMainCommand { get; }
	}
}
