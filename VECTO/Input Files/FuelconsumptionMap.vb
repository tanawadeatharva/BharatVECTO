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

Public Class FuelconsumptionMap
	'Implements IFuelConsumptionMap

	Private _angularSpeedList As List(Of Single)
	Private _torqueList As List(Of Single)
	Private _fuelconsumptionList As List(Of Single)

	Private _filePath As String

	Private Sub ResetMe()
		_fuelconsumptionList = Nothing
		_torqueList = Nothing
		_angularSpeedList = Nothing
	End Sub

	Public Function ReadFile(Optional ByVal ShowMsg As Boolean = True) As Boolean
		Dim file As CsvFile
		Dim line As String()
		Dim nU As Double
		Dim MsgSrc As String


		MsgSrc = "Main/ReadInp/MAP"

		'Reset
		ResetMe()

		'Stop if there's no file
		If _filePath = "" OrElse Not IO.File.Exists(_filePath) Then
			If ShowMsg Then WorkerMsg(MessageType.Err, "Map file not found! (" & _filePath & ")", MsgSrc)
			Return False
		End If

		'Open file
		file = New CsvFile
		If Not file.OpenRead(_filePath) Then

			If ShowMsg Then WorkerMsg(MessageType.Err, "Failed to open file (" & _filePath & ") !", MsgSrc)
			Return False
		End If

		'Skip Header
		file.ReadLine()

		'Initi Lists (before version check so ReadOldFormat works)
		_fuelconsumptionList = New List(Of Single)
		_torqueList = New List(Of Single)
		_angularSpeedList = New List(Of Single)

		Dim lineCount As Integer = -1

		Try
			Do While Not file.EndOfFile

				'Line read
				line = file.ReadLine

				'Line counter up (was reset in ResetMe)
				lineCount += 1

				'Revolutions
				nU = CDbl(line(0))

				_angularSpeedList.Add(nU)

				'Power
				_torqueList.Add(line(1))

				'FC
				'Check sign
				If CSng(line(2)) < 0 Then
					file.Close()
					If ShowMsg Then WorkerMsg(MessageType.Err, "FC < 0 in map at " & nU & " [1/min], " & line(1) & " [Nm]", MsgSrc)
					Return False
				End If

				_fuelconsumptionList.Add(CSng(line(2)))


			Loop
		Catch ex As Exception

			If ShowMsg Then _
				WorkerMsg(MessageType.Err, "Error during file read! Line number " & lineCount + 1 & " (" & _filePath & ")", MsgSrc,
						_filePath)
			GoTo lbEr

		End Try

		'Close file
		file.Close()


		Return True


		'ERROR-label for clean Abort
lbEr:
		file.Close()


		Return False
	End Function


#Region "Properties"

	Public Property FilePath() As String
		Get
			Return _filePath
		End Get
		Set(ByVal value As String)
			_filePath = value
		End Set
	End Property

	Public ReadOnly Property Tq As List(Of Single)
		Get
			Return _torqueList
		End Get
	End Property

	Public ReadOnly Property nU As List(Of Single)
		Get
			Return _angularSpeedList
		End Get
	End Property

#End Region
End Class


