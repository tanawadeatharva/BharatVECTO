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
using System.Runtime.Serialization;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	/// <summary>
	/// Class for Gearbox Data. Gears can be accessed via Gears-Dictionary and range from 1 upwards.
	/// </summary>
	/// <remarks>The Axle Gear has its own Property "AxleGearData" and is *not included* in the Gears-Dictionary.</remarks>
	[DataContract]
	public class GearboxData : SimulationComponentData
	{
		/// <summary>
		/// The gear data.
		/// </summary>
		[Required, ValidateObject] public Dictionary<uint, GearData> Gears = new Dictionary<uint, GearData>();

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		public GearboxType Type { get; internal set; }

		/// <summary>
		/// Gets the inertia.
		/// </summary>
		/// <value>
		/// The inertia.
		/// </value>
		[Required, SIRange(0, 10)]
		public KilogramSquareMeter Inertia { get; internal set; }

		/// <summary>
		/// Gets the traction interruption.
		/// </summary>
		/// <value>
		/// The traction interruption.
		/// </value>
		[Required, SIRange(0, 5)]
		public Second TractionInterruption { get; internal set; }

		/// <summary>
		///	[%] (0-1) The torque reserve for shift strategy (early upshift, skipgears)
		/// </summary>
		[Required, Range(0, 0.5)]
		public double TorqueReserve { get; internal set; }

		/// <summary>
		///	Indicates if gears can be skipped in Gear Shift Strategy.
		/// </summary>
		public bool SkipGears { get; internal set; }

		/// <summary>
		/// Gets the minimum time between shifts.
		/// </summary>
		[Required, SIRange(0, 5)]
		public Second ShiftTime { get; internal set; }

		/// <summary>
		/// True if the gearbox should do early up shifts.
		/// </summary>
		public bool EarlyShiftUp { get; internal set; }

		/// <summary>
		/// [%] (0-1) The starting torque reserve for finding the starting gear after standstill.
		/// </summary>
		[Required, Range(0, 0.5)]
		public double StartTorqueReserve { get; internal set; }

		/// <summary>
		/// Gets the start speed.
		/// </summary>
		/// <value>
		/// The start speed.
		/// </value>
		[Required, SIRange(double.Epsilon, 5)]
		public MeterPerSecond StartSpeed { get; internal set; }

		/// <summary>
		/// Gets the start acceleration.
		/// </summary>
		/// <value>
		/// The start acceleration.
		/// </value>
		[Required, SIRange(double.Epsilon, 2)]
		public MeterPerSquareSecond StartAcceleration { get; internal set; }

		/// <summary>
		/// Gets a value indicating whether this instance has torque converter.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has torque converter; otherwise, <c>false</c>.
		/// </value>
		public bool HasTorqueConverter { get; internal set; }
	}
}