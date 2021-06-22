using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Shell.Interop;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Common;

namespace VECTO3GUI2020.ViewModel
{

	public class OutputViewModel : ViewModelBase, IOutputViewModel
	{
		#region MembersAndProperties
		private object _messageLock = new Object();
		private ObservableCollection<MessageEntry> _messages = new ObservableCollection<MessageEntry>();
		private int _progress;
		private string _statusMessage;
		private ICommand _openFolderCommand;
		private ICommand _openFileCommand;

		public ObservableCollection<MessageEntry> Messages
		{
			get { return _messages; }
		}



		public int Progress
		{
			get => _progress;
			set => SetProperty(ref _progress, value);
		}

		public string StatusMessage
		{
			get { return _statusMessage; }
			set { SetProperty(ref _statusMessage, value); }
		}



		#endregion

		public void AddMessage(MessageEntry messageEntry)
		{
			lock (_messageLock)
			{
				Messages.Add(messageEntry);
			}
		}


		public OutputViewModel()
		{
			BindingOperations.EnableCollectionSynchronization(Messages, _messageLock);
		}


		#region Commands

		// ReSharper disable once UnusedMember.Global
		public ICommand OpenFolderCommand =>
			_openFolderCommand ?? (_openFolderCommand = new RelayCommand<string>(
				OpenFolderExecute));

		// ReSharper disable once UnusedMember.Global
		public ICommand OpenFileCommand =>
			_openFileCommand ?? (_openFileCommand = new RelayCommand<string>(
				OpenFileExecute));

		private void OpenFolderExecute(string link)
		{
			if (link == null) {
				return;
			}

			link = Path.GetFullPath(link);

			var explorerCommandStrBuilder = new StringBuilder();
			explorerCommandStrBuilder.Append("explorer.exe");
			explorerCommandStrBuilder.Append(" /select ");
			explorerCommandStrBuilder.Append(link);

			//var directoryPath = Path.GetDirectoryName(link);
			//StartProcess(directoryPath);

			StartProcess("explorer.exe", ("/select," + link));
		}

		private void OpenFileExecute(string link){
			if (link == null) {
				return;
			}

			StartProcess(link);

		
		}

		private void StartProcess(string command, params string[]arguments)
		{
			string argumentsString = "";
			if (arguments != null) {
				var argumentsStrBuilder = new StringBuilder();
				foreach (var argument in arguments) {
					argumentsStrBuilder.Append(argument);
					if(argument != arguments.Last()) {
						argumentsStrBuilder.Append(" ");
					}
				}

				argumentsString = argumentsStrBuilder.ToString();
				Debug.WriteLine(argumentsString);
			}
			

			try
			{
				Process.Start(command, argumentsString );
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}

		#endregion
	}




	public interface IOutputViewModel : IMainViewModel
	{
		//ObservableCollection<MessageEntry> Messages { get; }

		int Progress { get; set; }
		string StatusMessage { get; set; }
		void AddMessage(MessageEntry messageEntry);
	}
}