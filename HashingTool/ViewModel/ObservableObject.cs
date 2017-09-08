using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using HashingTool.Helper;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel
{
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		protected IOService IoService = new WPFIoService();

		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected bool? IsManufacturerReport(XmlDocument x, Collection<string> errorLog)
		{
			if (x == null || x.DocumentElement == null) {
				return null;
			}
			var valid = x.DocumentElement.LocalName == XMLNames.VectoManufacturerReport;
			if (!valid) {
				errorLog.Add(string.Format("Invalid XML file given ({0}). Expected Manufacturer Report XML ({1})!",
					x.DocumentElement.LocalName, XMLNames.VectoManufacturerReport));
			}
			return valid;
		}

		protected bool? IsCustomerReport(XmlDocument x, Collection<string> errorLog)
		{
			if (x == null || x.DocumentElement == null) {
				return null;
			}
			var valid = x.DocumentElement != null && x.DocumentElement.LocalName == XMLNames.VectoCustomerReport;
			if (!valid) {
				errorLog.Add(string.Format("Invalid XML file given ({0}). Expected Customer Report XML ({1})!",
					x.DocumentElement.LocalName, XMLNames.VectoCustomerReport));
			}
			return valid;
		}

		protected static bool? IsJobFile(XmlDocument x, Collection<string> errorLog)
		{
			if (x == null || x.DocumentElement == null) {
				return null;
			}
			var valid = x.DocumentElement.LocalName == XMLNames.VectoInputDeclaration &&
						x.DocumentElement.FirstChild.LocalName == XMLNames.Component_Vehicle;
			if (!valid) {
				errorLog.Add(string.Format("Invalid XML file given ({0}/{1}). Expected Vehicle XML ({2}/{3})!",
					x.DocumentElement.LocalName, x.DocumentElement.FirstChild.LocalName, XMLNames.VectoInputDeclaration,
					XMLNames.Component_Vehicle));
			}
			return valid;
		}

		protected static bool? IsComponentFile(XmlDocument x, Collection<string> errorLog)
		{
			if (x.DocumentElement == null) {
				return null;
			}

			if (x.DocumentElement.LocalName != XMLNames.VectoInputDeclaration) {
				errorLog.Add(string.Format("Invalid XML file given ({0}). Expected Component XML ({1})!",
					x.DocumentElement.LocalName, XMLNames.VectoInputDeclaration));
			
				return false;
			}

			var localName = x.DocumentElement.FirstChild.LocalName;
			var components = new[] {
				VectoComponents.Engine, VectoComponents.Airdrag, VectoComponents.Angledrive, VectoComponents.Axlegear,
				VectoComponents.Gearbox, VectoComponents.Retarder, VectoComponents.TorqueConverter, VectoComponents.Tyre
			};
			var valid = components.Where(c => c.XMLElementName() == localName).Any();
			if (!valid) {
				errorLog.Add(string.Format("Invalid XML file given ({0}). Expected Component XML ({1})!",
					localName, string.Join(", ", components.Select(c => c.XMLElementName()))));
			}
			return valid;
		}
	}
}
