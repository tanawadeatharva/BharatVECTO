using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData
{
	public interface IJobInputData
	{
		bool SavedInDeclarationMode { get; }

		IVehicleInputData Vehicle { get; }

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

		string JobName { get; }
	}

	public interface IVehicleInputData
	{
		bool SavedInDeclarationMode { get; }

		/// <summary>
		/// P036
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		VehicleCategory VehicleCategory { get; }

		/// <summary>
		/// P038  Curb Weight Vehicle
		/// </summary>
		Kilogram CurbWeight { get; }

		/// <summary>
		/// P039  Curb Weight Extra Trailer/Body
		/// </summary>
		Kilogram CurbWeightExtra { get; }

		/// <summary>
		/// P041  Max. vehicle weight
		/// </summary>
		Kilogram GrossVehicleMassRating { get; }

		/// <summary>
		/// P040  Loading
		/// </summary>
		Kilogram Loading { get; }

		/// <summary>
		/// P049
		/// </summary>
		Meter DynamicTyreRadius { get; }

		/// <summary>
		/// P146  DragCoefficient * Cross Section Area - Truck & Trailer
		/// </summary>
		SquareMeter AirDragArea { get; }

		/// <summary>
		/// P147  DragCoefficient * Cross Section Area - Rigid
		/// </summary>
		SquareMeter AirDragAreaRigidTruck { get; } // without trailer


		/// <summary>
		/// P117  Powered axle tyres/rims
		/// </summary>
		string Rim { get; }

		/// <summary>
		/// P037  
		/// </summary>
		AxleConfiguration AxleConfiguration { get; }

		/// <summary>
		/// parameters for every axle
		/// P044, P045, P046, P047, P048, P108
		/// </summary>
		IList<IAxleInputData> Axles { get; }

		/// <summary>
		/// P050
		/// </summary>
		CrossWindCorrectionMode CrossWindCorrectionMode { get; }

		/// <summary>
		/// P051
		/// P055, P056
		/// </summary>
		DataTable CrosswindCorrectionMap { get; }
	}

	public interface IRetarderInputData
	{
		bool SavedInDeclarationMode { get; }

		/// <summary>
		/// P052  
		/// </summary>
		RetarderData.RetarderType Type { get; }

		/// <summary>
		/// P053
		/// </summary>
		double Ratio { get; }

		/// <summary>
		/// P054
		/// P057, P058
		/// </summary>
		DataTable LossMap { get; }
	}

	public interface IAxleInputData
	{
		/// <summary>
		/// P108  
		/// </summary>
		string Wheels { get; }

		/// <summary>
		/// P045
		/// </summary>
		bool TwinTyres { get; }

		/// <summary>
		/// P046
		/// </summary>
		double RollResistanceCoefficient { get; }

		/// <summary>
		/// P047
		/// </summary>
		Newton TyreTestLoad { get; }

		/// <summary>
		/// P044 (0 - 1)
		/// </summary>
		double AxleWeightShare { get; }

		/// <summary>
		/// P048
		/// </summary>
		KilogramSquareMeter Inertia { get; }
	}

	public interface IGearboxInputData
	{
		bool SavedInDeclarationMode { get; }

		/// <summary>
		/// P075
		/// </summary>
		string ModelName { get; }

		/// <summary>
		/// P076
		/// </summary>
		GearboxType Type { get; }

		/// <summary>
		/// P080
		/// </summary>
		KilogramSquareMeter Inertia { get; }

		/// <summary>
		/// P081
		/// </summary>
		Second TractionInterruption { get; }

		/// <summary>
		/// P078, P079, P077, P082, P145 (for every gear)
		/// </summary>
		IList<ITransmissionInputData> Gears { get; }

		/// <summary>
		/// P084
		/// </summary>
		bool SkipGears { get; }

		/// <summary>
		/// P086
		/// </summary>
		Second ShiftTime { get; }

		/// <summary>
		/// P083
		/// </summary>
		bool EarlyShiftUp { get; }

		/// <summary>
		/// P085
		/// </summary>
		double TorqueReserve { get; }

		/// <summary>
		/// P087
		/// </summary>
		MeterPerSecond StartSpeed { get; }

		/// <summary>
		/// P088
		/// </summary>
		MeterPerSquareSecond StartAcceleration { get; }

		/// <summary>
		/// P089
		/// [%] (0-1)
		/// </summary>
		double StartTorqueReserve { get; }

		/// <summary>
		/// P090, P091, P092, P127
		/// </summary>
		ITorqueConverterInputData TorqueConverter { get; }
	}

	public interface ITransmissionInputData
	{
		int Gear { get; }

		/// <summary>
		/// P078
		/// </summary>
		double Ratio { get; }

		/// <summary>
		/// P079
		/// P096, P097, P098
		/// </summary>
		DataTable LossMap { get; }

		/// <summary>
		/// P145
		/// </summary>
		DataTable FullLoadCurve { get; }

		/// <summary>
		/// P082
		/// P093, P094, P095
		/// </summary>
		DataTable ShiftPolygon { get; }

		/// <summary>
		/// P077
		/// </summary>
		bool TorqueConverterActive { get; }
	}

	public interface IAxleGearInputData
	{
		bool SavedInDeclarationMode { get; }

		/// <summary>
		/// P078
		/// </summary>
		double Ratio { get; }

		/// <summary>
		/// P079
		/// P096, P097, P098
		/// </summary>
		DataTable LossMap { get; }
	}

	public interface ITorqueConverterInputData
	{
		/// <summary>
		/// P090
		/// </summary>
		bool Enabled { get; }

		/// <summary>
		/// P092
		/// </summary>
		PerSecond ReferenceRPM { get; }

		/// <summary>
		/// P127
		/// </summary>
		KilogramSquareMeter Inertia { get; }

		/// <summary>
		/// P091
		/// P099, P100, P101
		/// </summary>
		DataTable TCData { get; }
	}

	public interface IEngineInputData
	{
		bool SavedInDeclarationMode { get; }

		/// <summary>
		/// P059
		/// </summary>
		string ModelName { get; }

		/// <summary>
		/// P061
		/// </summary>
		CubicMeter Displacement { get; }

		/// <summary>
		/// P063
		/// </summary>
		PerSecond IdleSpeed { get; }

		/// <summary>
		/// P067
		/// P072, P073, P074
		/// engine speed in rpm, torque in NM, fuel consumption in g/h
		/// </summary>
		DataTable FuelConsumptionMap { get; }

		/// <summary>
		/// P144
		/// P068, P069, P70, P71
		/// </summary>
		DataTable FullLoadCurve { get; }

		/// <summary>
		/// P062
		/// </summary>
		KilogramSquareMeter Inertia { get; }

		/// <summary>
		/// P111
		/// </summary>
		KilogramPerWattSecond WHTCMotorway { get; }

		/// <summary>
		/// P110
		/// </summary>
		KilogramPerWattSecond WHTCRural { get; }

		/// <summary>
		/// P109
		/// </summary>
		KilogramPerWattSecond WHTCUrban { get; }
	}

	public interface IAuxiliariesInputData
	{
		bool SavedInDeclarationMode { get; }

		IList<IAuxiliaryInputData> Auxiliaries { get; }
	}

	public interface ICycleData
	{
		string Name { get; }

		/// <summary>
		/// P028, P029, P030, P031, P032, P119, P120, P121, P122, P123, P124, P125, P126
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		DataTable CycleData { get; }
	}

	public interface IDriverInputData
	{
		bool SavedInDeclarationMode { get; }

		IStartStopInputData StartStop { get; }
		ILookaheadCoastingInputData Lookahead { get; }
		IOverSpeedEcoRollInputData OverSpeedEcoRoll { get; }

		/// <summary>
		/// P009; P033, P034, P035
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		DataTable AccelerationCurve { get; }
	}

	public interface IOverSpeedEcoRollInputData
	{
		/// <summary>
		/// P015
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		DriverData.DriverMode Mode { get; }

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

	public interface IStartStopInputData
	{
		/// <summary>
		/// P010  StartStop - enabled
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		bool Enabled { get; }

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

	public interface IAuxiliaryInputData
	{
		bool SavedInDeclarationMode { get; }

		/// <summary>
		/// P006  Aux-ID
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		string ID { get; }

		/// <summary>
		/// P005  Aux-Type
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		string Type { get; }

		/// <summary>
		/// P118  Aux-Technology
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		string Technology { get; }

		/// <summary>
		/// P143  Aux-Techlist
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<string> TechList { get; }

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