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

using System;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Configuration
{
	public static class Constants
	{
		public const double RPMToRad = 2 * Math.PI / 60;
		public const double Kilo = 1000;
		public const double MeterPerSecondToKMH = 3.6;

		// mk-2016-10-11: const never used. Delete?
		[Obsolete] public const double SecondsPerHour = 3600;

		public static class Auxiliaries
		{
			public const string Cycle = "cycle";
			public const string Prefix = "AUX_";
			public const string PowerPrefix = "P_";

			public static class IDs
			{
				public static string PTOTransmission = "PTO_TRANSM";
				public const string Fan = "FAN";
				public const string SteeringPump = "STP";
				public const string ElectricSystem = "ES";
				public const string HeatingVentilationAirCondition = "AC";
				public const string PneumaticSystem = "PS";
				public const string PTOConsumer = "PTO_CONSUM";
			}

			public static class Names
			{
				public const string Fan = "Fan";
				public const string SteeringPump = "Steering pump";
				public const string ElectricSystem = "Electric System";
				public const string HeatingVentilationAirCondition = "HVAC";
				public const string PneumaticSystem = "Pneumatic System";
			}
		}

		public static class FileExtensions
		{
			public const string PDFReport = ".pdf";
			public const string ModDataFile = ".vmod";

			public const string SumFile = ".vsum";

			public const string VectoJobFile = ".vecto";

			public const string VectoXMLDeclarationFile = ".xml";

			public const string EngineDataFile = ".veng";

			public const string VehicleDataFile = ".vveh";

			public const string GearboxDataFile = ".vgbx";

			public const string CycleFile = ".vdri";

			public const string DriverAccelerationCurve = ".vacc";
		}

		public static class SimulationSettings
		{
			/// <summary>
			/// base time interval for the simulation. the distance is estimated to reach this time interval as good as possible
			/// </summary>
			public static readonly Second TargetTimeInterval = 0.5.SI<Second>();

			/// <summary>
			/// maximum time interval for the simulation in measured speed mode.
			/// </summary>
			public static readonly Second MeasuredSpeedTargetTimeInterval = 1.SI<Second>();

			/// <summary>
			/// The lower time bound before braking is initiated.
			/// </summary>
			public static readonly Second LowerBoundTimeInterval = 0.25.SI<Second>();

			/// <summary>
			/// simulation interval if the vehicle stands still
			/// </summary>
			public static readonly Meter DriveOffDistance = 0.25.SI<Meter>();

			public static readonly Meter BrakeNextTargetDistance = 2.5.SI<Meter>();

			public static readonly MeterPerSecond MinVelocityForCoast = 5.KMPHtoMeterPerSecond();

			/// <summary>
			/// threshold for changes in the road gradient. changes below this threshold will be considered to be equal for filtering out the driving cycle.
			/// altitude computation is done before filtering! 
			/// </summary>
			public static readonly Radian DrivingCycleRoadGradientTolerance = 1E-12.SI<Radian>();

			//VectoMath.InclinationToAngle(0.25 / 100.0).Value();

			public const int DriverSearchLoopThreshold = 200;

			/// <summary>
			/// Tolerance for searching operating point with line search.
			/// </summary>
			public const double LineSearchTolerance = 0.5;

			/// <summary>
			/// Tolerance for searching operating point with interpolating search.
			/// </summary>
			public const double InterpolateSearchTolerance = 1e-3;

			public const double ClutchClosingSpeedNorm = 0.065;

			public static readonly MeterPerSecond ClutchDisengageWhenHaltingSpeed = 15.KMPHtoMeterPerSecond();

			public static readonly MeterPerSecond ATGearboxDisengageWhenHaltingSpeed = 5.KMPHtoMeterPerSecond();

			public static readonly MeterPerSquareSecond MinimumAcceleration = 0.1.SI<MeterPerSquareSecond>();

			public static Meter DriverActionDistanceTolerance = 0.25.SI<Meter>();

			public static MeterPerSecond VehicleSpeedHaltTolerance = 1e-3.SI<MeterPerSecond>();

			/// <summary>
			/// The initial search interval for the operating point search in the driver.
			/// </summary>
			public static readonly MeterPerSquareSecond OperatingPointInitialSearchIntervalAccelerating =
				0.1.SI<MeterPerSquareSecond>();

			public static readonly PerSecond EngineIdlingSearchInterval = 10.SI<PerSecond>();

			public const int EngineSearchLoopThreshold = 100;

			public const int MaximumIterationCountForSimulationStep = 30;

			public static readonly MeterPerSecond VehicleStopClutchDisengageSpeed = 10.KMPHtoMeterPerSecond();

			public static readonly Meter GearboxLookaheadForAccelerationEstimation = 100.SI<Meter>();

			public static Kilogram MaximumGrossVehicleWeight = 40000.SI<Kilogram>();
			public static Kilogram MaximumGrossVehicleWeightEMS = 60000.SI<Kilogram>();

			// the torque converter characteristics curve has to be defined up to this speed ratio
			public const double RequiredTorqueConverterSpeedRatio = 2.2;
		}
	}
}