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
using System.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class VehicleData : SimulationComponentData
	{
		//public string BasePath { get; internal set; }
		private List<Axle> _axleData;

		public VehicleCategory VehicleCategory { get; internal set; }
		public VehicleClass VehicleClass { get; internal set; }
		//public CrossWindCorrectionMode CrossWindCorrectionMode { get; internal set; }

		public ICrossWindCorrection CrossWindCorrectionCurve { get; internal set; }

		/// <summary>
		///     Set the properties for all axles of the vehicle
		/// </summary>
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
		public Kilogram CurbWeight { get; internal set; }
		public Kilogram CurbWeigthExtra { get; internal set; }
		public Kilogram Loading { get; internal set; }
		public Kilogram GrossVehicleMassRating { get; internal set; }
		public Meter DynamicTyreRadius { get; internal set; }
		public KilogramSquareMeter WheelsInertia { get; internal set; }
		// public Kilogram ReducedMassWheels { get; private set; }
		public string Rim { get; internal set; }
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

			var RRC = 0.0.SI<Scalar>();
			var wheelsInertia = 0.0.SI<KilogramSquareMeter>();
			foreach (var axle in _axleData) {
				var nrWheels = axle.TwinTyres ? 4 : 2;
				RRC += axle.AxleWeightShare * axle.RollResistanceCoefficient *
						Math.Pow((axle.AxleWeightShare * TotalVehicleWeight() * g / axle.TyreTestLoad / nrWheels).Value(),
							Physics.RollResistanceExponent - 1);
				wheelsInertia += nrWheels * axle.Inertia;
			}
			TotalRollResistanceCoefficient = RRC;
			WheelsInertia = wheelsInertia;
		}
	}
}