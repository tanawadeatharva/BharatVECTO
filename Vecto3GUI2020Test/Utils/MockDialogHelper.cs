using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Moq;
using VECTO3GUI2020.Helper;

namespace Vecto3GUI2020Test.Utils;

public class MockDialogHelper : IDialogHelper
{
	private IDialogHelper _dialogHelperImplementation;

	public class Dialog
	{
		public enum DialogType
		{
			Error
		}

		public DialogType Type
		{
			get;
			set;
		}
		public string Message
		{
			get;
			set;
		}
	}

	
	private readonly IList<Dialog> _dialogs = new List<Dialog>();
	public MockDialogHelper()
	{
		
	}

	public IReadOnlyList<Dialog> Dialogs => _dialogs.ToList();

	public int NrErrors => _dialogs.Count(d => d.Type == Dialog.DialogType.Error);
	
	#region Implementation of IDialogHelper

	public string OpenFileDialog(string filter = "All files (*.*)|*.*", string initialDirectory = null)
	{
		throw new System.NotImplementedException();
	}

	public string[] OpenFilesDialog(string filter = "All files (*.*|*.*", string initialDirectory = null)
	{
		throw new System.NotImplementedException();
	}

	public string OpenXMLFileDialog(string initialDirectory)
	{
		throw new System.NotImplementedException();
	}

	public string OpenXMLFileDialog()
	{
		throw new System.NotImplementedException();
	}

	public string[] OpenXMLFilesDialog(string initialDirectory = null)
	{
		throw new System.NotImplementedException();
	}

	public string OpenFolderDialog(string initialDirectory = null)
	{
		throw new System.NotImplementedException();
	}

	public string OpenJsonFileDialog(string initialDirectory = null)
	{
		throw new System.NotImplementedException();
	}

	public string SaveToDialog(string initialDirectory = null, string filter = "All files (*.*|*.*")
	{
		throw new System.NotImplementedException();
	}

	public string SaveToXMLDialog(string initialDirectory = null)
	{
		throw new System.NotImplementedException();
	}

	public string SaveToVectoJobDialog(string initialDirectory = null)
	{
		throw new System.NotImplementedException();
	}

	public string OpenXMLAndVectoFileDialog(string initialDirectory = null)
	{
		throw new System.NotImplementedException();
	}

	public MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
	{
		lock (_dialogs) {
			_dialogs.Add(new Dialog() {
				Message = $"[{caption}] {messageBoxText}",
			});
		}

		return MessageBoxResult.OK;
	}

	public MessageBoxResult ShowMessageBox(string messageBoxText, string caption)
	{
		return ShowMessageBox(messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Asterisk);
	}

	public MessageBoxResult ShowErrorMessage(string errorMessage, string caption)
	{
		lock (_dialogs) {
			_dialogs.Add(new Dialog() {
				Message = $"[{caption}] {errorMessage}",
				Type = Dialog.DialogType.Error,
			});
		}

		return MessageBoxResult.OK;
	}

	public MessageBoxResult ShowErrorMessage(string errorMessage)
	{
		return ShowErrorMessage(errorMessage, "-");
	}

	#endregion
}