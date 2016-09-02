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
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports Newtonsoft.Json

''' <summary>
''' uses JSON.NET http://json.codeplex.com/
''' </summary>
''' <remarks></remarks>
Public Class JSON
	Public Content As Dictionary(Of String, Object)

	Public Sub New()
		Content = New Dictionary(Of String, Object)
	End Sub

	''' <summary>
	''' Reads a JSON File into the Content variable.
	''' </summary>
	''' <param name="path"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function ReadFile(path As String) As Boolean
		Dim file As TextFieldParser
		Dim str As String

		Content.Clear()

		If Not IO.File.Exists(path) Then
			Return False
		End If

		Try
			file = New TextFieldParser(path)
		Catch ex As Exception
			Return False
		End Try

		If file.EndOfData Then
			file.Close()
			Return False
		End If

		str = file.ReadToEnd

		file.Close()

		Try
			Content = JsonConvert.DeserializeObject(str, Content.GetType)
		Catch ex As Exception
			Return False
		End Try

		Return True
	End Function

	''' <summary>
	''' Writes the Content variable into a JSON file.
	''' </summary>
	''' <param name="path"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function WriteFile(path As String) As Boolean
		Dim file As StreamWriter
		Dim str As String

		If Content.Count = 0 Then
			Return False
		End If

		Try
			str = JsonConvert.SerializeObject(Content, Formatting.Indented)
			file = My.Computer.FileSystem.OpenTextFileWriter(path, False)
		Catch ex As Exception
			Return False
		End Try

		file.Write(str)
		file.Close()

		Return True
	End Function
End Class
