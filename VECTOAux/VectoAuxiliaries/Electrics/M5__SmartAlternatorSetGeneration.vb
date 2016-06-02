' Copyright 2015 European Union.
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

Namespace Electrics
	Public Class M5__SmartAlternatorSetGeneration
		Implements IM5_SmartAlternatorSetGeneration

		Private _powerNetVoltage As Volt
		Private _m05 As M0_5_SmartAlternatorSetEfficiency
		Private _alternatorGearEfficiency As Double

		'Constructor
		Public Sub New(m05 As M0_5_SmartAlternatorSetEfficiency, ByVal powernetVoltage As Volt,
						alternatorGearEfficiency As Double)

			'sanity check
			If m05 Is Nothing Then Throw New ArgumentException("Please supply a valid module M05")
			If powernetVoltage < ElectricConstants.PowenetVoltageMin OrElse powernetVoltage > ElectricConstants.PowenetVoltageMax _
				Then Throw New ArgumentException("Powernet Voltage out of range")
			If alternatorGearEfficiency < 0 Or alternatorGearEfficiency > 1 Then _
				Throw New ArgumentException("AlternatorGearEfficiency Out of bounds, should be 0 to 1")

			'assign private variables.
			_m05 = m05
			_powerNetVoltage = powernetVoltage
			_alternatorGearEfficiency = alternatorGearEfficiency
		End Sub

		'Public class outputs (Functions)
		Public Function AlternatorsGenerationPowerAtCrankIdleWatts() As Watt _
			Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankIdleWatts

			Return _
				(_m05.SmartIdleCurrent() * _powerNetVoltage) *
				(1 / (_m05.AlternatorsEfficiencyIdleResultCard() * _alternatorGearEfficiency))
		End Function

		Public Function AlternatorsGenerationPowerAtCrankOverrunWatts() As Watt _
			Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankOverrunWatts

			Return _
				(_m05.SmartOverrunCurrent() * _powerNetVoltage) * (1 /
																(_m05.AlternatorsEfficiencyOverrunResultCard() * _alternatorGearEfficiency))
		End Function

		Public Function AlternatorsGenerationPowerAtCrankTractionOnWatts() As Watt _
			Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankTractionOnWatts

			Return _
				(_m05.SmartTractionCurrent() * _powerNetVoltage) * (1 /
																(_m05.AlternatorsEfficiencyTractionOnResultCard() * _alternatorGearEfficiency))
		End Function
	End Class
End Namespace


