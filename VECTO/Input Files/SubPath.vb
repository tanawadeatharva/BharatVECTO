' Copyright 2017 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.
Namespace Input_Files
	Public Class SubPath
		Private _fullPath As String
		Private _originalPath As String
		Private _valid As Boolean

		Public Sub New()
			_valid = False
		End Sub

		Public Sub Init(parentDir As String, path As String)
			If CheckFilenameGiven(path) = "" Then
				_valid = False
			Else
				_valid = True
				_originalPath = path
				_fullPath = FileRepl(path, parentDir)
			End If
		End Sub

		Private Function CheckFilenameGiven(f As String) As String
			If Trim(UCase(f)) = NoFile Then
				Return ""
			Else
				Return f
			End If
		End Function


		Public Sub Clear()
			_valid = False
		End Sub

		Public ReadOnly Property FullPath() As String
			Get
				If _valid Then
					Return _fullPath
				Else
					Return ""
				End If
			End Get
		End Property

		Public ReadOnly Property OriginalPath() As String
			Get
				If _valid Then
					Return _originalPath
				Else
					Return ""
				End If
			End Get
		End Property

		Public ReadOnly Property PathOrDummy() As String
			Get
				If _valid Then
					Return _originalPath
				Else
					Return NoFile
				End If
			End Get
		End Property
	End Class
End Namespace