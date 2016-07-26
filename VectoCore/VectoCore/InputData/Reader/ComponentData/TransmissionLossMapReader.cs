using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
{
	public sealed class TransmissionLossMapReader
	{
		public static TransmissionLossMap ReadFromFile(string fileName, double gearRatio, string gearName)
		{
			try {
				var data = VectoCSVFile.Read(fileName, true);
				return Create(data, gearRatio, gearName);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading TransmissionLossMap: " + ex.Message);
			}
		}

		/// <summary>
		/// Create a TransmissionLoss Map from a DataTable.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="gearRatio"></param>
		/// <param name="gearName"></param>
		/// <returns></returns>
		public static TransmissionLossMap Create(DataTable data, double gearRatio, string gearName)
		{
			if (data.Columns.Count < 3) {
				throw new VectoException("TransmissionLossMap Data File for {0} must consist of at least 3 columns.", gearName);
			}

			if (data.Rows.Count < 4) {
				throw new VectoException(
					"TransmissionLossMap for {0} must consist of at least four lines with numeric values (below file header", gearName);
			}

			List<TransmissionLossMap.GearLossMapEntry> entries;
			if (HeaderIsValid(data.Columns)) {
				entries = CreateFromColumnNames(data);
			} else {
				LoggingObject.Logger<TransmissionLossMap>().Warn(
					"TransmissionLossMap {5}: Header line is not valid. Expected: '{0}, {1}, {2}, <{3}>'. Got: '{4}'. Falling back to column index.",
					TransmissionLossMapReader.Fields.InputSpeed, TransmissionLossMapReader.Fields.InputTorque,
					TransmissionLossMapReader.Fields.TorqeLoss, TransmissionLossMapReader.Fields.Efficiency,
					", ".Join(data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Reverse()), gearName);

				entries = CreateFromColumIndizes(data);
			}

			return new TransmissionLossMap(entries, gearRatio, gearName);
		}

		/// <summary>
		/// Create a DataTable from an efficiency value.
		/// </summary>
		/// <param name="efficiency"></param>
		/// <param name="gearRatio"></param>
		/// <param name="gearName"></param>
		/// <returns></returns>
		public static TransmissionLossMap Create(double efficiency, double gearRatio, string gearName)
		{
			var entries = new List<TransmissionLossMap.GearLossMapEntry> {
				new TransmissionLossMap.GearLossMapEntry(0.RPMtoRad(), 1e5.SI<NewtonMeter>(),
					(1 - efficiency) * 1e5.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(0.RPMtoRad(), -1e5.SI<NewtonMeter>(),
					(1 - efficiency) * 1e5.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(0.RPMtoRad(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(5000.RPMtoRad(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(5000.RPMtoRad(), -1e5.SI<NewtonMeter>(),
					(1 - efficiency) * 1e5.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(5000.RPMtoRad(), 1e5.SI<NewtonMeter>(),
					(1 - efficiency) * 1e5.SI<NewtonMeter>()),
			};
			return new TransmissionLossMap(entries, gearRatio, gearName);
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(TransmissionLossMapReader.Fields.InputSpeed) &&
					columns.Contains(TransmissionLossMapReader.Fields.InputTorque) &&
					columns.Contains(TransmissionLossMapReader.Fields.TorqeLoss);
		}

		private static List<TransmissionLossMap.GearLossMapEntry> CreateFromColumnNames(DataTable data)
		{
			return (from DataRow row in data.Rows
				select new TransmissionLossMap.GearLossMapEntry(
					inputSpeed: row.ParseDouble(Fields.InputSpeed).RPMtoRad(),
					inputTorque: row.ParseDouble(Fields.InputTorque).SI<NewtonMeter>(),
					torqueLoss: row.ParseDouble(Fields.TorqeLoss).SI<NewtonMeter>()))
				.ToList();
		}

		private static List<TransmissionLossMap.GearLossMapEntry> CreateFromColumIndizes(DataTable data)
		{
			return (from DataRow row in data.Rows
				select new TransmissionLossMap.GearLossMapEntry(
					inputSpeed: row.ParseDouble(0).RPMtoRad(),
					inputTorque: row.ParseDouble(1).SI<NewtonMeter>(),
					torqueLoss: row.ParseDouble(2).SI<NewtonMeter>()))
				.ToList();
		}

		public static class Fields
		{
			/// <summary>[rpm]</summary>
			public const string InputSpeed = "Input Speed";

			/// <summary>[Nm]</summary>
			public const string InputTorque = "Input Torque";

			/// <summary>[Nm]</summary>
			public const string TorqeLoss = "Torque Loss";

			/// <summary>[-]</summary>
			public const string Efficiency = "Eff";
		}
	}
}