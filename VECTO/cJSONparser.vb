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
Imports System.Text
Imports Microsoft.VisualBasic.FileIO
Imports Newtonsoft.Json

''' <summary>
''' uses JSON.NET http://json.codeplex.com/
''' </summary>
''' <remarks></remarks>
Public Class JSON
	Public Content As Dictionary(Of String, Object)
	Public ErrorMsg As String

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
			ErrorMsg = "file not found"
			Return False
		End If

		Try
			file = New TextFieldParser(path)
		Catch ex As Exception
			ErrorMsg = ex.Message
			Return False
		End Try

		If file.EndOfData Then
			file.Close()
			ErrorMsg = "file is empty"
			Return False
		End If

		str = file.ReadToEnd

		file.Close()

		Try
			Content = JsonConvert.DeserializeObject(str, Content.GetType)
		Catch ex As Exception
			ErrorMsg = ex.Message
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


#Region "old self-made parser"

	Private fullfile As String


	Private Function GetKeyValString(TabLvl As Integer, ByRef kv As KeyValuePair(Of String, Object)) As String
		Dim str As New StringBuilder
		Dim obj As Object
		Dim kv0 As KeyValuePair(Of String, Object)
		Dim First As Boolean

		str.Append(Tabs(TabLvl) & ChrW(34) & kv.Key & ChrW(34) & ": ")

		Select Case kv.Value.GetType

			Case GetType(Dictionary(Of String, Object))

				str.AppendLine("{")

				First = True
				For Each kv0 In kv.Value
					If First Then
						First = False
					Else
						str.AppendLine(",")
					End If
					str.Append(GetKeyValString(TabLvl + 1, kv0))
				Next

				str.AppendLine()
				str.Append(Tabs(TabLvl) & "}")

			Case GetType(List(Of Object))

				str.AppendLine("[")

				First = True
				For Each obj In kv.Value
					If First Then
						First = False
					Else
						str.AppendLine(",")
					End If
					str.Append(Tabs(TabLvl + 1) & GetObjString(TabLvl + 1, obj))
				Next

				str.AppendLine()
				str.Append(Tabs(TabLvl) & "]")

			Case Else

				str.Append(GetObjString(TabLvl + 1, kv.Value))

		End Select

		Return str.ToString
	End Function

	Private Function GetObjString(TabLvl As Integer, ByRef obj As Object) As String
		Dim kv0 As KeyValuePair(Of String, Object)
		Dim First As Boolean
		Dim str As StringBuilder

		If obj Is Nothing Then
			Return "null"
		Else
			Select Case obj.GetType

				Case GetType(Dictionary(Of String, Object))

					str = New StringBuilder
					str.AppendLine("{")

					First = True
					For Each kv0 In obj
						If First Then
							First = False
						Else
							str.AppendLine(",")
						End If
						str.Append(GetKeyValString(TabLvl + 1, kv0))
					Next

					str.AppendLine()
					str.Append(Tabs(TabLvl) & "}")

					Return str.ToString

				Case GetType(String)

					Return ChrW(34) & CStr(obj) & ChrW(34)

				Case GetType(Boolean)

					If CBool(obj) Then
						Return "true"
					Else
						Return "false"
					End If

				Case Else

					Return CDbl(obj).ToString

			End Select
		End If
	End Function

	Private Function Tabs(l As Integer) As String
		Dim i As Integer
		Dim str As String

		str = ""
		For i = 1 To l
			str &= vbTab
		Next

		Return str
	End Function


#End Region
End Class
