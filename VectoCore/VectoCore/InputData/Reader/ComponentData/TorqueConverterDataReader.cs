using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
{
	public class TorqueConverterDataReader {
		public static TorqueConverterData ReadFromFile(string filename)
		{
			return Create(VectoCSVFile.Read(filename));
		}

		public static TorqueConverterData ReadFromStream(Stream stream)
		{
			return Create(VectoCSVFile.ReadStream(stream));
		}

		public static TorqueConverterData Create(DataTable data)
		{
			if (data.Columns.Count != 3) {
				throw new VectoException("TorqueConverter Characteristics data must consist of 3 columns");
			}
			if (data.Rows.Count < 2) {
				throw new VectoException("TorqueConverter Characteristics data must contain at least 2 lines with numeric values");
			}

			List<TorqueRatioCurveEntry> torqueRatio;
			List<CharacteristicTorqueEntry> characteristicTorque;
			if (HeaderIsValid(data.Columns)) {
				torqueRatio = (from DataRow row in data.Rows
					select
						new TorqueRatioCurveEntry() {
							SpeedRatio = DataTableExtensionMethods.ParseDouble(row, (string)Fields.SpeedRatio),
							TorqueRatio = DataTableExtensionMethods.ParseDouble(row, (string)Fields.TorqueRatio)
						}).ToList();
				characteristicTorque = (from DataRow row in data.Rows
					select
						new CharacteristicTorqueEntry() {
							SpeedRatio = DataTableExtensionMethods.ParseDouble(row, (string)Fields.SpeedRatio),
							Torque = DataTableExtensionMethods.ParseDouble(row, (string)Fields.CharacteristicTorque).SI<NewtonMeter>()
						}).ToList();
			} else {
				torqueRatio = (from DataRow row in data.Rows
					select
						new TorqueRatioCurveEntry() {
							SpeedRatio = row.ParseDouble(0),
							TorqueRatio = row.ParseDouble(1)
						}).ToList();
				characteristicTorque = (from DataRow row in data.Rows
					select
						new CharacteristicTorqueEntry() {
							SpeedRatio = row.ParseDouble(0),
							Torque = row.ParseDouble(2).SI<NewtonMeter>()
						}).ToList();
			}
			return new TorqueConverterData(torqueRatio, characteristicTorque);
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.SpeedRatio) && columns.Contains(Fields.TorqueRatio) &&
					columns.Contains(Fields.CharacteristicTorque);
		}

		public static class Fields
		{
			public const string SpeedRatio = "Speed Ratio";
			public const string TorqueRatio = "Torque Ratio";
			public const string CharacteristicTorque = "MP1000";
		}
	}
}