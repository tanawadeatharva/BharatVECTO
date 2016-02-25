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
using System.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	internal class VAirBetaCrosswindCorrection : LoggingObject, ICrossWindCorrection
	{
		protected SquareMeter AirDragArea { get; set; }

		protected List<AirDragBetaEntry> AirDragEntries;
		protected IDataBus DataBus;

		public VAirBetaCrosswindCorrection(SquareMeter airDragArea, DataTable betaTable)
		{
			AirDragArea = airDragArea;
			if (betaTable.Columns.Count != 2) {
				throw new VectoException("VAir/Beta Crosswind Correction must consist of 2 columns");
			}
			if (betaTable.Rows.Count < 2) {
				throw new VectoException("VAir/Beta Crosswind Correction must consist of at least 2 rows");
			}
			if (HeaderIsValid(betaTable.Columns)) {
				AirDragEntries = CreateFromColumnNames(betaTable);
			} else {
				Log.Warn("VAir/Beta Crosswind Correction header Line is not valid");
				AirDragEntries = CreateFromColumnIndices(betaTable);
			}
		}

		private static List<AirDragBetaEntry> CreateFromColumnIndices(DataTable betaTable)
		{
			return (from DataRow row in betaTable.Rows
				select
					new AirDragBetaEntry() {
						Beta = row.ParseDouble(0),
						DeltaCdA = row.ParseDouble(1).SI<SquareMeter>()
					}).ToList();
		}

		private static List<AirDragBetaEntry> CreateFromColumnNames(DataTable betaTable)
		{
			return (from DataRow row in betaTable.Rows
				select
					new AirDragBetaEntry() {
						Beta = row.ParseDouble(Fields.Beta),
						DeltaCdA = row.ParseDouble(Fields.DeltaCdxA).SI<SquareMeter>()
					}).ToList();
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.Beta) && columns.Contains(Fields.DeltaCdxA);
		}


		public void SetDataBus(IDataBus dataBus)
		{
			DataBus = dataBus;
		}

		public Watt AverageAirDragPowerLoss(MeterPerSecond v1, MeterPerSecond v2, Second dt)
		{
			if (DataBus == null) {
				throw new VectoException("Databus is not set - can't access vAir, beta!");
			}
			var vAir = DataBus.CycleData.LeftSample.AirSpeedRelativeToVehicle;
			var beta = DataBus.CycleData.LeftSample.WindYawAngle;

			var airDragForce = (AirDragArea + DeltaCdA(beta)) * Physics.AirDensity / 2.0 * vAir * vAir;
			var vAverage = (v1 + v2) / 2;
			if (v1.IsEqual(v2)) {
				return (airDragForce * vAverage).Cast<Watt>();
			}
			var acceleration = (v2 - v1) / dt;
			return (airDragForce * (v2 * v2 - v1 * v1) / (2 * acceleration * dt)).Cast<Watt>();
		}

		protected SquareMeter DeltaCdA(double beta)
		{
			var idx = FindIndex(beta);
			return VectoMath.Interpolate(AirDragEntries[idx - 1].Beta, AirDragEntries[idx].Beta, AirDragEntries[idx - 1].DeltaCdA,
				AirDragEntries[idx].DeltaCdA, beta);
		}

		protected int FindIndex(double beta)
		{
			if (beta < AirDragEntries.First().Beta) {
				throw new VectoSimulationException("Beta / CdxA Lookup table does not cover beta={0}", beta);
			}
			if (beta > AirDragEntries.Last().Beta) {
				throw new VectoSimulationException("Beta / CdxA Lookup table does not cover beta={0}", beta);
			}
			int index;
			AirDragEntries.GetSection(x => x.Beta < beta, out index);
			return index + 1;
		}

		protected class AirDragBetaEntry
		{
			public double Beta;
			public SquareMeter DeltaCdA;
		}

		private static class Fields
		{
			public const string Beta = "Beta";

			public const string DeltaCdxA = "Delta CdA";
		}
	}
}