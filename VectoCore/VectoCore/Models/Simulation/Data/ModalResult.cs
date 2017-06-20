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
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

// ReSharper disable InconsistentNaming

namespace TUGraz.VectoCore.Models.Simulation.Data
{
	[DesignerCategory("")] // Full qualified attribute needed to disable design view in VisualStudio
	[Serializable]
	public class ModalResults : DataTable
	{
		public static class ExtendedPropertyNames
		{
			public const string Decimals = "decimals";
			public const string OutputFactor = "outputFactor";
			public const string ShowUnit = "showUnit";
		}

		protected ModalResults(SerializationInfo info, StreamingContext context) : base(info, context) {}

		public ModalResults()
		{
			foreach (var value in EnumHelper.GetValues<ModalResultField>()) {
				var col = new DataColumn(value.GetName(), value.GetAttribute().DataType) { Caption = value.GetCaption() };
				col.ExtendedProperties[ExtendedPropertyNames.Decimals] = value.GetAttribute().Decimals;
				col.ExtendedProperties[ExtendedPropertyNames.OutputFactor] = value.GetAttribute().OutputFactor;
				col.ExtendedProperties[ExtendedPropertyNames.ShowUnit] = value.GetAttribute().ShowUnit;
				Columns.Add(col);
			}
		}

		public static ModalResults ReadFromFile(string fileName)
		{
			var modalResults = new ModalResults();
			var data = VectoCSVFile.Read(fileName);

			foreach (DataRow row in data.Rows) {
				try {
					var newRow = modalResults.NewRow();
					foreach (DataColumn col in row.Table.Columns) {
						// In cols FC-AUXc and FC-WHTCc can be a "-"
						if (row.Field<string>(col) == "-"
							&& (col.ColumnName == ModalResultField.FCAUXc.GetName() || col.ColumnName == ModalResultField.FCWHTCc.GetName())) {
							continue;
						}

						// In col FC can sometimes be a "ERROR"
						if (row.Field<string>(col) == "ERROR" && col.ColumnName == ModalResultField.FCMap.GetName()) {
							continue;
						}

						if (col.ColumnName.StartsWith(ModalResultField.P_aux_.ToString()) &&
							!modalResults.Columns.Contains(col.ColumnName)) {
							modalResults.Columns.Add(col.ColumnName, typeof(SI));
						}

						if (typeof(SI).IsAssignableFrom(modalResults.Columns[col.ColumnName].DataType)) {
							newRow.SetField(col.ColumnName, row.ParseDoubleOrGetDefault(col.ColumnName).SI());
						} else {
							newRow.SetField(col.ColumnName, row.ParseDoubleOrGetDefault(col.ColumnName));
						}
					}
					modalResults.Rows.Add(newRow);
				} catch (VectoException ex) {
					throw new VectoException(string.Format("Row {0}: {1}", data.Rows.IndexOf(row), ex.Message), ex);
				}
			}

			return modalResults;
		}
	}
}