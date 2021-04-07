using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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


        //borrowed from Franz Josef Kober
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
	}
}
