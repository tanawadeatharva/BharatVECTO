using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationJobs;
using VECTO3GUI.Model.TempDataObject;
using VECTO3GUI.ViewModel.Interfaces;
using VECTO3GUI.Util;
using VECTO3GUI.Util.XML;

namespace VECTO3GUI.ViewModel.Impl
{
	public class CompleteVehicleBusJobViewModel : AbstractJobViewModel, IJobEditViewModel
	{
		#region Members
		
		private ICommand _saveComponentCommand;
		private ICommand _resetComponentCommand;

		#endregion

		#region Properties

		public Dictionary<Component, object> CompleteVehicleBusData { get; private set; }

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


		#region Commands

		



		
		protected override void DoSaveJob()
		{
			CompleteVehicleBusData = new Dictionary<Component, object> {
				{ Component.CompleteBusVehicle, _subModels[Component.CompleteBusVehicle].SaveComponentData()},
				{ Component.Airdrag, _subModels[Component.Airdrag].SaveComponentData()},
				{ Component.Auxiliaries, _subModels[Component.Auxiliaries].SaveComponentData()}
			};


			//var completedXml = new XMLCompletedBus();
			//var xmlDoc =  completedXml.GenerateCompletedBusDocument(CompleteVehicleBusData);

			//var writer = new XMLCompletedBusWriter();
			//writer.WriteCompletedBusXml(filePath, xmlDoc);
		}

		protected override void DoCloseJob(Window window)
		{
			window?.Close();
		}

		protected override void DoSaveToJob()
		{
			
		}

		public ICommand SaveComponent
		{
			get { return _saveComponentCommand ??
						(_saveComponentCommand = new RelayCommand<Component>(DoSaveComponent, CanSaveComponent)); }
		}

		private bool CanSaveComponent(Component component)
		{
			return ComponentsChanged(component);
		}

		private void DoSaveComponent(Component component)
		{
			switch (component) {
				case Component.CompleteBusVehicle:
					_subModels[Component.CompleteBusVehicle].SaveComponentData();
					break;
				case Component.Airdrag:
					_subModels[Component.Airdrag].SaveComponentData();
					break;
				case Component.Auxiliaries:
					_subModels[Component.Auxiliaries].SaveComponentData();
					break;
			}
		}


		public ICommand ResetComponent
		{
			get { return _resetComponentCommand ??
						(_resetComponentCommand = new RelayCommand<Component>(DoResetComponent, CanResetComponent)); }
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

		#endregion


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
