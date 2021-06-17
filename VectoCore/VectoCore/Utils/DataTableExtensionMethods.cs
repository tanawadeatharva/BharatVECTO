/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	public static class DataTableExtensionMethods
	{
		public static double ParseDoubleOrGetDefault(this DataRow row, string columnName,
			double defaultValue = default(double))
		{
			if (row.Table.Columns.Contains(columnName)) {
				if (double.TryParse(row.Field<string>(columnName), NumberStyles.Any, CultureInfo.InvariantCulture, out var result)) {
					return result;
				}
			}
			return defaultValue;
		}

		public static double ParseDouble(this DataRow row, int columnIndex)
		{
			return row.ParseDouble(row.Table.Columns[columnIndex]);
		}

		public static T SI<T>(this DataRow row, int columnIndex) where T : SIBase<T>
		{
			return row.ParseDouble(row.Table.Columns[columnIndex]).SI<T>();
		}

		public static T SI<T>(this DataRow row, string columnName) where T : SIBase<T>
		{
			return row.ParseDouble(columnName).SI<T>();
		}

		public static double ParseDouble(this DataRow row, string columnName)
		{
			if (!row.Table.Columns.Contains(columnName)) {
				throw new KeyNotFoundException($"Column {columnName} was not found in DataRow.");
			}
			return row.ParseDouble(row.Table.Columns[columnName]);
		}

		public static double ParseDouble(this DataRow row, DataColumn column)
		{
			try {
				return row.Field<string>(column).ToDouble();
			} catch (IndexOutOfRangeException e) {
				throw new VectoException($"Field {column} was not found in DataRow.", e);
			} catch (NullReferenceException e) {
				throw new VectoException($"Field {column} must not be null.", e);
			} catch (FormatException e) {
				throw new VectoException($"Field {column} is not in a valid number format: {row.Field<string>(column)}", e);
			} catch (OverflowException e) {
				throw new VectoException($"Field {column} has a value too high or too low: {row.Field<string>(column)}", e);
			} catch (ArgumentNullException e) {
				throw new VectoException($"Field {column} contains null which cannot be converted to a number.",
					e);
			} catch (Exception e) {
				throw new VectoException($"Field {column}: {e.Message}", e);
			}
		}

		public static bool ParseBoolean(this DataRow row, string columnName)
		{
			if (!row.Table.Columns.Contains(columnName)) {
				throw new KeyNotFoundException($"Column {columnName} was not found in DataRow.");
			}
			return row.Field<string>(row.Table.Columns[columnName]).ToBoolean();
		}

		public static bool? ParseBooleanOrGetDefault(this DataRow row, string columnName, bool? defaultValue = null)
		{
			if (!row.Table.Columns.Contains(columnName)) {
				return defaultValue;
			}
			return row.Field<string>(row.Table.Columns[columnName]).ToBoolean();
		}

		public static IEnumerable<T> Values<T>(this DataColumn column)
		{
			return column.Table.AsEnumerable().Select(r => r.Field<T>(column));
		}
	}
}