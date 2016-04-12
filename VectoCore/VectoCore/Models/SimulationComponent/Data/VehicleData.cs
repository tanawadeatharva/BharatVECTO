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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	[CustomValidation(typeof(VehicleData), "ValidateVehicleData")]
	public class VehicleData : SimulationComponentData
	{
		//public string BasePath { get; internal set; }
		private List<Axle> _axleData;

		public VehicleCategory VehicleCategory { get; internal set; }
		public VehicleClass VehicleClass { get; internal set; }
		//public CrossWindCorrectionMode CrossWindCorrectionMode { get; internal set; }

		[Required, ValidateObject]
		public ICrossWindCorrection CrossWindCorrectionCurve { get; internal set; }

		/// <summary>
		///     Set the properties for all axles of the vehicle
		/// </summary>
		[ValidateObject]
		public List<Axle> AxleData
		{
			get { return _axleData; }
			internal set
			{
				_axleData = value;
				ComputeRollResistanceAndReducedMassWheels();
			}
		}

		public AxleConfiguration AxleConfiguration { get; internal set; }

		[Required, SIRange(500, 40000)]
		public Kilogram CurbWeight { get; internal set; }

		[Required, SIRange(0, 40000)]
		public Kilogram CurbWeigthExtra { get; internal set; }

		[Required, SIRange(0, 40000)]
		public Kilogram Loading { get; internal set; }

		[Required, SIRange(3500, 40000)]
		public Kilogram GrossVehicleMassRating { get; internal set; }

		[Required, SIRange(0.1, 0.7)]
		public Meter DynamicTyreRadius { get; internal set; }

		public KilogramSquareMeter WheelsInertia { get; internal set; }


		public string Rim { get; internal set; }

		[Required, SIRange(0, 1E12)]
		public double TotalRollResistanceCoefficient { get; private set; }

		public CrossWindCorrectionMode CrossWindCorrectionMode { get; set; }

		public Kilogram TotalVehicleWeight()
		{
			var retVal = 0.SI<Kilogram>();
			retVal += CurbWeight ?? 0.SI<Kilogram>();
			retVal += CurbWeigthExtra ?? 0.SI<Kilogram>();
			retVal += Loading ?? 0.SI<Kilogram>();
			return retVal;
		}

		public Kilogram TotalCurbWeight()
		{
			var retVal = 0.SI<Kilogram>();
			retVal += CurbWeight ?? 0.SI<Kilogram>();
			retVal += CurbWeigthExtra ?? 0.SI<Kilogram>();
			return retVal;
		}

		protected void ComputeRollResistanceAndReducedMassWheels()
		{
			if (TotalVehicleWeight() == 0.SI<Kilogram>()) {
				throw new VectoException("Total vehicle weight must be greater than 0! Set CurbWeight and Loading before!");
			}
			if (DynamicTyreRadius == null) {
				throw new VectoException("Dynamic tyre radius must be set before axles!");
			}

			var g = Physics.GravityAccelleration;

			var rrc = 0.0.SI<Scalar>();
			var wheelsInertia = 0.0.SI<KilogramSquareMeter>();
			foreach (var axle in _axleData) {
				var nrWheels = axle.TwinTyres ? 4 : 2;
				var baseValue = (axle.AxleWeightShare * TotalVehicleWeight() * g / axle.TyreTestLoad / nrWheels).Value();
				if (baseValue == 0) {
					throw new VectoSimulationException(
						"Axle Roll Resistance Coefficient could not be calculated. One of the values is 0: AxleWeightShare: {0}, TotalVehicleWeight: {1}, TyreTestLoad: {2}, nrWheels: {3}",
						axle.AxleWeightShare, TotalVehicleWeight(), axle.TyreTestLoad, nrWheels);
				}
				rrc += axle.AxleWeightShare * axle.RollResistanceCoefficient *
						Math.Pow(baseValue, Physics.RollResistanceExponent - 1);
				wheelsInertia += nrWheels * axle.Inertia;
			}
			TotalRollResistanceCoefficient = rrc;
			WheelsInertia = wheelsInertia;
		}

		public static ValidationResult ValidateVehicleData(VehicleData vehicleData, ValidationContext validationContext)
		{
			var weightShareSum = vehicleData.AxleData.Sum(axle => axle.AxleWeightShare);
			if (!weightShareSum.IsEqual(1.0, 1E-10)) {
				return new ValidationResult(
					string.Format("Sum of axle weight share is not 1! sum: {0}, difference: {1}",
						weightShareSum, 1 - weightShareSum));
			}

			if (vehicleData.TotalVehicleWeight() > vehicleData.GrossVehicleMassRating) {
				return new ValidationResult(
					string.Format("Total Vehicle Weight is greater than GrossVehicleMassRating! sum: {0},  GVM: {1}",
						vehicleData.TotalVehicleWeight(), vehicleData.GrossVehicleMassRating));
			}

			return ValidationResult.Success;
		}
	}
}