using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using HashingTool.Helper;
using HashingTool.ViewModel.UserControl;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel
{
	public class VerifyComponentInputDataViewModel : HashedXMLFile, IMainView, INotifyPropertyChanged
	{
		private string _certificationNumber;
		//private bool _componentDataValid;

		public VerifyComponentInputDataViewModel()
			: base("Verify Component Data", HashingHelper.IsComponentFile, HashingHelper.ValidateDocumentHash)
		{
			_xmlFile.PropertyChanged += ComponentFilechanged;
		}

		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}

		public string CertificationNumber
		{
			get { return _certificationNumber; }
			private set {
				if (_certificationNumber == value)
					return;
				_certificationNumber = value;
				RaisePropertyChanged("CertificationNumber");
			}
		}

		private void ComponentFilechanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == GeneralUpdate) {
				ReadComponentName();
			}
		}

		private void ReadComponentName()
		{
			if (_xmlFile.ContentValid == null || !_xmlFile.ContentValid.Value || _xmlFile.Document == null) {
				Component = "";
				return;
			}
			try {
				var h = VectoHash.Load(_xmlFile.Document);

				if (h.GetContainigComponents().Count != 1) {
					IoService.Messagebox("Selected file is not a component file!", "Error reading XML File", MessageBoxButton.OK);
					throw new InvalidDataException();
				}
				Component = h.GetContainigComponents().First().XMLElementName();
				CertificationNumber = h.GetCertificationNumber(h.GetContainigComponents().First(), 0);
			} catch (Exception e) {
				Component = "";
				_xmlFile.LogError(e.Message);
			}
		}
	}
}
