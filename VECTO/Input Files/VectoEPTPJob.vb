
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports System.Xml
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.FileIO.XML.Declaration
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Utils

<CustomValidation(GetType(VectoVTPJob), "ValidateJob")>
Public Class VectoVTPJob
	Implements IVTPInputDataProvider, IVTPJobInputData

	Private _sFilePath As String
	Private _myPath As String

    Private ReadOnly _vehicleFile As SubPath

    Public ReadOnly CycleFiles As List(Of SubPath)
    Public FanCoefficients As Double()
    Private _fanDiameter As Meter


    Public Sub New()
        CycleFiles = New List(Of SubPath)
        _vehicleFile = New SubPath
    End Sub

    Public Property FilePath As String
		Get
			Return _sFilePath
		End Get
		Set(value As String)
			_sFilePath = value
			If _sFilePath = "" Then
				_myPath = ""
			Else
				_myPath = Path.GetDirectoryName(_sFilePath) & "\"
			End If
		End Set
	End Property


	Public Property PathVeh(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _vehicleFile.OriginalPath
			Else
				Return _vehicleFile.FullPath
			End If
		End Get
		Set(value As String)
			_vehicleFile.Init(_myPath, value)
		End Set
	End Property


	Public Function SaveFile() As Boolean
		Dim validationResults As IList(Of ValidationResult) =
				Validate(ExecutionMode.Declaration, Nothing, False)

		If validationResults.Count > 0 Then
			Dim messages As IEnumerable(Of String) =
					validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
			MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
					"Failed to save Vecto Job")
			Return False
		End If

		Try
			Dim writer As JSONFileWriter = JSONFileWriter.Instance
			writer.SaveJob(Me, _sFilePath)
		Catch ex As Exception
			MsgBox("Failed to save Job file: " + ex.Message)
			Return False
		End Try
		Return True
	End Function

	' ReSharper disable once UnusedMember.Global -- used by Validation
	Public Shared Function ValidateJob(vectoJob As VectoVTPJob, validationContext As ValidationContext) As ValidationResult
		Dim modeService As VectoValidationModeServiceContainer =
				TryCast(validationContext.GetService(GetType(VectoValidationModeServiceContainer)), 
						VectoValidationModeServiceContainer)
		Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)

		Return ValidateVehicleJob(vectoJob, mode)
	End Function

	Private Shared Function ValidateVehicleJob(vectoJob As VectoVTPJob, mode As ExecutionMode) As ValidationResult

        ' TODO!!

    End Function

	Public ReadOnly Property Vehicle As IVehicleDeclarationInputData Implements IVTPJobInputData.Vehicle
		Get
			If Not File.Exists(_vehicleFile.FullPath) Then Return Nothing
            'Return New JSONComponentInputData(_vehicleFile.FullPath).JobInputData.Vehicle
            Return New XMLDeclarationInputDataProvider(_vehicleFile.FullPath, True).JobInputData.Vehicle
        End Get
	End Property

	Public ReadOnly Property Cycles As IList(Of ICycleData) Implements IVTPJobInputData.Cycles
		Get
			Dim retVal As ICycleData() = New ICycleData(CycleFiles.Count - 1) {}
			Dim i As Integer = 0
			For Each cycleFile As SubPath In CycleFiles
				Dim cycleData As TableData
				If (File.Exists(cycleFile.FullPath)) Then
					cycleData = VectoCSVFile.Read(cycleFile.FullPath)
				Else
					Try
						Dim resourceName As String = DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
													cycleFile.OriginalPath + TUGraz.VectoCore.Configuration.Constants.FileExtensions.CycleFile
						Dim cycleDataRes As Stream = RessourceHelper.ReadStream(resourceName)
						cycleData = VectoCSVFile.ReadStream(cycleDataRes, source:=resourceName)
					Catch ex As Exception
						Throw New VectoException("Driving Cycle could not be read: " + cycleFile.OriginalPath)
					End Try
				End If
				retVal(i) = New CycleInputData With {
					.Name = Path.GetFileNameWithoutExtension(cycleFile.FullPath),
					.CycleData = cycleData
					}
				i += 1
			Next
			Return retVal

		End Get
	End Property

    Public ReadOnly Property FanPowerCoefficents As IEnumerable(Of Double) Implements IVTPJobInputData.FanPowerCoefficents
        Get
            Return FanCoefficients
        End Get
    End Property

    Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IVTPJobInputData.SavedInDeclarationMode
        Get
            Return False
        End Get
    End Property

    Public Property FanDiameter As Meter Implements IVTPJobInputData.FanDiameter
        Get
            Return _fanDiameter
        End Get
        Set
            _fanDiameter = value
        End Set
    End Property

    Public ReadOnly Property JobInputData As IVTPJobInputData Implements IVTPInputDataProvider.JobInputData
        Get
            Return Me
        End Get
    End Property
End Class
