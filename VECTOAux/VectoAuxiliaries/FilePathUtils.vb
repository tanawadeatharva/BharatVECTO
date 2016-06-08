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

Public Class FilePathUtils
	Public Shared Function ValidateFilePath(ByVal filePath As String, ByVal expectedExtension As String,
											ByRef message As String) As Boolean


		Dim illegalFileNameCharacters As Char() = {"<"c, ">"c, ":"c, """"c, "/"c, "\"c, "|"c, "?"c, "*"c, "~"c}
		Dim detectedExtention As String = fileExtentionOnly(filePath)
		Dim pathOnly As String = filePathOnly(filePath)
		Dim fileNameOnlyWithExtension As String = fileNameOnly(filePath, True)
		Dim fileNameOnlyNoExtension As String = fileNameOnly(filePath, False)

		'Is this filePath empty
		If filePath.trim.Length = 0 OrElse Right(filePath, 1) = "\" Then
			message = "A filename cannot be empty"
			Return False
		End If


		'Extension Expected, but not match
		If expectedExtension.Trim.Length > 0 Then
			If String.Compare(expectedExtension, detectedExtention, True) <> 0 Then
				message = String.Format("The file extension type does not match the expected type of {0}", expectedExtension)
				Return False
			End If
		End If

		'Extension Not Expected, but was supplied
		If expectedExtension.Trim.Length > 0 Then
			If detectedExtention.Length = 0 Then
				message = String.Format("No Extension was supplied, but an extension of {0}, this is not required",
										detectedExtention)
				Return False
			End If
		End If


		'Illegal characters
		If Not fileNameLegal(fileNameOnlyWithExtension) Then
			message = String.Format("The filenames have one or more illegal characters")
			Return False
		End If


		message = "OK"
		Return True
	End Function


	Public Shared Function fileNameLegal(fileName As String) As Boolean

		Dim illegalFileNameCharacters As Char() = {"<"c, ">"c, ":"c, """"c, "/"c, "\"c, "|"c, "?"c, "*"c, "~"c}


		'Illegal characters
		For Each ch As Char In illegalFileNameCharacters

			If fileName.Contains(ch) Then

				Return False

			End If
		Next
		Return True
	End Function


	Public Shared Function ResolveFilePath(vectoPath As String, filename As String) As String

		'No Vecto Path supplied
		If vectoPath = "" Then Return filename

		'This is not relative
		If filename.Contains(":\") Then

			'Filepath is already absolute
			Return filename
		Else
			Return vectoPath & filename
		End If
	End Function


	''' <summary>
	''' File name without the path    "C:\temp\TEST.txt"  >>  "TEST.txt" oder "TEST"
	''' </summary>
	''' <param name="filePath"></param>
	''' <param name="WithExtention"></param>
	''' <returns>Return file portion of the path, with or without the extension</returns>
	''' <remarks></remarks>
	Public Shared Function fileNameOnly(ByVal filePath As String, ByVal WithExtention As Boolean) As String
		Dim x As Integer
		x = filePath.LastIndexOf("\") + 1
		filePath = Microsoft.VisualBasic.Right(filePath, Microsoft.VisualBasic.Len(filePath) - x)
		If Not WithExtention Then
			x = filePath.LastIndexOf(".")
			If x > 0 Then filePath = Microsoft.VisualBasic.Left(filePath, x)
		End If
		Return filePath
	End Function


	''' <summary>
	''' Extension alone      "C:\temp\TEST.txt" >> ".txt"
	''' </summary>
	''' <param name="filePath"></param>
	''' <returns>Extension alone Including the dot IE  .EXT</returns>
	''' <remarks></remarks>
	Public Shared Function fileExtentionOnly(ByVal filePath As String) As String
		Dim x As Integer
		x = filePath.LastIndexOf(".")
		If x = - 1 Then
			Return ""
		Else
			Return Microsoft.VisualBasic.Right(filePath, Microsoft.VisualBasic.Len(filePath) - x)
		End If
	End Function

	''' <summary>
	''' File Path alone   "C:\temp\TEST.txt"  >>  "C:\temp\"
	'''                   "TEST.txt"          >>  ""
	''' </summary>
	''' <param name="filePath"></param>
	''' <returns>Filepath without the extension</returns>
	''' <remarks></remarks>
	Public Shared Function filePathOnly(ByVal filePath As String) As String
		Dim x As Integer
		If filePath Is Nothing OrElse filePath.Length < 3 OrElse filePath.Substring(1, 2) <> ":\" Then Return ""
		x = filePath.LastIndexOf("\")
		Return Microsoft.VisualBasic.Left(filePath, x + 1)
	End Function
End Class

