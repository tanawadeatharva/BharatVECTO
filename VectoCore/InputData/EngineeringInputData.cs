using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData
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
		/// P050
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		CrossWindCorrectionMode CrossWindCorrectionMode { get; }

		/// <summary>
		/// P051
		/// P055, P056
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		DataTable CrosswindCorrectionMap { get; }
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

		/// <summary>
		/// P083
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		bool EarlyShiftUp { get; }

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

		/// <summary>
		/// P084
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		bool SkipGears { get; }


		/// <summary>
		/// P090, P091, P092, P127
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		ITorqueConverterInputData TorqueConverter { get; }
	}

	public interface IEngineEngineeringInputData : IEngineDeclarationInputData
	{
		/// <summary>
		/// P062
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		KilogramSquareMeter Inertia { get; }
	}


	public interface IAuxiliariesEngineeringInputData : IAuxiliariesDeclarationInputData
	{
		new IList<IAuxiliaryEngineeringInputData> Auxiliaries { get; }
	}


	public interface IDriverEngineeringInputData : IDriverDeclarationInputData
	{
		new IStartStopEngineeringInputData StartStop { get; }

		new IOverSpeedEcoRollEngineeringInputData OverSpeedEcoRoll { get; }

		/// <summary>
		/// P009; P033, P034, P035
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		DataTable AccelerationCurve { get; }

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
		MeterPerSquareSecond Deceleration { get; }

		/// <summary>
		/// P021
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		MeterPerSecond MinSpeed { get; }
	}

	public interface IAuxiliaryEngineeringInputData : IAuxiliaryDeclarationInputData
	{
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
		DataTable DemandMap { get; }
	}
}