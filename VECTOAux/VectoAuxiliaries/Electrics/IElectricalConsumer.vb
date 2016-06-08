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

Imports System.ComponentModel
Imports TUGraz.VectoCommon.Utils

Namespace Electrics
	Public Interface IElectricalConsumer
		Inherits INotifyPropertyChanged

		Property Category As String
		Property ConsumerName As String
		Property BaseVehicle As Boolean
		Property NominalConsumptionAmps As Double
		Property PhaseIdle_TractionOn As Double
		Property NumberInActualVehicle As Integer
		Property PowerNetVoltage As Double
		Property AvgConsumptionAmps As Double
		Property Info As String
		Function TotalAvgConumptionAmps(Optional PhaseIdle_TractionOnBasedOnCycle As Double = Nothing) As Ampere
		Function TotalAvgConsumptionInWatts(Optional PhaseIdle_TractionOnBasedOnCycle As Double = 0.0) As Watt
	End Interface
End Namespace