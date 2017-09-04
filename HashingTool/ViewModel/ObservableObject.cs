using System.ComponentModel;
using HashingTool.Helper;

namespace HashingTool.ViewModel
{
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		protected IOService _ioService = new WPFIoService();

		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
