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
Imports VectoAuxiliaries.Pneumatics

Namespace Pneumatics
	Public Class M3_AveragePneumaticLoadDemand
		Implements IM3_AveragePneumaticLoadDemand

		Private _pneumaticUserInputsConfig As IPneumaticUserInputsConfig
		Private _pneumaticAuxillariesConfig As IPneumaticsAuxilliariesConfig
		Private _pneumaticsActuationsMap As IPneumaticActuationsMAP
		Private _pneumaticsCompressorFlowRateMap As ICompressorMap
		Private _averagePowerDemandPerCompressorUnitFlowRateInWPerLitresPerSecond As SI
		Private _vehicleMassKG As Kilogram
		Private _cycleName As String
		Private _cycleDurationMinutes As Double
		Private _totalAirDemand As NormLiter
		Private _signals As ISignals


		Private ReadOnly Property Sum1 As NormLiterPerSecond
			Get
				Return _totalAirDemand / _signals.TotalCycleTimeSeconds.SI(Of Second)()
			End Get
		End Property

		Private ReadOnly Property Sum2 As Watt
			Get
				Return (Sum7.Value() / Sum6.Value() * Sum1.Value()).SI(Of Watt)() ' Watt / Nl/s * Nl/s = Watt
			End Get
		End Property

		Private ReadOnly Property Sum3 As Watt
			Get
				Return _
					Sum2 +
					_pneumaticsCompressorFlowRateMap.GetPowerCompressorOff(
						_signals.EngineSpeed * _pneumaticUserInputsConfig.CompressorGearRatio)
			End Get
		End Property

		Private ReadOnly Property Sum4 As Watt
			Get
				Return Sum3 * (1 / _pneumaticUserInputsConfig.CompressorGearEfficiency)
			End Get
		End Property

		'Private ReadOnly Property Sum5 As Single
		'    Get
		'        'NOT USED.
		'    End Get
		'End Property

		Private ReadOnly Property Sum6 As NormLiterPerSecond
			Get
				Return _
					_pneumaticsCompressorFlowRateMap.GetFlowRate(_signals.EngineSpeed * _pneumaticUserInputsConfig.CompressorGearRatio) /
					60
			End Get
		End Property

		Private ReadOnly Property Sum7 As Watt
			Get

				Dim pon As Watt =
						_pneumaticsCompressorFlowRateMap.GetPowerCompressorOn(
							_signals.EngineSpeed * _pneumaticUserInputsConfig.CompressorGearRatio)
				Dim poff As Watt =
						_pneumaticsCompressorFlowRateMap.GetPowerCompressorOff(
							_signals.EngineSpeed * _pneumaticUserInputsConfig.CompressorGearRatio)
				Dim diff As Watt = pon - poff
				Return diff
			End Get
		End Property


		Public ReadOnly Property TotalAirDemand As NormLiter
			Get
				Return _totalAirDemand
			End Get
		End Property


		'Constructors
		Public Sub New(
					 ByRef pneumaticsUserInputConfig As IPneumaticUserInputsConfig,
					ByRef pneumaticsAuxillariesConfig As IPneumaticsAuxilliariesConfig,
					ByRef pneumaticsActuationsMap As IPneumaticActuationsMAP,
					ByRef pneumaticsCompressorFlowRateMap As ICompressorMap,
					vehicleMassKG As Kilogram,
					cycleName As String,
					signals As ISignals
					)


			_pneumaticUserInputsConfig = pneumaticsUserInputConfig
			_pneumaticAuxillariesConfig = pneumaticsAuxillariesConfig
			_pneumaticsActuationsMap = pneumaticsActuationsMap
			_pneumaticsCompressorFlowRateMap = pneumaticsCompressorFlowRateMap
			_vehicleMassKG = vehicleMassKG
			_cycleName = cycleName
			_signals = signals


			'Total up the blow demands from compressor map
			_averagePowerDemandPerCompressorUnitFlowRateInWPerLitresPerSecond =
				(_pneumaticsCompressorFlowRateMap.GetAveragePowerDemandPerCompressorUnitFlowRate() / 60).SI()

			'Calculate the Total Required Air Delivery Rate L / S
			_totalAirDemand = TotalAirDemandCalculation()
		End Sub


		Private Function TotalAirDemandCalculation() As NormLiter

			'These calculation are done directly from formulae provided from a supplied spreadsheet.

			Dim numActuationsPerCycle As Double
			Dim airConsumptionPerActuationNI As Double
			Dim TotalAirDemand As NormLiter

			'Consumers
			Dim Breaks As NormLiter
			Dim ParkBrakesplus2Doors As NormLiter
			Dim Kneeling As NormLiter
			Dim AdBlue As NormLiter
			Dim Regeneration As NormLiter
			Dim DeadVolBlowOuts As NormLiter
			Dim AirSuspension As NormLiter


			'**  Breaks **
			numActuationsPerCycle = _pneumaticsActuationsMap.GetNumActuations(New ActuationsKey("Brakes", _cycleName))
			'=IF(K10 = "yes", IF(COUNTBLANK(F33),G33,F33), IF(COUNTBLANK(F34),G34,F34))*K16
			airConsumptionPerActuationNI =
				If _
					(_pneumaticUserInputsConfig.RetarderBrake, _pneumaticAuxillariesConfig.BrakingWithRetarderNIperKG,
					_pneumaticAuxillariesConfig.BrakingNoRetarderNIperKG)
			Breaks = (numActuationsPerCycle * airConsumptionPerActuationNI * _vehicleMassKG.Value()).SI(Of NormLiter)()

			'** ParkBrakesBreakplus2Doors **                                                     Park break + 2 doors
			numActuationsPerCycle = _pneumaticsActuationsMap.GetNumActuations(New ActuationsKey("Park brake + 2 doors",
																								_cycleName))
			'=SUM(IF(K14="electric",0,IF(COUNTBLANK(F36),G36,F36)),PRODUCT(K16*IF(COUNTBLANK(F37),G37,F37)))
			airConsumptionPerActuationNI =
				If(_pneumaticUserInputsConfig.Doors = "Electric", 0, _pneumaticAuxillariesConfig.PerDoorOpeningNI)
			airConsumptionPerActuationNI += (_pneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG * _vehicleMassKG.Value())
			ParkBrakesplus2Doors = (numActuationsPerCycle * airConsumptionPerActuationNI).SI(Of NormLiter)()

			'** Kneeling **
			numActuationsPerCycle = _pneumaticsActuationsMap.GetNumActuations(New ActuationsKey("Kneeling", _cycleName))
			'=IF(COUNTBLANK(F35),G35,F35)*K11*K16
			airConsumptionPerActuationNI = _pneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM * _vehicleMassKG.Value() *
											_pneumaticUserInputsConfig.KneelingHeightMillimeters
			Kneeling = (numActuationsPerCycle * airConsumptionPerActuationNI).SI(Of NormLiter)()

			'** AdBlue **
			'=IF(K13="electric",0,G39*F54)- Supplied Spreadsheet
			AdBlue =
				If _
					(_pneumaticUserInputsConfig.AdBlueDosing = "Electric", 0,
					_pneumaticAuxillariesConfig.AdBlueNIperMinute * (_signals.TotalCycleTimeSeconds / 60)).SI(Of NormLiter)()

			'** Regeneration **   
			'=SUM(R6:R9)*IF(K9="yes",IF(COUNTBLANK(F41),G41,F41),IF(COUNTBLANK(F40),G40,F40)) - Supplied SpreadSheet
			Regeneration = (Breaks + ParkBrakesplus2Doors + Kneeling + AdBlue)
			Dim regenFraction As Double =
					If(_pneumaticUserInputsConfig.SmartRegeneration, _pneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand,
						_pneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand)
			Regeneration = Regeneration * regenFraction

			'** DeadVolBlowOuts **
			'=IF(COUNTBLANK(F43),G43,F43)/(F54/60) - Supplied SpreadSheet
			numActuationsPerCycle = _pneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour /
									(60 / (_signals.TotalCycleTimeSeconds / 60))
			airConsumptionPerActuationNI = _pneumaticAuxillariesConfig.DeadVolumeLitres
			DeadVolBlowOuts = (numActuationsPerCycle * airConsumptionPerActuationNI).SI(Of NormLiter)()

			'** AirSuspension  **
			'=IF(K12="electrically",0,G38*F54) - Suplied Spreadsheet
			AirSuspension =
				If _
					(_pneumaticUserInputsConfig.AirSuspensionControl = "Electrically", 0,
					_pneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute * _cycleDurationMinutes).SI(Of NormLiter)()

			'** Total Air Demand **
			TotalAirDemand = Breaks + ParkBrakesplus2Doors + Kneeling + AdBlue + Regeneration + DeadVolBlowOuts + AirSuspension


			Return TotalAirDemand
		End Function


		'Get Average Power Demand @ Crank From Pneumatics
		Public Function GetAveragePowerDemandAtCrankFromPneumatics() As Watt _
			Implements IM3_AveragePneumaticLoadDemand.GetAveragePowerDemandAtCrankFromPneumatics

			Return Sum4
		End Function

		'Get Total Required Air Delivery Rate
		Public Function AverageAirConsumedPerSecondLitre() As NormLiterPerSecond _
			Implements IM3_AveragePneumaticLoadDemand.AverageAirConsumedPerSecondLitre

			Return Sum1
		End Function
	End Class
End Namespace

