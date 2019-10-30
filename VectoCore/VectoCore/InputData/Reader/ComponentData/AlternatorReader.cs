using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class AlternatorReader
	{

		private static string[] HeaderColsCombinedAlt = new[] { Fields.AlternatorName, Fields.AlternatorSpeed, Fields.Current, Fields.Efficiency, Fields.PulleyRatio };
		private static string[] HeaderColsSingleAlt = new[] { Fields.AlternatorSpeed, Fields.Efficiency, Fields.Current };

		public static IAlternatorMap ReadMap(string filePath)
		{
			if (!File.Exists(filePath)) {
				throw new ArgumentException("Supplied input file does not exist");
			}

			return ReadMap(File.OpenRead(filePath), Path.GetFullPath(filePath));
		}

		
		public static IAlternatorMap ReadMap(Stream stream, string source = null)
		{
			//var returnValue = false;
			var map = new List<ICombinedAlternatorMapRow>();

			var data = VectoCSVFile.ReadStream(stream);
			if (HeaderCombinedAlternator(data.Columns)) {
				return ReadCombinedAlternator(data, source);
			}

			if (HeaderSingleAlternator(data.Columns)) {
				return ReadSingleAlternator(data, source);
			}

			throw new ArgumentException("Incorrect number of values in csv file");
		}

		private static IAlternatorMap ReadSingleAlternator(TableData data, string source)
		{
			var map = new List<ICombinedAlternatorMapRow>();
			if (data.Rows.Count < 2) {
				throw new ArgumentException("Insufficient rows in csv to build a usable map");
			}
			foreach (DataRow row in data.Rows) {
				map.Add(
					new CombinedAlternatorMapRow(
						"ALTERNATOR", row.ParseDouble(Fields.AlternatorSpeed).RPMtoRad(), row.ParseDouble(Fields.Current).SI<Ampere>(),
						row.ParseDouble(Fields.Efficiency), 1.0));
			}

			
			return new AlternatorMap(map, source);

		}

		private static IAlternatorMap ReadCombinedAlternator(TableData data, string source)
		{
			var map = new List<ICombinedAlternatorMapRow>();
			foreach (DataRow row in data.Rows) {
				map.Add(
					new CombinedAlternatorMapRow(
						row.Field<string>(Fields.AlternatorName), row.ParseDouble(Fields.AlternatorSpeed).RPMtoRad(), row.ParseDouble(Fields.Current).SI<Ampere>(),
						row.ParseDouble(Fields.Efficiency), row.ParseDouble(Fields.PulleyRatio)));
			}

			var g = map.GroupBy(x => x.AlternatorName);
			if (g.Any(x => x.Count() < 2)) {
				throw new ArgumentException(
					"Insufficient rows in csv to build a usable map for alternator {0}",
					string.Join(", ", g.Where(x => x.Count() < 2).Select(x => x.Key)));
			}
			return new CombinedAlternator(map, source);
		}

		private static bool HeaderCombinedAlternator(DataColumnCollection cols)
		{
			return HeaderColsCombinedAlt.All(x => cols.Contains(x));
		}

		private static bool HeaderSingleAlternator(DataColumnCollection cols)
		{
			return HeaderColsSingleAlt.All(x => cols.Contains(x));
		}

		public static class Fields
		{
			public static string AlternatorName = "AlternatorName";
			public const string AlternatorSpeed = "RPM";
			public const string Current = "Amps";
			public const string Efficiency = "Efficiency";
			public const string PulleyRatio = "PulleyRatio";
		}

		// Helpers
		public static IList<ICombinedAlternatorMapRow> CreateDefaultMap()
		{
			var map = new List<ICombinedAlternatorMapRow>();

			map.Add(new CombinedAlternatorMapRow("Alt1", 2000.RPMtoRad(), 10.SI<Ampere>(), 62, 3.6));
			map.Add(new CombinedAlternatorMapRow("Alt1", 2000.RPMtoRad(), 27.SI<Ampere>(), 70, 3.6));
			map.Add(new CombinedAlternatorMapRow("Alt1", 2000.RPMtoRad(), 53.SI<Ampere>(), 30, 3.6));

			map.Add(new CombinedAlternatorMapRow("Alt1", 4000.RPMtoRad(), 10.SI<Ampere>(), 64, 3.6));
			map.Add(new CombinedAlternatorMapRow("Alt1", 4000.RPMtoRad(), 63.SI<Ampere>(), 74, 3.6));
			map.Add(new CombinedAlternatorMapRow("Alt1", 4000.RPMtoRad(), 125.SI<Ampere>(), 68, 3.6));

			map.Add(new CombinedAlternatorMapRow("Alt1", 6000.RPMtoRad(), 10.SI<Ampere>(), 53, 3.6));
			map.Add(new CombinedAlternatorMapRow("Alt1", 6000.RPMtoRad(), 68.SI<Ampere>(), 70, 3.6));
			map.Add(new CombinedAlternatorMapRow("Alt1", 6000.RPMtoRad(), 136.SI<Ampere>(), 62, 3.6));

			map.Add(new CombinedAlternatorMapRow("Alt2", 2000.RPMtoRad(), 10.SI<Ampere>(), 62, 3));
			map.Add(new CombinedAlternatorMapRow("Alt2", 2000.RPMtoRad(), 27.SI<Ampere>(), 70, 3));
			map.Add(new CombinedAlternatorMapRow("Alt2", 2000.RPMtoRad(), 53.SI<Ampere>(), 30, 3));

			map.Add(new CombinedAlternatorMapRow("Alt2", 4000.RPMtoRad(), 10.SI<Ampere>(), 64, 3));
			map.Add(new CombinedAlternatorMapRow("Alt2", 4000.RPMtoRad(), 63.SI<Ampere>(), 74, 3));
			map.Add(new CombinedAlternatorMapRow("Alt2", 4000.RPMtoRad(), 125.SI<Ampere>(), 68, 3));

			map.Add(new CombinedAlternatorMapRow("Alt2", 6000.RPMtoRad(), 10.SI<Ampere>(), 53, 3));
			map.Add(new CombinedAlternatorMapRow("Alt2", 6000.RPMtoRad(), 68.SI<Ampere>(), 70, 3));
			map.Add(new CombinedAlternatorMapRow("Alt2", 6000.RPMtoRad(), 136.SI<Ampere>(), 62, 3));

			return map;
		}

	}
}
