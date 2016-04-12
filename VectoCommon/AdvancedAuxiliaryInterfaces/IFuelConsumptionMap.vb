Public Interface IFuelConsumptionMap
	''' <summary>
	''' 
	''' </summary>
	''' <param name="torque"></param>
	''' <param name="angularVelocity"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Function GetFuelConsumption(torque As Double, angularVelocity As Double) As Double
End Interface