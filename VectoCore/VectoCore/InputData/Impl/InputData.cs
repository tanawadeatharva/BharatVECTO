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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.InputData.Impl
{
	public class CycleInputData : ICycleData
	{
		public string Name { get; internal set; }

		public DataTable CycleData { get; internal set; }
	}

	public class StartStopInputData : IStartStopEngineeringInputData
	{
		public bool Enabled { get; internal set; }

		public MeterPerSecond MaxSpeed { get; internal set; }

		public Second MinTime { get; internal set; }

		public Second Delay { get; internal set; }
	}

	public class LookAheadCoastingInputData : ILookaheadCoastingInputData
	{
		public bool Enabled { get; internal set; }

		public MeterPerSquareSecond Deceleration { get; internal set; }

		public MeterPerSecond MinSpeed { get; internal set; }

		public double CoastingDecisionFactorOffset { get; internal set; }
		public double CoastingDecisionFactorScaling { get; internal set; }
		public double LookaheadDistanceFactor { get; internal set; }
		public DataTable CoastingDecisionFactorTargetSpeedLookup { get; internal set; }
		public DataTable CoastingDecisionFactorVelocityDropLookup { get; internal set; }
	}

	public class OverSpeedEcoRollInputData : IOverSpeedEcoRollEngineeringInputData
	{
		public DriverMode Mode { get; internal set; }

		public MeterPerSecond MinSpeed { get; internal set; }

		public MeterPerSecond OverSpeed { get; internal set; }

		public MeterPerSecond UnderSpeed { get; internal set; }
	}

	public class TransmissionInputData : ITransmissionInputData
	{
		public int Gear { get; internal set; }

		public double Ratio { get; internal set; }

		public DataTable LossMap { get; internal set; }

		public double Efficiency { get; internal set; }

		public DataTable FullLoadCurve { get; internal set; }

		public DataTable ShiftPolygon { get; internal set; }

		public bool TorqueConverterActive { get; internal set; }
	}

	public class AxleInputData : IAxleEngineeringInputData
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public bool SavedInDeclarationMode
		{
			get { throw new System.NotImplementedException(); }
		}

		public string Vendor { get; internal set; }

		public string ModelName { get; internal set; }

		public string Creator { get; internal set; }

		public string Date { get; internal set; }

		public string TypeId { get; internal set; }

		public string DigestValue { get; internal set; }

		public IntegrityStatus IntegrityStatus { get; internal set; }

		public string Wheels { get; internal set; }

		public bool TwinTyres { get; internal set; }

		public AxleType AxleType { get; internal set; }

		public double RollResistanceCoefficient { get; internal set; }

		public Newton TyreTestLoad { get; internal set; }

		public double AxleWeightShare { get; internal set; }

		public KilogramSquareMeter Inertia { get; internal set; }
	}

	public class AuxiliaryDataInputData : IAuxiliaryEngineeringInputData
	{
		public AuxiliaryDataInputData()
		{
			AuxiliaryType = AuxiliaryDemandType.Mapping;
			ConstantPowerDemand = 0.SI<Watt>();
		}

		public AuxiliaryDemandType AuxiliaryType { get; internal set; }

		public bool SavedInDeclarationMode { get; internal set; }

		public string ID { get; internal set; }

		public string Type { get; internal set; }

		public string Technology { get; internal set; }

		public IList<string> TechList { get; internal set; }

		public double TransmissionRatio { get; internal set; }

		public double EfficiencyToEngine { get; internal set; }

		public double EfficiencyToSupply { get; internal set; }

		public DataTable DemandMap { get; internal set; }

		public Watt ConstantPowerDemand { get; internal set; }
	}
}