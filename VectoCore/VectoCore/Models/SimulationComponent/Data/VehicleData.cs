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
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	/// <summary>
	/// Data Class for the Vehicle
	/// </summary>
	[CustomValidation(typeof(VehicleData), "ValidateVehicleData")]
	public class VehicleData : SimulationComponentData
	{
		public VehicleCategory VehicleCategory { get; internal set; }
		public VehicleClass VehicleClass { get; internal set; }
		public AxleConfiguration AxleConfiguration { get; internal set; }

		[Required, ValidateObject]
		public ICrossWindCorrection CrossWindCorrectionCurve { get; internal set; }

		[Required, ValidateObject] private List<Axle> _axleData;

		private KilogramSquareMeter _wheelsInertia;
		private double? _totalRollResistanceCoefficient;

		public List<Axle> AxleData
		{
			get { return _axleData; }
			internal set
			{
				_axleData = value;
				_wheelsInertia = null;
				_totalRollResistanceCoefficient = null;
			}
		}

		/// <summary>
		/// The Curb Weight of the vehicle 
		/// (+ Curb Weight of Standard-Body if it has one)
		/// (+ Curb Weight of Trailer if it has one)
		/// </summary>
		[Required, SIRange(500, 40000)]
		public Kilogram CurbWeight { get; internal set; }

		[Required, SIRange(0, 40000)]
		public Kilogram Loading { get; internal set; }

		/// <summary>
		/// The Gross Vehicle Weight of the Vehicle.
		/// </summary>
		[Required,
		SIRange(3500, 40000, ExecutionMode.Declaration),
		SIRange(0, 1000000, ExecutionMode.Engineering)]
		public Kilogram GrossVehicleWeight { get; internal set; }

		/// <summary>
		/// The Gross Vehicle Weight of the Trailer (if the vehicle has one).
		/// </summary>
		[Required, SIRange(0, 40000)]
		public Kilogram TrailerGrossVehicleWeight { get; internal set; }

		[Required, SIRange(0.1, 0.7)]
		public Meter DynamicTyreRadius { get; internal set; }

		public KilogramSquareMeter WheelsInertia
		{
			get
			{
				if (_wheelsInertia == null) {
					ComputeRollResistanceAndReducedMassWheels();
				}
				return _wheelsInertia;
			}
			internal set { _wheelsInertia = value; }
		}

		//[Required, SIRange(0, 1E12)]
		public double TotalRollResistanceCoefficient
		{
			get
			{
				if (_totalRollResistanceCoefficient == null) {
					ComputeRollResistanceAndReducedMassWheels();
				}
				return _totalRollResistanceCoefficient.GetValueOrDefault();
			}
			private set { _totalRollResistanceCoefficient = value; }
		}

		public CrossWindCorrectionMode CrossWindCorrectionMode { get; set; }

		public Kilogram TotalVehicleWeight
		{
			get
			{
				var retVal = 0.0;
				if (CurbWeight != null) {
					retVal += CurbWeight.Value();
				}
				if (Loading != null) {
					retVal += Loading.Value();
				}
				return retVal.SI<Kilogram>();
			}
		}

		public Kilogram TotalCurbWeight
		{
			get { return CurbWeight ?? 0.SI<Kilogram>(); }
		}

		protected void ComputeRollResistanceAndReducedMassWheels()
		{
			if (TotalVehicleWeight == 0.SI<Kilogram>()) {
				throw new VectoException("Total vehicle weight must be greater than 0! Set CurbWeight and Loading before!");
			}
			if (DynamicTyreRadius == null) {
				throw new VectoException("Dynamic tyre radius must be set before axles!");
			}

			var g = Physics.GravityAccelleration;

			var rrc = 0.0.SI<Scalar>();
			var wheelsInertia = 0.0.SI<KilogramSquareMeter>();
			foreach (var axle in _axleData) {
				if (axle.AxleWeightShare.IsEqual(0, 1e-12)) {
					continue;
				}
				var nrWheels = axle.TwinTyres ? 4 : 2;
				var baseValue = (axle.AxleWeightShare * TotalVehicleWeight * g / axle.TyreTestLoad / nrWheels).Value();

				rrc += axle.AxleWeightShare * axle.RollResistanceCoefficient *
						Math.Pow(baseValue, Physics.RollResistanceExponent - 1);
				wheelsInertia += nrWheels * axle.Inertia;
			}
			TotalRollResistanceCoefficient = rrc;
			WheelsInertia = wheelsInertia;
		}

		// ReSharper disable once UnusedMember.Global  -- used via Validation
		public static ValidationResult ValidateVehicleData(VehicleData vehicleData, ValidationContext validationContext)
		{
			var mode = GetExecutionMode(validationContext);

			if (vehicleData.AxleData.Count < 1) {
				return new ValidationResult("At least two axles need to be specified");
			}

			var weightShareSum = vehicleData.AxleData.Sum(axle => axle.AxleWeightShare);
			if (!weightShareSum.IsEqual(1.0, 1E-10)) {
				return new ValidationResult(
					string.Format("Sum of axle weight share is not 1! sum: {0}, difference: {1}",
						weightShareSum, 1 - weightShareSum));
			}
			for (var i = 0; i < vehicleData.AxleData.Count; i++) {
				if (vehicleData.AxleData[i].TyreTestLoad.IsSmallerOrEqual(0)) {
					return new ValidationResult(string.Format("Tyre test load (FzISO) for axle {0} must be greater than 0.", i));
				}
			}

			if (vehicleData.TotalRollResistanceCoefficient <= 0) {
				return
					new ValidationResult(string.Format("Total rolling resistance must be greater than 0! {0}",
						vehicleData.TotalRollResistanceCoefficient));
			}

			// total gvw is limited by max gvw (40t)
			var gvwTotal = VectoMath.Min(vehicleData.GrossVehicleWeight + vehicleData.TrailerGrossVehicleWeight,
				Constants.SimulationSettings.MaximumGrossVehicleWeight);
			if (mode != ExecutionMode.Declaration) {
				return ValidationResult.Success;
			}
			// vvvvvvv these checks apply only for declaration mode! vvvvvv

			//if (vehicleData.AxleConfiguration.NumAxles() != vehicleData.AxleData.Count) {
			//	return
			//		new ValidationResult(
			//			string.Format("For a {0} type vehicle exactly {1} number of axles have to pe specified. Found {2}",
			//				vehicleData.AxleConfiguration.GetName(), vehicleData.AxleConfiguration.NumAxles(), vehicleData.AxleData.Count));
			//}

			if (vehicleData.TotalVehicleWeight > gvwTotal) {
				return new ValidationResult(
					string.Format("Total Vehicle Weight is greater than GrossVehicleWeight! Weight: {0},  GVW: {1}",
						vehicleData.TotalVehicleWeight, gvwTotal));
			}
			return ValidationResult.Success;
		}
	}
}