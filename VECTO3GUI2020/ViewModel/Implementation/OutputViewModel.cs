using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Common;

namespace VECTO3GUI2020.ViewModel
{


	public class OutputViewModel : ViewModelBase, IOutputViewModel
	{
		private object _messageLock = new Object();
		private ObservableCollection<MessageEntry> _messages = new ObservableCollection<MessageEntry>();
		private int _progress;

		public ObservableCollection<MessageEntry> Messages
		{
			get
			{
				return _messages;
			}
		}

		public int Progress
		{
			get => _progress;
			set => SetProperty(ref _progress, value);
		}


		public OutputViewModel()
		{
			BindingOperations.EnableCollectionSynchronization(Messages, _messageLock );
		}



	}

	public interface IOutputViewModel : IMainViewModel
	{
		ObservableCollection<MessageEntry> Messages { get; }

		int Progress { get; set; }
	}
}