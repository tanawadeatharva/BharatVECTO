using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using VECTO3GUI2020.Properties;
using MessageBox = System.Windows.MessageBox;

namespace VECTO3GUI2020.Helper
{
    public class DialogHelper : IDialogHelper
	{
		private readonly string _defaultInitialDirectory = Settings.Default.DefaultFilePath;

		#region File and Folder Dialogs
		private string _xmlFilter = "XML Files (*.xml)|*.xml";

		private Dictionary<string, string> lastUsedDirectories = new Dictionary<string, string>();

		private string lastUsedDirectoryFolderPicker = null;
		private string[] OpenFilesDialog(string filter, string initialDirectory, bool multiselect)
		{
			if (initialDirectory == null) {
				initialDirectory = LookUpLastDir(filter);
			}


			
			using (OpenFileDialog fd = new OpenFileDialog {
				InitialDirectory = initialDirectory ?? _defaultInitialDirectory,
				Multiselect = multiselect,
				Filter = filter,
				RestoreDirectory = true
			}) {
				var result = fd.ShowDialog();
				if (result == DialogResult.OK) {
					lastUsedDirectories[filter] = Path.GetDirectoryName(fd.FileName);
					return fd.FileNames;
				}
			}


			return null;
		}

		private string LookUpLastDir(string filter)
		{
			string lastUsedDirectory = null;
			if (lastUsedDirectories.TryGetValue(filter, out lastUsedDirectory)) {
				return lastUsedDirectory;
			} else {
				return Settings.Default.DefaultFilePath;
			}
		}

		public string OpenFileDialog(string filter = "All files (*.*)|*.*", string initialDirectory = null)
		{
			return OpenFilesDialog(filter, initialDirectory)?[0];
		}

		public string[] OpenFilesDialog(string filter, string initialDirectory)
		{
			return OpenFilesDialog(filter, initialDirectory, true);
		}

		public string OpenXMLFileDialog()
		{
			return OpenXMLFileDialog(null);
		}



		public string[] OpenXMLFilesDialog(string initialDirectory)
		{
			return OpenFilesDialog(_xmlFilter, initialDirectory);
		}

		public string OpenXMLFileDialog(string initialDirectory)
		{
			return OpenFilesDialog(_xmlFilter, initialDirectory, false)?[0];
		}


		public string OpenFolderDialog(string initialDirectory = null)
		{

			if (initialDirectory == null) {
				initialDirectory = lastUsedDirectoryFolderPicker;
			}
			using (var dialog = new CommonOpenFileDialog())
			{
				dialog.InitialDirectory = initialDirectory;
				dialog.IsFolderPicker = true;
				dialog.Multiselect = false;
				dialog.RestoreDirectory = true;

				var result = dialog.ShowDialog();
				if (result == CommonFileDialogResult.Ok) {
					lastUsedDirectoryFolderPicker = Path.GetDirectoryName(dialog.FileName);
					return dialog.FileName;
				}
			}
			return null;
		}

		#endregion

		#region Messagebox

		public MessageBoxResult ShowMessageBox(string messageBoxText,
			string caption,
			MessageBoxButton button,
			MessageBoxImage icon)
		{
			return MessageBox.Show(messageBoxText, caption, button, icon);
		}

		public MessageBoxResult ShowMessageBox(string messageBoxTest, string caption)
		{
			return MessageBox.Show(messageBoxTest, caption);
		}

		public string SaveToDialog(string initialDirectory, string filter)
		{
			using (var saveFileDialog = new SaveFileDialog {
				Filter = filter
			}) {
				saveFileDialog.InitialDirectory = initialDirectory ?? _defaultInitialDirectory;

				return saveFileDialog.ShowDialog() == DialogResult.OK ? saveFileDialog.FileName : null;
			}
		}


		public string SaveToXMLDialog(string initialDirectory)
		{
			return SaveToDialog(initialDirectory, _xmlFilter);
		}



		#endregion
	}

	public interface IDialogHelper
	{
		/// <summary>
		/// Opens a dialog to open a file
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="initialDirectory">If no directory is specified the location of the assembly is used</param>
		/// <returns></returns>
		string OpenFileDialog(string filter = "All files (*.*)|*.*", string initialDirectory = null);

		/// <summary>
		/// Opens a dialog to open files
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="initialDirectory">If no directory is specified the location of the assembly is used</param>
		/// <returns></returns>
		string[] OpenFilesDialog(string filter = "All files (*.*|*.*", string initialDirectory = null);

		/// <summary>
		/// Opens a dialog to open a XML-file
		/// </summary>
		/// <param name="initialDirectory">If no directory is specified the location of the assembly is used</param>
		/// <returns></returns>
		string OpenXMLFileDialog(string initialDirectory);

		string OpenXMLFileDialog();


		/// <summary>
		/// Opens a dialog to open XML-files
		/// </summary>
		/// <param name="initialDirectory">If no directory is specified the location of the assembly is used</param>
		/// <returns></returns>
		string[] OpenXMLFilesDialog(string initialDirectory = null);

		/// <summary>
		/// Opens a dialog to pick a folder, if no initialdirectory is specified the location of the assembly is used
		/// </summary>
		/// <param name="initialDirectory"></param>
		/// <returns></returns>
		string OpenFolderDialog(string initialDirectory = null);

		/// <summary>
		/// Displays a messagebox
		/// </summary>
		/// <param name="messageBoxText"></param>
		/// <param name="caption"></param>
		/// <param name="button"></param>
		/// <param name="icon"></param>
		/// <returns></returns>
		MessageBoxResult ShowMessageBox(string messageBoxText,
			string caption,
			MessageBoxButton button,
			MessageBoxImage icon);

		/// <summary>
		/// Displays a messagebox
		/// </summary>
		/// <param name="messageBoxTest"></param>
		/// <param name="caption"></param>
		/// <returns></returns>
		MessageBoxResult ShowMessageBox(string messageBoxTest, string caption);


		string SaveToDialog(string initialDirectory = null, string filter = "All files (*.*|*.*");

		string SaveToXMLDialog(string initialDirectory = null);
	}
}
