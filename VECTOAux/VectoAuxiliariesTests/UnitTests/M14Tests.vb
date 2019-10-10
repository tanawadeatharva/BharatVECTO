

Imports VectoAuxiliaries.Hvac
Imports NUnit.Framework
Imports Moq
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC


Namespace UnitTests
	'This implements the ISSMTOOL and returns 50% of the EngineHeatWaste as a Fueling Value
	'for the purpose of this test.
	Public Class SSMToolMock
		Implements ISSMTOOL

		Public Property Calculate As ISSMCalculate Implements ISSMTOOL.Calculate
		Public Property SSMDisabled As Boolean Implements ISSMTOOL.SSMDisabled
		Public Property HVACConstants As IHVACConstants Implements ISSMTOOL.HVACConstants

		'Public Sub Clone(from As ISSMTOOL) Implements ISSMTOOL.Clone
		'End Sub

		Public ReadOnly Property ElectricalWAdjusted As Watt Implements ISSMTOOL.ElectricalWAdjusted
			Get
				Throw New NotImplementedException
			End Get
		End Property

		Public ReadOnly Property ElectricalWBase As Watt Implements ISSMTOOL.ElectricalWBase
			Get
				Throw New NotImplementedException
			End Get
		End Property

		Public ReadOnly Property FuelPerHBase As KilogramPerSecond Implements ISSMTOOL.FuelPerHBase
			Get
				Throw New NotImplementedException
			End Get
		End Property

		Public ReadOnly Property FuelPerHBaseAdjusted As KilogramPerSecond Implements ISSMTOOL.FuelPerHBaseAdjusted
			Get
				Throw New NotImplementedException
			End Get
		End Property

		Public Function FuelPerHBaseAsjusted(AverageUseableEngineWasteHeatKW As Watt) As KilogramPerSecond _
		    Implements ISSMTOOL.FuelPerHBaseAsjusted

			Return (0.5 * (AverageUseableEngineWasteHeatKW.Value() * 0.835).SI(Unit.SI.Liter.Per.Hour).Value()).SI(of KilogramPerSecond)
		End Function

		Public Property GenInputs As ISSMGenInputs Implements ISSMTOOL.GenInputs

			Get
				Return New SSMGenInputs()
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

		Public ReadOnly Property MechanicalWBase As Watt Implements ISSMTOOL.MechanicalWBase
			Get
				Throw New NotImplementedException
			End Get
		End Property

		Public ReadOnly Property MechanicalWBaseAdjusted As Watt Implements ISSMTOOL.MechanicalWBaseAdjusted
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
			Dim ip1 As Double = 1000.0
			Dim ip5 As Double = 3114

			Dim expectedOut1 As Double = 1799.3334	' 780333.4 
			Dim expectedOut2 As Double = 2.13093

			Dim m13 As New Mock(Of IM13)
			Dim hvacSSM As New Mock(Of ISSMTOOL)
			Dim signals As New Mock(Of ISignals)
			Dim ssmMock As ISSMTOOL = New SSMToolMock()
			Dim constants As IHVACConstants = New HVACConstants(835.SI(Of KilogramPerCubicMeter))

			'Moq' Arrangements
			m13.Setup(Function(x) x.WHTCTotalCycleFuelConsumptionGrams).Returns((ip1 / 1000).SI(Of Kilogram))
			signals.Setup(Function(x) x.CurrentCycleTimeInSeconds).Returns(ip5)


			'Act
			Dim m14 As New M14Impl(m13.Object, ssmMock, constants, signals.Object)

			'Assert
            Assert.AreEqual(expectedOut1.SI(Unit.SI.Gramm).Value(), m14.TotalCycleFCGrams.Value(), 0.1)
            Assert.AreEqual(expectedOut2.SI(Of Liter).Value(), m14.TotalCycleFCLitres.Value(), 0.00001)
		End Sub
	End Class
End Namespace


