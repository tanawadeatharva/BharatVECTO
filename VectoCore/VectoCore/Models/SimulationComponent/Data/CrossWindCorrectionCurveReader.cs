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

using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class CrossWindCorrectionCurveReader : LoggingObject
	{
		public static List<CrossWindCorrectionEntry> GetNoCorrectionCurve(SquareMeter aerodynamicDragArea)
		{
			return new[] {
				new CrossWindCorrectionEntry {
					Velocity = 0.KMPHtoMeterPerSecond(),
					EffectiveCrossSectionArea = aerodynamicDragArea
				},
				new CrossWindCorrectionEntry {
					Velocity = 150.KMPHtoMeterPerSecond(),
					EffectiveCrossSectionArea = aerodynamicDragArea
				}
			}.ToList();
		}

		public static List<CrossWindCorrectionEntry> ReadSpeedDependentCorrectionFromFile(string fileName,
			SquareMeter aerodynamicDragArea)
		{
			var data = VectoCSVFile.Read(fileName);
			return ParseSpeedDependent(data, aerodynamicDragArea);
		}

		public static List<CrossWindCorrectionEntry> ReadSpeedDependentCorrectionCurveFromStream(Stream inputData,
			SquareMeter aerodynamicDragArea)
		{
			var data = VectoCSVFile.ReadStream(inputData);
			return ParseSpeedDependent(data, aerodynamicDragArea);
		}

		public static List<CrossWindCorrectionEntry> ReadSpeedDependentCorrectionCurve(DataTable data,
			SquareMeter aerodynamicDragArea)
		{
			return ParseSpeedDependent(data, aerodynamicDragArea);
		}

		public static List<AirDragBetaEntry> ReadCdxABetaTable(DataTable betaTable)
		{
			if (betaTable.Columns.Count != 2) {
				throw new VectoException("VAir/Beta Crosswind Correction must consist of 2 columns");
			}
			if (betaTable.Rows.Count < 2) {
				throw new VectoException("VAir/Beta Crosswind Correction must consist of at least 2 rows");
			}
			if (HeaderIsValid(betaTable.Columns)) {
				return ParseCdxABetaFromColumnNames(betaTable);
			}
			Logger<CrossWindCorrectionCurveReader>().Warn("VAir/Beta Crosswind Correction header Line is not valid");
			return ParseCdxABetaFromColumnIndices(betaTable);
		}

		protected static List<CrossWindCorrectionEntry> ParseSpeedDependent(DataTable data,
			SquareMeter aerodynamicDragArea)
		{
			if (data.Columns.Count != 2) {
				throw new VectoException("Crosswind correction file must consist of 2 columns.");
			}
			if (data.Rows.Count < 2) {
				throw new VectoException("Crosswind correction file must consist of at least two entries");
			}

			if (SpeedDependentHeaderIsValid(data.Columns)) {
				return ParseSpeedDependentFromColumnNames(data, aerodynamicDragArea);
			}
			Logger<CrossWindCorrectionCurveReader>()
				.Warn(
					"Crosswind correction file: Header line is not valid. Expected: '{0}, {1}', Got: '{2}'. Falling back to column index.",
					FieldsSpeedDependent.Velocity, FieldsSpeedDependent.Cd,
					", ".Join(data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Reverse()));
			return ParseSpeedDependentFromColumnIndizes(data, aerodynamicDragArea);
		}

		protected static List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> ParseSpeedDependentFromColumnIndizes(
			DataTable data,
			SquareMeter aerodynamicDragArea)
		{
			return (from DataRow row in data.Rows
				select new CrossWindCorrectionEntry {
					Velocity = row.ParseDouble(0).KMPHtoMeterPerSecond(),
					EffectiveCrossSectionArea = row.ParseDouble(1) * aerodynamicDragArea
				}).ToList();
		}

		protected static List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> ParseSpeedDependentFromColumnNames(
			DataTable data,
			SquareMeter aerodynamicDragArea)
		{
			return (from DataRow row in data.Rows
				select new CrossWindCorrectionEntry {
					Velocity = row.ParseDouble(FieldsSpeedDependent.Velocity).KMPHtoMeterPerSecond(),
					EffectiveCrossSectionArea = row.ParseDouble(FieldsSpeedDependent.Cd) * aerodynamicDragArea
				}).ToList();
		}

		private static bool SpeedDependentHeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(FieldsSpeedDependent.Velocity) && columns.Contains(FieldsSpeedDependent.Cd);
		}

		private static List<AirDragBetaEntry> ParseCdxABetaFromColumnIndices(DataTable betaTable)
		{
			return (from DataRow row in betaTable.Rows
				select
					new AirDragBetaEntry() {
						Beta = row.ParseDouble(0),
						DeltaCdA = row.ParseDouble(1).SI<SquareMeter>()
					}).ToList();
		}

		private static List<AirDragBetaEntry> ParseCdxABetaFromColumnNames(DataTable betaTable)
		{
			return (from DataRow row in betaTable.Rows
				select
					new AirDragBetaEntry() {
						Beta = row.ParseDouble(FieldsCdxABeta.Beta),
						DeltaCdA = row.ParseDouble(FieldsCdxABeta.DeltaCdxA).SI<SquareMeter>()
					}).ToList();
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(FieldsCdxABeta.Beta) && columns.Contains(FieldsCdxABeta.DeltaCdxA);
		}

		private static class FieldsCdxABeta
		{
			public const string Beta = "beta";

			public const string DeltaCdxA = "delta CdA";
		}

		public class AirDragBetaEntry
		{
			public double Beta;
			public SquareMeter DeltaCdA;
		}

		public class FieldsSpeedDependent
		{
			public static readonly string Velocity = "v_veh";
			public static readonly string Cd = "Cd";
		}

		public class CrossWindCorrectionEntry
		{
			public SquareMeter EffectiveCrossSectionArea;
			public MeterPerSecond Velocity;
		}
	}
}