using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationJobs;
using VECTO3GUI.Helper;
using VECTO3GUI.ViewModel.Interfaces;
using VECTO3GUI.Util;
using VECTO3GUI.Util.XML;


namespace VECTO3GUI.ViewModel.Impl
{
	public class CompleteVehicleBusJobViewModel : AbstractJobViewModel, IJobEditViewModel
	{
		#region Members

		private readonly XMLCompletedBus _xmlCompletedBus;
		private readonly XMLCompletedBusWriter _xmlCompletedBusWriter;

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
			IsNewJob = inputData == null;
			JobViewModel = this;
			CreateComponentModel(Component.CompleteBusVehicle);
			CreateComponentModel(Component.Airdrag);
			CreateComponentModel(Component.Auxiliaries);
			CurrentComponent = GetComponentViewModel(Component.CompleteBusVehicle);

			SetXmlFilePath(inputData?.JobInputData.Vehicle.XMLSource.BaseURI);

			_xmlCompletedBus = new XMLCompletedBus();
			_xmlCompletedBusWriter = new XMLCompletedBusWriter();
		}


		#region Commands

		protected override bool CanSaveJob(Window window)
		{
			return !IsNewJob;
		}
		protected override void DoSaveJob(Window window)
		{
			var dialogSettings = new MetroDialogSettings()
			{
				AffirmativeButtonText = "Yes",
				NegativeButtonText = "Cancel",
				AnimateShow = true,
				AnimateHide = true
			};
			
			var dialogResult = MetroDialogHelper.GetModalDialogBox(this, "Save",
				"The existing file will be overwritten, do you want to continue?", 
				MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
			
			if (dialogResult == MessageDialogResult.Affirmative) {
				SetCurrentDataToSave();
				var xDoc = _xmlCompletedBus.GenerateCompletedBusDocument(CompleteVehicleBusData);

				if (XmlHelper.ValidateXDocument(xDoc)) {
					_xmlCompletedBusWriter.WriteCompletedBusXml(XmlFilePath, xDoc);
					CloseWindow(window);
				}
			}
		}

		protected override void DoCloseJob(Window window)
		{
			if (CloseWindowDialog()) {
				CloseWindow(window);
			}
		}

		protected override void DoSaveToJob(Window window)
		{
			var filePath = FileDialogHelper.SaveXmlFileToDialog(SettingsModel.XmlFilePathFolder);

			SetCurrentDataToSave();
			var xDocument = _xmlCompletedBus.GenerateCompletedBusDocument(CompleteVehicleBusData);

			if (XmlHelper.ValidateXDocument(xDocument)) {
				_xmlCompletedBusWriter.WriteCompletedBusXml(filePath, xDocument);
				CloseWindow(window);
			}
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
		}private void DoResetComponent(Component component)
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

		private void SetCurrentDataToSave()
		{
			CompleteVehicleBusData = new Dictionary<Component, object> {
				{ Component.CompleteBusVehicle, _subModels[Component.CompleteBusVehicle].SaveComponentData()},
				{ Component.Airdrag, _subModels[Component.Airdrag].SaveComponentData()},
				{ Component.Auxiliaries, _subModels[Component.Auxiliaries].SaveComponentData()}
			};
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

		private void CloseWindow(Window window)
		{
			WindowAlreadyClosed = true;
			window?.Close();
		}


		public string JobFile { get; }
		public IInputDataProvider InputDataProvider { get; set; }
	}
}
