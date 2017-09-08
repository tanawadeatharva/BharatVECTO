using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using HashingTool.ViewModel.UserControl;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel
{
	public class VerifyJobInputDataViewModel : ObservableObject, IMainView
	{
		private string _digestValueComputed;
		private bool _componentDataValid;
		private readonly XMLFile _jobFile;


		public VerifyJobInputDataViewModel()
		{
			_jobFile = new XMLFile(IoService, true, IsJobFile);
			_jobFile.PropertyChanged += JobFilechanged;

			// TODO!
			CanonicalizationMethods = new ObservableCollection<string>() {
				"urn:vecto:xml:2017:canonicalization",
				"http://www.w3.org/2001/10/xml-exc-c14n#"
			};
			Components = new ObservableCollection<ComponentEntry>();
		}

		
		public string Name
		{
			get { return "Verify VECTO Job"; }
		}

		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}

		public XMLFile JobFile
		{
			get { return _jobFile; }
		}

		public ObservableCollection<ComponentEntry> Components { get; private set; }

		public ObservableCollection<string> CanonicalizationMethods { get; private set; }

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

		public bool JobDataValid
		{
			get { return _componentDataValid; }
			set {
				if (_componentDataValid == value) {
					return;
				}
				_componentDataValid = value;
				RaisePropertyChanged("JobDataValid");
			}
		}

		private void JobFilechanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Document") {
				DoValidateHash();
			}
		}

		private void DoValidateHash()
		{
			if (_jobFile.ContentValid == null || !_jobFile.ContentValid.Value) {
				return;
			}
			try {
				Components.Clear();
				var h = VectoHash.Load(_jobFile.Document);
				var allValid = true;
				var components = h.GetContainigComponents().GroupBy(s => s)
					.Select(g => new { Entry = g.Key, Count = g.Count() });
				foreach (var component in components) {
					if (component.Entry == VectoComponents.Vehicle) {
						continue;
					}
					for (var i = 0; i < component.Count; i++) {
						var entry = new ComponentEntry();
						entry.Component = component.Count == 1
							? component.Entry.XMLElementName()
							: string.Format("{0} ({1})", component.Entry.XMLElementName(), i + 1);
						entry.Valid = h.ValidateHash(component.Entry, i);
						entry.CanonicalizationMethod = new[] {
							"urn:vecto:xml:2017:canonicalization",
							"http://www.w3.org/2001/10/xml-exc-c14n#"
						};
						entry.DigestValueRead = h.ReadHash(component.Entry, i);
						entry.DigestValueComputed = h.ComputeHash(component.Entry, i);
						Components.Add(entry);
						allValid &= entry.Valid;
					}
				}

				DigestValueComputed = h.ComputeHash();
				JobDataValid = allValid;
			} catch (Exception e) {
				DigestValueComputed = "";
				JobDataValid = false;
				_jobFile.XMLValidationErrors.Add(e.Message);
			}
		}
	}

	public class ComponentEntry
	{
		public string Component { get; set; }
		public string DigestValueRead { get; set; }
		public string DigestValueComputed { get; set; }
		public string[] CanonicalizationMethod { get; set; }
		public bool Valid { get; set; }
	}
}
