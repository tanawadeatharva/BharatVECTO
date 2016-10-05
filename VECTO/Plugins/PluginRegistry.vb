Imports System.Collections.Generic
Imports System.Linq
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.OutputData

Public Class PluginRegistry
	Private Shared _instance As PluginRegistry

	Private _exportPlugins As Dictionary(Of String, IExportPlugin) = New Dictionary(Of String, IExportPlugin)()
	Private _importPlugins As Dictionary(Of String, IImportPlugin) = New Dictionary(Of String, IImportPlugin)

	Public Shared ReadOnly Property Instance As PluginRegistry
		Get
			If _instance Is Nothing Then _instance = New PluginRegistry()
			Return _instance
		End Get
	End Property

	Public Sub RegisterPlugin(ByRef plugin As IExportPlugin)
		_exportPlugins.Add(plugin.Key, plugin)
	End Sub

	Public Sub RegisterPlugin(ByRef plugin As IImportPlugin)
		_importPlugins.Add(plugin.Key, plugin)
	End Sub

	Public Function GetExportPlugin(key As String) As IExportPlugin
		If Not _exportPlugins.ContainsKey(key) Then Return Nothing
		Return _exportPlugins(key)
	End Function

	Public Function GetImportPlugin(key As String) As IImportPlugin
		If Not _importPlugins.ContainsKey(key) Then Return Nothing
		Return _importPlugins(key)
	End Function

	'Public Function GetExportPluginList() As Dictionary(Of String, String)
	'	Return _exportPlugins.ToDictionary(Function(x) x.Key, Function(e) e.Value.Name)
	'End Function
End Class