using System;
using System.Collections.Generic;
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
		private string _statusMessage;

		public ObservableCollection<MessageEntry> Messages
		{
			get
			{
				return _messages;
			}
		}

		public double SumProgress
		{
			get => _sumProgress;
			set => SetProperty(ref _sumProgress, value);
		}

		public string StatusMessage
		{
			get { return _statusMessage; }
			set { SetProperty(ref _statusMessage, value); }
		}


		public OutputViewModel()
		{
			BindingOperations.EnableCollectionSynchronization(Messages, _messageLock );
		}

		public void SetProgress(double sumProgress, IList<double> subProgress)
		{
			SumProgress = sumProgress;

			SubProgress = subProgress;

		}

	public interface IOutputViewModel : IMainViewModel
	{
		ObservableCollection<MessageEntry> Messages { get; }

		int Progress { get; set; }
		string StatusMessage { get; set; }
	}
}