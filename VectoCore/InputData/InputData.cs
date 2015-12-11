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
		/// P050
		/// </summary>
		CrossWindCorrectionMode CrossWindCorrectionMode { get; }

		string Rim { get; }

		//IRetarderInputData Retarder { get; }

		AxleConfiguration AxleConfiguration { get; }
		IList<IAxleInputData> Axles { get; }
	}

	public interface IRetarderInputData
	{
		bool SavedInDeclarationMode { get; }

		RetarderData.RetarderType Type { get; }

		double Ratio { get; }

		DataTable LossMap { get; }
	}

	public interface IAxleInputData
	{
		string Wheels { get; }
		bool TwinTyres { get; }

		double RollResistanceCoefficient { get; }
		Newton TyreTestLoad { get; }

		double AxleWeightShare { get; }

		KilogramSquareMeter Inertia { get; }
	}

	public interface IGearboxInputData
	{
		bool SavedInDeclarationMode { get; }
		string ModelName { get; }

		GearboxType Type { get; }

		KilogramSquareMeter Inertia { get; }

		Second TractionInterruption { get; }

		IList<ITransmissionInputData> Gears { get; }

		bool SkipGears { get; }
		Second ShiftTime { get; }
		bool EarlyShiftUp { get; }

		double TorqueReserve { get; }

		MeterPerSecond StartSpeed { get; }

		MeterPerSquareSecond StartAcceleration { get; }

		/// <summary>
		/// [%] (0-1)
		/// </summary>
		double StartTorqueReserve { get; }

		ITorqueConverterInputData TorqueConverter { get; }
	}

	public interface ITransmissionInputData
	{
		int Gear { get; }
		double Ratio { get; }
		DataTable LossMap { get; }

		DataTable FullLoadCurve { get; }

		DataTable ShiftPolygon { get; }

		bool TorqueConverterActive { get; }
	}

	public interface IAxleGearInputData
	{
		bool SavedInDeclarationMode { get; }

		double Ratio { get; }

		DataTable LossMap { get; }
	}

	public interface ITorqueConverterInputData
	{
		bool Enabled { get; }

		RoundsPerMinute ReferenceRPM { get; }

		KilogramSquareMeter Inertia { get; }

		DataTable TCData { get; }
	}

	public interface IEngineInputData
	{
		bool SavedInDeclarationMode { get; }

		string ModelName { get; }
		CubicMeter Displacement { get; }

		PerSecond IdleSpeed { get; }

		/// <summary>
		/// engine speed in rpm, torque in NM, fuel consumption in g/h
		/// </summary>
		DataTable FuelConsumptionMap { get; }

		DataTable FullLoadCurve { get; }

		KilogramSquareMeter Inertia { get; }

		KilogramPerWattSecond WHTCMotorway { get; }
		KilogramPerWattSecond WHTCRural { get; }
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