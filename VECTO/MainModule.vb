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
Imports System.Reflection
Imports System.Runtime.Remoting
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.OutputData
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
Imports VectoAuxiliaries

''' <summary>
''' Main calculation routines.
''' </summary>
''' <remarks></remarks>
Module MainModule
	Public JobFileList As List(Of String)

	Public Function ConvertToEngineData(fld As EngineFullLoadCurve, nIdle As PerSecond) As CombustionEngineData

		Dim retVal As CombustionEngineData = New CombustionEngineData()
		retVal.FullLoadCurve = fld
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

	Public Sub DetectPlugins()

		Dim dllFileNames As String()
		Dim Path As String = "."
		'If Directory.Exists(Path) Then
		dllFileNames = Directory.GetFiles(Path, "*.dll")

		Dim assemblies As ICollection(Of Assembly) = New List(Of Assembly)(dllFileNames.Length)
		For Each dllFile As String In dllFileNames
			Dim an As AssemblyName = AssemblyName.GetAssemblyName(dllFile)
			Dim assembly As Assembly = assembly.Load(an)
			assemblies.Add(assembly)
		Next

		Dim exportPluginType As Type = GetType(IExportPlugin)
		Dim importPluginType As Type = GetType(IImportPlugin)
		Dim exportPluginTypes As ICollection(Of Type) = New List(Of Type)
		For Each assembly As Assembly In assemblies
			If assembly <> Nothing Then
				Dim types As Type() = assembly.GetTypes()

				For Each type As Type In types
					If type.IsInterface Or type.IsAbstract Then
						Continue For
					Else
						If type.GetInterface(exportPluginType.FullName) <> Nothing Then
							Dim plugin As IExportPlugin = TryCast(Activator.CreateInstance(type), IExportPlugin)
							If Not plugin Is Nothing Then
								PluginRegistry.Instance.RegisterPlugin(plugin)
							End If
						End If
						If type.GetInterface(importPluginType.FullName) <> Nothing Then
							Dim plugin As IImportPlugin = TryCast(Activator.CreateInstance(type), IImportPlugin)
							If Not plugin Is Nothing Then
								PluginRegistry.Instance.RegisterPlugin(plugin)
							End If
						End If
					End If
				Next
			End If
		Next

		'End If



	End Sub
End Module
