Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries

Public Class MockFuel50PC
	Implements IFuelConsumptionMap

	Public Function fFCdelaunay_Intp(nU As Double, Tq As Double) As Double

		Return (nU + Tq) * 0.5
	End Function


	Public Function GetFuelConsumption(torque As NewtonMeter, angularVelocity As Double) As KilogramPerSecond _
		Implements IFuelConsumptionMap.GetFuelConsumption
		Return (fFCdelaunay_Intp(angularVelocity, torque.Value()) / 3600 / 1000).SI(Of KilogramPerSecond)()
	End Function
End Class

