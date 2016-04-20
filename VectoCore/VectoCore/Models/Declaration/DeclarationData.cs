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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.interfaces;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class DeclarationData
	{
		private static DeclarationData _instance;
		private Segments _segments;
		private Rims _rims;
		private Wheels _wheels;
		private PT1 _pt1;
		private ElectricSystem _electricSystem;
		private Fan _fan;
		private HeatingVentilationAirConditioning _heatingVentilationAirConditioning;
		private PneumaticSystem _pneumaticSystem;
		private SteeringPump _steeringPump;
		private WHTCCorrection _whtcCorrection;
		private AirDrag _airDrag;
		private TorqueConverter _torqueConverter;

		public static Wheels Wheels
		{
			get { return Instance()._wheels ?? (Instance()._wheels = new Wheels()); }
		}

		public static Rims Rims
		{
			get { return Instance()._rims ?? (Instance()._rims = new Rims()); }
		}

		public static Segments Segments
		{
			get { return Instance()._segments ?? (Instance()._segments = new Segments()); }
		}

		public static PT1 PT1
		{
			get { return Instance()._pt1 ?? (Instance()._pt1 = new PT1()); }
		}

		public static ElectricSystem ElectricSystem
		{
			get { return Instance()._electricSystem ?? (Instance()._electricSystem = new ElectricSystem()); }
		}

		public static Meter DynamicTyreRadius(string wheels, string rims)
		{
			var wheelsEntry = Wheels.Lookup(wheels.RemoveWhitespace());
			try {
			var rimsEntry = Rims.Lookup(rims);

			var correction = wheelsEntry.SizeClass != "a" ? rimsEntry.F_b : rimsEntry.F_a;

			return wheelsEntry.DynamicTyreRadius * correction / (2 * Math.PI);
			} catch (KeyNotFoundException) {
				throw new VectoException(
					"Calculating Dynamic Tyre Radius not possible: Declaration Lookup could not find Key '{0}' for rim.", rims);
		}
		}

		public static Fan Fan
		{
			get { return Instance()._fan ?? (Instance()._fan = new Fan()); }
		}

		public static HeatingVentilationAirConditioning HeatingVentilationAirConditioning
		{
			get
			{
				return Instance()._heatingVentilationAirConditioning ??
						(Instance()._heatingVentilationAirConditioning = new HeatingVentilationAirConditioning());
			}
		}

		public static PneumaticSystem PneumaticSystem
		{
			get { return Instance()._pneumaticSystem ?? (Instance()._pneumaticSystem = new PneumaticSystem()); }
		}

		public static SteeringPump SteeringPump
		{
			get { return Instance()._steeringPump ?? (Instance()._steeringPump = new SteeringPump()); }
		}

		public static WHTCCorrection WHTCCorrection
		{
			get { return Instance()._whtcCorrection ?? (Instance()._whtcCorrection = new WHTCCorrection()); }
		}

		public static AirDrag AirDrag
		{
			get { return Instance()._airDrag ?? (Instance()._airDrag = new AirDrag()); }
		}

		public static TorqueConverter TorqueConverter
		{
			get { return Instance()._torqueConverter ?? (Instance()._torqueConverter = new TorqueConverter()); }
		}

		public static int PoweredAxle()
		{
			return 1;
		}

		private static DeclarationData Instance()
		{
			return _instance ?? (_instance = new DeclarationData());
		}

		//			Public Const SSspeed As Single = 5
		//Public Const SStime As Single = 5
		//Public Const SSdelay As Single = 5
		//Public Const LACa As Single = -0.5
		//Public Const LACvmin As Single = 50
		//Public Const Overspeed As Single = 5
		//Public Const Underspeed As Single = 5
		//Public Const ECvmin As Single = 50

		//Public Const AirDensity As Single = 1.188
		//Public Const FuelDens As Single = 0.832
		//Public Const CO2perFC As Single = 3.16

		//Public Const AuxESeff As Single = 0.7

		public static class Driver
		{
			public static class LookAhead
			{
				public const bool Enabled = true;

				public static readonly MeterPerSquareSecond Deceleration = -0.5.SI<MeterPerSquareSecond>();

				public static readonly MeterPerSecond MinimumSpeed = 50.KMPHtoMeterPerSecond();
			}

			public static class OverSpeedEcoRoll
			{
				public static readonly IList<DriverMode> AllowedModes = new List<DriverMode> {
					DriverMode.EcoRoll,
					DriverMode.Overspeed
				};

				public static readonly MeterPerSecond MinSpeed = 50.KMPHtoMeterPerSecond();
				public static readonly MeterPerSecond OverSpeed = 5.KMPHtoMeterPerSecond();
				public static readonly MeterPerSecond UnderSpeed = 5.KMPHtoMeterPerSecond();
			}

			public static class StartStop
			{
				public static readonly MeterPerSecond MaxSpeed = 5.KMPHtoMeterPerSecond();
				public static readonly Second Delay = 5.SI<Second>();
				public static readonly Second MinTime = 5.SI<Second>();
			}
		}

		public static class Trailer
		{
			public const double RollResistanceCoefficient = 0.00555;
			public const double TyreTestLoad = 37500;
			public const bool TwinTyres = false;
			public const string WheelsType = "385/65 R 22.5";
		}

		public static class Engine
		{
			public static readonly KilogramSquareMeter ClutchInertia = 1.3.SI<KilogramSquareMeter>();
			public static readonly KilogramSquareMeter EngineBaseInertia = 0.41.SI<KilogramSquareMeter>();
			public static readonly SI EngineDisplacementInertia = (0.27 * 1000).SI().Kilo.Gramm.Per.Meter; // [kg/m]

			public static KilogramSquareMeter EngineInertia(SI displacement)
			{
				// VB Code:    Return 1.3 + 0.41 + 0.27 * (Displ / 1000)
				return ClutchInertia + EngineBaseInertia + EngineDisplacementInertia * displacement;
			}
		}

		public static class Gearbox
		{
			public const double TorqueReserve = 0.2;
			public const double TorqueReserveStart = 0.2;
			public static readonly MeterPerSecond StartSpeed = 2.SI<MeterPerSecond>();
			public static readonly MeterPerSquareSecond StartAcceleration = 0.6.SI<MeterPerSquareSecond>();
			public static readonly KilogramSquareMeter Inertia = 0.SI<KilogramSquareMeter>();

			public static readonly MeterPerSecond TruckMaxAllowedSpeed = 85.KMPHtoMeterPerSecond();
			public static double ShiftPolygonRPMMargin = 7;
			private static double ShiftPolygonEngineFldMargin = 0.98;

			public static readonly Second MinTimeBetweenGearshifts = 2.SI<Second>();

			/// <summary>
			/// computes the shift polygons for a single gear according to the whitebook 2016
			/// </summary>
			/// <param name="gear">index of the gear to compute the shift polygons for</param>
			/// <param name="fullLoadCurve">engine full load curve, potentially limited by the gearbox</param>
			/// <param name="gears">list of gears</param>
			/// <param name="engine">engine data</param>
			/// <param name="axlegearRatio"></param>
			/// <param name="dynamicTyreRadius"></param>
			/// <returns></returns>
			public static ShiftPolygon ComputeShiftPolygon(int gear, FullLoadCurve fullLoadCurve,
				IList<ITransmissionInputData> gears, CombustionEngineData engine, double axlegearRatio, Meter dynamicTyreRadius)
			{
				var engineSpeed85kmhLastGear = ComputeEngineSpeed85kmh(gears[gears.Count - 1], axlegearRatio, dynamicTyreRadius,
					engine);
				var engineSpeed85kmhSecondToLastGear = ComputeEngineSpeed85kmh(gears[gears.Count - 2], axlegearRatio,
					dynamicTyreRadius, engine);

				var maxDragTorque = engine.FullLoadCurve.MaxDragTorque * 1.1;

				var p1 = new Point(engine.IdleSpeed.Value() / 2, 0);
				var p2 = new Point(engine.IdleSpeed.Value() * 1.1, 0);
				var p3 = new Point(engineSpeed85kmhLastGear.Value() * 0.9,
					fullLoadCurve.FullLoadStationaryTorque(engineSpeed85kmhLastGear * 0.9).Value());

				var p4 =
					new Point((engineSpeed85kmhLastGear + (engineSpeed85kmhSecondToLastGear - engineSpeed85kmhLastGear) / 3).Value(), 0);
				var p5 = new Point(fullLoadCurve.RatedSpeed.Value() * 0.95, fullLoadCurve.MaxTorque.Value());

				var p6 = new Point(p2.X, VectoMath.Interpolate(p1, p3, p2.X));
				var p7 = new Point(p4.X, VectoMath.Interpolate(p2, p5, p4.X));

				var fldMargin = ShiftPolygonFldMargin(fullLoadCurve.FullLoadEntries, engineSpeed85kmhLastGear * 0.9);
				var downshiftCorr = MoveDownshiftBelowFld(Edge.Create(p6, p3), fldMargin, 1.1 * fullLoadCurve.MaxTorque);
				var downShift =
					new[] { p2, downshiftCorr.P1, downshiftCorr.P2 }.Select(
						point => new ShiftPolygon.ShiftPolygonEntry() {
							AngularSpeed = point.X.SI<PerSecond>(),
							Torque = point.Y.SI<NewtonMeter>()
						}).ToList();

				downShift[0].Torque = maxDragTorque;

				var upShift = new List<ShiftPolygon.ShiftPolygonEntry>();
				if (gear >= gears.Count - 1) {
					return new ShiftPolygon(downShift, upShift);
				}

				var gearRatio = gears[gear].Ratio / gears[gear + 1].Ratio;
				var rpmMarginFactor = 1 + DeclarationData.Gearbox.ShiftPolygonRPMMargin / 100.0;

				var p2p = new Point(p2.X * gearRatio * rpmMarginFactor, p2.Y / gearRatio);
				var p3p = new Point(p3.X * gearRatio * rpmMarginFactor, p3.Y / gearRatio);
				var p6p = new Point(p6.X * gearRatio * rpmMarginFactor, p6.Y / gearRatio);
				var edgeP6pP3p = new Edge(p6p, p3p);
				var p3pExt = new Point((1.1 * p5.Y - edgeP6pP3p.OffsetXY) / edgeP6pP3p.SlopeXY, 1.1 * p5.Y);

				upShift = IntersectShiftPolygon(new[] { p4, p7, p5 }.ToList(), new[] { p2p, p6p, p3pExt }.ToList())
					.Select(point => new ShiftPolygon.ShiftPolygonEntry() {
						AngularSpeed = point.X.SI<PerSecond>(),
						Torque = point.Y.SI<NewtonMeter>()
					}).ToList();
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
			internal static IEnumerable<Point> ShiftPolygonFldMargin(List<FullLoadCurve.FullLoadCurveEntry> fullLoadCurve,
				PerSecond rpmLimit)
			{
				return
					fullLoadCurve.TakeWhile(fldEntry => fldEntry.EngineSpeed < rpmLimit)
						.Select(
							fldEntry =>
								new Point(fldEntry.EngineSpeed.Value(), fldEntry.TorqueFullLoad.Value() * ShiftPolygonEngineFldMargin))
						.ToList();
			}


			internal static PerSecond ComputeEngineSpeed85kmh(ITransmissionInputData gear, double axleRatio,
				Meter dynamicTyreRadius, CombustionEngineData engine)
			{
				var engineSpeed = TruckMaxAllowedSpeed / dynamicTyreRadius * axleRatio * gear.Ratio;
				if (engineSpeed < engine.IdleSpeed) {
					throw new VectoException("engine speed at velocity {0} in gear {1} is below engine's idle speed! {2}",
						DeclarationData.Gearbox.TruckMaxAllowedSpeed, gear.Gear, engineSpeed);
				}
				if (engineSpeed > engine.FullLoadCurve.FullLoadEntries.Last().EngineSpeed) {
					throw new VectoException("engine speed at velocity {0} in gear {1} is above engine's max speed! {2}",
						DeclarationData.Gearbox.TruckMaxAllowedSpeed, gear.Gear, engineSpeed);
				}
				return engineSpeed;
			}


			internal static List<Point> IntersectShiftPolygon(List<Point> orig, List<Point> transformedDownshift)
			{
				var intersections = new List<Point>();
				// compute all intersection points between both line segments
				foreach (var origLine in orig.Pairwise(Edge.Create)) {
					foreach (var transformedLine in transformedDownshift.Pairwise(Edge.Create)) {
						var isect = VectoMath.Intersect(origLine, transformedLine);
						if (isect != null) {
							intersections.Add(isect);
						}
					}
				}

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
				return shiftPolygon.OrderBy(pt => pt.X).ThenBy(pt => pt.Y).ToList();
			}

			private static List<Point> ProjectPointsToLineSegments(List<Point> lineSegments, List<Point> points)
			{
				var pointSet = new List<Point>();
				foreach (var segment in lineSegments.Pairwise(Edge.Create)) {
					if (segment.P1.X.IsEqual(segment.P2.X)) {
						continue;
					}
					var k = segment.SlopeXY;
					var d = segment.P1.Y - segment.P1.X * k;
					pointSet.AddRange(points.Select(point => new Point(point.X, point.X * k + d)));
				}
				return pointSet;
			}
		}

		public static IEnumerable<string> AuxiliaryIDs()
		{
			return new[] {
				Constants.Auxiliaries.IDs.Fan, Constants.Auxiliaries.IDs.SteeringPump,
				Constants.Auxiliaries.IDs.HeatingVentilationAirCondition, Constants.Auxiliaries.IDs.ElectricSystem,
				Constants.Auxiliaries.IDs.PneumaticSystem
			};
		}
	}
}