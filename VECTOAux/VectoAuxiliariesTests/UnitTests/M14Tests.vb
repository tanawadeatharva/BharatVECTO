Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq


Namespace UnitTests
	'This implements the ISSMTOOL and returns 50% of the EngineHeatWaste as a Fueling Value
	'for the purpose of this test.
	Public Class SSMToolMock
		Implements ISSMTOOL

		Public Property Calculate As ISSMCalculate Implements ISSMTOOL.Calculate
		Public Property SSMDisabled As Boolean Implements ISSMTOOL.SSMDisabled
		Public Property HVACConstants As IHVACConstants Implements ISSMTOOL.HVACConstants

		Public Sub Clone(from As ISSMTOOL) Implements ISSMTOOL.Clone
		End Sub

		Public ReadOnly Property ElectricalWAdjusted As Single Implements ISSMTOOL.ElectricalWAdjusted
			Get
				Throw New NotImplementedException
			End Get
		End Property

		Public ReadOnly Property ElectricalWBase As Single Implements ISSMTOOL.ElectricalWBase
			Get
				Throw New NotImplementedException
			End Get
		End Property

		Public ReadOnly Property FuelPerHBase As Single Implements ISSMTOOL.FuelPerHBase
			Get
				Throw New NotImplementedException
			End Get
		End Property

		Public ReadOnly Property FuelPerHBaseAdjusted As Single Implements ISSMTOOL.FuelPerHBaseAdjusted
			Get
				Throw New NotImplementedException
			End Get
		End Property

		Public Function FuelPerHBaseAsjusted(AverageUseableEngineWasteHeatKW As Single) As Single _
			Implements ISSMTOOL.FuelPerHBaseAsjusted

			Return 0.5 * AverageUseableEngineWasteHeatKW
		End Function

		Public Property GenInputs As ISSMGenInputs Implements ISSMTOOL.GenInputs

			Get
				Return New SSMGenInputs(True)
			End Get
			Set(value As ISSMGenInputs)
			End Set
		End Property

		Public Function IsEqualTo(source As ISSMTOOL) As Boolean Implements ISSMTOOL.IsEqualTo
			Throw New NotImplementedException
		End Function

		Public Function Load(filePath As String) As Boolean Implements ISSMTOOL.Load
			Throw New NotImplementedException
		End Function

		Public ReadOnly Property MechanicalWBase As Single Implements ISSMTOOL.MechanicalWBase
			Get
				Throw New NotImplementedException
			End Get
		End Property

		Public ReadOnly Property MechanicalWBaseAdjusted As Single Implements ISSMTOOL.MechanicalWBaseAdjusted
			Get
				Throw New NotImplementedException
			End Get
		End Property

		Public Function Save(filePath As String) As Boolean Implements ISSMTOOL.Save
			Throw New NotImplementedException
		End Function

		Public Property TechList As ISSMTechList Implements ISSMTOOL.TechList

		Public Event Message(ByRef sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType) _
			Implements ISSMTOOL.Message
	End Class


	<TestFixture()>
	Public Class M14Tests
		<Test()>
		Public Sub ValuesTest()

			'Arrange
			Dim ip1 As Single = 1000.0F
			Dim ip5 As Single = 3114
			Dim ip6 As Single = 3114

			Dim expectedOut1 As Single = 780333.4F
			Dim expectedOut2 As Single = 0.934531F

			Dim m13 As New Mock(Of IM13)
			Dim hvacSSM As New Mock(Of ISSMTOOL)
			Dim signals As New Mock(Of ISignals)
			Dim ssmMock As ISSMTOOL = New SSMToolMock()
			Dim constants As IHVACConstants = New HVACConstants(835)

			'Moq' Arrangements
			m13.Setup(Function(x) x.WHTCTotalCycleFuelConsumptionGrams).Returns(ip1)
			signals.Setup(Function(x) x.CurrentCycleTimeInSeconds).Returns(ip5)


			'Act
			Dim m14 As New M14(m13.Object, ssmMock, constants, signals.Object)

			'Assert
			Assert.AreEqual(expectedOut1, m14.TotalCycleFCGrams)
			Assert.AreEqual(expectedOut2, m14.TotalCycleFCLitres)
		End Sub
	End Class
End Namespace


