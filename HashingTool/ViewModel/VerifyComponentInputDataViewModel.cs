using System;
using System.Collections.ObjectModel;
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
	public class VerifyComponentInputDataViewModel : ObservableObject, IMainView
	{
		private string _digestValueComputed;
		private string _digestValueRead;
		private bool _componentDataValid;
		private string _componentType;
		private readonly XMLFile _componentFile;


		public VerifyComponentInputDataViewModel()
		{
			_componentFile = new XMLFile(IoService, true, HashingHelper.IsComponentFile);
			_componentFile.PropertyChanged += ComponentFilechanged;

			// TODO!
			CanonicalizaitionMethods = new ObservableCollection<string>() {
				"urn:vecto:xml:2017:canonicalization",
				"http://www.w3.org/2001/10/xml-exc-c14n#"
			};
		}

		public string Name
		{
			get { return "Verify Component Data"; }
		}

		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}

		public XMLFile ComponentFile
		{
			get { return _componentFile; }
		}

		public string Component
		{
			get { return _componentType; }
			set {
				if (_componentType == value) {
					return;
				}
				_componentType = value;
				RaisePropertyChanged("Component");
			}
		}

		public ObservableCollection<string> CanonicalizaitionMethods { get; private set; }

		public string DigestValueComputed
		{
			get { return _digestValueComputed; }
			set {
				if (_digestValueComputed == value) {
					return;
				}
				_digestValueComputed = value;
				RaisePropertyChanged("DigestValueComputed");
			}
		}

		public string DigestValueRead
		{
			get { return _digestValueRead; }
			set {
				if (_digestValueRead == value) {
					return;
				}
				_digestValueRead = value;
				RaisePropertyChanged("DigestValueRead");
			}
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
			if (e.PropertyName == "Document") {
				DoValidateHash();
			}
		}

		private void DoValidateHash()
		{
			if (_componentFile.ContentValid == null || !_componentFile.ContentValid.Value || _componentFile.Document == null) {
				ComponentDataValid = false;
				DigestValueComputed = "";
				DigestValueRead = "";
				Component = "";
				return;
			}
			try {
				var h = VectoHash.Load(_componentFile.Document);

				if (h.GetContainigComponents().Count != 1) {
					IoService.Messagebox("Selected file is not a component file!", "Error reading XML File", MessageBoxButton.OK);
					throw new InvalidDataException();
				}
				Component = h.GetContainigComponents().First().XMLElementName();

				DigestValueRead = h.ReadHash();
				DigestValueComputed = h.ComputeHash();
				ComponentDataValid = h.ValidateHash();
			} catch (Exception e) {
				ComponentDataValid = false;
				DigestValueComputed = "";
				DigestValueRead = "";
				Component = "";
				_componentFile.XMLValidationErrors.Add(e.Message);
			}
		}
	}
}
