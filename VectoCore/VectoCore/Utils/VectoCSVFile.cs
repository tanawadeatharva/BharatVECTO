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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	/// <summary>
	/// Class for Reading and Writing VECTO CSV Files.
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
	public static class VectoCSVFile
	{
		private static readonly Regex HeaderFilter = new Regex(@"\[.*?\]|\<|\>", RegexOptions.Compiled);
		private const char Delimiter = ',';
		private const char Comment = '#';

		/// <summary>
		/// Reads a CSV file which is stored in Vecto-CSV-Format.
		/// </summary>
		/// <param name="fileName">the filename</param>
		/// <param name="ignoreEmptyColumns">set true, if empty columns should be ignored. default: false.</param>
		/// <param name="fullHeader">set true is column names should be preserved. Otherwise units are trimed away. default: false.</param>
		/// <returns>A DataTable which represents the CSV File.</returns>
		public static DataTable Read(string fileName, bool ignoreEmptyColumns = false, bool fullHeader = false)
		{
			try {
				using (var fs = new FileStream(fileName, FileMode.Open)) {
					return ReadStream(fs, ignoreEmptyColumns, fullHeader);
				}
			} catch (Exception e) {
				LogManager.GetLogger(typeof(VectoCSVFile).FullName).Error(e);
				throw new VectoException("Error File {0}: {1}", fileName, e.Message);
			}
		}

		/// <summary>
		/// Reads a CSV file which is stored in Vecto-CSV-Format.
		/// </summary>
		/// <param name="stream">the stream to read</param>
		/// <param name="ignoreEmptyColumns">set true, if empty columns should be ignored. default: false.</param>
		/// <param name="fullHeader">set true is column names should be preserved. Otherwise units are trimed away. default: false.</param>
		/// <returns>A DataTable which represents the CSV File.</returns>
		public static DataTable ReadStream(Stream stream, bool ignoreEmptyColumns = false, bool fullHeader = false)
		{
			try {
				return ReadData(ReadLines(stream), ignoreEmptyColumns, fullHeader);
			} catch (Exception e) {
				LogManager.GetLogger(typeof(VectoCSVFile).FullName).Error(e);
				throw new VectoException("Failed to read stream: " + e.Message, e);
			}
		}

		private static IEnumerable<string> ReadLines(Stream stream)
		{
			using (var reader = new StreamReader(stream, Encoding.UTF8)) {
				while (!reader.EndOfStream) {
					yield return reader.ReadLine();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="allLines"></param>
		/// <param name="ignoreEmptyColumns"></param>
		/// <param name="fullHeader"></param>
		/// <returns></returns>
		private static DataTable ReadData(IEnumerable<string> allLines, bool ignoreEmptyColumns = false,
			bool fullHeader = false)
		{
			// trim, remove comments and filter empty lines
			var lines = allLines
				.Select(l => l.Trim())
				.Select(l => l.Contains(Comment) ? l.Substring(0, l.IndexOf(Comment)) : l)
				.Where(l => !string.IsNullOrWhiteSpace(l))
				.GetEnumerator();

			// start the enumerable
			lines.MoveNext();

			// add columns
			var line = lines.Current;
			if (!fullHeader) {
				line = HeaderFilter.Replace(line, "");
			}
			double tmp;
			var splittedColumns = line
				.Split(Delimiter);

			var columns = splittedColumns
				.Select(col => col.Trim())
				.Where(col => !double.TryParse(col, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp))
				.ToList();

			if (columns.Count > 0) {
				// first line was a valid header: advance to first data line
				lines.MoveNext();
			} else {
				LogManager.GetLogger(typeof(VectoCSVFile).FullName)
					.Warn("No valid Data Header found. Interpreting the first line as data line.");
				// set the validColumns to: {"0", "1", "2", "3", ...} for all columns in first line.
				columns = splittedColumns.Select((_, index) => index.ToString()).ToList();
			}

			var table = new DataTable();
			foreach (var col in columns) {
				table.Columns.Add(col);
			}
			if (lines.Current == null) {
			if (lines.Current == null) {
				return table;
			// read data into table
			var i = 0;
			do {
				i++;
				line = lines.Current;

				var cells = line.Split(Delimiter).Select(s => s.Trim()).ToArray();
				if (cells.Length != table.Columns.Count && !ignoreEmptyColumns) {
					throw new CSVReadException(
						string.Format("Line {0}: The number of values is not correct. Expected {1} Columns, Got {2} Columns", i,
							table.Columns.Count, cells.Length));
				}

				try {
					// ReSharper disable once CoVariantArrayConversion
					table.Rows.Add(cells);
				} catch (InvalidCastException e) {
					throw new CSVReadException(
						string.Format("Line {0}: The data format of a value is not correct. {1}", i, e.Message), e);
				}
			} while (lines.MoveNext());
			return table;
		}

		/// <summary>
		/// Writes the datatable to the csv file.
		/// Uses the column caption as header (with fallback to column name) for the csv header.
		/// </summary>
		/// <param name="fileName">Path to the file.</param>
		/// <param name="table">The Datatable.</param>
		public static void Write(string fileName, DataTable table)
		{
			using (var sw = new StreamWriter(new FileStream(fileName, FileMode.Create), Encoding.UTF8)) {
				Write(sw, table);
			}
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
			writer.WriteLine(Delimiter.ToString().Join(header));

			foreach (DataRow row in table.Rows) {
				var row1 = row;
				var formattedList = table.Columns.Cast<DataColumn>().Select(col => {
					var item = row1[col];
					var decimals = (uint?)col.ExtendedProperties["decimals"];
					var outputFactor = (double?)col.ExtendedProperties["outputFactor"];
					var showUnit = (bool?)col.ExtendedProperties["showUnit"];

					var si = item as SI;
					return si != null
						? si.ToOutputFormat(decimals, outputFactor, showUnit)
						: string.Format(CultureInfo.InvariantCulture, "{0}", item);
				});

				writer.WriteLine(Delimiter.ToString().Join(formattedList));
			}
		}
	}
}