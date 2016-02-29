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
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
{
	public class ShiftPolygon : SimulationComponentData
	{
		private readonly List<ShiftPolygonEntry> _upShiftPolygon;
		private readonly List<ShiftPolygonEntry> _downShiftPolygon;

		internal ShiftPolygon(List<ShiftPolygonEntry> downshift, List<ShiftPolygonEntry> upShift)
		{
			_upShiftPolygon = upShift;
			_downShiftPolygon = downshift;
		}


		public static ShiftPolygon ReadFromFile(string fileName)
		{
			try {
				var data = VectoCSVFile.Read(fileName);
				return Create(data);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading ShiftPolygon: " + ex.Message);
			}
		}

		public static ShiftPolygon Create(DataTable data)
		{
			if (data.Columns.Count != 3) {
				throw new VectoException("ShiftPolygon Data File must contain exactly 3 columns.");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("ShiftPolygon must have at least two entries");
			}

			List<ShiftPolygonEntry> entriesDown, entriesUp;
			if (HeaderIsValid(data.Columns)) {
				entriesDown = CreateFromColumnNames(data, Fields.AngularSpeedDown);
				entriesUp = CreateFromColumnNames(data, Fields.AngularSpeedUp);
			} else {
				Logger<ShiftPolygon>()
					.Warn(
						"ShiftPolygon: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: '{3}'. Falling back to column index",
						Fields.Torque, Fields.AngularSpeedUp, Fields.AngularSpeedDown,
						", ".Join(data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Reverse()));
				entriesDown = CreateFromColumnIndizes(data, 1);
				entriesUp = CreateFromColumnIndizes(data, 2);
			}
			return new ShiftPolygon(entriesDown, entriesUp);
		}

		public ReadOnlyCollection<ShiftPolygonEntry> Upshift
		{
			get { return _upShiftPolygon.AsReadOnly(); }
		}

		public ReadOnlyCollection<ShiftPolygonEntry> Downshift
		{
			get { return _downShiftPolygon.AsReadOnly(); }
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.Torque) && columns.Contains(Fields.AngularSpeedUp) &&
					columns.Contains((Fields.AngularSpeedDown));
		}

		private static List<ShiftPolygonEntry> CreateFromColumnNames(DataTable data, string columnName)
		{
			return (from DataRow row in data.Rows
				select new ShiftPolygonEntry {
					Torque = row.ParseDouble(Fields.Torque).SI<NewtonMeter>(),
					AngularSpeed = row.ParseDouble(columnName).RPMtoRad(),
				}).ToList();
		}

		private static List<ShiftPolygonEntry> CreateFromColumnIndizes(DataTable data, int column)
		{
			return (from DataRow row in data.Rows
				select
					new ShiftPolygonEntry {
						Torque = row.ParseDouble(0).SI<NewtonMeter>(),
						AngularSpeed = row.ParseDouble(column).RPMtoRad(),
					}).ToList();
		}

		private static class Fields
		{
			/// <summary>
			///		[Nm] torque
			/// </summary>
			public const string Torque = "engine torque";

			/// <summary>
			///		[rpm] threshold for upshift
			/// </summary>
			public const string AngularSpeedUp = "upshift rpm";

			/// <summary>
			///		[rpm] threshold for downshift
			/// </summary>
			public const string AngularSpeedDown = "downshift rpm";
		}

		[DebuggerDisplay("{Torque}, {AngularSpeed}")]
		public class ShiftPolygonEntry
		{
			/// <summary>
			///		[Nm] engine torque
			/// </summary>
			public NewtonMeter Torque { get; set; }

			/// <summary>
			///		[1/s] angular velocity threshold
			/// </summary>
			public PerSecond AngularSpeed { get; set; }
		}
	}
}