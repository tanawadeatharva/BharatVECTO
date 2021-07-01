using System.Diagnostics;
using System.IO;
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
		private readonly DataSource _dataSource;
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
			Title = $"Edit Stage Input - {Path.GetFileNameWithoutExtension(_dataSource.SourceFile)}";

			Init();
		}

		private void Init()
		{
			Components.Add("vehicle", VehicleViewModel as IViewModelBase);
			Components.Add("auxiliaries", VehicleViewModel.MultistageAuxiliariesViewModel as IViewModelBase);
			Components.Add("airdrag", VehicleViewModel.MultistageAirdragViewModel as IViewModelBase);
			CurrentView = VehicleViewModel as IViewModelBase;

			ShowSaveAndCloseButtons = true;
		}

		#region Implementation of IDocumentViewModel

		public string DocumentName => _documentName;

		public XmlDocumentType DocumentType => _documentType;

		public DataSource DataSource => _dataSource;

		public IEditViewModel EditViewModel => this;

		public bool Selected
		{
			get => _selected;
			set => SetProperty(ref _selected, value);
		}

		#endregion

		#region Implementation of IEditViewModel

		public string Name => "Edit Stage Input";

		#endregion
	}
}