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
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Configuration
Imports VectoAuxiliaries

Public Class cMAP
	'Implements IFuelConsumptionMap

	Private LnU As List(Of Single)
	Private LTq As List(Of Single)
	Private lFC As List(Of Single)

	Private sFilePath As String
	Private iMapDim As Integer

	Private Sub ResetMe()
		lFC = Nothing
		LTq = Nothing
		LnU = Nothing
		iMapDim = -1

	End Sub

	Public Function ReadFile(Optional ByVal ShowMsg As Boolean = True) As Boolean
		Dim file As cFile_V3
		Dim line As String()
		Dim nU As Double
		Dim MsgSrc As String


		MsgSrc = "Main/ReadInp/MAP"

		'Reset
		ResetMe()

		'Stop if there's no file
		If sFilePath = "" OrElse Not IO.File.Exists(sFilePath) Then
			If ShowMsg Then WorkerMsg(tMsgID.Err, "Map file not found! (" & sFilePath & ")", MsgSrc)
			Return False
		End If

		'Open file
		file = New cFile_V3
		If Not file.OpenRead(sFilePath) Then

			If ShowMsg Then WorkerMsg(tMsgID.Err, "Failed to open file (" & sFilePath & ") !", MsgSrc)
			Return False
		End If

		'Skip Header
		file.ReadLine()

		'Initi Lists (before version check so ReadOldFormat works)
		lFC = New List(Of Single)
		LTq = New List(Of Single)
		LnU = New List(Of Single)

		Try
			Do While Not file.EndOfFile

				'Line read
				line = file.ReadLine

				'Line counter up (was reset in ResetMe)
				iMapDim += 1

				'Revolutions
				nU = CDbl(line(0))

				LnU.Add(nU)

				'Power
				LTq.Add(line(1))

				'FC
				'Check sign
				If CSng(line(2)) < 0 Then
					file.Close()
					If ShowMsg Then WorkerMsg(tMsgID.Err, "FC < 0 in map at " & nU & " [1/min], " & line(1) & " [Nm]", MsgSrc)
					Return False
				End If

				lFC.Add(CSng(line(2)))


			Loop
		Catch ex As Exception

			If ShowMsg Then _
				WorkerMsg(tMsgID.Err, "Error during file read! Line number " & iMapDim + 1 & " (" & sFilePath & ")", MsgSrc,
						sFilePath)
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
			Return sFilePath
		End Get
		Set(ByVal value As String)
			sFilePath = value
		End Set
	End Property

	Public ReadOnly Property Tq As List(Of Single)
		Get
			Return LTq
		End Get
	End Property

	Public ReadOnly Property nU As List(Of Single)
		Get
			Return LnU
		End Get
	End Property

#End Region

	'Public Function GetFuelConsumption(torque As NewtonMeter, angularVelocity As PerSecond) As KilogramPerSecond _
	'	Implements IFuelConsumptionMap.GetFuelConsumption
	'	'MQ: TODO: check units!
	'	Return _
	'		(fFCdelaunay_Intp(angularVelocity.Value() / Constants.RPMToRad, torque.Value()) / 3600 / 1000).SI(Of KilogramPerSecond)()
	'End Function
End Class


