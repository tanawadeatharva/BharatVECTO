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

Namespace Pneumatics
	Public Class PneumaticsAuxilliariesConfig
		Implements IPneumaticsAuxilliariesConfig

		Public Property AdBlueNIperMinute As Double Implements IPneumaticsAuxilliariesConfig.AdBlueNIperMinute

		Public Property AirControlledSuspensionNIperMinute As Double _
			Implements IPneumaticsAuxilliariesConfig.AirControlledSuspensionNIperMinute

		Public Property BrakingNoRetarderNIperKG As Double Implements IPneumaticsAuxilliariesConfig.BrakingNoRetarderNIperKG

		Public Property BrakingWithRetarderNIperKG As Double _
			Implements IPneumaticsAuxilliariesConfig.BrakingWithRetarderNIperKG

		Public Property BreakingPerKneelingNIperKGinMM As Double _
			Implements IPneumaticsAuxilliariesConfig.BreakingPerKneelingNIperKGinMM

		Public Property DeadVolBlowOutsPerLitresperHour As Double _
			Implements IPneumaticsAuxilliariesConfig.DeadVolBlowOutsPerLitresperHour

		Public Property DeadVolumeLitres As Double Implements IPneumaticsAuxilliariesConfig.DeadVolumeLitres

		Public Property NonSmartRegenFractionTotalAirDemand As Double _
			Implements IPneumaticsAuxilliariesConfig.NonSmartRegenFractionTotalAirDemand

		Public Property OverrunUtilisationForCompressionFraction As Double _
			Implements IPneumaticsAuxilliariesConfig.OverrunUtilisationForCompressionFraction

		Public Property PerDoorOpeningNI As Double Implements IPneumaticsAuxilliariesConfig.PerDoorOpeningNI

		Public Property PerStopBrakeActuationNIperKG As Double _
			Implements IPneumaticsAuxilliariesConfig.PerStopBrakeActuationNIperKG

		Public Property SmartRegenFractionTotalAirDemand As Double _
			Implements IPneumaticsAuxilliariesConfig.SmartRegenFractionTotalAirDemand


		Public Sub New(Optional setToDefaults As Boolean = False)

			If setToDefaults Then SetDefaults()
		End Sub

		Public Sub SetDefaults()
			AdBlueNIperMinute = 21.25
			AirControlledSuspensionNIperMinute = 15
			BrakingNoRetarderNIperKG = 0.00081
			BrakingWithRetarderNIperKG = 0.0006
			BreakingPerKneelingNIperKGinMM = 0.000066
			DeadVolBlowOutsPerLitresperHour = 24
			DeadVolumeLitres = 30
			NonSmartRegenFractionTotalAirDemand = 0.26
			OverrunUtilisationForCompressionFraction = 0.97
			PerDoorOpeningNI = 12.7
			PerStopBrakeActuationNIperKG = 0.00064
			SmartRegenFractionTotalAirDemand = 0.12
		End Sub
	End Class
End Namespace


