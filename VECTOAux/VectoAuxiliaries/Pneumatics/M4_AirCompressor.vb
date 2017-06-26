' Copyright 2017 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.
Imports TUGraz.VectoCommon.Utils

Namespace Pneumatics
	Public Class M4_AirCompressor
		Implements IM4_AirCompressor

		Private Const MinRatio As Double = 1
		Private Const MaxRatio As Double = 10
		Private Const MinEff As Double = 0
		Private Const MaxEff As Double = 1

		Private _pulleyGearRatio As Double
		Private _pulleyGearEfficiency As Double
		Private _map As ICompressorMap
		Private _signals As ISignals


		''' <summary>
		''' Ratio of Gear or Pulley used to drive the compressor
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property PulleyGearRatio() As Double Implements IM4_AirCompressor.PulleyGearRatio
			Get
				Return _pulleyGearRatio
			End Get
			Set(value As Double)
				If (value < MinRatio OrElse value > MaxRatio) Then
					Throw _
						New ArgumentOutOfRangeException("pulleyGearRatio", value,
														String.Format("Invalid value, should be in the range {0} to {1}", MinRatio, MaxRatio))
				Else
					_pulleyGearRatio = value
				End If
			End Set
		End Property

		''' <summary>
		''' Efficiency of the Pulley or Gear used to drive the compressor
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property PulleyGearEfficiency() As Double Implements IM4_AirCompressor.PulleyGearEfficiency
			Get
				Return _pulleyGearEfficiency
			End Get
			Set(value As Double)
				If (value < MinEff OrElse value > MaxEff) Then
					Throw _
						New ArgumentOutOfRangeException("pulleyGearEfficiency", value,
														String.Format("Invalid value, should be in the range {0} to {1}", MinEff, MaxEff)
														)
				Else
					_pulleyGearEfficiency = value
				End If
			End Set
		End Property


		'''<summary>
		''' Creates a new instance of the AirCompressor Class
		''' </summary>
		''' <param name="map">map of compressor values against compressor rpm</param>
		''' <param name="pulleyGearRatio">Ratio of Pulley/Gear</param>
		''' <param name="pulleyGearEfficiency">Efficiency of Pulley/Gear</param>
		''' <remarks></remarks>
		Public Sub New(ByVal map As ICompressorMap, ByRef pulleyGearRatio As Double, ByRef pulleyGearEfficiency As Double,
						signals As ISignals)

			_map = map
			_pulleyGearRatio = pulleyGearRatio
			_pulleyGearEfficiency = pulleyGearEfficiency
			_signals = signals
		End Sub

		''' <summary>
		''' Initialises the AirCompressor Class
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function Initialise() As Boolean Implements IM4_AirCompressor.Initialise
			Return _map.Initialise()
		End Function

		'Queryable Compressor Methods
		'
		'Compressor ( Speed ) Flow Rate 
		'Power @ Crank From Pnumatics compressor off ( A )
		'Power @ Crank From Pnumatics compressor On  ( B )
		'Power   Delta ( A ) vs ( B )


		'Return Average Power Demand Per Compressor Unit Flow Rate


		''' <summary>
		''' Returns the flow rate [litres/second] of compressor for the given engine rpm
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function GetFlowRate() As NormLiterPerSecond Implements IM4_AirCompressor.GetFlowRate
			Dim compressorRpm As Double = _signals.EngineSpeed.AsRPM * PulleyGearRatio

			''Flow Rate in the map is Litres/min so divide by 60 to get Units per second.
			Return _map.GetFlowRate(compressorRpm) / 60
		End Function

		''' <summary>
		''' Returns the power consumed for the given engine rpm when compressor is off
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function GetPowerCompressorOff() As Watt Implements IM4_AirCompressor.GetPowerCompressorOff
			Return GetCompressorPower(False)
		End Function

		''' <summary>
		''' Returns the power consumed for the given engine rpm when compressor is on
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function GetPowerCompressorOn() As Watt Implements IM4_AirCompressor.GetPowerCompressorOn
			Return GetCompressorPower(True)
		End Function

		''' <summary>
		''' Returns the difference in power between compressonr on and compressor off operation at the given engine rpm
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function GetPowerDifference() As Watt Implements IM4_AirCompressor.GetPowerDifference
			Dim powerOn As Watt = GetPowerCompressorOn()
			Dim powerOff As Watt = GetPowerCompressorOff()
			Return powerOn - powerOff
		End Function

		''' <summary>
		''' Looks up the compressor power from the map at given engine speed
		''' </summary>
		''' <param name="compressorOn">Is compressor on</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function GetCompressorPower(ByVal compressorOn As Boolean) As Watt
			Dim compressorRpm As Double = _signals.EngineSpeed.AsRPM * PulleyGearRatio
			If compressorOn Then
				Return _map.GetPowerCompressorOn(compressorRpm)
			Else
				Return _map.GetPowerCompressorOff(compressorRpm)
			End If
		End Function

		''' <summary>
		''' Aver
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function GetAveragePowerDemandPerCompressorUnitFlowRate() As SI _
			Implements IM4_AirCompressor.GetAveragePowerDemandPerCompressorUnitFlowRate

			Return _map.GetAveragePowerDemandPerCompressorUnitFlowRate().SI()
		End Function
	End Class
End Namespace