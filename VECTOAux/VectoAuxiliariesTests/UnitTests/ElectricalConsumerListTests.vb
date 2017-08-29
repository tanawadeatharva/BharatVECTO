Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries.Electrics

<TestFixture()>
Public Class ElectricalConsumerListTests
	Private TestConsumerList As ElectricalConsumerList = New ElectricalConsumerList(26.3, 0.096, True)


	Sub New()
	End Sub


	<Test()>
	Public Sub CreateNewTest()

		Dim target As New ElectricalConsumerList(26.3, 0.096, True)

		Assert.IsNotNull(target)
	End Sub


	<Test()>
	Public Sub SumAllConsumersTest()

		TestConsumerList.Items.First(Function(item) item.ConsumerName = "Controllers,Valves etc").NumberInActualVehicle = 1

		Dim actual As Ampere = TestConsumerList.GetTotalAverageDemandAmps(False)

		TestConsumerList.Items.First(Function(item) item.ConsumerName = "Controllers,Valves etc").NumberInActualVehicle = 0

		Dim expected = 60.63

		Assert.AreEqual(expected, actual.Value(), 0.01)
	End Sub

	<Test()>
	Public Sub SumNonExcludedConsumersTest()

		TestConsumerList.Items.First(Function(item) item.ConsumerName = "Controllers,Valves etc").NumberInActualVehicle = 1
		Dim actual As Ampere = TestConsumerList.GetTotalAverageDemandAmps(True)
		TestConsumerList.Items.First(Function(item) item.ConsumerName = "Controllers,Valves etc").NumberInActualVehicle = 0
		Dim expected = 35.63
		Assert.AreEqual(expected, actual.Value(), 0.01)
	End Sub


    <Test()>
    Public Sub DuplicateConsumersTest_ThrowsArgumentException()

        Dim target As New ElectricalConsumerList(0.096, 26.3)
        'Add two OnBaseVehicle consumers
        target.AddConsumer(New ElectricalConsumer(True, "TEST", "Exclude1", 10, 1, 26.3, 1, ""))
        Assert.That(Sub() target.AddConsumer(New ElectricalConsumer(True, "TEST", "Exclude1", 10, 1, 26.3, 1, "")), Throws.InstanceOf(Of System.ArgumentException))


    End Sub
End Class
