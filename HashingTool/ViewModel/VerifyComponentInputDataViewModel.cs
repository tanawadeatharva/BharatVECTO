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
		private bool _componentDataValid;

		public VerifyComponentInputDataViewModel()
			: base("Verify Component Data", HashingHelper.IsComponentFile)
		{
			_xmlFile.PropertyChanged += ComponentFilechanged;
		}

		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}


		public bool ComponentDataValid
		{
			get { return _componentDataValid; }
			set {
				if (_componentDataValid == value) {
					return;
				}
				_componentDataValid = value;
				RaisePropertyChanged("ComponentDataValid");
			}
		}

		private void ComponentFilechanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "UPDATED") {
				DoValidateHash();
			}
		}

		private void DoValidateHash()
		{
			if (_xmlFile.ContentValid == null || !_xmlFile.ContentValid.Value || _xmlFile.Document == null) {
				ComponentDataValid = false;
				DigestValueComputed = "";
				DigestValueRead = "";
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

				DigestValueRead = h.ReadHash();
				DigestValueComputed = h.ComputeHash();
				ComponentDataValid = h.ValidateHash();
				DigestMethod = h.GetDigestMethod();
				SetCanonicalizationMethod(h.GetCanonicalizationMethods());
				
			} catch (Exception e) {
				ComponentDataValid = false;
				DigestValueComputed = "";
				DigestValueRead = "";
				Component = "";
				SetCanonicalizationMethod(new string[] { });
				DigestMethod = "";
				_xmlFile.XMLValidationErrors.Add(e.Message);
			}
		}

	}
}
