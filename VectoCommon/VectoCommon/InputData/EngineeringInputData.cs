/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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
using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.InputData
{
	public interface IEngineeringJobInputData : IDeclarationJobInputData
	{
		new IVehicleEngineeringInputData Vehicle { get; }

		/// <summary>
		/// P008  Cycles
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<ICycleData> Cycles { get; }

		/// <summary>
		/// P001
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		bool EngineOnlyMode { get; }
	}

	public interface IVehicleEngineeringInputData : IVehicleDeclarationInputData
	{
		/// <summary>
		/// P039  Curb Weight Extra Trailer/Body
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Kilogram CurbWeightExtra { get; }

		/// <summary>
		/// P040  Loading
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Kilogram Loading { get; }

		/// <summary>
		/// parameters for every axle
		/// P044, P045, P046, P047, P048, P108
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		new IList<IAxleEngineeringInputData> Axles { get; }

		/// <summary>
		/// P049
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Meter DynamicTyreRadius { get; }

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

		TableData PTOCycle { get; }
	}

	public interface IAxleEngineeringInputData : IAxleDeclarationInputData
	{
		/// <summary>
		/// P044 (0 - 1)
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double AxleWeightShare { get; }

		/// <summary>
		/// P048
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		KilogramSquareMeter Inertia { get; }
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

		/// <summary>
		/// P086
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Second ShiftTime { get; }

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

		/// <summary>
		/// P090, P091, P092, P127
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		ITorqueConverterEngineeringInputData TorqueConverter { get; }

		[Required, SIRange(0, double.MaxValue)]
		Second DownshiftAferUpshiftDelay { get; }

		[Required, SIRange(0, double.MaxValue)]
		Second UpshiftAfterDownshiftDelay { get; }

		[Required, SIRange(0, double.MaxValue)]
		MeterPerSquareSecond UpshiftMinAcceleration { get; }
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
	}

	public interface IEngineEngineeringInputData : IEngineDeclarationInputData
	{
		/// <summary>
		/// P062
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		KilogramSquareMeter Inertia { get; }

		/// <summary>
		/// P170
		/// </summary>
		double WHTCEngineering { get; }
	}

	public interface IAuxiliariesEngineeringInputData
	{
		IList<IAuxiliaryEngineeringInputData> Auxiliaries { get; }

		// Advanced Auxiliaries
		AuxiliaryModel AuxiliaryAssembly { get; }

		string AuxiliaryVersion { get; }

		string AdvancedAuxiliaryFilePath { get; }
	}

	public interface IDriverEngineeringInputData : IDriverDeclarationInputData
	{
		new IStartStopEngineeringInputData StartStop { get; }

		new IOverSpeedEcoRollEngineeringInputData OverSpeedEcoRoll { get; }

		/// <summary>
		/// P009; P033, P034, P035
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData AccelerationCurve { get; }

		ILookaheadCoastingInputData Lookahead { get; }
	}

	public interface IOverSpeedEcoRollEngineeringInputData : IOverSpeedEcoRollDeclarationInputData
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

		/// <summary>
		/// P018
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		MeterPerSecond UnderSpeed { get; }
	}

	public interface IStartStopEngineeringInputData : IStartStopDeclarationInputData
	{
		/// <summary>
		/// P011  StartStop - Max speed
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		MeterPerSecond MaxSpeed { get; }

		/// <summary>
		/// P012  StartStop - Min ICE-ON Time
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Second MinTime { get; }

		/// <summary>
		/// P013  StartStop - Activation Delay
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Second Delay { get; }
	}

	public interface ILookaheadCoastingInputData
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

	public interface IAuxiliaryEngineeringInputData
	{
		/// <summary>
		/// P006  Aux-ID
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		string ID { get; }

		/// <summary>
		/// either mapping or constant
		/// </summary>
		AuxiliaryDemandType AuxiliaryType { get; }

		/// <summary>
		/// P022  Aux-InputFile: transmission ratio
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double TransmissionRatio { get; }

		/// <summary>
		/// P023  Aux-InputFile: efficiency to engine
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double EfficiencyToEngine { get; }

		/// <summary>
		/// P024  Aux-InputFile: efficiency to supply
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double EfficiencyToSupply { get; }

		/// <summary>
		/// P025, P026, P027  Aux-InputFile: map
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData DemandMap { get; }

		/// <summary>
		/// P178
		/// additional constant auxiliary load, similar to Padd; not specified in the cycle but as auxiliary
		/// </summary>
		Watt ConstantPowerDemand { get; }
	}
}