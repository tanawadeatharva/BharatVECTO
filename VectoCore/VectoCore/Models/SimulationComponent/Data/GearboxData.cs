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
using System.Runtime.Serialization;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	/// <summary>
	/// Class for Gearbox Data. Gears can be accessed via Gears-Dictionary and range from 1 upwards.
	/// </summary>
	/// <remarks>The Axle Gear has its own Property "AxleGearData" and is *not included* in the Gears-Dictionary.</remarks>
	[DataContract, CustomValidation(typeof(GearboxData), "ValidateGearboxData")]
	public class GearboxData : SimulationComponentData
	{
		public GearboxType Type { get; internal set; }

		/// <summary>
		/// The gear data.
		/// </summary>
		[Required, ValidateObject] public Dictionary<uint, GearData> Gears = new Dictionary<uint, GearData>();

		public TorqueConverterData TorqueConverterData { get; internal set; }

		[Required, SIRange(0, 10)]
		public KilogramSquareMeter Inertia { get; internal set; }

		[Required, SIRange(0, 5)]
		public Second TractionInterruption { get; internal set; }

		/// <summary>
		///	[%] (0-1) The torque reserve for shift strategy (early upshift, skipgears)
		/// </summary>
		[Required, Range(0, 0.5)]
		public double TorqueReserve { get; internal set; }

		/// <summary>
		/// Gets the minimum time between shifts.
		/// </summary>
		[Required, SIRange(0, 5)]
		public Second ShiftTime { get; internal set; }

		/// <summary>
		/// [%] (0-1) The starting torque reserve for finding the starting gear after standstill.
		/// </summary>
		[Required, Range(0, 0.5)]
		public double StartTorqueReserve { get; internal set; }

		// MQ: TODO: move to Driver Data ?
		[Required, SIRange(double.Epsilon, 5)]
		public MeterPerSecond StartSpeed { get; internal set; }

		// MQ: TODO: move to Driver Data ?
		[Required, SIRange(double.Epsilon, 2)]
		public MeterPerSquareSecond StartAcceleration { get; internal set; }

		///// <summary>
		///// Gets a value indicating whether this instance has torque converter.
		///// </summary>
		///// <value>
		///// <c>true</c> if this instance has torque converter; otherwise, <c>false</c>.
		///// </value>
		//public bool HasTorqueConverter { get; internal set; }

		[Required, SIRange(0, double.MaxValue)]
		public Second UpshiftAfterDownshiftDelay { get; internal set; }

		[Required, SIRange(0, double.MaxValue)]
		public Second DownshiftAfterUpshiftDelay { get; internal set; }

		[Required, SIRange(0, double.MaxValue)]
		public MeterPerSquareSecond UpshiftMinAcceleration { get; internal set; }

		// ReSharper disable once UnusedMember.Global -- used via Validation
		public static ValidationResult ValidateGearboxData(GearboxData gearboxData, ValidationContext validationContext)
		{
			var mode = GetExecutionMode(validationContext);
			//var gbxType = GetGearboxType(validationContext);

			var result = new List<ValidationResult>();
			if (gearboxData.Type.AutomaticTransmission()) {
				gearboxData.TorqueConverterData.RequiredSpeedRatio =
					Math.Round(Constants.SimulationSettings.RequiredTorqueConverterSpeedRatio / gearboxData.Gears[1].Ratio *
								gearboxData.Gears[1].TorqueConverterRatio, 4);
				result.AddRange(gearboxData.TorqueConverterData.Validate(mode, gearboxData.Type));
			}

			if (result.Any()) {
				return new ValidationResult("Validation of Gearbox Data failed", result.Select(x => x.ErrorMessage));
			}
			return ValidationResult.Success;
		}
	}
}