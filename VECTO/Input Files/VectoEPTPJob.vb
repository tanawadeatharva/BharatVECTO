
Imports System.Collections.Generic
Imports System.IO
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.InputData

Public Class VectoEPTPJob
	Implements IEPTPInputDataProvider
	Private _sFilePath As String
	Private _myPath As String

	Private ReadOnly _vehicleFile As SubPath


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


	Public ReadOnly Property Vehicle As IDeclarationInputDataProvider Implements IEPTPInputDataProvider.Vehicle
		Get
		End Get
	End Property

	Public ReadOnly Property Cycles As IList(Of ICycleData) Implements IEPTPInputDataProvider.Cycles
		Get
		End Get
	End Property
End Class
