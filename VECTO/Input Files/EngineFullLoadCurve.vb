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
Imports System.Linq

''' <summary>
''' Full load/motoring curve input file
''' </summary>
''' <remarks></remarks>
Public Class EngineFullLoadCurve
	''' <summary>
	''' List of full load torque values [Nm]
	''' </summary>
	''' <remarks></remarks>
	Public MaxTorqueList As List(Of Single)

	''' <summary>
	''' List of motoring torque values [Nm]
	''' </summary>
	''' <remarks></remarks>
	Public DragTorqueList As List(Of Single)

	''' <summary>
	''' List of engine speed values [1/min]
	''' </summary>
	''' <remarks></remarks>
	Public EngineSpeedList As List(Of Single)

	''' <summary>
	''' List of PT1 values [s]
	''' </summary>
	''' <remarks></remarks>
	Private _pt1List As List(Of Single)

	''' <summary>
	''' Last index of lists (items count - 1)
	''' </summary>
	''' <remarks></remarks>
	'Private _iDim As Integer

	''' <summary>
	''' Read file. FilePath must be set before calling. 
	''' </summary>
	''' <returns>True if successful.</returns>
	''' <remarks></remarks>   
	Public Function ReadFile(tqOnly As Boolean, Optional ByVal showMsg As Boolean = True) As Boolean
		Dim pt1Set As Boolean

		Const msgSrc As String = "Main/ReadInp/FLD"

		'Reset
		MaxTorqueList = Nothing
		DragTorqueList = Nothing
		EngineSpeedList = Nothing
		_pt1List = Nothing
		Dim lineCount As Integer = -1

		'Stop if there's no file
		If FilePath = "" OrElse Not IO.File.Exists(FilePath) Then
			If showMsg Then WorkerMsg(MessageType.Err, "FLD file '" & FilePath & "' not found!", msgSrc)
			Return False
		End If

		'Open file
		Dim file As CsvFile = New CsvFile
		If Not file.OpenRead(FilePath) Then
			If showMsg Then WorkerMsg(MessageType.Err, "Failed to open file (" & FilePath & ") !", msgSrc)

			Return False
		End If

		'Skip Header
		file.ReadLine()

		'Initialize Lists
		MaxTorqueList = New List(Of Single)
		DragTorqueList = New List(Of Single)
		EngineSpeedList = New List(Of Single)
		_pt1List = New List(Of Single)

		Dim firstLine As Boolean = True
		Try

			Do While Not file.EndOfFile

				'Read Line
				Dim line As String() = file.ReadLine

				'VECTO: M => Pe
				Dim rpm As Double = CDbl(line(0))

				EngineSpeedList.Add(rpm)
				MaxTorqueList.Add(CDbl(line(1)))

				If tqOnly Then
					DragTorqueList.Add(0)
				Else
					DragTorqueList.Add(CDbl(line(2)))
				End If

				If firstLine Then
					pt1Set = (Not tqOnly) AndAlso (UBound(line) > 2)
					firstLine = False
				End If

				'If PT1 not defined, use default value (0)
				If pt1Set Then
					_pt1List.Add(CSng(line(3)))
				Else
					_pt1List.Add(0)
				End If

				'Line-counter up (was reset in ResetMe)
				lineCount += 1


			Loop

		Catch ex As Exception

			If showMsg Then _
				WorkerMsg(MessageType.Err, "Error during file read! Line number: " & lineCount + 1 & " (" & FilePath & ")", msgSrc,
						FilePath)
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

	''' <summary>
	''' Returns stationary full load power [kW] at given engine speed.
	''' </summary>
	''' <param name="nU">engine speed [1/min]</param>
	''' <returns>stationary full load power [kW]</returns>
	''' <remarks></remarks>
	Public Function Pfull(nU As Single) As Single
		Dim i As Int32

		'Extrapolation for x < x(1)
		If EngineSpeedList(0) >= nU Then
			'If LnU(0) > nU Then MODdata.ModErrors.FLDextrapol = "n= " & nU & " [1/min]"
			i = 1
			GoTo lbInt
		End If

		i = 0
		Do While EngineSpeedList(i) < nU And i < EngineSpeedList.Count - 1
			i += 1
		Loop

		'Extrapolation for x > x(imax)
		If EngineSpeedList(i) < nU Then
			'MODdata.ModErrors.FLDextrapol = "n= " & nU & " [1/min]"
		End If

lbInt:
		'Interpolation
		Return _
			nMtoPe(nU,
					(nU - EngineSpeedList(i - 1)) * (MaxTorqueList(i) - MaxTorqueList(i - 1)) /
					(EngineSpeedList(i) - EngineSpeedList(i - 1)) + MaxTorqueList(i - 1))
	End Function

	''' <summary>
	''' Returns stationary full load torque [Nm] at given engine speed.
	''' </summary>
	''' <param name="nU">engine speed [1/min]</param>
	''' <returns>stationary full load torque [Nm]</returns>
	''' <remarks></remarks>
	Private Function Torque(ByVal nU As Single) As Single
		Dim i As Int32

		'Extrapolation for x < x(1)
		If EngineSpeedList(0) >= nU Then
			'If LnU(0) > nU Then MODdata.ModErrors.FLDextrapol = "n= " & nU & " [1/min]"
			i = 1
			GoTo lbInt
		End If

		i = 0
		Do While EngineSpeedList(i) < nU And i < EngineSpeedList.Count - 1
			i += 1
		Loop

		'Extrapolation for x > x(imax)
		If EngineSpeedList(i) < nU Then
			'MODdata.ModErrors.FLDextrapol = "n= " & nU & " [1/min]"
		End If

lbInt:
		'Interpolation
		Return _
			(nU - EngineSpeedList(i - 1)) * (MaxTorqueList(i) - MaxTorqueList(i - 1)) / (EngineSpeedList(i) - EngineSpeedList(i - 1)) +
			MaxTorqueList(i - 1)
	End Function

	'	''' <summary>
	'	''' Calculates and returns Npref [1/min]. Speed at 51% torque/speed-integral between idling and N95h. Defined in Init.
	'	''' </summary>
	'	''' <returns>Npref [1/min]</returns>
	'	''' <remarks></remarks>
	'	Public Function fNpref(ByVal Nidle As Single) As Single
	'		Dim i As Integer
	'		Dim Amax As Single
	'		Dim N95h As Single
	'		Dim n As Single
	'		Dim T0 As Single
	'		Dim dn As Single
	'		Dim A As Single
	'		Dim k As Single
	'
	'
	'		dn = 0.001
	'
	'		N95h = fnUofPfull(0.95 * Pfull(fnUrated), False)
	'
	'		If N95h < 0 Then Return -1
	'
	'		Amax = Area(Nidle, N95h)
	'
	'		For i = 0 To iDim - 1
	'
	'			If Area(Nidle, LnU(i + 1)) > 0.51 * Amax Then
	'
	'				n = LnU(i)
	'				T0 = LTq(i)
	'				A = Area(Nidle, n)
	'
	'				k = (LTq(i + 1) - LTq(i)) / (LnU(i + 1) - LnU(i))
	'
	'				Do While A < 0.51 * Amax
	'					n += dn
	'					A += dn * (2 * T0 + k * dn) / 2
	'				Loop
	'
	'				Exit For
	'
	'			End If
	'
	'		Next
	'
	'		Return n
	'	End Function

	'	''' <summary>
	'	''' Calculates torque/speed-integral between two engine speed limits. Used for Npref.
	'	''' </summary>
	'	''' <param name="nFrom">lower engine speed limit [1/min]</param>
	'	''' <param name="nTo">upper engine speed limit [1/min]</param>
	'	''' <returns>torque/speed-integral between nFrom and nTo [Nm/min]</returns>
	'	''' <remarks></remarks>
	'	Private Function Area(ByVal nFrom As Single, ByVal nTo As Single) As Single
	'		Dim A As Single
	'		Dim i As Integer
	'
	'
	'		A = 0
	'		For i = 1 To iDim
	'
	'			If LnU(i - 1) >= nTo Then Exit For
	'
	'			If LnU(i - 1) >= nFrom Then
	'
	'
	'				If LnU(i) <= nTo Then
	'
	'					'Add full segment
	'					A += (LnU(i) - LnU(i - 1)) * (LTq(i) + LTq(i - 1)) / 2
	'
	'				Else
	'
	'					'Add segment till nTo
	'					A += (nTo - LnU(i - 1)) * (Tq(nTo) + LTq(i - 1)) / 2
	'
	'				End If
	'
	'			Else
	'
	'				If LnU(i) > nFrom Then
	'
	'					'Add segment starting from nFrom
	'					A += (LnU(i) - nFrom) * (LTq(i) + Tq(nFrom)) / 2
	'
	'				End If
	'
	'			End If
	'
	'		Next
	'
	'		Return A
	'	End Function

	''' <summary>
	''' Calculates and returns engine speed at maximum power [1/min]. 
	''' </summary>
	''' <returns>engine speed at maximum power [1/min]</returns>
	''' <remarks></remarks>
	Public Function EngineRatedSpeed() As Single

		Dim stepSize As Single = 1
		Dim maxPower As Single = 0
		Dim rpm As Single = EngineSpeedList(0)
		Dim maxSpeed As Single = EngineSpeedList.Last()
		Dim ratedSpeed As Single = rpm

		Do
			Dim power As Single = nMtoPe(rpm, Torque(rpm))
			If power > maxPower Then
				maxPower = power
				ratedSpeed = rpm
			End If
			rpm += stepSize
		Loop Until rpm > maxSpeed

		Return ratedSpeed
	End Function


	''' <summary>
	''' Get or set Filepath before calling ReadFile
	''' </summary>
	''' <value></value>
	''' <returns>Full filepath</returns>
	''' <remarks></remarks>
	Public Property FilePath As String
End Class
