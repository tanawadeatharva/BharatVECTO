using Ninject;

namespace VECTO3GUI.ViewModel.Impl
{
	public class OutputWindowViewModel : ValidatingViewModel
	{
		private object _viewModel;
		
		public object ViewModel
		{
			get { return _viewModel; }
			set { SetProperty(ref _viewModel, value); }
		}
		
		public OutputWindowViewModel(IKernel kernel, object viewModel)
		{
			Kernel = kernel;
			_viewModel = viewModel;
		}
	}
}
