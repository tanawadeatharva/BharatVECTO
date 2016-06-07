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
	Public Interface IM0_5_SmartAlternatorSetEfficiency
		''' <summary>
		''' Smart Idle Current (A)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property SmartIdleCurrent() As Ampere

		''' <summary>
		''' Alternators Efficiency In Idle ( Fraction )
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property AlternatorsEfficiencyIdleResultCard() As Double

		''' <summary>
		''' Smart Traction Current (A)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property SmartTractionCurrent As Ampere

		''' <summary>
		''' Alternators Efficiency In Traction ( Fraction )
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property AlternatorsEfficiencyTractionOnResultCard() As Double

		''' <summary>
		''' Smart Overrrun Current (A)
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property SmartOverrunCurrent As Ampere

		''' <summary>
		''' Alternators Efficiency In Overrun ( Fraction )
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property AlternatorsEfficiencyOverrunResultCard() As Double
	End Interface
End Namespace


