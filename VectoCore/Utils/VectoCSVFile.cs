/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models;

namespace TUGraz.VectoCore.Utils
{
	/// <summary>
	///     Class for Reading and Writing VECTO CSV Files.
	/// </summary>
	/// <remarks>
	///     The following format applies to all CSV (Comma-separated values) Input Files used in VECTO:
	///     List DELIMITER: Comma ","
	///     Decimal-Mark: Dot "."
	///     Comments: "#" at the beginning of the comment line. Number and position of comment lines is not limited.
	///     Header: One header line (not a comment line) at the beginning of the file.
	///     All Combinations between max-format and min-format possible. Only "id"-field is used.
	///     max: id (name) [unit], id (name) [unit], ...
	///     min: id,id,...
	/// </remarks>
	public class VectoCSVFile : LoggingObject
	{
		private const char Delimiter = ',';
		private const char Comment = '#';

		/// <summary>
		///     Reads a CSV file which is stored in Vecto-CSV-Format.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="ignoreEmptyColumns"></param>
		/// <param name="fullHeader"></param>
		/// <exception cref="FileIOException"></exception>
		/// <returns>A DataTable which represents the CSV File.</returns>
		public static DataTable Read(string fileName, bool ignoreEmptyColumns = false, bool fullHeader = false)
		{
			try {
				return ReadData(File.ReadAllLines(fileName), ignoreEmptyColumns, fullHeader);
			} catch (Exception e) {
				Logger<VectoCSVFile>().Error(e);
				throw new VectoException("File {0}: {1}", fileName, e.Message);
			}
		}

		/// <summary>
		///     Reads a CSV file which is stored in Vecto-CSV-Format.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="ignoreEmptyColumns"></param>
		/// <exception cref="FileIOException"></exception>
		/// <returns>A DataTable which represents the CSV File.</returns>
		public static DataTable ReadStream(Stream stream, bool ignoreEmptyColumns = false)
		{
			try {
				var lines = new List<string>();
				using (var reader = new StreamReader(stream)) {
					while (!reader.EndOfStream) {
						lines.Add(reader.ReadLine());
					}
				}
				return ReadData(lines.ToArray(), ignoreEmptyColumns);
			} catch (Exception e) {
				Logger<VectoCSVFile>().Error(e);
				throw new VectoException("failed to read stream", e);
			}
		}

		private static DataTable ReadData(string[] data, bool ignoreEmptyColumns = false, bool fullHeader = false)
		{
			var lines = RemoveComments(data);

			var validColumns = GetValidHeaderColumns(lines.First(), fullHeader).ToArray();

			if (validColumns.Length > 0) {
				// Valid Columns found => header was valid => skip header line
				lines = lines.Skip(1).ToArray();
			} else {
				Logger<VectoCSVFile>().Warn("No valid Data Header found. Interpreting the first line as data line.");
				// set the validColumns to: {"0", "1", "2", "3", ...} for all columns in first line.
				validColumns = GetColumns(lines.First()).Select((_, index) => index.ToString()).ToArray();
			}

			var table = new DataTable();
			foreach (var col in validColumns) {
				table.Columns.Add(col);
			}

			for (var i = 0; i < lines.Length; i++) {
				var line = lines[i];

				var cells = line.Split(Delimiter);
				if (!ignoreEmptyColumns && cells.Length != table.Columns.Count) {
					throw new CSVReadException(string.Format("Line {0}: The number of values is not correct.", i));
				}

				try {
					table.Rows.Add(line.Split(Delimiter));
				} catch (InvalidCastException e) {
					throw new CSVReadException(
						string.Format("Line {0}: The data format of a value is not correct. {1}", i, e.Message), e);
				}
			}

			return table;
		}

		private static IEnumerable<string> GetValidHeaderColumns(string line, bool fullHeader = false)
		{
			double test;
			var validColumns = GetColumns(line, fullHeader).
				Where(col => !double.TryParse(col, NumberStyles.Any, CultureInfo.InvariantCulture, out test));
			return validColumns.ToArray();
		}

		private static IEnumerable<string> GetColumns(string line, bool fullHeader = false)
		{
			if (!fullHeader) {
				line = Regex.Replace(line, @"\[.*?\]", "");
				line = line.Replace("<", "");
				line = line.Replace(">", "");
			}
			return line.Split(Delimiter).Select(col => col.Trim());
		}

		private static string[] RemoveComments(string[] lines)
		{
			Contract.Requires(lines != null);

			lines = lines.
				Select(line => line.Contains('#') ? line.Substring(0, line.IndexOf(Comment)) : line).
				Where(line => !string.IsNullOrEmpty(line)).
				ToArray();
			return lines;
		}

		/// <summary>
		///     Writes the datatable to the csv file.
		///     Uses the column caption as header (with fallback to column name) for the csv header.
		/// </summary>
		/// <param name="fileName">Path to the file.</param>
		/// <param name="table">The Datatable.</param>
		public static void Write(string fileName, DataTable table)
		{
			var stream = new StreamWriter(fileName);
			Write(stream, table);
			stream.Close();
		}

		/// <summary>
		/// writes the datatable to a csv file.
		/// Uses the column caption as header (with fallback to column name) for the csv header.
		/// <remarks>Note: the callee has to make suree to close the stream after use.</remarks>
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="table"></param>
		public static void Write(StreamWriter writer, DataTable table)
		{
			if (writer == null) {
				return;
			}
			var header = table.Columns.Cast<DataColumn>().Select(col => col.Caption ?? col.ColumnName);
			writer.WriteLine(string.Join(Delimiter.ToString(), header));

			foreach (DataRow row in table.Rows) {
				var row1 = row;
				var formattedList = table.Columns.Cast<DataColumn>().Select(col => {
					var item = row1[col];
					var decimals = (uint?)col.ExtendedProperties["decimals"];
					var outputFactor = (double?)col.ExtendedProperties["outputFactor"];
					var showUnit = (bool?)col.ExtendedProperties["showUnit"];

					var si = item as SI;
					return (si != null
						? si.ToOutputFormat(decimals, outputFactor, showUnit)
						: string.Format(CultureInfo.InvariantCulture, "{0}", item));
				});

				writer.WriteLine(string.Join(Delimiter.ToString(), formattedList));
			}
		}
	}
}