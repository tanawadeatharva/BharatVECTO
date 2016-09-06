' Copyright 2016 European Union.
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
Imports TUGraz.VectoCore.Models.Declaration

Public Class Configuration
	Public FilePath As String
	Public GnUfromCycle As Boolean
	Public ModOut As Boolean
	Public Mod1Hz As Boolean
	Public LogSize As Single
	Public AirDensity As Single
	Public OpenCmd As String
	Public OpenCmdName As String
	Public FuelDens As Single
	Public CO2perFC As Single
	Public FirstRun As Boolean
	Public DeclMode As Boolean

	Private Const FormatVersion As Short = 2

	Public Sub New()
		SetDefault()
	End Sub

	Public Sub DeclInit()
		AirDensity = DeclarationData.Physics.AirDensity.Value()	' cDeclaration.AirDensity
		FuelDens = DeclarationData.Physics.FuelDensity.Value()	' cDeclaration.FuelDens
		CO2perFC = DeclarationData.Physics.CO2PerFuelWeight		' cDeclaration.CO2perFC
		GnUfromCycle = False
	End Sub

	Public Sub SetDefault()
		GnUfromCycle = True
		ModOut = True
		Mod1Hz = False
		LogSize = 2
		AirDensity = 1.2
		OpenCmd = "notepad"
		OpenCmdName = "Notepad"
		FuelDens = DeclarationData.Physics.FuelDensity.Value()
		CO2perFC = DeclarationData.Physics.CO2PerFuelWeight
		FirstRun = True
		DeclMode = True
	End Sub

	Public Sub Load()
		SetDefault()

		If Not File.Exists(FilePath) Then
			Exit Sub
		End If

		Dim json As New JSONParser
		If Not json.ReadFile(FilePath) Then
			GUImsg(MessageType.Err, "Failed to load settings! Using default settings.")
			Exit Sub
		End If

		Try
			Try
				Mod1Hz = json.Content("Body")("Mod1Hz")
			Catch
			End Try

			ModOut = json.Content("Body")("ModOut")
			GnUfromCycle = json.Content("Body")("UseGnUfromCycle")
			LogSize = json.Content("Body")("LogSize")
			AirDensity = json.Content("Body")("AirDensity")
			FuelDens = json.Content("Body")("FuelDensity")
			CO2perFC = json.Content("Body")("CO2perFC")
			OpenCmd = json.Content("Body")("OpenCmd")
			OpenCmdName = json.Content("Body")("OpenCmdName")
			FirstRun = json.Content("Body")("FirstRun")
			DeclMode = json.Content("Body")("DeclMode")
		Catch ex As Exception
			GUImsg(MessageType.Err, "Error while loading settings!")
		End Try
	End Sub

	Public Sub Save()
		Dim json As New JSONParser
		Dim dic As Dictionary(Of String, Object)

		dic = New Dictionary(Of String, Object)
		dic.Add("CreatedBy", Lic.LicString & " (" & Lic.GUID & ")")
		dic.Add("Date", Now.ToUniversalTime().ToString("o"))
		dic.Add("AppVersion", VECTOvers)
		dic.Add("FileVersion", FormatVersion)
		json.Content.Add("Header", dic)

		dic = New Dictionary(Of String, Object)
		dic.Add("ModOut", ModOut)
		dic.Add("Mod1Hz", Mod1Hz)
		dic.Add("LogSize", LogSize)
		dic.Add("AirDensity", AirDensity)
		dic.Add("FuelDensity", FuelDens)
		dic.Add("CO2perFC", CO2perFC)
		dic.Add("OpenCmd", OpenCmd)
		dic.Add("OpenCmdName", OpenCmdName)
		dic.Add("FirstRun", FirstRun)
		dic.Add("DeclMode", DeclMode)
		json.Content.Add("Body", dic)

		json.WriteFile(FilePath)
	End Sub
End Class

