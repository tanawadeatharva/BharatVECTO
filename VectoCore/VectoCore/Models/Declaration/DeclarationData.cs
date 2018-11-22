/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public static class DeclarationData
	{
		/// <summary>
		/// The standard acceleration for gravity on earth.
		/// http://physics.nist.gov/Pubs/SP330/sp330.pdf (page 52)
		/// </summary>
		public static readonly MeterPerSquareSecond GravityAccelleration = 9.80665.SI<MeterPerSquareSecond>();

		public static readonly KilogramPerCubicMeter AirDensity = 1.188.SI<KilogramPerCubicMeter>();


		public const string DeclarationDataResourcePrefix = "TUGraz.VectoCore.Resources.Declaration";

		public static readonly Watt MinEnginePowerForEMS = 300e3.SI<Watt>();

		public static readonly Segments Segments = new Segments();
		public static readonly Wheels Wheels = new Wheels();
		public static readonly PT1 PT1 = new PT1();
		public static readonly FuelData FuelData = FuelData.Instance();
		public static readonly ElectricSystem ElectricSystem = new ElectricSystem();
		public static readonly Fan Fan = new Fan();

		public static readonly HeatingVentilationAirConditioning HeatingVentilationAirConditioning =
			new HeatingVentilationAirConditioning();

		public static readonly PneumaticSystem PneumaticSystem = new PneumaticSystem();
		public static readonly SteeringPump SteeringPump = new SteeringPump();
		public static readonly WHTCCorrection WHTCCorrection = new WHTCCorrection();
		public static readonly AirDrag AirDrag = new AirDrag();
		public static readonly StandardBodies StandardBodies = new StandardBodies();
		public static readonly Payloads Payloads = new Payloads();
		public static readonly PTOTransmission PTOTransmission = new PTOTransmission();

		public static readonly ADASCombinations ADASCombinations = new ADASCombinations();
		public static readonly ADASBenefits ADASBenefits = new ADASBenefits();

		/// <summary>
		/// Formula for calculating the payload for a given gross vehicle weight.
		/// (so called "pc-formula", Whitebook Apr 2016, Part 1, p.187)
		/// </summary>
		public static Kilogram GetPayloadForGrossVehicleWeight(Kilogram grossVehicleWeight, string equationName)
		{
			if (equationName.ToLower().StartsWith("pc10")) {
				return Payloads.Lookup10Percent(grossVehicleWeight);
			}
			if (equationName.ToLower().StartsWith("pc75")) {
				return Payloads.Lookup75Percent(grossVehicleWeight);
			}
			return Payloads.Lookup50Percent(grossVehicleWeight);
		}

		/// <summary>
		/// Returns the payload for a trailer. This is 75% of (GVW-CurbWeight).
		/// </summary>
		public static Kilogram GetPayloadForTrailerWeight(Kilogram grossVehicleWeight, Kilogram curbWeight, bool lowLoading)
		{
			return
				(Math.Round(
					(Payloads.LookupTrailer(grossVehicleWeight, curbWeight) / (lowLoading ? 7.5 : 1)).LimitTo(0.SI<Kilogram>(),
						grossVehicleWeight - curbWeight).Value() / 100, 0) * 100).SI<Kilogram>();
		}

		public static int PoweredAxle()
		{
			return 1;
		}

		public static class Driver
		{
			public static class LookAhead
			{
				public const bool Enabled = true;

				public const double DecisionFactorCoastingOffset = 2.5;
				public const double DecisionFactorCoastingScaling = 1.5;
				public const double LookAheadDistanceFactor = 10;
				public static readonly MeterPerSecond MinimumSpeed = 50.KMPHtoMeterPerSecond();
			}

			public static class OverSpeedEcoRoll
			{
				public static readonly IList<DriverMode> AllowedModes = new List<DriverMode> {
					DriverMode.EcoRoll,
					DriverMode.Overspeed
				};

				public static readonly MeterPerSecond MinSpeed = 50.KMPHtoMeterPerSecond();
				public static readonly MeterPerSecond OverSpeed = 2.5.KMPHtoMeterPerSecond();
				public static readonly MeterPerSecond UnderSpeed = 5.KMPHtoMeterPerSecond();
			}

			//public static class StartStop
			//{
			//	public static readonly MeterPerSecond MaxSpeed = 5.KMPHtoMeterPerSecond();
			//	public static readonly Second Delay = 5.SI<Second>();
			//	public static readonly Second MinTime = 5.SI<Second>();
			//}
		}

		public static class Trailer
		{
			public const double RollResistanceCoefficient = 0.0055;
			public const double TyreTestLoad = 37500;

			public const bool TwinTyres = false;
			//public const string WheelsType = "385/65 R 22.5";
		}

		public static class Engine
		{
			public static readonly KilogramSquareMeter ClutchInertia = 1.3.SI<KilogramSquareMeter>();
			public static readonly KilogramSquareMeter TorqueConverterInertia = 1.2.SI<KilogramSquareMeter>();

			public static readonly KilogramSquareMeter EngineBaseInertia = 0.41.SI<KilogramSquareMeter>();
		    public static readonly SI EngineDisplacementInertia = (0.27 * 1000).SI(Unit.SI.Kilo.Gramm.Per.Meter); // [kg/m]

			public const double TorqueLimitGearboxFactor = 0.9;
			public const double TorqueLimitVehicleFactor = 0.95;

			public static KilogramSquareMeter EngineInertia(CubicMeter displacement, GearboxType gbxType)
			{
				// VB Code:    Return 1.3 + 0.41 + 0.27 * (Displ / 1000)
				return (gbxType.AutomaticTransmission() ? TorqueConverterInertia : ClutchInertia) + EngineBaseInertia +
						EngineDisplacementInertia * displacement;
			}
		}

		public static class Gearbox
		{
			public const double TorqueReserve = 0.2;
			public const double TorqueReserveStart = 0.2;
			public static readonly MeterPerSecond StartSpeed = 1.3.SI<MeterPerSecond>();
			public static readonly MeterPerSquareSecond StartAcceleration = 0.6.SI<MeterPerSquareSecond>();
			public static readonly KilogramSquareMeter Inertia = 0.SI<KilogramSquareMeter>();

			public static readonly MeterPerSecond TruckMaxAllowedSpeed = 85.KMPHtoMeterPerSecond();
			public const double ShiftPolygonRPMMargin = 7; // %
			private const double ShiftPolygonEngineFldMargin = 0.98;

			public static readonly Second MinTimeBetweenGearshifts = 1.5.SI<Second>();
			public static readonly Second DownshiftAfterUpshiftDelay = 10.SI<Second>();
			public static readonly Second UpshiftAfterDownshiftDelay = 10.SI<Second>();

			public static readonly MeterPerSquareSecond UpshiftMinAcceleration = 0.1.SI<MeterPerSquareSecond>();

			//public static readonly PerSecond TorqueConverterSpeedLimit = 1600.RPMtoRad();
			public static double TorqueConverterSecondGearThreshold(VehicleCategory category)
			{
				return category.IsTruck() ? 1.8 : 1.85;
			}

			public static readonly Second PowershiftShiftTime = 0.8.SI<Second>();

			/// <summary>
			/// computes the shift polygons for a single gear according to the whitebook 2016
			/// </summary>
			/// <param name="type"></param>
			/// <param name="gearIdx">index of the gear to compute the shift polygons for  -- gear number - 1!</param>
			/// <param name="fullLoadCurve">engine full load curve, potentially limited by the gearbox</param>
			/// <param name="gears">list of gears</param>
			/// <param name="engine">engine data</param>
			/// <param name="axlegearRatio"></param>
			/// <param name="dynamicTyreRadius"></param>
			/// <returns></returns>
			public static ShiftPolygon ComputeShiftPolygon(GearboxType type, int gearIdx, EngineFullLoadCurve fullLoadCurve,
				IList<ITransmissionInputData> gears, CombustionEngineData engine, double axlegearRatio, Meter dynamicTyreRadius)
			{
				return type.AutomaticTransmission()
					? TorqueConverter.ComputeShiftPolygon(fullLoadCurve, gearIdx == 0, gearIdx >= gears.Count - 1)
					// That's the same for all gears, so call the same method...
					: ComputeManualTransmissionShiftPolygon(gearIdx, fullLoadCurve, gears, engine, axlegearRatio, dynamicTyreRadius);
			}

			public static ShiftPolygon ComputeManualTransmissionShiftPolygon(int gearIdx, EngineFullLoadCurve fullLoadCurve,
				IList<ITransmissionInputData> gears, CombustionEngineData engine, double axlegearRatio, Meter dynamicTyreRadius)
			{
				if (gears.Count < 2) {
					throw new VectoException("ComputeShiftPolygon needs at least 2 gears. {0} gears given.", gears.Count);
				}

				// ReSharper disable once InconsistentNaming
				var engineSpeed85kmhLastGear = ComputeEngineSpeed85kmh(gears[gears.Count - 1], axlegearRatio, dynamicTyreRadius);

				var nVHigh = VectoMath.Min(engineSpeed85kmhLastGear, engine.FullLoadCurves[0].RatedSpeed);

				var diffRatio = gears[gears.Count - 2].Ratio / gears[gears.Count - 1].Ratio - 1;

				var maxDragTorque = fullLoadCurve.MaxDragTorque * 1.1;

				var p1 = new Point(engine.IdleSpeed.Value() / 2, 0);
				var p2 = new Point(engine.IdleSpeed.Value() * 1.1, 0);
				var p3 = new Point(nVHigh.Value() * 0.9,
					fullLoadCurve.FullLoadStationaryTorque(nVHigh * 0.9).Value());

				var p4 = new Point((nVHigh * (1 + diffRatio / 3)).Value(), 0);
				var p5 = new Point(fullLoadCurve.N95hSpeed.Value(), fullLoadCurve.MaxTorque.Value());

				var p6 = new Point(p2.X, VectoMath.Interpolate(p1, p3, p2.X));
				var p7 = new Point(p4.X, VectoMath.Interpolate(p2, p5, p4.X));

				var fldMargin = ShiftPolygonFldMargin(fullLoadCurve.FullLoadEntries, (p3.X * 0.95).SI<PerSecond>());
				var downshiftCorr = MoveDownshiftBelowFld(Edge.Create(p6, p3), fldMargin, 1.1 * fullLoadCurve.MaxTorque);

				var downShift = new List<ShiftPolygon.ShiftPolygonEntry>();
				if (gearIdx > 0) {
					downShift =
						new[] { p2, downshiftCorr.P1, downshiftCorr.P2 }.Select(
							point => new ShiftPolygon.ShiftPolygonEntry(point.Y.SI<NewtonMeter>(), point.X.SI<PerSecond>())).ToList();

					downShift[0].Torque = maxDragTorque;
				}
				var upShift = new List<ShiftPolygon.ShiftPolygonEntry>();
				if (gearIdx >= gears.Count - 1) {
					return new ShiftPolygon(downShift, upShift);
				}

				var gearRatio = gears[gearIdx].Ratio / gears[gearIdx + 1].Ratio;
				var rpmMarginFactor = 1 + ShiftPolygonRPMMargin / 100.0;

				// ReSharper disable InconsistentNaming
				var p2p = new Point(p2.X * gearRatio * rpmMarginFactor, p2.Y / gearRatio);
				var p3p = new Point(p3.X * gearRatio * rpmMarginFactor, p3.Y / gearRatio);
				var p6p = new Point(p6.X * gearRatio * rpmMarginFactor, p6.Y / gearRatio);
				var edgeP6pP3p = new Edge(p6p, p3p);
				var p3pExt = new Point((1.1 * p5.Y - edgeP6pP3p.OffsetXY) / edgeP6pP3p.SlopeXY, 1.1 * p5.Y);
				// ReSharper restore InconsistentNaming

				var upShiftPts = IntersectTakeHigherShiftLine(new[] { p4, p7, p5 }, new[] { p2p, p6p, p3pExt });
				if (gears[gearIdx].MaxInputSpeed != null) {
					var maxSpeed = gears[gearIdx].MaxInputSpeed.Value();
					upShiftPts = IntersectTakeLowerShiftLine(upShiftPts,
						new[] { new Point(maxSpeed, 0), new Point(maxSpeed, upShiftPts.Max(pt => pt.Y)) });
				}
				upShift =
					upShiftPts.Select(point => new ShiftPolygon.ShiftPolygonEntry(point.Y.SI<NewtonMeter>(), point.X.SI<PerSecond>()))
						.ToList();
				upShift[0].Torque = maxDragTorque;
				return new ShiftPolygon(downShift, upShift);
			}

			/// <summary>
			/// ensures the original downshift line is below the (already reduced) full-load curve
			/// </summary>
			/// <param name="shiftLine">second part of the shift polygon (slope)</param>
			/// <param name="fldMargin">reduced full-load curve</param>
			/// <param name="maxTorque">max torque</param>
			/// <returns>returns a corrected shift polygon segment (slope) that is below the full load curve and reaches given maxTorque. the returned segment has the same slope</returns>
			internal static Edge MoveDownshiftBelowFld(Edge shiftLine, IEnumerable<Point> fldMargin, NewtonMeter maxTorque)
			{
				var slope = shiftLine.SlopeXY;
				var d = shiftLine.P2.Y - slope * shiftLine.P2.X;

				d = fldMargin.Select(point => point.Y - slope * point.X).Concat(new[] { d }).Min();
				var p6Corr = new Point(shiftLine.P1.X, shiftLine.P1.X * slope + d);
				var p3Corr = new Point((maxTorque.Value() - d) / slope, maxTorque.Value());
				return Edge.Create(p6Corr, p3Corr);
			}

			/// <summary>
			/// reduce the torque of the full load curve up to the given rpms
			/// </summary>
			/// <param name="fullLoadCurve"></param>
			/// <param name="rpmLimit"></param>
			/// <returns></returns>
			internal static IEnumerable<Point> ShiftPolygonFldMargin(List<EngineFullLoadCurve.FullLoadCurveEntry> fullLoadCurve,
				PerSecond rpmLimit)
			{
				return fullLoadCurve.TakeWhile(fldEntry => fldEntry.EngineSpeed < rpmLimit)
					.Select(fldEntry =>
						new Point(fldEntry.EngineSpeed.Value(), fldEntry.TorqueFullLoad.Value() * ShiftPolygonEngineFldMargin))
					.ToList();
			}

			// ReSharper disable once InconsistentNaming
			private static PerSecond ComputeEngineSpeed85kmh(ITransmissionInputData gear, double axleRatio,
				Meter dynamicTyreRadius)
			{
				var engineSpeed = TruckMaxAllowedSpeed / dynamicTyreRadius * axleRatio * gear.Ratio;
				return engineSpeed;
			}

			internal static Point[] IntersectTakeHigherShiftLine(Point[] orig, Point[] transformedDownshift)
			{
				var intersections = Intersect(orig, transformedDownshift);

				// add all points (i.e. intersecting points and both line segments) to a single list
				var pointSet = new List<Point>(orig);
				pointSet.AddRange(transformedDownshift);
				pointSet.AddRange(intersections);
				pointSet.AddRange(ProjectPointsToLineSegments(orig, transformedDownshift));
				pointSet.AddRange(ProjectPointsToLineSegments(transformedDownshift, orig));

				// line sweeping from max_X to 0: select point with lowest Y coordinate, abort if a point has Y = 0
				var shiftPolygon = new List<Point>();
				foreach (var xCoord in pointSet.Select(pt => pt.X).Distinct().OrderBy(x => x).Reverse()) {
					var coord = xCoord;
					var xPoints = pointSet.Where(pt => pt.X.IsEqual(coord) && !pt.Y.IsEqual(0)).ToList();
					shiftPolygon.Add(xPoints.MinBy(pt => pt.Y));
					var tmp = pointSet.Where(pt => pt.X.IsEqual(coord)).Where(pt => pt.Y.IsEqual(0)).ToList();
					if (!tmp.Any()) {
						continue;
					}
					shiftPolygon.Add(tmp.First());
					break;
				}

				// find and remove colinear points
				var toRemove = new List<Point>();
				for (var i = 0; i < shiftPolygon.Count - 2; i++) {
					var edge = new Edge(shiftPolygon[i], shiftPolygon[i + 2]);
					if (edge.ContainsXY(shiftPolygon[i + 1])) {
						toRemove.Add(shiftPolygon[i + 1]);
					}
				}
				foreach (var point in toRemove) {
					shiftPolygon.Remove(point);
				}

				// order points first by x coordinate and the by Y coordinate ASC
				return shiftPolygon.OrderBy(pt => pt.X).ThenBy(pt => pt.Y).ToArray();
			}

			internal static Point[] IntersectTakeLowerShiftLine(Point[] upShiftPts, Point[] upperLimit)
			{
				var intersections = Intersect(upShiftPts, upperLimit);
				if (!intersections.Any()) {
					return upShiftPts[0].X < upperLimit[0].X ? upShiftPts : upperLimit;
				}
				var pointSet = new List<Point>(upShiftPts);
				pointSet.AddRange(upperLimit);
				pointSet.AddRange(intersections);
				pointSet.AddRange(ProjectPointsToLineSegments(upShiftPts, upperLimit, true));
				pointSet.AddRange(ProjectPointsToLineSegments(upperLimit, upShiftPts, true));

				var shiftPolygon = new List<Point>();
				foreach (var yCoord in pointSet.Select(pt => pt.Y).Distinct().OrderBy(y => y).Reverse()) {
					var yPoints = pointSet.Where(pt => pt.Y.IsEqual(yCoord)).ToList();
					shiftPolygon.Add(yPoints.MinBy(pt => pt.X));
				}

				// find and remove colinear points
				var toRemove = new List<Point>();
				for (var i = 0; i < shiftPolygon.Count - 2; i++) {
					var edge = new Edge(shiftPolygon[i], shiftPolygon[i + 2]);
					if (edge.ContainsXY(shiftPolygon[i + 1])) {
						toRemove.Add(shiftPolygon[i + 1]);
					}
				}
				foreach (var point in toRemove) {
					shiftPolygon.Remove(point);
				}

				// order points first by x coordinate and the by Y coordinate ASC
				return shiftPolygon.OrderBy(pt => pt.X).ThenBy(pt => pt.Y).ToArray();
			}

			private static Point[] Intersect(Point[] orig, Point[] transformedDownshift)
			{
				var intersections = new List<Point>();
				// compute all intersection points between both line segments
				// ReSharper disable once LoopCanBeConvertedToQuery
				foreach (var origLine in orig.Pairwise(Edge.Create)) {
					// ReSharper disable once LoopCanBeConvertedToQuery
					foreach (var transformedLine in transformedDownshift.Pairwise(Edge.Create)) {
						var isect = VectoMath.Intersect(origLine, transformedLine);
						if (isect != null) {
							intersections.Add(isect);
						}
					}
				}
				return intersections.ToArray();
			}

			private static IEnumerable<Point> ProjectPointsToLineSegments(IEnumerable<Point> lineSegments, Point[] points,
				bool projectToVertical = false)
			{
				var pointSet = new List<Point>();
				foreach (var segment in lineSegments.Pairwise(Edge.Create)) {
					if (segment.P1.X.IsEqual(segment.P2.X)) {
						if (projectToVertical) {
							pointSet.AddRange(
								points.Select(point => new Point(segment.P1.X, point.Y)).Where(pt => pt.Y.IsBetween(segment.P1.Y, segment.P2.Y)));
						}
						continue;
					}
					var k = segment.SlopeXY;
					var d = segment.P1.Y - segment.P1.X * k;
					pointSet.AddRange(points.Select(point => new Point(point.X, point.X * k + d)));
				}
				return pointSet;
			}
		}

		public static class TorqueConverter
		{
			public static readonly PerSecond ReferenceRPM = 1000.RPMtoRad();
			public static readonly PerSecond MaxInputSpeed = 5000.RPMtoRad();
			public static readonly MeterPerSquareSecond CLUpshiftMinAcceleration = 0.1.SI<MeterPerSquareSecond>();
			public static readonly MeterPerSquareSecond CCUpshiftMinAcceleration = 0.1.SI<MeterPerSquareSecond>();

			private static readonly PerSecond DownshiftPRM = 700.RPMtoRad();
			private static readonly PerSecond UpshiftLowRPM = 900.RPMtoRad();
			private static readonly PerSecond UpshiftHighRPM = 1150.RPMtoRad();

			public static ShiftPolygon ComputeShiftPolygon(EngineFullLoadCurve fullLoadCurve, bool first = false,
				bool last = false)
			{
				var maxDragTorque = fullLoadCurve.MaxDragTorque * 1.1;
				var maxTorque = fullLoadCurve.MaxTorque * 1.1;
				var p0 = new Point(UpshiftLowRPM.Value(), maxDragTorque.Value());
				var p1 = new Point(UpshiftLowRPM.Value(), 0);
				var p2 = new Point(UpshiftHighRPM.Value(), fullLoadCurve.FullLoadStationaryTorque(UpshiftHighRPM).Value());
				var edge = new Edge(p1, p2);
				var p2corr = new Point((maxTorque.Value() - edge.OffsetXY) / edge.SlopeXY, maxTorque.Value());

				var downshift = new[] {
					new ShiftPolygon.ShiftPolygonEntry(maxDragTorque, DownshiftPRM),
					new ShiftPolygon.ShiftPolygonEntry(maxTorque, DownshiftPRM)
				};
				var upshift = new[] { p0, p1, p2corr }.Select(
					pt => new ShiftPolygon.ShiftPolygonEntry(pt.Y.SI<NewtonMeter>(), pt.X.SI<PerSecond>()));

				return new ShiftPolygon(first ? new List<ShiftPolygon.ShiftPolygonEntry>() : downshift.ToList(),
					last ? new List<ShiftPolygon.ShiftPolygonEntry>() : upshift.ToList());
			}

			public static IEnumerable<TorqueConverterEntry> GetTorqueConverterDragCurve(double ratio)
			{
				var resourceId = DeclarationDataResourcePrefix + ".TorqueConverter.csv";
				var data = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(resourceId), source: resourceId);
				var characteristicTorque = (from DataRow row in data.Rows
					select
						new TorqueConverterEntry() {
							SpeedRatio = row.ParseDouble(TorqueConverterDataReader.Fields.SpeedRatio),
							Torque = row.ParseDouble(TorqueConverterDataReader.Fields.CharacteristicTorque).SI<NewtonMeter>(),
							TorqueRatio = row.ParseDouble(TorqueConverterDataReader.Fields.TorqueRatio)
						}).ToArray();
				foreach (var torqueConverterEntry in characteristicTorque) {
					torqueConverterEntry.SpeedRatio = torqueConverterEntry.SpeedRatio * ratio;
					torqueConverterEntry.TorqueRatio = torqueConverterEntry.TorqueRatio / ratio;
				}
				return characteristicTorque.Where(x => x.SpeedRatio >= ratio).ToArray();
			}
		}

		public static class PTO
		{
			public const string DefaultPTOTechnology =
				"only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel";

			public const string DefaultPTOIdleLosses =
				DeclarationDataResourcePrefix + ".MissionCycles.MunicipalUtility_PTO_generic.vptol";

			public const string DefaultPTOActivationCycle =
				DeclarationDataResourcePrefix + ".MissionCycles.MunicipalUtility_PTO_generic.vptoc";
		}

		public static class VTPMode
		{
			public static readonly Meter RunInThreshold = 15000.SI(Unit.SI.Kilo.Meter).Cast<Meter>();
			public const double EvolutionCoefficient = 0.98;

			public const MissionType SelectedMission = MissionType.LongHaul;
			public const LoadingType SelectedLoading = LoadingType.ReferenceLoad;

			// verification of input data

			public const double WheelSpeedDifferenceFactor = 1.4;
			public const double WheelTorqueDifferenceFactor = 3;

			public static readonly PerSecond WheelSpeedZeroTolerance = 0.1.RPMtoRad();
			public static readonly PerSecond MaxWheelSpeedDifferenceStandstill = 1.RPMtoRad();

			public static readonly NewtonMeter MaxWheelTorqueDifference = 200.SI<NewtonMeter>();

			public static readonly PerSecond MinFanSpeed = 20.RPMtoRad();
			public static readonly PerSecond MaxFanSpeed = 4000.RPMtoRad();

			public static readonly Second SamplingInterval = 0.5.SI<Second>();

			public static readonly WattSecond MinPosWorkAtWheelsForFC = 1.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>();
			public static readonly SpecificFuelConsumption LowerFCThreshold = 180.SI(Unit.SI.Gramm.Per.Kilo.Watt.Hour).Cast<SpecificFuelConsumption>();
			public static readonly SpecificFuelConsumption UpperFCThreshold = 600.SI(Unit.SI.Gramm.Per.Kilo.Watt.Hour).Cast<SpecificFuelConsumption>();
			public static readonly Second FCAccumulationWindow = 10.SI(Unit.SI.Minute).Cast<Second>();
		}

		public static class Vehicle {
			public const bool DualFuelVehicleDefault = false;
			public const bool HybridElectricHDVDefault = false;
			public const bool ZeroEmissionVehicleDefault = false;
			public const NgTankSystem NgTankSystemDefault = NgTankSystem.Compressed;
			public const bool SleeperCabDefault = true;
			public const bool VocationalVehicleDefault = false;

			public static class ADAS {
				public const PredictiveCruiseControlType PredictiveCruiseControlDefault = PredictiveCruiseControlType.None;
				public const bool EcoRollWithEngineStop = false;
				public const bool EcoRollWitoutEngineStop = false;
				public const bool EngineStopStartDefault = false;
			}
		}
	}
}