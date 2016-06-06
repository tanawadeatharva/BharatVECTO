Imports TUGraz.VectoCommon.Utils

Public Interface IFuelConsumptionMap
	''' <summary>
	''' 
	''' </summary>
	''' <param name="torque"></param>
	''' <param name="angularVelocity"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Function GetFuelConsumption(torque As NewtonMeter, angularVelocity As PerSecond) As KilogramPerSecond
End Interface