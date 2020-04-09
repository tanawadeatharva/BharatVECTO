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

		public static string[] ShowSelectFilesDialog(bool multiselect, string initialDirectory = null)
		{
			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = initialDirectory;
				openFileDialog.Multiselect = multiselect;
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


	}
}
