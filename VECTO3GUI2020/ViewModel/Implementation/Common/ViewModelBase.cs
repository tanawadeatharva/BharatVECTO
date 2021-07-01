using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using Ninject;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Interfaces.Common;

namespace VECTO3GUI2020.ViewModel.Implementation.Common
{
    /// <summary>
    /// Base Implementation of INotifyPropertyChanged
    /// https://docs.microsoft.com/en-us/dotnet/framework/wpf/data/how-to-implement-property-change-notification
    /// </summary>
    /// 
    public class ViewModelBase : INotifyPropertyChanged, IViewModelBase
	{
		private string _error;
		public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Needs to be called when a Property is changed
        /// </summary>
        ///
        /// <param name="name">Is automatically set to CallerMemberName</param>
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}


        //borrowed from Franz Kober
        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            bool propertyChanged = false;

            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                propertyChanged = true;
            }
            return propertyChanged;
        }

		public virtual string Title { get; set; } = "No Title Set";

        [Inject]
        public IDialogHelper DialogHelper { get; set; }

		protected bool AskForConfirmationOnClose { get; set; } = false;

		private ICommand _closeWindowCommand;
		public ICommand CloseWindowCommand
		{
			get
			{
				return _closeWindowCommand ?? new RelayCommand<Window>(window => CloseWindow(window, DialogHelper, AskForConfirmationOnClose), window => true);
			}
		}


        protected void CloseWindow(Window window, IDialogHelper dialogHelper, bool showDialog = true)
		{
			MessageBoxResult result;
			if (showDialog) {
				result = dialogHelper.ShowMessageBox("Do you really want to close?", "Close", MessageBoxButton.YesNo,
					MessageBoxImage.Question);
            } else {
				result = MessageBoxResult.Yes;
			}
			

			if (result == MessageBoxResult.Yes) {
				window?.Close();
			}
		}
	}
}
