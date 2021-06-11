using System.Windows.Input;

namespace VECTO3GUI2020.ViewModel.Interfaces
{
    public interface IMainWindowViewModel
    {
        IMainViewModel CurrentViewModelTop { get; set; }
        IMainViewModel CurrentViewModelBottom { get; set; }

        #region Commands
        ICommand OpenSettings { get;}
        #endregion
    }
}
