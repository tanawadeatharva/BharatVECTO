using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
{
	public class TorqueConverterDataReader
	{
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

			List<TorqueConverterEntry> characteristicTorque;
			if (HeaderIsValid(data.Columns)) {
				characteristicTorque = (from DataRow row in data.Rows
					select
						new TorqueConverterEntry() {
							SpeedRatio = row.ParseDouble((string)Fields.SpeedRatio),
							Torque = row.ParseDouble((string)Fields.CharacteristicTorque).SI<NewtonMeter>(),
							TorqueRatio = row.ParseDouble((string)Fields.TorqueRatio)
						}).ToList();
			} else {
				characteristicTorque = (from DataRow row in data.Rows
					select
						new TorqueConverterEntry() {
							SpeedRatio = row.ParseDouble(0),
							Torque = row.ParseDouble(2).SI<NewtonMeter>(),
							TorqueRatio = row.ParseDouble(1)
						}).ToList();
			}
			return new TorqueConverterData(characteristicTorque);
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