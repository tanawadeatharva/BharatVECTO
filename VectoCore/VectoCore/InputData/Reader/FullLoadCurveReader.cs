using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader
{
	public class FullLoadCurveReader : LoggingObject
	{
		public static FullLoadCurve ReadFromFile(string fileName, bool declarationMode = false, bool engineFld = false)
		{
			try {
				var data = VectoCSVFile.Read(fileName);
				return Create(data, declarationMode, engineFld);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading FullLoadCurve File: " + ex.Message);
			}
		}


		public static FullLoadCurve Create(DataTable data, bool declarationMode = false, bool engineFld = false)
		{
			if (engineFld) {
				if (data.Columns.Count < 3) {
					throw new VectoException("Engine FullLoadCurve Data File must consist of at least 3 columns.");
				}
			} else {
				if (data.Columns.Count < 2) {
					throw new VectoException("Gearbox FullLoadCurve Data File must consist of at least 2 columns.");
				}
			}

			if (data.Rows.Count < 2) {
				throw new VectoException(
					"FullLoadCurve must consist of at least two lines with numeric values (below file header)");
			}

			List<FullLoadCurve.FullLoadCurveEntry> entriesFld;
			if (HeaderIsValid(data.Columns, engineFld)) {
				entriesFld = CreateFromColumnNames(data, engineFld);
			} else {
				Logger<FullLoadCurve>().Warn(
					"FullLoadCurve: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: '{3}'. Falling back to column index.",
					Fields.EngineSpeed, Fields.TorqueFullLoad,
					Fields.TorqueDrag, ", ".Join(data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

				entriesFld = CreateFromColumnIndizes(data, engineFld);
			}

			LookupData<PerSecond, Second> tmp;
			if (declarationMode) {
				tmp = new PT1();
			} else {
				tmp = PT1Curve.Create(data);
			}
			entriesFld.Sort((entry1, entry2) => entry1.EngineSpeed.Value().CompareTo(entry2.EngineSpeed.Value()));
			return new FullLoadCurve { FullLoadEntries = entriesFld, PT1Data = tmp };
		}

		private static bool HeaderIsValid(DataColumnCollection columns, bool engineFld)
		{
			return columns.Contains(Fields.EngineSpeed)
					&& columns.Contains(Fields.TorqueFullLoad)
					&& (!engineFld || columns.Contains(Fields.TorqueDrag));
		}

		private static List<FullLoadCurve.FullLoadCurveEntry> CreateFromColumnNames(DataTable data, bool engineFld)
		{
			return (from DataRow row in data.Rows
				select new FullLoadCurve.FullLoadCurveEntry {
					EngineSpeed = row.ParseDouble(Fields.EngineSpeed).RPMtoRad(),
					TorqueFullLoad = row.ParseDouble(Fields.TorqueFullLoad).SI<NewtonMeter>(),
					TorqueDrag = (engineFld ? row.ParseDouble(Fields.TorqueDrag).SI<NewtonMeter>() : null)
				}).ToList();
		}

		private static List<FullLoadCurve.FullLoadCurveEntry> CreateFromColumnIndizes(DataTable data, bool engineFld)
		{
			return (from DataRow row in data.Rows
				select new FullLoadCurve.FullLoadCurveEntry {
					EngineSpeed = row.ParseDouble(0).RPMtoRad(),
					TorqueFullLoad = row.ParseDouble(1).SI<NewtonMeter>(),
					TorqueDrag = (engineFld ? row.ParseDouble(2).SI<NewtonMeter>() : null)
				}).ToList();
		}


		private static class Fields
		{
			/// <summary>
			/// [rpm] engine speed
			/// </summary>
			public const string EngineSpeed = "engine speed";

			/// <summary>
			/// [Nm] full load torque
			/// </summary>
			public const string TorqueFullLoad = "full load torque";

			/// <summary>
			/// [Nm] motoring torque
			/// </summary>
			public const string TorqueDrag = "motoring torque";
		}
	}
}