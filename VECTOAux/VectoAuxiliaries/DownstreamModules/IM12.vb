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

Namespace DownstreamModules
	Public Interface IM12
		''' <summary>
		''' Fuel consumption with smart Electrics and Average Pneumatic Power Demand
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand As Gram

		''' <summary>
		''' Base Fuel Consumption With Average Auxiliary Loads
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property BaseFuelConsumptionWithTrueAuxiliaryLoads As Gram

		''' <summary>
		''' Stop Start Correction
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property StopStartCorrection As Double


		'Diagnostic Signals Only For Testing - No Material interference with operation of class.
		ReadOnly Property P1X As Joule
		ReadOnly Property P1Y As Gram
		ReadOnly Property P2X As Joule
		ReadOnly Property P2Y As Gram
		ReadOnly Property P3X As Joule
		ReadOnly Property P3Y As Gram
		ReadOnly Property XTAIN As Joule
		ReadOnly Property INTRP1 As Gram
		ReadOnly Property INTRP2 As Gram
	End Interface
End Namespace


