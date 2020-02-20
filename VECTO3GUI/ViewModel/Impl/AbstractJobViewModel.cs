using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ninject;
using VECTO3.Util;
using VECTO3.ViewModel.Interfaces;

namespace VECTO3.ViewModel.Impl
{
	public abstract class AbstractJobViewModel : AbstractViewModel
	{
		protected bool IsDeclarationMode;
		private IComponentViewModel _currentComponent;

		public override bool DeclarationMode
		{
			get { return IsDeclarationMode; }
		}

		public IComponentViewModel CurrentComponent
		{
			get { return _currentComponent; }
			protected set {
				SetProperty(ref _currentComponent, value);
			}
		}

		public ICommand SaveJob { get { return new RelayCommand(DoSaveJob); } }

		protected abstract void DoSaveJob();

		public ICommand CloseJob { get { return new RelayCommand<UserControl>(DoCloseJob);} }

		protected virtual void DoCloseJob(UserControl ctl)
		{
			Window.GetWindow(ctl).Close();
		}


		public ICommand EditComponent { get { return new RelayCommand<Component>(DoEditComponent); } }

		protected virtual void DoEditComponent(Component component)
		{
			var nextView = GetComponentViewModel(component);

			CurrentComponent = nextView ?? Kernel.Get<INoneViewModel>();

		}

		protected void CreateComponentModel(Component component)
		{
			var viewModelType = ViewModelFactory.ComponentViewModelMapping[component];
			if (!typeof(IComponentViewModel).IsAssignableFrom(viewModelType)) {
				throw new Exception("Invalid entry in ViewModel Mapping");
			}

			var subModels = GetSubmodels().ToArray();
			if (!subModels.Contains(component)) {
				var viewModel = (IComponentViewModel)Kernel.Get(viewModelType);
				RegisterSubmodel(component, viewModel);
			}
		}
	}
}
