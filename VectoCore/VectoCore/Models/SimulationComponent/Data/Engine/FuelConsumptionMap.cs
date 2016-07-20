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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
{
	public class FuelConsumptionMap : SimulationComponentData
	{
		[Required, ValidateObject] private readonly DelaunayMap _fuelMap = new DelaunayMap("FuelConsumptionMap");

		private FuelConsumptionMap() {}

		public static FuelConsumptionMap ReadFromFile(string fileName)
		{
			try {
				var data = VectoCSVFile.Read(fileName);
				return Create(data);
			} catch (Exception e) {
				throw new VectoException(string.Format("File {0}: {1}", fileName, e.Message), e);
			}
		}

		public static FuelConsumptionMap Create(DataTable data)
		{
			var headerValid = HeaderIsValid(data.Columns);
			if (!headerValid) {
				Logger<FuelConsumptionMap>().Warn(
					"FuelConsumptionMap: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: {3}",
					Fields.EngineSpeed, Fields.Torque, Fields.FuelConsumption,
					", ".Join(data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			}
			var fuelConsumptionMap = new FuelConsumptionMap();

			foreach (DataRow row in data.Rows) {
				try {
					var entry = headerValid ? CreateFromColumNames(row) : CreateFromColumnIndizes(row);

					// Delaunay map works only as expected, when the angularVelocity is in rpm.
					fuelConsumptionMap._fuelMap.AddPoint(entry.Torque.Value(),
						headerValid ? row.ParseDouble(Fields.EngineSpeed) : row.ParseDouble(0),
						entry.FuelConsumption.Value());
				} catch (Exception e) {
					throw new VectoException(string.Format("Line {0}: {1}", data.Rows.IndexOf(row), e.Message), e);
				}
			}

			fuelConsumptionMap._fuelMap.Triangulate();
			return fuelConsumptionMap;
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.EngineSpeed) && columns.Contains(Fields.Torque) &&
					columns.Contains(Fields.FuelConsumption);
		}

		private static FuelConsumptionEntry CreateFromColumnIndizes(DataRow row)
		{
			return new FuelConsumptionEntry(
				engineSpeed: row.ParseDouble(0).RPMtoRad(),
				torque: row.ParseDouble(1).SI<NewtonMeter>(),
				fuelConsumption:
					row.ParseDouble(2).SI().Gramm.Per.Hour.ConvertTo().Kilo.Gramm.Per.Second.Cast<KilogramPerSecond>()
				);
		}

		private static FuelConsumptionEntry CreateFromColumNames(DataRow row)
		{
			return new FuelConsumptionEntry(
				engineSpeed: row.ParseDouble(Fields.EngineSpeed).SI().Rounds.Per.Minute.Cast<PerSecond>(),
				torque: row.ParseDouble(Fields.Torque).SI<NewtonMeter>(),
				fuelConsumption:
					row.ParseDouble(Fields.FuelConsumption)
						.SI()
						.Gramm.Per.Hour.ConvertTo()
						.Kilo.Gramm.Per.Second.Cast<KilogramPerSecond>()
				);
		}

		/// <summary>
		/// Calculates the fuel consumption based on the given fuel map, the angularVelocity and the torque.
		/// </summary>
		public KilogramPerSecond GetFuelConsumption(NewtonMeter torque, PerSecond angularVelocity,
			bool allowExtrapolation = false)
		{
			// delaunay map needs is initialised with rpm, therefore the angularVelocity has to be converted.
			var value = _fuelMap.Interpolate(torque.Value(), angularVelocity.AsRPM);
			if (value.HasValue) {
				return value.Value.SI().Kilo.Gramm.Per.Second.Cast<KilogramPerSecond>();
			}

			if (allowExtrapolation) {
				return
					_fuelMap.Extrapolate(torque.Value(), angularVelocity.AsRPM).SI().Kilo.Gramm.Per.Second.Cast<KilogramPerSecond>();
			}

			throw new VectoException("FuelConsumptionMap: Interpolation failed. torque: {0}, n: {1}", torque.Value(),
				angularVelocity.AsRPM);
		}

		public static class Fields
		{
			/// <summary>
			/// [rpm]
			/// </summary>
			public const string EngineSpeed = "engine speed";

			/// <summary>
			/// [Nm]
			/// </summary>
			public const string Torque = "torque";

			/// <summary>
			/// [g/h]
			/// </summary>
			public const string FuelConsumption = "fuel consumption";
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
		private class FuelConsumptionEntry
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