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
Imports VectoAuxiliaries.Hvac

Namespace DownstreamModules
	Public Interface IM7
		''' <summary>
		''' Smart Electrical And Pneumatic Aux: Alternator Power Gen At Crank (W)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property SmartElectricalAndPneumaticAuxAltPowerGenAtCrank As Watt

		''' <summary>
		''' Smart Electrical And Pneumatic Aux : Air Compressor Power Gen At Crank (W)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank As Watt

		''' <summary>
		''' Smart Electrical Only Aux : Alternator Power Gen At Crank (W)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property SmartElectricalOnlyAuxAltPowerGenAtCrank As Watt

		''' <summary>
		''' Smart Pneumatic Only Aux : Air Comppressor Power Gen At Crank (W)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property SmartPneumaticOnlyAuxAirCompPowerGenAtCrank As Watt
	End Interface
End Namespace


