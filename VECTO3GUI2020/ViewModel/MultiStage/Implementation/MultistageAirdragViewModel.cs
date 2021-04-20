using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.Utils;
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
		private IComponentViewModelFactory _componentViewModelFactory;
		private IAirDragViewModel _airdragViewModel;
		private bool _airdragModified;

		public string AirdragFilePath
		{
			get => _airdragFilePath;
			set => SetProperty(ref _airdragFilePath, value);
		}

		public IAirDragViewModel AirDragViewModel
		{
			get => _airdragViewModel;
			set => SetProperty(ref _airdragViewModel, value);
		}

		public IAirdragDeclarationInputData ConsolidatedAirdragData
		{
			get => _consolidatedAirdragInputData;
			set => SetProperty(ref _consolidatedAirdragInputData, value);
		}


		#region Commands

		private ICommand _loadAirdragFileCommand;

		private Dictionary<string, string> _validationErrors;

		private IAirdragDeclarationInputData _consolidatedAirdragInputData;
		private readonly IDeclarationInjectFactory _injectFactory;
		private string _airdragFilePath;

		public ICommand LoadAirdragFileCommand
		{
			get => _loadAirdragFileCommand ?? new RelayCommand(LoadAirdragFileCommandExecute, () => true);
		}

		public void LoadAirdragFileCommandExecute()
		{
			var fileName = _dialogHelper.OpenXMLFileDialog(Settings.Default.DefaultFilePath);
			var success = true;
			var errorStringBuilder = new StringBuilder();
			try {
				var xDoc = XDocument.Load(fileName);
				var doc = new XmlDocument();
				doc.Load(fileName);

				var airdragElements = xDoc.Descendants().Where(e => e.Name.LocalName == XMLNames.Component_AirDrag);
				if (airdragElements.Count() == 1) {
					//GET FROM FILE
					var dataProviderVersion = XMLDeclarationAirdragDataProviderV28.QUALIFIED_XSD_TYPE;
					//var dataProviderVersion = XMLDeclarationAirdragDataProviderV28.QUALIFIED_XSD_TYPE;

					XElement airdragElement = airdragElements.First();
					XmlNode airdragNode = airdragElement.ToXmlNode();

					var airDragInputData = _injectFactory.CreateAirdragData(dataProviderVersion, null, airdragNode, fileName);
					AirDragViewModel = _componentViewModelFactory.CreateComponentViewModel(airDragInputData) as IAirDragViewModel;
					success = false;
				} else {
					success = false;
				}
			}
			catch (Exception e) {
				_dialogHelper.ShowMessageBox(e.Message, 
					"Invalid File", 
					MessageBoxButton.OK,
					MessageBoxImage.Error);
				success = false;
			}

			if (success) {
				AirdragFilePath = fileName;
			}
			
		}

		private void ValidationErrorAction(XmlSeverityType arg1, ValidationEvent arg2)
		{
			var xmlException = arg2?.ValidationEventArgs?.Exception as XmlSchemaValidationException;
			if (xmlException != null)
			{
				var message = xmlException.InnerException;
				var sourceObject = xmlException.SourceObject as XmlElement;
				var localName = sourceObject?.LocalName;

				if (sourceObject != null)
					_validationErrors.Add(localName, message?.Message);
			}
		}
		#endregion




		public MultistageAirdragViewModel(IDialogHelper dialogHelper, IDeclarationInjectFactory injectFactory, IComponentViewModelFactory componentViewModelFactory)
		{
			_dialogHelper = dialogHelper;
			_injectFactory = injectFactory;
			_componentViewModelFactory = componentViewModelFactory;
			_airdragFilePath = "Select Airdrag File - if no file is selected a default Airdrag Component is loaded";
		}

		public MultistageAirdragViewModel(IAirdragDeclarationInputData consolidatedAirdragInputData,
			IDialogHelper dialogHelper,
			IDeclarationInjectFactory injectFactory,
			IComponentViewModelFactory componentViewModelFactory) : this(dialogHelper, injectFactory, componentViewModelFactory)
		{
			ConsolidatedAirdragData = consolidatedAirdragInputData;
		}

		public DataSource DataSource => _airdragViewModel.DataSource;

		public bool SavedInDeclarationMode => _airdragViewModel.SavedInDeclarationMode;

		public string Manufacturer => _airdragViewModel.Manufacturer;

		public string Model => _airdragViewModel.Model;

		public DateTime Date => _airdragViewModel.Date;

		public string AppVersion => _airdragViewModel.AppVersion;

		public CertificationMethod CertificationMethod => _airdragViewModel.CertificationMethod;

		public string CertificationNumber => _airdragViewModel.CertificationNumber;

		public DigestData DigestValue => _airdragViewModel.DigestValue;

		public SquareMeter AirDragArea => _airdragViewModel.AirDragArea;

		public SquareMeter TransferredAirDragArea => _airdragViewModel.TransferredAirDragArea;

		public SquareMeter AirDragArea_0 => _airdragViewModel.AirDragArea_0;
	}
}