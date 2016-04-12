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
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader
{
	public class DrivingCycleDataReader : LoggingObject
	{
		/// <summary>
		/// Detects the appropriate cycle type for a cycle in a DataTable.
		/// </summary>
		/// <param name="cycleData">The cycle data.</param>
		/// <returns></returns>
		/// <exception cref="VectoException">CycleFile Format is unknown.</exception>
		public static CycleType DetectCycleType(DataTable cycleData)
		{
			var cols = cycleData.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();

			if (PWheelCycleDataParser.ValidateHeader(cols, false)) {
				return CycleType.PWheel;
			}
			if (MeasuredSpeedGearDataParser.ValidateHeader(cols, false)) {
				return CycleType.MeasuredSpeedGear;
			}
			if (MeasuredSpeedDataParser.ValidateHeader(cols, false)) {
				return CycleType.MeasuredSpeed;
			}
			if (EngineOnlyCycleDataParser.ValidateHeader(cols, false)) {
				return CycleType.EngineOnly;
			}
			if (DistanceBasedCycleDataParser.ValidateHeader(cols, false)) {
				return CycleType.DistanceBased;
			}
			throw new VectoException("CycleFile format is unknown.");
		}

		private static ICycleDataParser GetDataParser(CycleType type)
		{
			switch (type) {
				case CycleType.EngineOnly:
					return new EngineOnlyCycleDataParser();
				case CycleType.DistanceBased:
					return new DistanceBasedCycleDataParser();
				case CycleType.PWheel:
					return new PWheelCycleDataParser();
				case CycleType.MeasuredSpeedGear:
					return new MeasuredSpeedGearDataParser();
				case CycleType.MeasuredSpeed:
					return new MeasuredSpeedDataParser();
				default:
					throw new ArgumentOutOfRangeException("type");
			}
		}

		/// <summary>
		/// Reads a cycle from a file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="type">The type.</param>
		/// <param name="crossWindRequired"></param>
		/// <returns></returns>
		/// <exception cref="VectoException">ERROR while reading DrivingCycle File:  + ex.Message</exception>
		public static DrivingCycleData ReadFromFile(string fileName, CycleType type, bool crossWindRequired)
		{
			try {
				var stream = File.OpenRead(fileName);
				return ReadFromStream(stream, type, Path.GetFileNameWithoutExtension(fileName), crossWindRequired);
			} catch (Exception ex) {
				throw new VectoException("ERROR while opening DrivingCycle File: " + ex.Message);
			}
		}

		/// <summary>
		/// Reads the cycle from a stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="type">The type.</param>
		/// <param name="crossWindRequired"></param>
		/// <param name="name">the name of the cycle</param>
		/// <returns></returns>
		public static DrivingCycleData ReadFromStream(Stream stream, CycleType type, string name, bool crossWindRequired)
		{
			try {
				return ReadFromDataTable(VectoCSVFile.ReadStream(stream), type, name, crossWindRequired);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading DrivingCycle Stream: " + ex.Message);
			}
		}

		/// <summary>
		/// Creates a cycle from a DataTable with automatic determination of Cycle Type.
		/// </summary>
		/// <param name="data">The cycle data.</param>
		/// <param name="name">The name.</param>
		/// <param name="crossWindRequired"></param>
		/// <returns></returns>
		public static DrivingCycleData ReadFromDataTable(DataTable data, string name, bool crossWindRequired)
		{
			if (data == null) {
				Logger<DistanceBasedCycleDataParser>().Warn("Invalid data for DrivingCycle -- dataTable is null");
				throw new VectoException("Invalid data for DrivingCycle -- dataTable is null");
			}
			return ReadFromDataTable(data, DetectCycleType(data), name, crossWindRequired);
		}

		/// <summary>
		/// Creates a cycle from a DataTable
		/// </summary>
		/// <param name="data">The cycle data.</param>
		/// <param name="type">The type.</param>
		/// <param name="name">The name.</param>
		/// <param name="crossWindRequired"></param>
		/// <returns></returns>
		public static DrivingCycleData ReadFromDataTable(DataTable data, CycleType type, string name, bool crossWindRequired)
		{
			if (data == null) {
				Logger<DistanceBasedCycleDataParser>().Warn("Invalid data for DrivingCycle -- dataTable is null");
				throw new VectoException("Invalid data for DrivingCycle -- dataTable is null");
			}
			var entries = GetDataParser(type).Parse(data, crossWindRequired).ToList();

			if (type == CycleType.DistanceBased) {
				entries = FilterDrivingCycleEntries(entries);
			}
			var cycle = new DrivingCycleData {
				Entries = entries,
				CycleType = type,
				Name = name
			};
			return cycle;
		}


		private static List<DrivingCycleData.DrivingCycleEntry> FilterDrivingCycleEntries(
			List<DrivingCycleData.DrivingCycleEntry> entries)
		{
			var filtered = new List<DrivingCycleData.DrivingCycleEntry>();
			var current = entries.First();
			current.Altitude = 0.SI<Meter>();
			filtered.Add(current);
			var distance = current.Distance;
			var altitude = current.Altitude;
			for (var i = 0; i < entries.Count; i++) {
				var entry = entries[i];
				if (!entry.StoppingTime.IsEqual(0) && !entry.VehicleTargetSpeed.IsEqual(0)) {
					throw new VectoException(
						"Error in DrivingCycle: stop time specified but target-speed > 0! Distance: {0}, stop-time: {1}, target speed: {2}",
						entry.Distance, entry.StoppingTime, entry.VehicleTargetSpeed);
				}
				if (entry.Distance < distance) {
					throw new VectoException(
						"Error in DrivingCycle: distance entry is smaller than last distance! last distance: {0}, current distance: {1} ",
						distance, entry.Distance);
				}
				if (i > 0) {
					altitude += (entry.Distance - distance) * entries[i - 1].RoadGradientPercent / 100.0;
				}
				entry.Altitude = altitude;
				// if something changes in the cycle, add it to the filtered cycle but always add last entry
				if (!CycleEntriesAreEqual(current, entry) || i == entries.Count - 1) {
					entry.Altitude = altitude;
					filtered.Add(entry);
					current = entry;
				}
				if (!entry.StoppingTime.IsEqual(0) && entry.VehicleTargetSpeed.IsEqual(0)) {
					// vehicle stops. duplicate current distance entry with 0 waiting time
					var tmp = new DrivingCycleData.DrivingCycleEntry(entry) {
						StoppingTime = 0.SI<Second>(),
						RoadGradient = entry.RoadGradient,
						RoadGradientPercent = entry.RoadGradientPercent,
						VehicleTargetSpeed = i < entries.Count - 1 ? entries[i + 1].VehicleTargetSpeed : 0.SI<MeterPerSecond>()
					};
					filtered.Add(tmp);
					current = tmp;
				}

				distance = entry.Distance;
			}
			Logger<DrivingCycleDataReader>()
				.Info("Data loaded. Number of Entries: {0}, filtered Entries: {1}", entries.Count, filtered.Count);
			entries = filtered;

			AdjustDistanceAfterStop(entries);

			return entries;
		}

		private static void AdjustDistanceAfterStop(List<DrivingCycleData.DrivingCycleEntry> entries)
		{
			var currentIt = entries.GetEnumerator();
			var nextIt = entries.GetEnumerator();
			nextIt.MoveNext();
			while (currentIt.MoveNext() && nextIt.MoveNext()) {
				if (currentIt.Current != null && !currentIt.Current.StoppingTime.IsEqual(0)) {
					if (nextIt.Current != null) {
						nextIt.Current.Distance = currentIt.Current.Distance;
					}
				}
			}
		}

		private static bool CycleEntriesAreEqual(DrivingCycleData.DrivingCycleEntry first,
			DrivingCycleData.DrivingCycleEntry second)
		{
			if (first.Distance.IsEqual(second.Distance)) {
				return true;
			}

			if (first.VehicleTargetSpeed != second.VehicleTargetSpeed) {
				return false;
			}

			if (!first.RoadGradient.IsEqual(second.RoadGradient, Constants.SimulationSettings.DrivingCycleRoadGradientTolerance)) {
				return false;
			}

			if (!(first.StoppingTime.IsEqual(0) && second.StoppingTime.IsEqual(0))) {
				return false;
			}

			if (first.AdditionalAuxPowerDemand != second.AdditionalAuxPowerDemand) {
				return false;
			}

			if (first.AuxiliarySupplyPower.Count != second.AuxiliarySupplyPower.Count) {
				return false;
			}

			if (!first.AuxiliarySupplyPower.SequenceEqual(second.AuxiliarySupplyPower)) {
				return false;
			}

			return true;
		}

		// todo MK-2016-01-19: move fields to resource file
		private static class Fields
		{
			public const string PWheel = "Pwheel";
			public const string Distance = "s";
			public const string Time = "t";
			public const string VehicleSpeed = "v";
			public const string RoadGradient = "grad";
			public const string StoppingTime = "stop";
			public const string AuxiliarySupplyPower = "Aux_";
			public const string EngineSpeed = "n";
			public const string Gear = "gear";
			public const string AdditionalAuxPowerDemand = "Padd";
			public const string AirSpeedRelativeToVehicle = "vair_res";
			public const string WindYawAngle = "vair_beta";
			public const string EnginePower = "Pe";
			public const string EngineTorque = "Me";
		}

		#region DataParser

		private interface ICycleDataParser
		{
			/// <summary>
			/// Parses a datatable to a list of drivingcycleentries for the current implementation.
			/// </summary>
			/// <param name="table"></param>
			/// <param name="crossWindRequired"></param>
			/// <returns></returns>
			IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired);
		}

		private abstract class AbstractCycleDataParser : ICycleDataParser
		{
			protected static bool CheckColumns(string[] header, IEnumerable<string> allowedCols, IEnumerable<string> requiredCols,
				bool throwExceptions, bool allowAux)
			{
				var diff = header.GroupBy(c => c).Where(g => g.Count() > 2).SelectMany(g => g).ToList();
				if (diff.Any()) {
					if (throwExceptions) {
						throw new VectoException("Column(s) defined more than once: " + ", ".Join(diff.OrderBy(x => x)));
					}
					return false;
				}

				if (allowAux) {
					header = header.Where(c => !c.StartsWith(Fields.AuxiliarySupplyPower)).ToArray();
				}

				diff = header.Except(allowedCols).ToList();
				if (diff.Any()) {
					if (throwExceptions) {
						throw new VectoException("Column(s) not allowed: " + ", ".Join(diff));
					}
					return false;
				}

				diff = requiredCols.Except(header).ToList();
				if (diff.Any()) {
					if (throwExceptions) {
						throw new VectoException("Column(s) required: " + ", ".Join(diff));
					}
					return false;
				}
				return true;
			}

			protected static bool CheckComboColumns(string[] header, string[] cols, bool throwExceptions)
			{
				var colCount = header.Intersect(cols).Count();
				if (colCount != 0 && colCount != cols.Length) {
					if (throwExceptions) {
						throw new VectoException("Either all columns have to be defined or none of them: {0}", ", ".Join(cols));
					}
					return false;
				}

				return true;
			}

			public abstract IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired);
		}

		private class DistanceBasedCycleDataParser : AbstractCycleDataParser
		{
			public override IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired)
			{
				ValidateHeader(table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray());

				return table.Rows.Cast<DataRow>().Select(row => new DrivingCycleData.DrivingCycleEntry {
					Distance = row.ParseDouble(Fields.Distance).SI<Meter>(),
					VehicleTargetSpeed = row.ParseDouble(Fields.VehicleSpeed).KMPHtoMeterPerSecond(),
					RoadGradientPercent = row.ParseDoubleOrGetDefault(Fields.RoadGradient),
					RoadGradient = VectoMath.InclinationToAngle(row.ParseDoubleOrGetDefault(Fields.RoadGradient) / 100.0),
					StoppingTime = row.ParseDouble(Fields.StoppingTime).SI<Second>(),
					AdditionalAuxPowerDemand = row.ParseDoubleOrGetDefault(Fields.AdditionalAuxPowerDemand).SI().Kilo.Watt.Cast<Watt>(),
					AngularVelocity = row.ParseDoubleOrGetDefault(Fields.EngineSpeed).RPMtoRad(),
					Gear = (uint)row.ParseDoubleOrGetDefault(Fields.Gear),
					AirSpeedRelativeToVehicle =
						crossWindRequired ? row.ParseDouble(Fields.AirSpeedRelativeToVehicle).KMPHtoMeterPerSecond() : null,
					WindYawAngle = crossWindRequired ? row.ParseDoubleOrGetDefault(Fields.WindYawAngle) : 0,
					AuxiliarySupplyPower = row.GetAuxiliaries()
				});
			}

			public static bool ValidateHeader(string[] header, bool throwExceptions = true)
			{
				var requiredCols = new[] {
					Fields.VehicleSpeed,
					Fields.Distance,
					Fields.StoppingTime
				};

				var allowedCols = new[] {
					Fields.Distance,
					Fields.VehicleSpeed,
					Fields.RoadGradient,
					Fields.StoppingTime,
					Fields.EngineSpeed,
					Fields.Gear,
					Fields.AdditionalAuxPowerDemand,
					Fields.AirSpeedRelativeToVehicle,
					Fields.WindYawAngle
				};

				return CheckColumns(header, allowedCols, requiredCols, throwExceptions, allowAux: true) &&
						CheckComboColumns(header, new[] { Fields.AirSpeedRelativeToVehicle, Fields.WindYawAngle }, throwExceptions);
			}
		}

		private class EngineOnlyCycleDataParser : AbstractCycleDataParser
		{
			public override IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired)
			{
				ValidateHeader(table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray());

				var absTime = 0;
				foreach (DataRow row in table.Rows) {
					var entry = new DrivingCycleData.DrivingCycleEntry {
						Time = row.ParseDoubleOrGetDefault(Fields.Time, absTime).SI<Second>(),
						AngularVelocity = row.ParseDoubleOrGetDefault(Fields.EngineSpeed).RPMtoRad(),
						AdditionalAuxPowerDemand =
							row.ParseDoubleOrGetDefault(Fields.AdditionalAuxPowerDemand).SI().Kilo.Watt.Cast<Watt>(),
						AuxiliarySupplyPower = row.GetAuxiliaries()
					};

					if (row.Table.Columns.Contains(Fields.EngineTorque)) {
						if (row.Field<string>(Fields.EngineTorque).Equals("<DRAG>")) {
							entry.Drag = true;
						} else {
							entry.Torque = row.ParseDouble(Fields.EngineTorque).SI<NewtonMeter>();
						}
					} else {
						if (row.Field<string>(Fields.EnginePower).Equals("<DRAG>")) {
							entry.Drag = true;
						} else {
							entry.Torque = row.ParseDouble(Fields.EnginePower).SI().Kilo.Watt.Cast<Watt>() / entry.AngularVelocity;
						}
					}
					absTime += 1;

					yield return entry;
				}
			}

			public static bool ValidateHeader(string[] header, bool throwExceptions = true)
			{
				var allowedCols = new[] {
					Fields.Time,
					Fields.EngineSpeed,
					Fields.EngineTorque,
					Fields.EnginePower,
					Fields.AdditionalAuxPowerDemand
				};

				var requiredCols = new[] {
					Fields.EngineSpeed
				};

				if (!CheckColumns(header, allowedCols, requiredCols, throwExceptions, allowAux: false)) {
					return false;
				}

				var containsNone = !header.Contains(Fields.EngineTorque) && !header.Contains(Fields.EnginePower);
				if (containsNone) {
					if (throwExceptions) {
						throw new VectoException("Column(s) missing: Either column '{0}' or column '{1}' must be defined.",
							Fields.EngineTorque, Fields.EnginePower);
					}
					return false;
				}

				var containsBoth = header.Contains(Fields.EngineTorque) && header.Contains(Fields.EnginePower);
				if (containsBoth) {
					Logger<DrivingCycleDataReader>().Warn("Found column '{0}' and column '{1}': Only column '{0}' will be used.",
						Fields.EngineTorque, Fields.EnginePower);
				}
				return true;
			}
		}

		/// <summary>
		/// Parser for PWheel (SiCo) Mode.
		/// </summary>
		// <t>, <Pwheel>, <Gear>, <n>, <Padd>
		private class PWheelCycleDataParser : AbstractCycleDataParser
		{
			public override IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired)
			{
				ValidateHeader(table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray());

				var entries = table.Rows.Cast<DataRow>().Select(row => new DrivingCycleData.DrivingCycleEntry {
					Time = row.ParseDouble(Fields.Time).SI<Second>(),
					PWheel = row.ParseDouble(Fields.PWheel).SI().Kilo.Watt.Cast<Watt>(),
					Gear = (uint)row.ParseDouble(Fields.Gear),
					AngularVelocity = row.ParseDouble(Fields.EngineSpeed).RPMtoRad(),
					AdditionalAuxPowerDemand = row.ParseDoubleOrGetDefault(Fields.AdditionalAuxPowerDemand).SI().Kilo.Watt.Cast<Watt>(),
				}).ToArray();

				return entries;
			}

			public static bool ValidateHeader(string[] header, bool throwExceptions = true)
			{
				var allowedCols = new[] {
					Fields.Time,
					Fields.PWheel,
					Fields.Gear,
					Fields.EngineSpeed,
					Fields.AdditionalAuxPowerDemand
				};
				var requiredCols = new[] {
					Fields.Time,
					Fields.PWheel,
					Fields.Gear,
					Fields.EngineSpeed
				};

				return CheckColumns(header, allowedCols, requiredCols, throwExceptions, allowAux: false);
			}
		}

		/// <summary>
		/// Parser for Measured Speed Mode Option 1.
		/// </summary>
		// <t>, <v>, <grad>, <Padd>[, <vair_res>, <vair_beta>][, Aux_...]
		private class MeasuredSpeedDataParser : AbstractCycleDataParser
		{
			public override IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired)
			{
				ValidateHeader(table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray());

				var entries = table.Rows.Cast<DataRow>().Select(row => new DrivingCycleData.DrivingCycleEntry {
					Time = row.ParseDouble(Fields.Time).SI<Second>(),
					VehicleTargetSpeed = row.ParseDouble(Fields.VehicleSpeed).KMPHtoMeterPerSecond(),
					RoadGradient = VectoMath.InclinationToAngle(row.ParseDoubleOrGetDefault(Fields.RoadGradient) / 100.0),
					AdditionalAuxPowerDemand = row.ParseDoubleOrGetDefault(Fields.AdditionalAuxPowerDemand).SI().Kilo.Watt.Cast<Watt>(),
					AirSpeedRelativeToVehicle =
						crossWindRequired ? row.ParseDouble(Fields.AirSpeedRelativeToVehicle).KMPHtoMeterPerSecond() : null,
					WindYawAngle = crossWindRequired ? row.ParseDouble(Fields.WindYawAngle) : 0,
					AuxiliarySupplyPower = row.GetAuxiliaries()
				}).ToArray();

				return entries;
			}

			public static bool ValidateHeader(string[] header, bool throwExceptions = true)
			{
				//todo mk-2016-02-15: check if vair_res, and vair_beta only when needed
				var allowedCols = new[] {
					Fields.Time,
					Fields.VehicleSpeed,
					Fields.RoadGradient,
					Fields.AdditionalAuxPowerDemand,
					Fields.AirSpeedRelativeToVehicle,
					Fields.WindYawAngle
				};
				var requiredCols = new[] {
					Fields.Time,
					Fields.VehicleSpeed,
					Fields.RoadGradient,
				};

				return CheckColumns(header, allowedCols, requiredCols, throwExceptions, allowAux: true) &&
						CheckComboColumns(header, new[] { Fields.AirSpeedRelativeToVehicle, Fields.WindYawAngle }, throwExceptions);
			}
		}


		/// <summary>
		/// Parser for Measured Speed Mode Option 2.
		/// </summary>
		// <t>, <v>, <grad>, <Padd>, <n>, <gear>[, <vair_res>, <vair_beta>][, Aux_...]
		private class MeasuredSpeedGearDataParser : AbstractCycleDataParser
		{
			public override IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired)
			{
				ValidateHeader(table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray());

				var entries = table.Rows.Cast<DataRow>().Select(row => new DrivingCycleData.DrivingCycleEntry {
					Time = row.ParseDouble(Fields.Time).SI<Second>(),
					VehicleTargetSpeed = row.ParseDouble(Fields.VehicleSpeed).KMPHtoMeterPerSecond(),
					RoadGradient = VectoMath.InclinationToAngle(row.ParseDoubleOrGetDefault(Fields.RoadGradient) / 100.0),
					AdditionalAuxPowerDemand = row.ParseDoubleOrGetDefault(Fields.AdditionalAuxPowerDemand).SI().Kilo.Watt.Cast<Watt>(),
					AngularVelocity = row.ParseDouble(Fields.EngineSpeed).RPMtoRad(),
					Gear = (uint)row.ParseDouble(Fields.Gear),
					AirSpeedRelativeToVehicle =
						crossWindRequired ? row.ParseDouble(Fields.AirSpeedRelativeToVehicle).KMPHtoMeterPerSecond() : null,
					WindYawAngle = crossWindRequired ? row.ParseDoubleOrGetDefault(Fields.WindYawAngle) : 0,
					AuxiliarySupplyPower = row.GetAuxiliaries()
				}).ToArray();

				return entries;
			}

			public static bool ValidateHeader(string[] header, bool throwExceptions = true)
			{
				//todo mk-2016-02-15: check if vair_res, and vair_beta only when needed
				var allowedCols = new[] {
					Fields.Time,
					Fields.VehicleSpeed,
					Fields.RoadGradient,
					Fields.AdditionalAuxPowerDemand,
					Fields.EngineSpeed,
					Fields.Gear,
					Fields.AirSpeedRelativeToVehicle,
					Fields.WindYawAngle
				};

				var requiredCols = new[] {
					Fields.Time,
					Fields.VehicleSpeed,
					Fields.RoadGradient,
					Fields.EngineSpeed,
					Fields.Gear
				};

				return CheckColumns(header, allowedCols, requiredCols, throwExceptions, allowAux: true) &&
						CheckComboColumns(header, new[] { Fields.AirSpeedRelativeToVehicle, Fields.WindYawAngle }, throwExceptions);
			}
		}
	}

	#endregion
}