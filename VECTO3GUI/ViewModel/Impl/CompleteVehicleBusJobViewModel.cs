using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI.ViewModel.Interfaces;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Impl
{
	public class CompleteVehicleBusJobViewModel : AbstractJobViewModel, IJobEditViewModel
	{

		#region Commands

		private ICommand _saveComponentCommand;
		private ICommand _resetComponentCommand;

		#endregion


		public CompleteVehicleBusJobViewModel(IKernel kernel, IDeclarationInputDataProvider inputData)
		{
			Kernel = kernel;
			InputDataProvider = inputData;
			JobViewModel = this;
			CreateComponentModel(Component.CompleteBusVehicle);
			CreateComponentModel(Component.Airdrag);
			CreateComponentModel(Component.Auxiliaries);
			CurrentComponent = GetComponentViewModel(Component.CompleteBusVehicle);
		}

	
		protected override void DoSaveJob()
		{
			throw new NotImplementedException();
		}

		public ICommand SaveComponent
		{
			get { return _saveComponentCommand ?? new RelayCommand<Component>(DoSaveComponent, CanSaveComponent); }
		}

		private bool CanSaveComponent(Component component)
		{
			return ComponentsChanged(component);
		}

		private void DoSaveComponent(Component component)
		{

		}


		public ICommand ResetComponent
		{
			get { return _resetComponentCommand ?? new RelayCommand<Component>(DoResetComponent, CanResetComponent); }
		}


		private bool CanResetComponent(Component component)
		{
			return ComponentsChanged(component);
		}

		private void DoResetComponent(Component component)
		{
			switch (component)
			{
				case Component.CompleteBusVehicle:
					_subModels[Component.CompleteBusVehicle].ResetComponentData();
					break;
				case Component.Airdrag:
					_subModels[Component.Airdrag].ResetComponentData();
					break;
				case Component.Auxiliaries:
					_subModels[Component.Auxiliaries].ResetComponentData();
					break;
			}
		}

		
		private bool ComponentsChanged(Component component)
		{
			switch (component) {
				case Component.CompleteBusVehicle :
					return _subModels[Component.CompleteBusVehicle].IsComponentDataChanged();
				case Component.Airdrag :
					return _subModels[Component.Airdrag].IsComponentDataChanged();
				case Component.Auxiliaries:
					return _subModels[Component.Auxiliaries].IsComponentDataChanged();
				default:
					return false;
			}
		}


		public string JobFile { get; }
		public IInputDataProvider InputDataProvider { get; set; }
	}
}
