' Copyright 2014 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.FileIO

Public Class CsvFile
	Private _parser As TextFieldParser
	Private _writer As StreamWriter
	Private _mode As FileMode
	Private _path As String
	Private _separator As String
	Private _skipComments As Boolean
	Private _fileOpen As Boolean
	Private _preLine As String()
	Private _endOfFile As Boolean

	Public Sub New()
		Reset()
	End Sub

	Private Sub Reset()
		_fileOpen = False
		_mode = FileMode.Undefined
		_preLine = Nothing
		_endOfFile = False
	End Sub

	Public Function OpenRead(ByVal fileName As String, Optional ByVal separator As String = ",",
							Optional ByVal skipComment As Boolean = True) As Boolean
		Reset()

		_path = fileName
		_separator = separator
		_skipComments = skipComment
		If Not (_mode = FileMode.Undefined) Then Return False
		If Not File.Exists(_path) Then Return False
		_mode = FileMode.Read
		Try
			_parser = New TextFieldParser(_path, Encoding.Default)
			_fileOpen = True
		Catch ex As Exception
			Return False
		End Try
		_parser.TextFieldType = FieldType.Delimited
		_parser.Delimiters = New String() {_separator}

		'If TxtFldParser.EndOfData Then Return False

		ReadLine()
		Return True
	End Function

	Public Function ReadLine() As String()
		Dim line As String()
		Dim line0 As String

		line = _preLine

lb10:
		If _parser.EndOfData Then
			_endOfFile = True
		Else
			_preLine = _parser.ReadFields
			line0 = UCase(Trim(_preLine(0)))

			If _skipComments Then
				If Left(line0, 1) = "#" Then GoTo lb10
			End If

		End If

		Return line
	End Function

	Public Sub Close()
		Select Case _mode
			Case FileMode.Read
				If _fileOpen Then _parser.Close()
				_parser = Nothing
			Case FileMode.Write
				If _fileOpen Then _writer.Close()
				_writer = Nothing
		End Select
		Reset()
	End Sub

	Public ReadOnly Property EndOfFile() As Boolean
		Get
			Return _endOfFile
		End Get
	End Property

	Public Function OpenWrite(ByVal FileName As String, Optional ByVal Separator As String = ",",
							Optional ByVal AutoFlush As Boolean = False, Optional ByVal Append As Boolean = False) As Boolean
		Reset()
		_path = FileName
		_separator = Separator
		If Not (_mode = FileMode.Undefined) Then Return False
		_mode = FileMode.Write
		Try
			_writer = My.Computer.FileSystem.OpenTextFileWriter(_path, Append, FileFormat)
			_fileOpen = True
		Catch ex As Exception
			Return False
		End Try
		_writer.AutoFlush = AutoFlush
		Return True
	End Function

	Public Sub WriteLine(ByVal ParamArray x() As Object)
		Dim St As String
		Dim StB As New StringBuilder
		Dim Skip As Boolean
		Skip = True
		For Each St In x
			If Skip Then
				StB.Append(St)
				Skip = False
			Else
				StB.Append(_separator & St)
			End If
		Next
		_writer.WriteLine(StB.ToString)
	End Sub

	Public Sub WriteLine(ByVal x As String)
		_writer.WriteLine(x)
	End Sub

	Private Enum FileMode
		Undefined
		Read
		Write
	End Enum
End Class
