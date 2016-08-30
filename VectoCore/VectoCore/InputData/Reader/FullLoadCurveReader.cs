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
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
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
					Fields.TorqueDrag, string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

				entriesFld = CreateFromColumnIndizes(data, engineFld);
			}

			LookupData<PerSecond, Second> tmp;
			if (declarationMode) {
				tmp = new PT1();
			} else {
				if (data.Columns.Count > 3) {
					tmp = PT1Curve.Create(data);
				} else {
					tmp = new PT1();
				}
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

		public static class Fields
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