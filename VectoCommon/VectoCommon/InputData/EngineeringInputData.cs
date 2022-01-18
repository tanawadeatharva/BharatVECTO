/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Collections.Generic;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCommon.InputData
{
	public interface IEngineeringJobInputData : IDeclarationJobInputData
	{
		new IVehicleEngineeringInputData Vehicle { get; }


		IHybridStrategyParameters HybridStrategyParameters { get; }

		/// <summary>
		/// P008  Cycles
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<ICycleData> Cycles { get; }

		//ToDo Remove use JobType
		IEngineEngineeringInputData EngineOnly { get; }
	}

	public enum VectoSimulationJobType
	{
		ConventionalVehicle,
		ParallelHybridVehicle,
		SerialHybridVehicle,
		BatteryElectricVehicle,
		EngineOnlySimulation,
	}

	public interface IHybridStrategyParameters
	{
		double EquivalenceFactorDischarge { get; }
		
		double EquivalenceFactorCharge { get; }

		double MinSoC { get; }

		double MaxSoC { get; }

		double TargetSoC { get; }

		string Source { get;  }
		Second MinimumICEOnTime { get; }
		Second AuxBufferTime { get; }
		Second AuxBufferChargeTime { get; }
		double ICEStartPenaltyFactor { get; }

		double CostFactorSOCExpponent { get; }
	}

	public interface IVehicleEngineeringInputData : IVehicleDeclarationInputData
	{
		/// <summary>
		/// P039  Curb Weight Extra Trailer/Body
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Kilogram CurbMassExtra { get; }

		/// <summary>
		/// P040  Loading
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Kilogram Loading { get; }

		/// <summary>
		/// P049
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Meter DynamicTyreRadius { get; }

		Meter Height { get; }

		
		new IVehicleComponentsEngineering Components { get; }

		new IAdvancedDriverAssistantSystemsEngineering ADAS { get; }
		
		double InitialSOC { get; }

		// input parameters for road sweeper use case


		VectoSimulationJobType VehicleType { get; }
		GearshiftPosition PTO_DriveGear { get; }

		PerSecond PTO_DriveEngineSpeed { get; }
	}

	public interface IAdvancedDriverAssistantSystemsEngineering : IAdvancedDriverAssistantSystemDeclarationInputData
	{
		DataSource DataSource { get; }
	}


	public interface IVehicleComponentsEngineering
	{
		IAirdragEngineeringInputData AirdragInputData { get; }

		IGearboxEngineeringInputData GearboxInputData { get; }

		ITorqueConverterEngineeringInputData TorqueConverterInputData { get; }

		IAxleGearInputData AxleGearInputData { get; }

		IAngledriveInputData AngledriveInputData { get; }

		IEngineEngineeringInputData EngineInputData { get; }

		IAuxiliariesEngineeringInputData AuxiliaryInputData { get; }

		IRetarderInputData RetarderInputData { get; }

		IPTOTransmissionInputData PTOTransmissionInputData { get; }

		/// <summary>
		/// parameters for every axle
		/// P044, P045, P046, P047, P048, P108
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IAxlesEngineeringInputData AxleWheels { get; }

		//IBusAuxiliariesEngineeringData BusAuxiliaries { get; }

		IElectricStorageSystemEngineeringInputData ElectricStorage { get; }

		IElectricMachinesEngineeringInputData ElectricMachines { get; }
	}

	public interface IAxlesEngineeringInputData
	{
		/// <summary>
		/// parameters for every axle
		/// P044, P045, P046, P047, P048, P108
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<IAxleEngineeringInputData> AxlesEngineering { get; }

		DataSource DataSource { get; }
	}

	public interface IAirdragEngineeringInputData : IAirdragDeclarationInputData
	{
		/// <summary>
		/// P050 - Cross Wind Correction Mode
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		CrossWindCorrectionMode CrossWindCorrectionMode { get; }

		/// <summary>
		/// P051
		/// P055, P056
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData CrosswindCorrectionMap { get; }
	}

	public interface IPTOTransmissionInputData
	{
		/// <summary>
		/// The transmission type for the constant pto transmission losses.
		/// </summary>
		string PTOTransmissionType { get; }

		/// <summary>
		/// The PTO Loss map for idling losses of the "consumer" part.
		/// </summary>
		TableData PTOLossMap { get; }

		TableData PTOCycleDuringStop { get; }

		TableData PTOCycleWhileDriving { get; }

		PTOShaftGearWheel? PTOShaftGearWheel { get; }

		PTOOtherElement? PTOOtherElement { get; }
	}
	
	public interface IAxleEngineeringInputData : IAxleDeclarationInputData
	{
		/// <summary>
		/// P044 (0 - 1)
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double AxleWeightShare { get; }

		new ITyreEngineeringInputData Tyre { get; }
	}

	public interface ITyreEngineeringInputData : ITyreDeclarationInputData
	{
		/// <summary>
		/// P048
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		KilogramSquareMeter Inertia { get; }

		Meter DynamicTyreRadius { get; }
	}

	public interface IGearboxEngineeringInputData : IGearboxDeclarationInputData
	{
		/// <summary>
		/// P080
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		KilogramSquareMeter Inertia { get; }

		/// <summary>
		/// P081
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Second TractionInterruption { get; }

		Second PowershiftShiftTime { get; }
	}

	public interface IGearshiftEngineeringInputData : ITorqueConverterEngineeringShiftParameterInputData, IDriverModelData
	{
		/// <summary>
		/// P086
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Second MinTimeBetweenGearshift { get; }

		///// <summary>
		///// P083
		///// cf. VECTO Input Parameters.xlsx
		///// </summary>
		//bool EarlyShiftUp { get; }

		/// <summary>
		/// P085
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double TorqueReserve { get; }

		/// <summary>
		/// P087
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		MeterPerSecond StartSpeed { get; }

		/// <summary>
		/// P088
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		MeterPerSquareSecond StartAcceleration { get; }

		/// <summary>
		/// P089
		/// [%] (0-1)
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double StartTorqueReserve { get; }

		///// <summary>
		///// P084
		///// cf. VECTO Input Parameters.xlsx
		///// </summary>
		//bool SkipGears { get; }

		Second DownshiftAfterUpshiftDelay { get; }

		Second UpshiftAfterDownshiftDelay { get; }

		MeterPerSquareSecond UpshiftMinAcceleration { get; }

		// ACEA/Scania GS Parameters
		Second GearResidenceTime { get; }
		double? DnT99LHMin1 { get; }
		double? DnT99LHMin2 { get; }
		int? AllowedGearRangeUp { get; }
		int? AllowedGearRangeDown { get; }
		Second LookBackInterval { get; }
		Watt AvgCardanPowerThresholdPropulsion { get; }
		Watt CurrCardanPowerThresholdPropulsion { get; }
		double? TargetSpeedDeviationFactor { get; }
		double? EngineSpeedHighDriveOffFactor { get; }
		double? RatingFactorCurrentGear { get; }
		TableData AccelerationReserveLookup { get; }
		TableData ShareTorque99L { get; }
		TableData PredictionDurationLookup { get; }
		TableData ShareIdleLow { get; }
		TableData ShareEngineHigh { get; }
		string Source { get; }
		Second DriverAccelerationLookBackInterval { get; }
		MeterPerSquareSecond DriverAccelerationThresholdLow { get; }

		// FC-Based GS parameters
		double? RatioEarlyUpshiftFC { get; }
		double? RatioEarlyDownshiftFC { get; }

		int? AllowedGearRangeFC { get; }

		double? VeloictyDropFactor { get; }

		double? AccelerationFactor { get; }

		// Voith GS Parameters
		TableData LoadStageShiftLines { get; }
		IList<double> LoadStageThresholdsUp { get; }
		IList<double> LoadStageThresholdsDown { get; }
		PerSecond MinEngineSpeedPostUpshift { get; }

		Second ATLookAheadTime { get; }
		double[][] ShiftSpeedsTCToLocked { get; }

		double? PEV_TargetSpeedBrakeNorm { get; }

		double? PEV_DeRatingDownshiftSpeedFactor { get; }
		double? PEV_DownshiftMinSpeedFactor { get; }
	}

	public interface ITorqueConverterEngineeringShiftParameterInputData
	{
		/// <summary>
		/// Min Acceleration after C->L upshifts.
		/// </summary>
		MeterPerSquareSecond CLUpshiftMinAcceleration { get; }

		/// <summary>
		/// Min Acceleration after C->C upshifts.
		/// </summary>
		MeterPerSquareSecond CCUpshiftMinAcceleration { get; }
	}

	public interface ITorqueConverterEngineeringInputData : ITorqueConverterDeclarationInputData
	{
		///// <summary>
		///// P090
		///// cf. VECTO Input Parameters.xlsx
		///// </summary>
		//bool Enabled { get; }   // deprecated

		/// <summary>
		/// P092
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		// ReSharper disable once InconsistentNaming
		PerSecond ReferenceRPM { get; }

		/// <summary>
		/// P127
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		KilogramSquareMeter Inertia { get; }

		TableData ShiftPolygon { get; }

		PerSecond MaxInputSpeed { get; }
	}

	public interface IEngineEngineeringInputData : IEngineDeclarationInputData
	{
		/// <summary>
		/// P062
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		KilogramSquareMeter Inertia { get; }

		new IList<IEngineModeEngineeringInputData> EngineModes { get; }

		Second EngineStartTime { get; }
	}

	public interface IEngineModeEngineeringInputData : IEngineModeDeclarationInputData
	{
		new IList<IEngineFuelEngineeringInputData> Fuels { get; }
	}

	public interface IEngineFuelEngineeringInputData : IEngineFuelDeclarationInputData
	{
		/// <summary>
		/// P170
		/// </summary>
		double WHTCEngineering { get; }
	}

	public interface IAuxiliariesEngineeringInputData
	{
		IAuxiliaryEngineeringInputData Auxiliaries { get; }

		IBusAuxiliariesEngineeringData BusAuxiliariesData { get; }

	}

	public interface IBusAuxiliariesEngineeringData
	{
		DataSource DataSource { get; }

		IBusAuxPneumaticSystemEngineeringData PneumaticSystem { get; }

		IBusAuxElectricSystemEngineeringData ElectricSystem { get; }

		IBusAuxHVACData HVACData { get; }
	}

	public interface IBusAuxPneumaticSystemEngineeringData
	{
		TableData CompressorMap { get; }

		NormLiterPerSecond AverageAirConsumed { get; }

		bool SmartAirCompression { get; }

		double GearRatio { get; }
	}

	public interface IBusAuxElectricSystemEngineeringData
	{
		double AlternatorEfficiency { get; }

		double DCDCConverterEfficiency { get; }

		Ampere CurrentDemand { get; }

		Ampere CurrentDemandEngineOffDriving { get; }

		Ampere CurrentDemandEngineOffStandstill { get; }

		AlternatorType AlternatorType { get; }

		WattSecond ElectricStorageCapacity { get; }

		Watt MaxAlternatorPower { get; }

		bool ESSupplyFromHEVREESS { get; }
		double ElectricStorageEfficiency { get; }
	}

	public interface IBusAuxHVACData
	{
		Watt ElectricalPowerDemand { get; }

		Watt MechanicalPowerDemand { get; }

		Joule AverageHeatingDemand { get; }

		Watt AuxHeaterPower { get; }
	}

	public interface IElectricMotorEngineeringInputData : IElectricMotorDeclarationInputData
	{
		double OverloadRecoveryFactor { get; }
	}

	public interface IElectricMachinesEngineeringInputData : IElectricMachinesDeclarationInputData
	{
		new IList<ElectricMachineEntry<IElectricMotorEngineeringInputData>> Entries { get; }
	}

	public interface IElectricStorageEngineeringInputData : IElectricStorageDeclarationInputData
	{

	}

	public interface IElectricStorageSystemEngineeringInputData  : IElectricStorageSystemDeclarationInputData
	{
		new IList<IElectricStorageEngineeringInputData> ElectricStorageElements { get; }
	}

	public interface IBatteryPackEngineeringInputData : IBatteryPackDeclarationInputData
	{
	}

	public interface ISuperCapEngineeringInputData : ISuperCapDeclarationInputData
	{
	}

	public interface IDriverModelData { }

	public interface IDriverAccelerationData : IDriverModelData
	{
		TableData AccelerationCurve { get; }
	}


	public interface IDriverEngineeringInputData : IDriverDeclarationInputData
	{
		//new IStartStopEngineeringInputData StartStop { get; }

		IOverSpeedEngineeringInputData OverSpeedData { get; }

		/// <summary>
		/// P009; P033, P034, P035
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IDriverAccelerationData AccelerationCurve { get; }

		ILookaheadCoastingInputData Lookahead { get; }

		IGearshiftEngineeringInputData GearshiftInputData { get; }

		IEngineStopStartEngineeringInputData EngineStopStartData { get; }

		IEcoRollEngineeringInputData EcoRollData { get; }
		IPCCEngineeringInputData PCCData { get; }
	}

	public interface IEcoRollEngineeringInputData
	{
		MeterPerSecond MinSpeed { get; }

		Second ActivationDelay { get; }

		MeterPerSecond UnderspeedThreshold { get; }

		MeterPerSquareSecond AccelerationUpperLimit { get; }
	}

	public interface IPCCEngineeringInputData
	{
		MeterPerSecond PCCEnabledSpeed { get; }

		MeterPerSecond MinSpeed { get; }

		Meter PreviewDistanceUseCase1 { get; }

		Meter PreviewDistanceUseCase2 { get; }

		MeterPerSecond Underspeed { get; }

		MeterPerSecond OverspeedUseCase3 { get; }
	}

	public interface IOverSpeedEngineeringInputData : IOverSpeedEcoRollDeclarationInputData
	{
		/// <summary>
		/// P016
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		MeterPerSecond MinSpeed { get; }

		/// <summary>
		/// P017
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		MeterPerSecond OverSpeed { get; }

	}

	public interface ILookaheadCoastingInputData : IDriverModelData
	{
		/// <summary>
		/// P019
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		bool Enabled { get; }

		/// <summary>
		/// P020
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		//MeterPerSquareSecond Deceleration { get; }
		/// <summary>
		/// P021
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		MeterPerSecond MinSpeed { get; }

		double CoastingDecisionFactorOffset { get; }

		double CoastingDecisionFactorScaling { get; }

		double LookaheadDistanceFactor { get; }

		TableData CoastingDecisionFactorTargetSpeedLookup { get; }

		TableData CoastingDecisionFactorVelocityDropLookup { get; }
	}

	public interface IEngineStopStartEngineeringInputData
	{
		Second ActivationDelay { get; }
		Second MaxEngineOffTimespan { get; }
		double UtilityFactorStandstill { get; }
		double UtilityFactorDriving { get;}
	}

	public interface IAuxiliaryEngineeringInputData
	{
		
		/// <summary>
		/// P178
		/// additional constant auxiliary load, similar to Padd; not specified in the cycle but as auxiliary
		/// </summary>
		Watt ConstantPowerDemand { get; }

		Watt PowerDemandICEOffDriving { get; }
		
		Watt PowerDemandICEOffStandstill { get; }


		Watt ElectricPowerDemand { get; }
	}
	
	public enum PTOShaftGearWheel
	{
		none,
		only_the_drive_shaft_of_the_PTO,
		drive_shaft_and_or_up_to_2_gear_wheels,
		drive_shaft_and_or_more_than_2_gear_wheels,
		only_one_engaged_gearwheel_above_oil_level
	}

	public static class PTOShaftGearWheelHelper
	{
		public static PTOShaftGearWheel? Parse(string value)
		{
			switch (value)
			{
				case "none":
					return PTOShaftGearWheel.none;
				case "only the drive shaft of the PTO":
					return PTOShaftGearWheel.only_the_drive_shaft_of_the_PTO;
				case "drive shaft and/or up to 2 gear wheels":
					return PTOShaftGearWheel.drive_shaft_and_or_up_to_2_gear_wheels;
				case "drive shaft and/or more than 2 gear wheels":
					return PTOShaftGearWheel.drive_shaft_and_or_more_than_2_gear_wheels;
				case "only one engaged gearwheel above oil level":
					return PTOShaftGearWheel.only_one_engaged_gearwheel_above_oil_level;
				default:
					return null;
			}
		}

		public static string ToXMLFormat(this PTOShaftGearWheel ptoGearWheel)
		{
			switch (ptoGearWheel)
			{
				case PTOShaftGearWheel.none:
					return "none";
				case PTOShaftGearWheel.only_the_drive_shaft_of_the_PTO:
					return "only the drive shaft of the PTO";
				case PTOShaftGearWheel.drive_shaft_and_or_up_to_2_gear_wheels:
					return "drive shaft and/or up to 2 gear wheels";
				case PTOShaftGearWheel.drive_shaft_and_or_more_than_2_gear_wheels:
					return "drive shaft and/or more than 2 gear wheels";
				case PTOShaftGearWheel.only_one_engaged_gearwheel_above_oil_level:
					return "only one engaged gearwheel above oil level";
				default:
					return null;
			}
		}
	}
	
	public enum PTOOtherElement
	{
		none,
		shift_claw_synchronizer_sliding_gearwheel,
		multi_disc_clutch,
		multi_disc_clutch_oil_pump
	}
	
	public static class PTOOtherElementHelper
	{
		public static PTOOtherElement? Parse(string value)
		{
			switch (value)
			{
				case "none":
					return PTOOtherElement.none;
				case "shift claw, synchronizer, sliding gearwheel":
					return PTOOtherElement.shift_claw_synchronizer_sliding_gearwheel;
				case "multi-disc clutch":
					return PTOOtherElement.multi_disc_clutch;
				case "multi-disc clutch, oil pump":
					return PTOOtherElement.multi_disc_clutch_oil_pump;
				default:
					return null;
			}
		}

		public static string ToXMLFormat(this PTOOtherElement ptoOtherElement)
		{
			switch (ptoOtherElement)
			{
				case PTOOtherElement.none:
					return "none";
				case PTOOtherElement.shift_claw_synchronizer_sliding_gearwheel:
					return "shift claw, synchronizer, sliding gearwheel";
				case PTOOtherElement.multi_disc_clutch:
					return "multi-disc clutch";
				case PTOOtherElement.multi_disc_clutch_oil_pump:
					return "multi-disc clutch, oil pump";
				default:
					return null;
			}
		}
	}
}
