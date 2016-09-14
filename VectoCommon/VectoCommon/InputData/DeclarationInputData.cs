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
using System.Data;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.InputData
{
	public interface IDeclarationJobInputData
	{
		bool SavedInDeclarationMode { get; }

		IVehicleDeclarationInputData Vehicle { get; }

		string JobName { get; }
	}

	public interface IComponentInputData
	{
		bool SavedInDeclarationMode { get; }

		string Vendor { get; }

		string ModelName { get; }

		string Creator { get; }

		string Date { get; }

		string TypeId { get; }

		string DigestValue { get; }

		IntegrityStatus IntegrityStatus { get; }
	}

	public interface IVehicleDeclarationInputData : IComponentInputData
	{
		/// <summary>
		/// P036
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		VehicleCategory VehicleCategory { get; }

		/// <summary>
		/// P037  
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		AxleConfiguration AxleConfiguration { get; }

		/// <summary>
		/// P038  Curb Weight Vehicle
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Kilogram CurbWeightChassis { get; }

		/// <summary>
		/// P041  Max. vehicle weight
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Kilogram GrossVehicleMassRating { get; }

		/// <summary>
		/// P146, P147  DragCoefficient * Cross Section Area - Rigid
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		SquareMeter AirDragArea { get; } // without trailer

		///// <summary>
		///// P117  Powered axle tyres/rims
		///// cf. VECTO Input Parameters.xlsx
		///// </summary>
		//string Rim { get; }  // deprecated

		/// <summary>
		/// parameters for every axle
		/// P044, P045, P046, P047, P048, P108
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<IAxleDeclarationInputData> Axles { get; }
	}

	public interface IRetarderInputData : IComponentInputData
	{
		/// <summary>
		/// P052  
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		RetarderType Type { get; }

		/// <summary>
		/// P053
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double Ratio { get; }

		/// <summary>
		/// P054
		/// P057, P058
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData LossMap { get; }
	}

	public interface IAngularGearInputData : IComponentInputData
	{
		/// <summary>
		/// P180
		/// </summary>
		AngularGearType Type { get; }

		/// <summary>
		/// P176
		/// </summary>
		double Ratio { get; }

		/// <summary>
		/// P173, P174, P175
		/// </summary>
		TableData LossMap { get; }

		/// <summary>
		/// P177
		/// </summary>
		double Efficiency { get; }
	}

	public interface IAxleDeclarationInputData : IComponentInputData
	{
		/// <summary>
		/// P108  
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		string Wheels { get; }

		/// <summary>
		/// P045
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		bool TwinTyres { get; }

		AxleType AxleType { get; }

		/// <summary>
		/// P046
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double RollResistanceCoefficient { get; }

		/// <summary>
		/// P047
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Newton TyreTestLoad { get; }
	}

	public interface IGearboxDeclarationInputData : IComponentInputData
	{
		/// <summary>
		/// P076
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		GearboxType Type { get; }

		/// <summary>
		/// P078, P079, P077, P082, P145 (for every gear)
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<ITransmissionInputData> Gears { get; }
	}


	public interface ITransmissionInputData
	{
		int Gear { get; }

		/// <summary>
		/// P078
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double Ratio { get; }

		/// <summary>
		/// P079
		/// P096, P097, P098
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData LossMap { get; }

		/// <summary>
		/// P079
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double Efficiency { get; }

		///// <summary>
		///// P145
		///// cf. VECTO Input Parameters.xlsx
		///// </summary>
		//DataTable FullLoadCurve { get; } // deprecated

		/// <summary>
		/// P157
		/// </summary>
		NewtonMeter MaxTorque { get; }

		/// <summary>
		/// P082
		/// P093, P094, P095
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData ShiftPolygon { get; }

		///// <summary>
		///// P077
		///// cf. VECTO Input Parameters.xlsx
		///// </summary>
		//bool HasTorqueConverter { get; }     // DEPRECATED
	}

	public interface IAxleGearInputData : IComponentInputData
	{
		/// <summary>
		/// P078
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double Ratio { get; }

		/// <summary>
		/// P079
		/// P096, P097, P098
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		DataTable LossMap { get; }

		/// <summary>
		/// P079
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double Efficiency { get; }
	}

	public interface ITorqueConverterDeclarationInputData
	{
		/// <summary>
		/// P091
		/// P099, P100, P101
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		// ReSharper disable once InconsistentNaming
		TableData TCData { get; }
	}

	public interface IEngineDeclarationInputData : IComponentInputData
	{
		/// <summary>
		/// P061
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		CubicMeter Displacement { get; }

		/// <summary>
		/// P063
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		PerSecond IdleSpeed { get; }

		/// <summary>
		/// P111
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double WHTCMotorway { get; }

		/// <summary>
		/// P110
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double WHTCRural { get; }

		/// <summary>
		/// P109
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double WHTCUrban { get; }

		/// <summary>
		/// P067
		/// P072, P073, P074
		/// cf. VECTO Input Parameters.xlsx
		/// engine speed in rpm, torque in NM, fuel consumption in g/h
		/// </summary>
		TableData FuelConsumptionMap { get; }

		/// <summary>
		/// P144
		/// P068, P069, P70, P71
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData FullLoadCurve { get; }
	}

	public interface IAuxiliariesDeclarationInputData
	{
		bool SavedInDeclarationMode { get; }

		IList<IAuxiliaryDeclarationInputData> Auxiliaries { get; }
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

	public interface IDriverDeclarationInputData
	{
		bool SavedInDeclarationMode { get; }

		IStartStopDeclarationInputData StartStop { get; }

		IOverSpeedEcoRollDeclarationInputData OverSpeedEcoRoll { get; }
	}

	public interface IOverSpeedEcoRollDeclarationInputData
	{
		/// <summary>
		/// P015
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		DriverMode Mode { get; }
	}

	public interface IStartStopDeclarationInputData
	{
		/// <summary>
		/// P010  StartStop - enabled
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		bool Enabled { get; }
	}

	public interface IAuxiliaryDeclarationInputData
	{
		/// <summary>
		/// P005  Aux-Type
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		AuxiliaryType Type { get; }

		/// <summary>
		/// P118  Aux-Technology
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<string> Technology { get; }
	}
}