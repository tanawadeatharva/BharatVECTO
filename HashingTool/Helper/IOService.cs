using System.IO;
using Microsoft.Win32;

namespace HashingTool.Helper
{
	public interface IOService
	{
		Stream OpenFileDialog(string defaultPath, string defaultExt, string filter, out string location);

		Stream SaveData(string defaultPath, string defaultExt, string filter, out string location);
	}

	public class WPFIoService : IOService
	{
		public Stream OpenFileDialog(string defaultPath, string defaultExt, string filter, out string location)
		{
			var dlg = new OpenFileDialog {
				DefaultExt = defaultExt,
				Filter = filter
			};
			var result = dlg.ShowDialog();
			if (result != true) {
				location = null;
				return null;
			}
			location = dlg.FileName;
			return File.OpenRead(dlg.FileName);
		}

		public Stream SaveData(string defaultPath, string defaultExt, string filter, out string location)
		{
			var dlg = new SaveFileDialog() {
				DefaultExt = defaultExt,
				Filter = filter
			};
			var result = dlg.ShowDialog();
			if (result != true) {
				location = null;
				return null;
			}
			location = dlg.FileName;
			return new FileStream(dlg.FileName, FileMode.Create);
		}
	}
}
