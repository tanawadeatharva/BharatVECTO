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
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac

Namespace DownstreamModules
	Public Interface IM11
		''' <summary>
		''' Smart Electrical Total Cycle Electrical Energy Generated During Overrun Only(J)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly As Joule

		''' <summary>
		''' Smart Electrical Total Cycle Eletrical EnergyGenerated (J)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property SmartElectricalTotalCycleEletricalEnergyGenerated As Joule

		''' <summary>
		''' Total Cycle Electrical Demand (J)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property TotalCycleElectricalDemand As Joule

		''' <summary>
		''' Total Cycle Fuel Consumption: Smart Electrical Load (g)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property TotalCycleFuelConsumptionSmartElectricalLoad As Kilogram

		''' <summary>
		''' Total Cycle Fuel Consumption: Zero Electrical Load (g)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property TotalCycleFuelConsumptionZeroElectricalLoad As Kilogram

		''' <summary>
		''' Stop Start Sensitive: Total Cycle Electrical Demand (J)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property StopStartSensitiveTotalCycleElectricalDemand As Joule

		''' <summary>
		''' Total Cycle Fuel Consuption : Average Loads (g)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property TotalCycleFuelConsuptionAverageLoads As Kilogram

		''' <summary>
		''' Clears aggregated values ( Sets them to zero ).
		''' </summary>
		''' <remarks></remarks>
		Sub ClearAggregates()

		''' <summary>
		''' Increments all aggregated outputs
		''' </summary>
		''' <param name="stepTimeInSeconds">Single : Mutiplies the values to be aggregated by number of seconds</param>
		''' <remarks></remarks>
		Sub CycleStep(stepTimeInSeconds As Second)
	End Interface
End Namespace


