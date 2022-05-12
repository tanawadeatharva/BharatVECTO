Imports TUGraz.VectoCore.InputData.FileIO.JSON

Public Class IEPCForm
    Public IEPCFilePath As String = ""

    Public Sub ReadIEPCFile(file As String)
        Dim inputData = JSONInputDataFactory.ReadIEPCEngineeringInputData(file, true)
      
        

    End Sub
End Class