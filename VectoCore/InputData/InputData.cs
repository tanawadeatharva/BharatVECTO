using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TUGraz.VectoCore.InputData.FileIO.DeclarationFile;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData
{
	public interface IJobInputData
	{
		bool SavedInDeclarationMode { get; }

		IVehicleInputData Vehicle { get; }

		IList<ICycleData> Cycles { get; }

		bool EngineOnlyMode { get; }

		string JobName { get; }
	}

	public interface IVehicleInputData
	{
		bool SavedInDeclarationMode { get; }

		VehicleCategory VehicleCategory { get; }

		Kilogram CurbWeight { get; }

		Kilogram CurbWeightExtra { get; }

		Kilogram GrossVehicleMassRating { get; }

		Kilogram Loading { get; }

		Meter DynamicTyreRadius { get; }

		SquareMeter DragCoefficient { get; }

		SquareMeter DragCoefficientRigidTruck { get; } // without trailer

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
		DataTable CycleData { get; }
	}

	public interface IDriverInputData
	{
		bool SavedInDeclarationMode { get; }

		IStartStopInputData StartStop { get; }
		ILookaheadCoastingInputData Lookahead { get; }
		IOverspeedEcoRollInputData OverspeedEcoRoll { get; }

		DataTable AccelerationCurve { get; }
	}

	public interface IOverspeedEcoRollInputData
	{
		DriverData.DriverMode Mode { get; }
		MeterPerSecond MinSpeed { get; }
		MeterPerSecond OverSpeed { get; }
		MeterPerSecond UnderSpeed { get; }
	}

	public interface ILookaheadCoastingInputData
	{
		bool Enabled { get; }
		MeterPerSquareSecond Deceleration { get; }
		MeterPerSecond MinSpeed { get; }
	}

	public interface IStartStopInputData
	{
		bool Enabled { get; }
		MeterPerSecond MaxSpeed { get; }
		Second MinTime { get; }
		Second Delay { get; }
	}

	public interface IAuxiliaryInputData
	{
		bool SavedInDeclarationMode { get; }

		string ID { get; }

		string Type { get; }

		string Technology { get; }

		IList<string> TechList { get; }

		double TransmissionRatio { get; }

		double EfficiencyToEngine { get; }

		double EfficiencyToSupply { get; }

		DataTable DemandMap { get; }
	}
}