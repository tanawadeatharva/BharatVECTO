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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
{
	[CustomValidation(typeof(TorqueConverterData), "ValidateData")]
	public class TorqueConverterData : SimulationComponentData
	{
		protected internal readonly TorqueConverterEntry[] TorqueConverterEntries;

		[Required, SIRange(0, double.MaxValue)]
		public PerSecond ReferenceSpeed { get; protected internal set; }

		[Required, SIRange(0, double.MaxValue)]
		public MeterPerSquareSecond CLUpshiftMinAcceleration { get; internal set; }

		[Required, SIRange(0, double.MaxValue)]
		public MeterPerSquareSecond CCUpshiftMinAcceleration { get; internal set; }

		[Required, SIRange(0, double.MaxValue)]
		public PerSecond TorqueConverterSpeedLimit { get; protected internal set; }

		internal double RequiredSpeedRatio; // only used for validation!

		protected internal TorqueConverterData(IEnumerable<TorqueConverterEntry> torqueConverterEntries,
			PerSecond referenceSpeed, PerSecond maxRpm, MeterPerSquareSecond clUpshiftMinAcceleration,
			MeterPerSquareSecond ccUpshiftMinAcceleration)
		{
			TorqueConverterEntries = torqueConverterEntries.ToArray();
			ReferenceSpeed = referenceSpeed;
			TorqueConverterSpeedLimit = maxRpm;
			CLUpshiftMinAcceleration = clUpshiftMinAcceleration;
			CCUpshiftMinAcceleration = ccUpshiftMinAcceleration;
		}

		/// <summary>
		/// find an operating point for the torque converter
		/// 
		/// find the input speed and input torque for the given output torque and output speed. 
		/// </summary>
		/// <param name="torqueOut">torque provided at the TC output</param>
		/// <param name="angularSpeedOut">angular speed at the TC output</param>
		/// <param name="minSpeed"></param>
		/// <returns></returns>
		public IList<TorqueConverterOperatingPoint> FindOperatingPoint(NewtonMeter torqueOut, PerSecond angularSpeedOut,
			PerSecond minSpeed)
		{
			var solutions = new List<double>();
			var mpNorm = ReferenceSpeed.Value();

			var min = minSpeed == null ? 0 : minSpeed.Value();

			// Find analytic solution for torque converter operating point
			// mu = f(nu) = f(n_out / n_in) = T_out / T_in
			// MP1000 = f(nu) = f(n_out / n_in)
			// Tq_in = MP1000(nu) * (n_in/1000)^2 = T_out / mu
			//
			// mu(nu) and MP1000(nu) are provided as piecewise linear functions (y = k*x+d)
			// solving the equation above for n_in results in a quadratic equation
			foreach (var segment in TorqueConverterEntries.Pairwise(Tuple.Create)) {
				var mpEdge = Edge.Create(new Point(segment.Item1.SpeedRatio, segment.Item1.Torque.Value()),
					new Point(segment.Item2.SpeedRatio, segment.Item2.Torque.Value()));
				var muEdge = Edge.Create(new Point(segment.Item1.SpeedRatio, segment.Item1.TorqueRatio),
					new Point(segment.Item2.SpeedRatio, segment.Item2.TorqueRatio));

				var a = muEdge.OffsetXY * mpEdge.OffsetXY / (mpNorm * mpNorm);
				var b = angularSpeedOut.Value() * (muEdge.SlopeXY * mpEdge.OffsetXY + mpEdge.SlopeXY * muEdge.OffsetXY) /
						(mpNorm * mpNorm);
				var c = angularSpeedOut.Value() * angularSpeedOut.Value() * mpEdge.SlopeXY * muEdge.SlopeXY / (mpNorm * mpNorm) -
						torqueOut.Value();

				solutions.AddRange(VectoMath.QuadraticEquationSolver(a, b, c).Where(x => {
					var ratio = angularSpeedOut.Value() / x;
					return x > min && muEdge.P1.X <= ratio && ratio < muEdge.P2.X;
				}));
			}

			if (solutions.Count == 0) {
				Log.Debug(
					"TorqueConverterData::FindOperatingPoint No solution for input torque/input speed found! n_out: {0}, tq_out: {1}",
					angularSpeedOut, torqueOut);
			}

			return solutions.Select(sol => {
				var s = sol.SI<PerSecond>();
				var mu = MuLookup(angularSpeedOut / s);
				return new TorqueConverterOperatingPoint {
					OutTorque = torqueOut,
					OutAngularVelocity = angularSpeedOut,
					InAngularVelocity = s,
					SpeedRatio = angularSpeedOut / s,
					TorqueRatio = mu,
					InTorque = torqueOut / mu,
				};
			}).ToList();
		}

		/// <summary>
		/// find an operating point for the torque converter
		/// 
		/// find the input torque and output torque for the given input and output speeds.
		/// Computes the speed ratio nu of input and output. Interpolates MP1000 and mu, Computes input torque and output torque
		/// </summary>
		/// <param name="inAngularVelocity">speed at the input of the TC</param>
		/// <param name="outAngularVelocity">speed at the output of the TC</param>
		/// <returns></returns>
		public TorqueConverterOperatingPoint FindOperatingPoint(PerSecond inAngularVelocity, PerSecond outAngularVelocity)
		{
			var retVal = new TorqueConverterOperatingPoint {
				InAngularVelocity = inAngularVelocity,
				OutAngularVelocity = outAngularVelocity,
				SpeedRatio = outAngularVelocity.Value() / inAngularVelocity.Value(),
			};

			foreach (var segment in TorqueConverterEntries.Pairwise()) {
				if (retVal.SpeedRatio.IsBetween(segment.Item1.SpeedRatio, segment.Item2.SpeedRatio)) {
					var mpTorque = segment.Interpolate(x => x.SpeedRatio, y => y.Torque, retVal.SpeedRatio);
					retVal.TorqueRatio = segment.Interpolate(x => x.SpeedRatio, y => y.TorqueRatio, retVal.SpeedRatio);
					retVal.InTorque = mpTorque * (inAngularVelocity * inAngularVelocity / ReferenceSpeed / ReferenceSpeed).Value();
					retVal.OutTorque = retVal.InTorque * retVal.TorqueRatio;
					return retVal;
				}
			}

			// No solution found. Throw Error
			var nu = outAngularVelocity / inAngularVelocity;
			var nuMax = TorqueConverterEntries.Last().SpeedRatio;

			if (nu.IsGreater(nuMax)) {
				throw new VectoException(
					"Torque Converter: Range of torque converter data is not sufficient. Required nu: {0}, Got nu_max: {1}", nu, nuMax);
			}

			throw new VectoException(
				"Torque Converter: No solution for output speed/input speed found! n_out: {0}, n_in: {1}, nu: {2}, nu_max: {3}",
				outAngularVelocity, inAngularVelocity, nu, nuMax);
		}

		/// <summary>
		/// find an operating point for the torque converter
		/// 
		/// find the output torque and output speed for the given input torque and input speed
		/// </summary>
		/// <param name="inTorque"></param>
		/// <param name="inAngularVelocity"></param>
		/// <param name="outAngularSpeedEstimated"></param>
		/// <returns></returns>
		public TorqueConverterOperatingPoint FindOperatingPointForward(NewtonMeter inTorque, PerSecond inAngularVelocity,
			PerSecond outAngularSpeedEstimated)
		{
			var referenceTorque = inTorque.Value() / inAngularVelocity.Value() / inAngularVelocity.Value() *
								ReferenceSpeed.Value() * ReferenceSpeed.Value();
			var maxTorque = TorqueConverterEntries.Max(x => x.Torque.Value());
			if (referenceTorque.IsGreaterOrEqual(maxTorque)) {
				referenceTorque = outAngularSpeedEstimated != null
					? ReferenceTorqueLookup(outAngularSpeedEstimated / inAngularVelocity).Value()
					: 0.9 * maxTorque;
			}

			var solutions = new List<double>();
			foreach (var edge in TorqueConverterEntries.Pairwise(
				(p1, p2) => Edge.Create(new Point(p1.SpeedRatio, p1.Torque.Value()), new Point(p2.SpeedRatio, p2.Torque.Value())))) {
				var x = (referenceTorque - edge.OffsetXY) / edge.SlopeXY;
				if (x >= edge.P1.X && x < edge.P2.X) {
					solutions.Add(x * inAngularVelocity.Value());
				}
			}
			if (solutions.Count == 0) {
				throw new VectoSimulationException(
					"Torque Converter: Failed to find operating point for inputTorque/inputSpeed! n_in: {0}, tq_in: {1}",
					inAngularVelocity, inTorque);
			}
			return FindOperatingPoint(inAngularVelocity, solutions.Max().SI<PerSecond>());
		}

		public TorqueConverterOperatingPoint FindOperatingPointForPowerDemand(Watt power, PerSecond prevInputSpeed,
			PerSecond nextOutputSpeed, KilogramSquareMeter inertia, Second dt, Watt previousPower)
		{
			var solutions = new List<double>();
			var mpNorm = ReferenceSpeed.Value();

			foreach (var segment in TorqueConverterEntries.Pairwise(Tuple.Create)) {
				var mpEdge = Edge.Create(new Point(segment.Item1.SpeedRatio, segment.Item1.Torque.Value()),
					new Point(segment.Item2.SpeedRatio, segment.Item2.Torque.Value()));

				var a = mpEdge.OffsetXY / (2 * mpNorm * mpNorm);
				var b = inertia.Value() / (2 * dt.Value()) + mpEdge.SlopeXY * nextOutputSpeed.Value() / (2 * mpNorm * mpNorm);
				var c = 0;
				var d = -inertia.Value() * prevInputSpeed.Value() * prevInputSpeed.Value() / (2 * dt.Value()) - power.Value() +
						previousPower.Value() / 2;
				var sol = VectoMath.CubicEquationSolver(a, b, c, d);

				var selected = sol.Where(x => x > 0 && nextOutputSpeed / x >= mpEdge.P1.X && nextOutputSpeed / x < mpEdge.P2.X);
				solutions.AddRange(selected);
			}

			if (solutions.Count == 0) {
				throw new VectoException(
					"Failed to find operating point for power {0}, prevInputSpeed {1}, nextOutputSpeed {2}", power,
					prevInputSpeed, nextOutputSpeed);
			}
			solutions.Sort();

			return FindOperatingPoint(solutions.First().SI<PerSecond>(), nextOutputSpeed);
		}

		private double MuLookup(double speedRatio)
		{
			return TorqueConverterEntries.Interpolate(x => x.SpeedRatio, y => y.TorqueRatio, speedRatio);
		}

		private NewtonMeter ReferenceTorqueLookup(double speedRatio)
		{
			return TorqueConverterEntries.Interpolate(x => x.SpeedRatio, y => y.Torque, speedRatio);
		}

		// ReSharper disable once UnusedMember.Global -- used by validation
		public static ValidationResult ValidateData(TorqueConverterData data, ValidationContext validationContext)
		{
			var min = data.TorqueConverterEntries.Min(e => e.SpeedRatio);
			var max = data.TorqueConverterEntries.Max(e => e.SpeedRatio);
			if (min > 0 || max < data.RequiredSpeedRatio) {
				return new ValidationResult(string.Format(
					"Torque Converter Data invalid - Speedratio has to cover the range from 0.0 to {2}: given data only goes from {0} to {1}",
					min, max, data.RequiredSpeedRatio));
			}

			return ValidationResult.Success;
		}
	}

	public class TorqueConverterOperatingPoint
	{
		public PerSecond OutAngularVelocity;
		public NewtonMeter OutTorque;

		public PerSecond InAngularVelocity;
		public NewtonMeter InTorque;

		public double SpeedRatio;
		public double TorqueRatio;
		public bool Creeping;

		public override string ToString()
		{
			return string.Format("n_out: {0}, n_in: {1}, tq_out: {2}, tq_in {3}, nu: {4}, my: {5}", OutAngularVelocity,
				InAngularVelocity, OutTorque, InTorque, SpeedRatio, TorqueRatio);
		}
	}

	public class TorqueConverterEntry
	{
		public double SpeedRatio;
		public NewtonMeter Torque;
		public double TorqueRatio;
	}
}