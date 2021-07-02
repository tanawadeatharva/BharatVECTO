using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Castle.Core.Smtp;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public class StageInputViewModel : StageViewModelBase, IDocumentViewModel, IJobEditViewModel
	{
		private bool _canBeEdited;
		private DataSource _dataSource;
		private readonly XmlDocumentType _documentType;
		private readonly string _documentName;
		private bool _selected;

		public StageInputViewModel(bool exemptedVehicle, IMultiStageViewModelFactory multiStageViewModelFactory) : base(multiStageViewModelFactory)
		{
			_vehicleViewModel = _viewModelFactory.CreateStageInputVehicleViewModel(
				exemptedVehicle
				? InterimStageBusVehicleViewModel_v2_8.VERSION_EXEMPTED
				: InterimStageBusVehicleViewModel_v2_8.VERSION) as IMultistageVehicleViewModel;

			Debug.Assert(_vehicleViewModel != null);
			Title = "Edit Stage Input - New File";
			Init();
		}

		public StageInputViewModel(IDeclarationInputDataProvider inputData, IMultiStageViewModelFactory multiStageViewModelFactory) : base(multiStageViewModelFactory)
		{
			_documentName = inputData.JobInputData.JobName;
			_vehicleViewModel =
				_viewModelFactory.CreateStageInputVehicleViewModel(inputData.JobInputData.Vehicle) as IMultistageVehicleViewModel;
			(_vehicleViewModel as InterimStageBusVehicleViewModel_v2_8).ShowConsolidatedData = false;
			_dataSource = inputData.DataSource;
			_documentType = XmlDocumentType.DeclarationJobData;
			Title = $"Edit Stage Input - {Path.GetFileName(_dataSource.SourceFile)}";

			Init();
		}

		#region Overrides of StageViewModelBase

		protected override bool LoadStageInputData(IDeclarationInputDataProvider inputData)
		{
			base.LoadStageInputData(inputData);
			DataSource = inputData.DataSource;
			return true;
		}

		#endregion

		private void SetTitle()
		{
			Title = "Edit Stage Input - " + ((_dataSource?.SourceFile != null)
				? Path.GetFileName(_dataSource.SourceFile)
				: "New File");
		}

		private void Init()
		{
			SetTitle();
			Components.Add("vehicle", VehicleViewModel as IViewModelBase);
			Components.Add("auxiliaries", VehicleViewModel.MultistageAuxiliariesViewModel as IViewModelBase);
			Components.Add("airdrag", VehicleViewModel.MultistageAirdragViewModel as IViewModelBase);
			CurrentView = VehicleViewModel as IViewModelBase;

			ShowSaveAndCloseButtons = true;
		}

		#region Implementation of IDocumentViewModel

		public string DocumentName => _documentName;

		public XmlDocumentType DocumentType => _documentType;

		public DataSource DataSource
		{
			get => _dataSource;
			set
			{
				SetProperty(ref _dataSource, value);
				SetTitle();
			}
		}

		public IEditViewModel EditViewModel => this;

		public bool Selected
		{
			get => _selected;
			set => SetProperty(ref _selected, value);
		}

		public bool CanBeSimulated
		{
			get => false;
			set => throw new System.NotImplementedException();
		}

		#endregion

		#region Implementation of IEditViewModel

		public string Name => "Edit Stage Input";

		#endregion
	}
}