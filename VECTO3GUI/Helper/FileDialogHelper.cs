using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;


namespace VECTO3GUI.Helper
{
	public static class FileDialogHelper
	{
		private const string XMLFilter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
		private const string JobFilter = "Job Files (*.vectojob|*.vectojob|All Files (*.*)|*.*";


		public static string[] ShowSelectFilesDialog(bool multiselect, string initialDirectory = null)
		{
			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = initialDirectory;
				openFileDialog.Multiselect = multiselect;
				openFileDialog.Filter = XMLFilter;
				var result = openFileDialog.ShowDialog();

				if (result == DialogResult.OK)
				{
					return openFileDialog.FileNames;
				}
			}

			return null;
		}


		public static string ShowSelectDirectoryDialog(string initialDirectory = null)
		{
			using (var dialog = new CommonOpenFileDialog())
			{
				dialog.InitialDirectory = initialDirectory;
				dialog.IsFolderPicker = true;

				var result = dialog.ShowDialog();
				if (result == CommonFileDialogResult.Ok)
				{
					return dialog.FileName;
				}
			}

			return null;
		}

		public static string SaveJobFileToDialog(string initialDirectory = null)
		{
			var saveFileDialog = new SaveFileDialog {
				Filter =  JobFilter
			};

			if (initialDirectory != null)
				saveFileDialog.InitialDirectory = initialDirectory;

			return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : null;
		}
	}
}
