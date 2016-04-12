Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries

public class MockFuel50PC
	Implements IFuelConsumptionMap

	Public Function fFCdelaunay_Intp(nU As Single, Tq As Single) As Single

		Return (nU + Tq) * 0.5
	End Function


	Public Function GetFuelConsumption(torque As Double, angularVelocity As Double) As Double _
		Implements IFuelConsumptionMap.GetFuelConsumption
		Return fFCdelaunay_Intp(angularVelocity, torque)
	End Function
End Class

