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

Public Interface IPneumaticsAuxilliariesConfig
	Property OverrunUtilisationForCompressionFraction As Double
	Property BrakingWithRetarderNIperKG As Double
	Property BrakingNoRetarderNIperKG As Double
	Property BreakingPerKneelingNIperKGinMM As Double
	Property PerDoorOpeningNI As Double
	Property PerStopBrakeActuationNIperKG As Double
	Property AirControlledSuspensionNIperMinute As Double
	Property AdBlueNIperMinute As Double
	Property NonSmartRegenFractionTotalAirDemand As Double
	Property SmartRegenFractionTotalAirDemand As Double
	Property DeadVolumeLitres As Double
	Property DeadVolBlowOutsPerLitresperHour As Double
End Interface
