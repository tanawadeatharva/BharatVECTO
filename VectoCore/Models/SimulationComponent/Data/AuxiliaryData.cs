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
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public enum AuxiliaryModel
	{
		Classic,
		Advanced
	}

	public class AuxiliaryModelHelper
	{
		public static AuxiliaryModel Parse(string auxAssemblyStr)
		{
			if (string.IsNullOrEmpty(auxAssemblyStr)) {
				return AuxiliaryModel.Classic;
			}
			switch (auxAssemblyStr) {
				case "BusAuxiliaries":
					return AuxiliaryModel.Advanced;
			}
			return AuxiliaryModel.Classic;
		}
	}

	[CustomValidation(typeof(AuxiliaryData), "ValidateAuxMap")]
	public class AuxiliaryData
	{
		[Required, Range(double.Epsilon, 1)]
		public double EfficiencyToSupply { get; set; }

		[Required, Range(double.Epsilon, double.MaxValue)]
		public double TransmissionRatio { get; set; }

		[Required, Range(double.Epsilon, 1)]
		public double EfficiencyToEngine { get; set; }

		[Required] private readonly DelauneyMap _map = new DelauneyMap();

		public Watt GetPowerDemand(PerSecond nAuxiliary, Watt powerAuxOut)
		{
			return _map.Interpolate(nAuxiliary.Value(), powerAuxOut.Value()).SI<Watt>();
		}

		public static AuxiliaryData ReadFromFile(string fileName)
		{
			var auxData = new AuxiliaryData();

			try {
				var stream = new StreamReader(fileName);
				stream.ReadLine(); // skip header "Transmission ration to engine rpm [-]"
				auxData.TransmissionRatio = stream.ReadLine().IndulgentParse();
				stream.ReadLine(); // skip header "Efficiency to engine [-]"
				auxData.EfficiencyToEngine = stream.ReadLine().IndulgentParse();
				stream.ReadLine(); // skip header "Efficiency auxiliary to supply [-]"
				auxData.EfficiencyToSupply = stream.ReadLine().IndulgentParse();

				var m = new MemoryStream(Encoding.UTF8.GetBytes(stream.ReadToEnd()));
				var table = VectoCSVFile.ReadStream(m);

				if (HeaderIsValid(table.Columns)) {
					FillFromColumnNames(table, auxData._map);
				} else {
					FillFromColumnIndizes(table, auxData._map);
				}

				auxData._map.Triangulate();

				return auxData;
			} catch (FileNotFoundException e) {
				throw new VectoException("Auxiliary file not found: " + fileName, e);
			}
		}

		private static void FillFromColumnIndizes(DataTable table, DelauneyMap map)
		{
			var data = table.Rows.Cast<DataRow>().Select(row => new {
				AuxiliarySpeed = row.ParseDouble(0).RPMtoRad(),
				MechanicalPower = row.ParseDouble(1).SI().Kilo.Watt.Cast<Watt>(),
				SupplyPower = row.ParseDouble(2).SI().Kilo.Watt.Cast<Watt>()
			});
			foreach (var d in data) {
				map.AddPoint(d.AuxiliarySpeed.Value(), d.SupplyPower.Value(), d.MechanicalPower.Value());
			}
		}

		private static void FillFromColumnNames(DataTable table, DelauneyMap map)
		{
			var data = table.Rows.Cast<DataRow>().Select(row => new {
				AuxiliarySpeed = row.ParseDouble(Fields.AuxSpeed).RPMtoRad(),
				MechanicalPower = row.ParseDouble(Fields.MechPower).SI().Kilo.Watt.Cast<Watt>(),
				SupplyPower = row.ParseDouble(Fields.SupplyPower).SI().Kilo.Watt.Cast<Watt>()
			});
			foreach (var d in data) {
				map.AddPoint(d.AuxiliarySpeed.Value(), d.SupplyPower.Value(), d.MechanicalPower.Value());
			}
		}

		internal AuxiliaryData(IAuxiliaryEngineeringInputData data)
		{
			TransmissionRatio = data.TransmissionRatio;
			EfficiencyToEngine = data.EfficiencyToEngine;
			EfficiencyToSupply = data.EfficiencyToSupply;
			if (HeaderIsValid(data.DemandMap.Columns)) {
				FillFromColumnNames(data.DemandMap, _map);
			} else {
				FillFromColumnIndizes(data.DemandMap, _map);
			}

			_map.Triangulate();
		}

		private AuxiliaryData() {}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.AuxSpeed) && columns.Contains(Fields.MechPower) &&
					columns.Contains(Fields.SupplyPower);
		}

		private static class Fields
		{
			/// <summary>
			/// [1/min]
			/// </summary>
			public const string AuxSpeed = "Auxiliary speed";

			/// <summary>
			/// [kW]
			/// </summary>
			public const string MechPower = "Mechanical power";

			/// <summary>
			/// [kW]
			/// </summary>
			public const string SupplyPower = "Supply power";
		}

		/// <summary>
		/// Validates the aux map.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="context">The validation context.</param>
		/// <returns></returns>
		public static ValidationResult ValidateAuxMap(AuxiliaryData data, ValidationContext context)
		{
			var xValidationRules = new[] { new RangeAttribute(0, double.MaxValue) };
			var yValidationRules = new[] { new RangeAttribute(0, 100.SI().Kilo.Watt.Value()) };
			var zValidationRules = new[] { new RangeAttribute(0, 100.SI().Kilo.Watt.Value()) };

			var results = new List<ValidationResult>();
			foreach (var entry in data._map.Points) {
				context.DisplayName = Fields.AuxSpeed;
				if (!Validator.TryValidateValue(entry.X, context, results, xValidationRules)) {
					return new ValidationResult(string.Concat(results));
				}

				context.DisplayName = Fields.SupplyPower;
				if (!Validator.TryValidateValue(entry.Y, context, results, yValidationRules)) {
					return new ValidationResult(string.Concat(results));
				}

				context.DisplayName = Fields.MechPower;
				if (!Validator.TryValidateValue(entry.Z, context, results, zValidationRules)) {
					return new ValidationResult(string.Concat(results));
				}
			}
			return ValidationResult.Success;
		}
	}
}