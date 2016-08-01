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
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TUGraz.VectoCommon.Configuration;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
{
	public class FuelConsumptionMap : SimulationComponentData
	{
		[Required, ValidateObject] private readonly DelaunayMap _fuelMap;

		protected internal FuelConsumptionMap(DelaunayMap fuelMap)
		{
			_fuelMap = fuelMap;
		}

		/// <summary>
		/// Calculates the fuel consumption based on the given fuel map, the angularVelocity and the torque.
		/// </summary>
		public FuelConsumptionResult GetFuelConsumption(NewtonMeter torque, PerSecond angularVelocity,
			bool allowExtrapolation = false)
		{
			var result = new FuelConsumptionResult();
			// delaunay map needs is initialised with rpm, therefore the angularVelocity has to be converted.
			var value = _fuelMap.Interpolate(torque.Value(), angularVelocity.AsRPM);
			if (value.HasValue) {
				result.Value = value.Value.SI().Kilo.Gramm.Per.Second.Cast<KilogramPerSecond>();
				return result;
			}

			if (allowExtrapolation) {
				result.Value =
					_fuelMap.Extrapolate(torque.Value(), angularVelocity.AsRPM).SI().Kilo.Gramm.Per.Second.Cast<KilogramPerSecond>();
				result.Extrapolated = true;
				return result;
			}

			throw new VectoException("FuelConsumptionMap: Interpolation failed. torque: {0}, n: {1}", torque.Value(),
				angularVelocity.AsRPM);
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
		public class FuelConsumptionEntry
		{
			[Required, SIRange(0, 5000 * Constants.RPMToRad)]
			public PerSecond EngineSpeed { get; set; }

			[Required]
			public NewtonMeter Torque { get; set; }

			[Required, SIRange(0, double.MaxValue)]
			public KilogramPerSecond FuelConsumption { get; set; }

			#region Equality members

			public FuelConsumptionEntry(PerSecond engineSpeed, NewtonMeter torque, KilogramPerSecond fuelConsumption)
			{
				EngineSpeed = engineSpeed;
				Torque = torque;
				FuelConsumption = fuelConsumption;
			}

			private bool Equals(FuelConsumptionEntry other)
			{
				return EngineSpeed.Equals(other.EngineSpeed) && Torque.Equals(other.Torque) &&
						FuelConsumption.Equals(other.FuelConsumption);
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
				return Equals((FuelConsumptionEntry)obj);
			}

			public override int GetHashCode()
			{
				unchecked {
					var hashCode = EngineSpeed.GetHashCode();
					hashCode = (hashCode * 397) ^ Torque.GetHashCode();
					hashCode = (hashCode * 397) ^ FuelConsumption.GetHashCode();
					return hashCode;
				}
			}

			#endregion
		}

		[DebuggerDisplay("{Value} (extrapolated: {Extrapolated})")]
		public class FuelConsumptionResult
		{
			public KilogramPerSecond Value;
			public bool Extrapolated;
		}

		#region Equality members

		protected bool Equals(FuelConsumptionMap other)
		{
			return Equals(_fuelMap, other._fuelMap);
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
			return Equals((FuelConsumptionMap)obj);
		}

		public override int GetHashCode()
		{
			return _fuelMap != null ? _fuelMap.GetHashCode() : 0;
		}

		#endregion
	}
}