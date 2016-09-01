'AA-TB

Public Class cAdvancedAuxiliary
	'Private properties. Set on Constructor
	Private ReadOnly _auxiliaryName As String
	Private ReadOnly _auxiliaryVersion As String
	Private ReadOnly _fileName As String
	Private ReadOnly _assemblyName As String


	'Public Readonly properties
	Public ReadOnly Property AuxiliaryName As String
		Get
			Return _AuxiliaryName
		End Get
	End Property

	Public ReadOnly Property AuxiliaryVersion As String
		Get
			Return _AuxiliaryVersion
		End Get
	End Property

	Public ReadOnly Property FileName As String
		Get
			Return _FileName
		End Get
	End Property

	Public ReadOnly Property AssemblyName As String
		Get
			Return _AssemblyName
		End Get
	End Property


	'Constructor(s)

	Public Sub New()

		_AuxiliaryName = "Classic Vecto Auxiliary"
		_AuxiliaryVersion = "CLASSIC"
		_FileName = "CLASSIC"
		_AssemblyName = "CLASSIC"
	End Sub

	Public Sub New(AuxiliaryName As String, AuxiliaryVersion As String, FileName As String, AssemblyName As String)

		_AuxiliaryName = AuxiliaryName
		_AuxiliaryVersion = AuxiliaryVersion
		_FileName = FileName
		_AssemblyName = AssemblyName
	End Sub
End Class
