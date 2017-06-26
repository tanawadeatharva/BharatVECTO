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
Imports System.Collections.Generic
Imports System.IO
Imports System.Reflection
Imports System.Runtime.Remoting
Imports TUGraz.VectoCommon
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.OutputData
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
Imports VectoAuxiliaries

''' <summary>
''' Main calculation routines.
''' </summary>
''' <remarks></remarks>
Module MainModule
	Public JobFileList As List(Of String)

	Public Function ConvertToEngineData(ByVal fld As EngineFullLoadCurve, ByVal nIdle As PerSecond, ByVal gear As Integer,
										ByVal maxTorque As NewtonMeter) As CombustionEngineData

		Dim retVal As CombustionEngineData = New CombustionEngineData()
		Dim fullLoadCurves As Dictionary(Of UInteger, EngineFullLoadCurve) = New Dictionary(Of UInteger, EngineFullLoadCurve)
		fullLoadCurves(0) = fld
		fullLoadCurves(CType(gear, UInteger)) = AbstractSimulationDataAdapter.IntersectFullLoadCurves(fld, maxTorque)
		retVal.FullLoadCurves = fullLoadCurves
		retVal.IdleSpeed = nIdle
		Return retVal
	End Function

	Public Function ConvPicPath(hdVclass As Integer, isLongHaul As Boolean) As Bitmap
		Select Case hdVclass
			Case 1, 2, 3
				Return My.Resources._4x2r ' resourcePath & "4x2r.png"
			Case 4
				If isLongHaul Then Return My.Resources._4x2rt
				Return My.Resources._4x2r 'resourcePath & "4x2r" & longHaulFlag & ".png"
			Case 5
				Return My.Resources._4x2tt ' resourcePath & "4x2tt.png"
			Case 9
				If isLongHaul Then Return My.Resources._6x2rt
				Return My.Resources._6x2r ' resourcePath & "6x2r" & longHaulFlag & ".png"
			Case 10
				Return My.Resources._6x2tt ' resourcePath & "6x2tt.png"
			Case Else
				Return My.Resources.Undef  ' resourcePath & "Undef.png"
		End Select
	End Function

	Public Function GetRelativePath(filePath As String, basePath As String) As String
		If (String.IsNullOrEmpty(filePath) OrElse String.IsNullOrEmpty(basePath)) Then
			Return ""
		End If
		If (Path.GetDirectoryName(filePath).StartsWith(basePath, StringComparison.OrdinalIgnoreCase)) Then
			Return Path.GetFullPath(filePath).Substring(basePath.Length + If(basePath.EndsWith("\"), 0, 1))
		End If
		Return filePath
	End Function
End Module
