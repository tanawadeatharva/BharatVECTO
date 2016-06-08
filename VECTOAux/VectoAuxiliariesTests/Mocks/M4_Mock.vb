Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Public Class M4_Mock
	Implements IM4_AirCompressor


	Public Property _AveragePowerDemandPerCompressorUnitFlowRate As SI
	Public Property _FlowRate As NormLiterPerSecond
	Public Property _PowerCompressorOff As Watt
	Public Property _PowerCompressorOn As Watt
	Public Property _PowerDifference As Watt

	Public Function GetAveragePowerDemandPerCompressorUnitFlowRate() As SI Implements IM4_AirCompressor.GetAveragePowerDemandPerCompressorUnitFlowRate
		Return _AveragePowerDemandPerCompressorUnitFlowRate
	End Function

	Public Function GetFlowRate() As NormLiterPerSecond Implements IM4_AirCompressor.GetFlowRate
		Return _FlowRate
	End Function

	Public Function GetPowerCompressorOff() As Watt Implements IM4_AirCompressor.GetPowerCompressorOff
		Return _PowerCompressorOff
	End Function

	Public Function PowerCompressorOn() As Watt Implements IM4_AirCompressor.GetPowerCompressorOn
		Return _PowerCompressorOn
	End Function

	Public Function GetPowerDifference() As Watt Implements IM4_AirCompressor.GetPowerDifference
		Return _PowerDifference
	End Function


	Public Sub New()

	End Sub

	Public Sub New(AveragePowerDemandPerCompressorUnitFlowRate As Double, _
				   FlowRate As Double, _
				   PowerCompressorOff As Double, _
				   PowerCompressorOn As Double, _
				   PowerDifference As Double)

		_AveragePowerDemandPerCompressorUnitFlowRate = AveragePowerDemandPerCompressorUnitFlowRate.SI()
		_FlowRate = FlowRate.SI(Of NormLiterPerSecond)()
		_PowerCompressorOff = PowerCompressorOff.SI(Of Watt)()
		_PowerCompressorOn = PowerCompressorOn.SI(Of Watt)()
		_PowerDifference = PowerDifference.SI(Of Watt)()

	End Sub


	'Non Essential 
	Public Function Initialise() As Boolean Implements IM4_AirCompressor.Initialise
		Return True
	End Function

	Public Property PulleyGearEfficiency As Double Implements IM4_AirCompressor.PulleyGearEfficiency
	Public Property PulleyGearRatio As Double Implements IM4_AirCompressor.PulleyGearRatio



End Class

