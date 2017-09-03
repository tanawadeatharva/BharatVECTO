/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	[CustomValidation(typeof(AuxiliaryData), "ValidateAuxMap")]
	public sealed class AuxiliaryData
	{
		[Required, Range(double.Epsilon, 1)]
		public double EfficiencyToSupply { get; private set; }

		[Required, Range(double.Epsilon, double.MaxValue)]
		public double TransmissionRatio { get; private set; }

		[Required, Range(double.Epsilon, 1)]
		public double EfficiencyToEngine { get; private set; }

		[Required] private readonly DelaunayMap _map;

		private string auxId;

		public Watt GetPowerDemand(PerSecond nAuxiliary, Watt powerAuxOut)
		{
			var value = _map.Interpolate(nAuxiliary.Value(), powerAuxOut.Value());
			if (value.HasValue) {
				return value.Value.SI<Watt>();
			}
			value = _map.Extrapolate(nAuxiliary.Value(), powerAuxOut.Value());
			return value.Value.SI<Watt>();
		}

		internal AuxiliaryData(string id, double transmissionRatio, double efficiencyToEngine, double efficiencyToSupply,
			DelaunayMap map)
		{
			auxId = id;
			_map = map;
			TransmissionRatio = transmissionRatio;
			EfficiencyToEngine = efficiencyToEngine;
			EfficiencyToSupply = efficiencyToSupply;
		}

		/// <summary>
		/// Validates the aux map.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="context">The validation context.</param>
		/// <returns></returns>
		// ReSharper disable once UnusedMember.Global
		public static ValidationResult ValidateAuxMap(AuxiliaryData data, ValidationContext context)
		{
			var xValidationRules = new[] { new RangeAttribute(0, double.MaxValue) };
            //var yValidationRules = new[] { new RangeAttribute(0, 100.SI().Kilo.Watt.Value()) };
		    var yValidationRules = new[] { new RangeAttribute(0, 100.SI(Unit.SI.Kilo.Watt).Value()) };
            //var zValidationRules = new[] { new RangeAttribute(0, 100.SI().Kilo.Watt.Value()) };
		    var zValidationRules = new[] { new RangeAttribute(0, 100.SI(Unit.SI.Kilo.Watt).Value()) };

            var results = new List<ValidationResult>();
			foreach (var entry in data._map.Entries) {
				context.DisplayName = AuxiliaryDataReader.Fields.AuxSpeed;
				if (!Validator.TryValidateValue(entry.X, context, results, xValidationRules)) {
					return new ValidationResult(string.Concat(results));
				}

				context.DisplayName = AuxiliaryDataReader.Fields.SupplyPower;
				if (!Validator.TryValidateValue(entry.Y, context, results, yValidationRules)) {
					return new ValidationResult(string.Concat(results));
				}

				context.DisplayName = AuxiliaryDataReader.Fields.MechPower;
				if (!Validator.TryValidateValue(entry.Z, context, results, zValidationRules)) {
					return new ValidationResult(string.Concat(results));
				}
			}
			return ValidationResult.Success;
		}
	}

	public class AdvancedAuxData
	{
		public AuxiliaryModel AuxiliaryAssembly;

		public string AdvancedAuxiliaryFilePath;

		public string AuxiliaryVersion;
	}
}