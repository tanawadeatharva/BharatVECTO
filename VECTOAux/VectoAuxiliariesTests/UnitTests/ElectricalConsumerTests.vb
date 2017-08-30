Imports VectoAuxiliaries.Electrics
Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils

Namespace UnitTests
	<TestFixture()>
	Public Class ElectricalConsumerTests

#Region "Helpers"

		Public Function GetGoodConsumer() As ElectricalConsumer
			Return New ElectricalConsumer(False, "Doors", "Doors per Door", 20, 0.5, 26.3, 1, "")
		End Function

#End Region


		<Test()>
		Public Sub CreateNewTest()
			Dim target As ElectricalConsumer = GetGoodConsumer()
			Assert.IsNotNull(target)
		End Sub

        '  <Test(), ExpectedException("System.ArgumentException")>

        <Test()>
        Public Sub ZeroLengthConsumerNameTest()
            Dim target As ElectricalConsumer
            Assert.That(Sub() target = New ElectricalConsumer(False, "Doors", "", 20, 0.5, 26.3, 1, ""), Throws.InstanceOf(Of ArgumentException))

        End Sub

        <Test()>
        Public Sub ZeroLengthCategoryNameTest_ThrowsArgumentException()
            Dim target As ElectricalConsumer
            Assert.That(Sub() target = New ElectricalConsumer(False, "", "Doors per Door", 20, 0.5, 26.3, 1, ""), Throws.InstanceOf(Of ArgumentException))
        End Sub


        'TooLow     NominalConsumption
        'TooHigh    NominalConsumption


        <Test()>
        Public Sub ToLow_PhaseIdleTractionOn_ThrowsArgumentException()
            Dim target As ElectricalConsumer
            Assert.That(Sub() target = New ElectricalConsumer(False, "", "Doors per Door", 20, ElectricConstants.PhaseIdleTractionOnMin - 1,
                                            26.3, 1, ""), Throws.InstanceOf(Of ArgumentException))

        End Sub

        <Test()>
        Public Sub ToHigh_PhaseIdleTractionOn_ThrowsArgumentException()
            Dim target As ElectricalConsumer
            Assert.That(Sub() target = New ElectricalConsumer(False, "", "Doors per Door", 20, ElectricConstants.PhaseIdleTractionMax + 1,
                                            26.3, 1, ""), Throws.InstanceOf(Of ArgumentException))
        End Sub


        <Test()>
        Public Sub ToLowNumberInVehicle_ThrowsArgumentException()
            Dim target As ElectricalConsumer
            Assert.That(Sub() target = New ElectricalConsumer(False, "", "Doors per Door", 20, 0.5, 26.3, -1, ""), Throws.InstanceOf(Of ArgumentException))
        End Sub

        'TooLow     PowerNetVoltage
        <Test()>
        Public Sub ToLowPowerNetVoltageTest_ThrowsArgumentException()
            Dim target As ElectricalConsumer
            Assert.That(Sub() target = New ElectricalConsumer(False, "", "Doors per Door", 20, 0.5,
                                            (ElectricConstants.PowenetVoltageMin - 1), 1, ""), Throws.InstanceOf(Of ArgumentException))

        End Sub

        'TooHigh    PowerNetVoltage
        <Test()>
        Public Sub ToHighPowerNetVoltageTest_ThrowsArgumentException()
            Dim target As ElectricalConsumer
            Assert.That(Sub() target = New ElectricalConsumer(False, "", "Doors per Door", 20, 0.5,
                                            (ElectricConstants.PowenetVoltageMax + 1), 1, ""), Throws.InstanceOf(Of ArgumentException))

        End Sub
	End Class
End Namespace