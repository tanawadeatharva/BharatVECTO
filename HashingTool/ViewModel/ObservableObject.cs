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

		protected bool? IsManufacturerReport(XmlDocument x)
		{
			if (x == null || x.DocumentElement == null)
				return null;
			return  x.DocumentElement.LocalName == XMLNames.VectoManufacturerReport;
		}

		protected bool? IsCustomerReport(XmlDocument x)
		{
			if (x == null || x.DocumentElement == null)
				return null;
			return x.DocumentElement != null && x.DocumentElement.LocalName == XMLNames.VectoCustomerReport;
		}

		protected static bool? IsJobFile(XmlDocument x)
		{
			if (x == null)
				return null;
			return x.DocumentElement != null && x.DocumentElement.LocalName == XMLNames.VectoInputDeclaration &&
					x.DocumentElement.FirstChild.LocalName == XMLNames.Component_Vehicle;
		}

		protected static bool? IsComponentFile(XmlDocument x)
		{
			if (x.DocumentElement == null) {
				return null;
			}

			if (x.DocumentElement.LocalName != XMLNames.VectoInputDeclaration) {
				return false;
			}

			var localName = x.DocumentElement.FirstChild.LocalName;
			var components = new[] {
				VectoComponents.Engine, VectoComponents.Airdrag, VectoComponents.Angledrive, VectoComponents.Axlegear,
				VectoComponents.Gearbox, VectoComponents.Retarder, VectoComponents.TorqueConverter, VectoComponents.Tyre
			};
			return components.Where(c => c.XMLElementName() == localName).Any();
		}
	}
}
