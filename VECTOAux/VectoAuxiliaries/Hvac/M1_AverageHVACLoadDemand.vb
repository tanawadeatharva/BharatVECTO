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
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics

Namespace Hvac
	Public Class M1_AverageHVACLoadDemand
		Implements IM1_AverageHVACLoadDemand

		Private _m0 As IM0_NonSmart_AlternatorsSetEfficiency
		Private _alternatorGearEfficiency As Double
		Private _compressorGearEfficiency As Double
		Private _signals As ISignals
		Private _powernetVoltage As Volt
		Private _steadyStateModel As ISSMTOOL

		Private _ElectricalPowerW As Watt
		Private _MechanicalPowerW As Watt
		Private _FuelingLPerH As LiterPerSecond


		'Constructor
		Public Sub New(m0 As IM0_NonSmart_AlternatorsSetEfficiency, altGearEfficiency As Double,
						compressorGearEfficiency As Double, powernetVoltage As Volt, signals As ISignals, ssm As ISSMTOOL)

			'Sanity Check - Illegal operations without all params.
			If m0 Is Nothing Then Throw New ArgumentException("Module0 as supplied is null")

			If _
				altGearEfficiency < ElectricConstants.AlternatorPulleyEfficiencyMin OrElse
				altGearEfficiency > ElectricConstants.AlternatorPulleyEfficiencyMax Then _
				Throw _
					New ArgumentException(String.Format("Gear efficiency must be between {0} and {1}",
														ElectricConstants.AlternatorPulleyEfficiencyMin, ElectricConstants.AlternatorPulleyEfficiencyMax))

			If signals Is Nothing Then Throw New Exception("Signals object as supplied is null")
			If powernetVoltage < ElectricConstants.PowenetVoltageMin OrElse powernetVoltage > ElectricConstants.PowenetVoltageMax _
				Then _
				Throw _
					New ArgumentException(String.Format("PowenetVoltage supplied must be in the range {0} to {1}",
														ElectricConstants.PowenetVoltageMin, ElectricConstants.PowenetVoltageMax))
			If ssm Is Nothing Then Throw New ArgumentException("Steady State model was not supplied")
			If compressorGearEfficiency < 0 OrElse altGearEfficiency > 1 Then _
				Throw New ArgumentException(String.Format("Compressor Gear efficiency must be between {0} and {1}", 0, 1))


			'Assign
			_m0 = m0
			_alternatorGearEfficiency = altGearEfficiency
			_signals = signals

			_compressorGearEfficiency = compressorGearEfficiency
			_powernetVoltage = powernetVoltage


			_steadyStateModel = ssm

			_ElectricalPowerW = ssm.ElectricalWAdjusted.SI(Of Watt)()
			_MechanicalPowerW = ssm.MechanicalWBaseAdjusted.SI(Of Watt)()
		    _FuelingLPerH = ssm.FuelPerHBaseAdjusted.SI(Unit.SI.Liter.Per.Hour).Cast(Of LiterPerSecond)() ' SI(Of LiterPerHour)()
			'_FuelingLPerH = ssm.FuelPerHBaseAdjusted.SI().Liter.Per.Hour.Cast(Of LiterPerSecond)() ' SI(Of LiterPerHour)()
		End Sub

		'Public Methods - Implementation
		Public Function AveragePowerDemandAtCrankFromHVACMechanicalsWatts() As Watt _
			Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACMechanicalsWatts

			Return _MechanicalPowerW * (1 / _compressorGearEfficiency)
		End Function

		Public Function AveragePowerDemandAtAlternatorFromHVACElectricsWatts() As Watt _
			Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtAlternatorFromHVACElectricsWatts

			Return _ElectricalPowerW
		End Function

		Public Function AveragePowerDemandAtCrankFromHVACElectricsWatts() As Watt _
			Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACElectricsWatts

			Return _ElectricalPowerW * (1 / _m0.AlternatorsEfficiency() / _alternatorGearEfficiency)
		End Function

		Public Function HVACFuelingLitresPerHour() As LiterPerSecond _
			Implements IM1_AverageHVACLoadDemand.HVACFuelingLitresPerHour

			Return _FuelingLPerH
		End Function
	End Class
End Namespace