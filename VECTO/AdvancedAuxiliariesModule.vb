Imports System.Collections.Generic
Imports VectoAuxiliaries
Imports System.IO
Imports System.Linq
Imports System.Runtime.Remoting

Module AdvancedAuxiliariesModule
	Public WithEvents AdvancedAuxModel As IAdvancedAuxiliaries

	Dim _returnList As Dictionary(Of String, AdvancedAuxiliary) = Nothing

	Public Sub AAEventAuxiliaryEvent(ByRef sender As Object, ByVal message As String,
									ByVal messageType As AdvancedAuxiliaryMessageType) Handles AdvancedAuxModel.AuxiliaryEvent


		WorkerMsg(CType(messageType, MessageType), message, "Advanced Auxiliaries")
	End Sub

	'AA-TB

	'AA-TB
	''' <summary>
	''' Discovers Advanced Auxiliaries Assemblies in 'targetDirectory' Directory
	''' </summary>
	''' <returns>List(Of cAdvancedAuxiliary)</returns>
	''' <remarks>Target Directory would normally be the executing directory, but can be in another location.</remarks>
	Public Function DiscoverAdvancedAuxiliaries() As Dictionary(Of String, AdvancedAuxiliary)

		If Not _returnList Is Nothing Then
			Return _returnList
		End If

		_returnList = New Dictionary(Of String, AdvancedAuxiliary)
		Dim o As ObjectHandle
		Dim iAdvancedAux As IAdvancedAuxiliaries


		'Create Default
		Dim classicAux As AdvancedAuxiliary = New AdvancedAuxiliary()
		_returnList.Add(classicAux.AssemblyName, classicAux)


		Try
			Dim fileEntries As String() = Directory.GetFiles(GetAAUXSourceDirectory)
			' Process the list of files found in the directory. 
			Dim fileName As String

			For Each fileName In fileEntries

				If fileName.Contains("Auxiliaries.dll") Then

					'Get filenamewith
					Dim fileNameWoPath As String = GetFilenameWithoutPath(fileName, True)
					Dim fileNameWoExtentsion As String = GetFilenameWithoutPath(fileName, False)

					o = Activator.CreateInstance(fileNameWoExtentsion, "VectoAuxiliaries.AdvancedAuxiliaries")

					iAdvancedAux = DirectCast(o.Unwrap, IAdvancedAuxiliaries)

					Dim advancedAuxiliary As AdvancedAuxiliary = New AdvancedAuxiliary(iAdvancedAux.AuxiliaryName,
																						iAdvancedAux.AuxiliaryVersion,
																						fileNameWoPath, fileNameWoExtentsion)
					_returnList.Add(advancedAuxiliary.AuxiliaryVersion, advancedAuxiliary)
				End If

			Next fileName

		Catch ex As Exception
			MessageBox.Show("Unable to obtain Advanced Auxiliary Assemblies")
		End Try


		Return _returnList
	End Function

	'AA-TB
	''' <summary>
	''' Invokes Advanced Auxiliaries Configuration Screen
	''' </summary>
	''' <param name="vectoFilePath">String : Contains the path of the vecto file.</param>
	''' <returns>Boolean. True if aauxFile is valid after operation , false of not.</returns>
	''' <remarks></remarks>
	Public Function ConfigureAdvancedAuxiliaries(ByVal assemblyName As String, ByVal version As String, filePath As String,
												vectoFilePath As String) As Boolean


		Dim auxList As Dictionary(Of String, AdvancedAuxiliary) = DiscoverAdvancedAuxiliaries()
		Dim o As ObjectHandle
		Dim iAdvancedAux As IAdvancedAuxiliaries
		Dim result As Boolean

		Dim chosenAssembly As KeyValuePair(Of String, AdvancedAuxiliary) =
				auxList.First(Function(x) x.Value.AssemblyName = assemblyName AndAlso x.Value.AuxiliaryVersion = version)

		If String.IsNullOrEmpty(chosenAssembly.Value.AssemblyName) Then Return False

		'Open Assembly and invoke the configuration using the paths supplied.

		Try
			o = Activator.CreateInstance(chosenAssembly.Value.AssemblyName, "VectoAuxiliaries.AdvancedAuxiliaries")
			iAdvancedAux = DirectCast(o.Unwrap, IAdvancedAuxiliaries)

			iAdvancedAux.Configure(filePath, vectoFilePath)

		Catch ex As Exception

			result = False

		End Try

		Return result
	End Function


	''' <summary>
	''' Gets location of Advanced Auxiliaries Directory which contains all the assemblies available.
	''' </summary>
	''' <returns>Path where Auxiliaries can be found : String</returns>
	''' <remarks></remarks>
	Public Function GetAAUXSourceDirectory() As String


		Return Path.GetDirectoryName(Application.ExecutablePath)
	End Function


	Public Function ResolveAAUXFilePath(vectoPath As String, filename As String) As String

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


	Public Function ValidateAAUXFile(ByVal absoluteAAuxPath As String,
									ByVal assemblyName As String,
									ByVal version As String,
									ByRef message As String) As Boolean

		Dim auxList As Dictionary(Of String, AdvancedAuxiliary) = DiscoverAdvancedAuxiliaries()
		Dim o As ObjectHandle
		Dim iAdvancedAux As IAdvancedAuxiliaries
		Dim result As Boolean


		Dim chosenAssembly As KeyValuePair(Of String, AdvancedAuxiliary) =
				auxList.First(Function(x) x.Value.AssemblyName = assemblyName AndAlso x.Value.AuxiliaryVersion = version)

		If String.IsNullOrEmpty(chosenAssembly.Value.AssemblyName) Then Return False


		'Open Assembly and invoke the validation using the paths supplied.
		Try
			o = Activator.CreateInstance(chosenAssembly.Value.AssemblyName, "VectoAuxiliaries.AdvancedAuxiliaries")
			iAdvancedAux = DirectCast(o.Unwrap, IAdvancedAuxiliaries)

			result = iAdvancedAux.ValidateAAUXFile(absoluteAAuxPath, message)

		Catch ex As Exception

			result = False

		End Try

		Return result
	End Function
End Module
