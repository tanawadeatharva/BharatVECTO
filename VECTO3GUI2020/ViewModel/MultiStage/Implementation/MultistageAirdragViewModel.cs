using System;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;
using ViewModelBase = VECTO3GUI2020.ViewModel.Implementation.Common.ViewModelBase;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public class MultistageAirdragViewModel : ViewModelBase, IMultistageAirdragViewModel
	{
		private IDialogHelper _dialogHelper;
		private IXMLInputDataReader _inputDataReader;
		private IComponentViewModelFactory _componentViewModelFactory;
		private IAirDragViewModel _airdragViewModel;
		private bool _airdragModified;


		public IAirDragViewModel AirDragViewModel
		{
			get => _airdragViewModel;
			set => SetProperty(ref _airdragViewModel, value);
		}


		#region Commands

		private ICommand _loadAirdragFileCommand;
		private IAirdragDeclarationInputData _consolidatedAirdragInputData;
		private DataSource _dataSource;
		private bool _savedInDeclarationMode;
		private string _manufacturer;
		private string _model;
		private DateTime _date;
		private string _appVersion;
		private CertificationMethod _certificationMethod;
		private string _certificationNumber;
		private DigestData _digestValue;
		private SquareMeter _airDragArea;

		public ICommand LoadAirdragFileCommand
		{
			get => _loadAirdragFileCommand ?? new RelayCommand(LoadAirdragFileCommandExecute, () => true);
		}

		public void LoadAirdragFileCommandExecute()
		{
			var fileName = _dialogHelper.OpenXMLFileDialog(Settings.Default.DefaultFilePath);

			try {
				var xDoc = new XmlDocument();
				xDoc.Load(fileName);

				//_inputDataReader.Create();
				//var xmlNodes = GetXmlNodes(filePath);
				//if (xmlNodes.IsNullOrEmpty())
				//	return;

				//var compReader = new XmlComponentReaderHelper(xmlNodes[0].ParentNode);
				//SetLoadedAirdragData(compReader.GetAirdragComponentData());

			}
			catch (Exception e) {
				_dialogHelper.ShowMessageBox(e.Message, "Invalid File", MessageBoxButton.OK,
					MessageBoxImage.Error);
			}
		}

		#endregion




		public MultistageAirdragViewModel(IDialogHelper dialogHelper, IXMLInputDataReader inputDataReader, IComponentViewModelFactory componentViewModelFactory)
		{
			_dialogHelper = dialogHelper;
			_inputDataReader = inputDataReader;
			_componentViewModelFactory = componentViewModelFactory;
		}

		public MultistageAirdragViewModel(IAirdragDeclarationInputData consolidatedAirdragInputData,
			IDialogHelper dialogHelper,
			IXMLInputDataReader inputDataReader,
			IComponentViewModelFactory componentViewModelFactory) : this(dialogHelper, inputDataReader, componentViewModelFactory)
		{
			_consolidatedAirdragInputData = consolidatedAirdragInputData;
			
		}

		public DataSource DataSource => _dataSource;

		public bool SavedInDeclarationMode => _savedInDeclarationMode;

		public string Manufacturer => _manufacturer;

		public string Model => _model;

		public DateTime Date => _date;

		public string AppVersion => _appVersion;

		public CertificationMethod CertificationMethod => _certificationMethod;

		public string CertificationNumber => _certificationNumber;

		public DigestData DigestValue => _digestValue;

		public SquareMeter AirDragArea => _airDragArea;
		public SquareMeter TransferredAirDragArea { get; }
		public SquareMeter AirDragArea_0 { get; }
	}
}