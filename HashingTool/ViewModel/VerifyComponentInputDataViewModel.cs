using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using HashingTool.Helper;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel
{
	public class VerifyComponentInputDataViewModel : HashedXMLFile, IMainView
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
			if (e.PropertyName == "Document" || e.PropertyName == "ContentValid") {
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
				CanonicalizationMethods.Clear();
				foreach (var c in h.GetCanonicalizationMethods().ToArray()) {
					CanonicalizationMethods.Add(c);
				}
			} catch (Exception e) {
				ComponentDataValid = false;
				DigestValueComputed = "";
				DigestValueRead = "";
				Component = "";
				CanonicalizationMethods.Clear();
				DigestMethod = "";
				_xmlFile.XMLValidationErrors.Add(e.Message);
			}
		}
	}
}
