using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Common;

namespace VECTO3GUI2020.ViewModel
{


	public class OutputViewModel : ViewModelBase, IOutputViewModel
	{
		private object _messageLock = new Object();
		private ObservableCollection<string> _messages = new ObservableCollection<string>();

		public ObservableCollection<string> Messages
		{
			get
			{
				return _messages;
			}
		}


		public OutputViewModel()
		{
			BindingOperations.EnableCollectionSynchronization(Messages, _messageLock );
		}

	}

	public interface IOutputViewModel
	{
		ObservableCollection<string> Messages { get; }
	}
}