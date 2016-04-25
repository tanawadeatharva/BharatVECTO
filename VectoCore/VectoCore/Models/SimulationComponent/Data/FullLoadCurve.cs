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
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class FullLoadCurve : SimulationComponentData
	{
		private Watt _maxPower;
		private PerSecond _ratedSpeed;
		private NewtonMeter _maxTorque;

		[Required, ValidateObject] internal List<FullLoadCurveEntry> FullLoadEntries;

		[Required] internal LookupData<PerSecond, Second> PT1Data;

		/// <summary>
		/// Get the rated speed from the given full-load curve (i.e. speed with max. power)
		/// </summary>
		[Required, SIRange(0, 5000 * Constants.RPMToRad)]
		public PerSecond RatedSpeed
		{
			get { return _ratedSpeed ?? ComputeRatedSpeed().Item1; }
		}

		/// <summary>
		/// Gets the maximum power.
		/// </summary>
		[Required, SIRange(0, 10000 * 5000 * Constants.RPMToRad)]
		public Watt MaxPower
		{
			get { return _maxPower ?? ComputeRatedSpeed().Item2; }
		}

		public NewtonMeter MaxTorque
		{
			get { return _maxTorque ?? FindMaxTorque(); }
		}

		public virtual NewtonMeter FullLoadStationaryTorque(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return VectoMath.Interpolate(FullLoadEntries[idx - 1].EngineSpeed, FullLoadEntries[idx].EngineSpeed,
				FullLoadEntries[idx - 1].TorqueFullLoad, FullLoadEntries[idx].TorqueFullLoad,
				angularVelocity);
		}

		public virtual NewtonMeter DragLoadStationaryTorque(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return VectoMath.Interpolate(FullLoadEntries[idx - 1].EngineSpeed, FullLoadEntries[idx].EngineSpeed,
				FullLoadEntries[idx - 1].TorqueDrag, FullLoadEntries[idx].TorqueDrag,
				angularVelocity);
		}

		private NewtonMeter FindMaxTorque()
		{
			_maxTorque = FullLoadEntries.Max(x => x.TorqueFullLoad);
			return _maxTorque;
		}

		/// <summary>
		/// Compute the engine's rated speed from the given full-load curve (i.e. engine speed with max. power)
		/// </summary>
		protected Tuple<PerSecond, Watt> ComputeRatedSpeed()
		{
			var max = new Tuple<PerSecond, Watt>(0.SI<PerSecond>(), 0.SI<Watt>());
			for (var idx = 1; idx < FullLoadEntries.Count; idx++) {
				var currentMax = FindMaxPower(FullLoadEntries[idx - 1], FullLoadEntries[idx]);
				if (currentMax.Item2 > max.Item2) {
					max = currentMax;
				}
			}

			_ratedSpeed = max.Item1;
			_maxPower = max.Item2;

			return max;
		}

		private Tuple<PerSecond, Watt> FindMaxPower(FullLoadCurveEntry p1, FullLoadCurveEntry p2)
		{
			if (p1.EngineSpeed.IsEqual(p2.EngineSpeed)) {
				return Tuple.Create(p1.EngineSpeed, p1.TorqueFullLoad * p1.EngineSpeed);
			}

			if (p2.EngineSpeed < p1.EngineSpeed) {
				var tmp = p1;
				p1 = p2;
				p2 = tmp;
			}

			// y = kx + d
			var k = (p2.TorqueFullLoad - p1.TorqueFullLoad) / (p2.EngineSpeed - p1.EngineSpeed);
			var d = p2.TorqueFullLoad - k * p2.EngineSpeed;
			if (k.IsEqual(0)) {
				return Tuple.Create(p2.EngineSpeed, p2.TorqueFullLoad * p2.EngineSpeed);
			}
			var engineSpeedMaxPower = -d / (2 * k);
			if (engineSpeedMaxPower.IsSmaller(p1.EngineSpeed) || engineSpeedMaxPower.IsGreater(p2.EngineSpeed)) {
				if (k.IsGreater(0)) {
					return Tuple.Create(p2.EngineSpeed, p2.TorqueFullLoad * p2.EngineSpeed);
				}
				return Tuple.Create(p1.EngineSpeed, p1.TorqueFullLoad * p1.EngineSpeed);
			}
			var engineTorqueMaxPower = FullLoadStationaryTorque(engineSpeedMaxPower);
			return Tuple.Create(engineSpeedMaxPower, engineTorqueMaxPower * engineSpeedMaxPower);
		}


		/// <summary>
		/// Get item index for the segment of the full-load curve where the angularVelocity lies within.
		/// </summary>
		protected int FindIndex(PerSecond angularVelocity)
		{
			if (angularVelocity < FullLoadEntries.First().EngineSpeed) {
				return 1;
			}
			if (angularVelocity > FullLoadEntries.Last().EngineSpeed) {
				return FullLoadEntries.Count - 1;
			}
			for (var index = 1; index < FullLoadEntries.Count; index++) {
				if (angularVelocity >= FullLoadEntries[index - 1].EngineSpeed &&
					angularVelocity <= FullLoadEntries[index].EngineSpeed) {
					return index;
				}
			}
			throw new VectoException("angular velocity {0} exceeds full load curve: min: {1}  max: {2}", angularVelocity,
				FullLoadEntries.First().EngineSpeed, FullLoadEntries.Last().EngineSpeed);
		}

		[DebuggerDisplay("n: {EngineSpeed}, fullTorque: {TorqueFullLoad}, dragTorque: {TorqueDrag}")]
		internal class FullLoadCurveEntry
		{
			[Required, SIRange(0, 5000 * Constants.RPMToRad)]
			public PerSecond EngineSpeed { get; set; }

			[Required, SIRange(0, 10000)]
			public NewtonMeter TorqueFullLoad { get; set; }

			[Required, SIRange(-10000, 0)]
			public NewtonMeter TorqueDrag { get; set; }

			#region Equality members

			protected bool Equals(FullLoadCurveEntry other)
			{
				return Equals(EngineSpeed, other.EngineSpeed) && Equals(TorqueFullLoad, other.TorqueFullLoad) &&
						Equals(TorqueDrag, other.TorqueDrag);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) {
					return false;
				}
				if (ReferenceEquals(this, obj)) {
					return true;
				}
				if (obj.GetType() != GetType()) {
					return false;
				}
				return Equals((FullLoadCurveEntry)obj);
			}

			public override int GetHashCode()
			{
				unchecked {
					var hashCode = (EngineSpeed != null ? EngineSpeed.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (TorqueFullLoad != null ? TorqueFullLoad.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (TorqueDrag != null ? TorqueDrag.GetHashCode() : 0);
					return hashCode;
				}
			}

			#endregion
		}
	}
}