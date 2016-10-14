' Copyright 2015 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

''' <summary>
''' File Browser for Open/Save File dialogs and Folder Browser. Features File History and Favorite Folders.
''' </summary>
''' <remarks>
''' Usage:
'''  1. Create new instance, preferably in FB_Global, e.g. fbTXT = New cFileBrowser("txt")
'''  2. Define extensions, e.g.  fbTXT.Extensions = New String() {"txt","log"}
'''  3. Use OpenDialog, SaveDialog, etc.
'''  4. Call Close method when closing application to write file history, e.g. fbTXT.Close 
''' File history is unique for each ID. Folder history is global.
''' </remarks>
Public Class FileBrowser
	Private _initialized As Boolean
	Private ReadOnly _id As String
	Private _extensionList As String()
	Private _dialog As FileBrowserDialog
	Private _noExtension As Boolean
	Private ReadOnly _folderBrowser As Boolean
	Private ReadOnly _lightMode As Boolean

	''' <summary>
	''' New cFileBrowser instance
	''' </summary>
	''' <param name="id">Needed to save the file history when not using LightMode.</param>
	''' <param name="folderBrowser">Browse folders instead of files.</param>
	''' <param name="lightMode">If enabled file history is not saved.</param>
	''' <remarks></remarks>
	Public Sub New(id As String, Optional ByVal folderBrowser As Boolean = False,
					Optional ByVal lightMode As Boolean = False)
		_initialized = False
		_id = ID
		_noExtension = True
		_folderBrowser = folderBrowser
		_lightMode = lightMode
	End Sub

	''' <summary>
	''' Opens dialog for OPENING files. Selected file must exist. Returns False if cancelled by user, else True.
	''' </summary>
	''' <param name="path">Initial selected file. If empty the last selected file is used. If file without directoy the last directory will be used.</param>
	''' <param name="multiFile">Allow selecting multiple files.</param>
	''' <param name="ext">Set extension. If not defined the first predefined extension is used.</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function OpenDialog(path As String, Optional ByVal multiFile As Boolean = False,
								Optional ByVal ext As String = "") As Boolean
		Return CustomDialog(path, True, False, FileBrowserFileExtensionMode.MultiExt, multiFile, ext, "Open")
	End Function

	''' <summary>
	''' Opens dialog for SAVING files. If file already exists user will be asked to overwrite. Returns False if cancelled by user, else True.
	''' </summary>
	''' <param name="path">Initial selected file. If empty the last selected file is used. If file without directoy the last directory will be used.</param>
	''' <param name="forceExt">Force predefined file extension.</param>
	''' <param name="ext">Set extension. If not defined the first predefined extension is used.</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function SaveDialog(path As String, Optional ByVal forceExt As Boolean = True, Optional ByVal ext As String = "") _
		As Boolean
		Dim x As FileBrowserFileExtensionMode
		If forceExt Then
			x = FileBrowserFileExtensionMode.ForceExt
		Else
			x = FileBrowserFileExtensionMode.SingleExt
		End If
		Return CustomDialog(path, False, True, x, False, ext, "Save As")
	End Function

	''' <summary>
	''' Custom open/save dialog. Returns False if cancelled by user, else True.
	''' </summary>
	''' <param name="path">Initial selected file. If empty the last selected file is used. If file without directoy the last directory will be used.</param>
	''' <param name="fileMustExist">Selected file must exist.</param>
	''' <param name="overwriteCheck">If file already exists user will be asked to overwrite.</param>
	''' <param name="extMode">ForceExt= First predefined extension (or Ext parameter) will be forced (Default for SaveDialog), MultiExt= All files with predefined extensions are shown (Default for OpenDialog), SingleExt= All files with the first predefined extension will be shown.</param>
	''' <param name="multiFile">Allow to select multiple files.</param>       
	''' <param name="ext">Set extension. If not defined the first predefined extension is used.</param>            
	''' <param name="title">Dialog title.</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function CustomDialog(path As String, fileMustExist As Boolean, overwriteCheck As Boolean, extMode As FileBrowserFileExtensionMode,
								multiFile As Boolean, ext As String, Optional title As String = "File Browser") As Boolean
		If Not _initialized Then Init()
		Return _dialog.Browse(path, fileMustExist, overwriteCheck, extMode, multiFile, ext, title)
	End Function

	'Manually update File History
	''' <summary>
	''' Add file to file history.
	''' </summary>
	''' <param name="path">File to be added to file history.</param>
	''' <remarks></remarks>
	Public Sub UpdateHistory(path As String)
		If Not _initialized Then Init()
		_dialog.UpdateHistory(path)
	End Sub

	''' <summary>
	''' Save file history (if not LightMode) and global folder history.
	''' </summary>
	''' <remarks></remarks>
	Public Sub Close()
		If _initialized Then
			_dialog.SaveAndClose()
			_initialized = False
		End If
		_dialog = Nothing
	End Sub

	Private Sub Init()
		_dialog = New FileBrowserDialog(_lightMode)
		_dialog.ID = _id
		If Not _noExtension Then _dialog.Extensions = _extensionList
		If _folderBrowser Then _dialog.SetFolderBrowser()
		_initialized = True
	End Sub

	''' <summary>
	''' Predefined file extensions. Must be set before Open/Save dialog is used for the first time.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property Extensions As String()
		Get
			Return _extensionList
		End Get
		Set(value As String())
			_extensionList = value
			_noExtension = False
		End Set
	End Property

	''' <summary>
	''' Selected file(s) oder folder (if FolderBrowser)
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public ReadOnly Property Files As String()
		Get
			If _initialized Then
				Return _dialog.Files
			Else
				Return New String() {""}
			End If
		End Get
	End Property
End Class


