Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries.Hvac

Public Interface ISSMTOOL
	Property GenInputs As ISSMGenInputs
	Property TechList As ISSMTechList
	Property Calculate As ISSMCalculate
	Property SSMDisabled As Boolean
	Property HVACConstants As IHVACConstants

	ReadOnly Property ElectricalWBase As Double	'Watt
	ReadOnly Property MechanicalWBase As Double	' Watt
	ReadOnly Property FuelPerHBase As Double ' LiterPerHour

	ReadOnly Property ElectricalWAdjusted As Double	' Watt
	ReadOnly Property MechanicalWBaseAdjusted As Double	' Watt
	ReadOnly Property FuelPerHBaseAdjusted As Double ' LiterPerHour

	Sub Clone(from As ISSMTOOL)

	Function Load(filePath As String) As Boolean

	Function Save(filePath As String) As Boolean

	Function IsEqualTo(source As ISSMTOOL) As Boolean

	''' <summary>
	''' This alters the waste heat and returns an adjusted fueling value
	''' </summary>
	''' <param name="AverageUseableEngineWasteHeatKW"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Function FuelPerHBaseAsjusted(AverageUseableEngineWasteHeatKW As Double) As Double

	Event Message(ByRef sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType)
End Interface
