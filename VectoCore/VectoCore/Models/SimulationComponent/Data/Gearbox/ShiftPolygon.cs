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
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
{
	[CustomValidation(typeof(ShiftPolygon), "ValidateShiftPolygon")]
	public class ShiftPolygon : SimulationComponentData
	{
		private readonly List<ShiftPolygonEntry> _upShiftPolygon;
		private readonly List<ShiftPolygonEntry> _downShiftPolygon;

		internal ShiftPolygon(List<ShiftPolygonEntry> downshift, List<ShiftPolygonEntry> upShift)
		{
			_upShiftPolygon = upShift;
			_downShiftPolygon = downshift;
		}


		public ReadOnlyCollection<ShiftPolygonEntry> Upshift
		{
			get { return _upShiftPolygon.AsReadOnly(); }
		}

		public ReadOnlyCollection<ShiftPolygonEntry> Downshift
		{
			get { return _downShiftPolygon.AsReadOnly(); }
		}

		/// <summary>
		/// Tests if current power request is on the left side of the shiftpolygon segment
		/// </summary>
		/// <param name="angularSpeed">The angular speed.</param>
		/// <param name="torque">The torque.</param>
		/// <param name="segment">Edge of the shift polygon</param>
		/// <returns><c>true</c> if current power request is on the left side of the shiftpolygon segment; otherwise, <c>false</c>.</returns>
		/// <remarks>Computes a simplified cross product for the vectors: from--X, from--to and checks
		/// if the z-component is positive (which means that X was on the right side of from--to).</remarks>
		public static bool IsLeftOf(PerSecond angularSpeed, NewtonMeter torque,
			Tuple<ShiftPolygonEntry, ShiftPolygonEntry> segment)
		{
			var abX = segment.Item2.AngularSpeed - segment.Item1.AngularSpeed;
			var abY = segment.Item2.Torque - segment.Item1.Torque;
			var acX = angularSpeed - segment.Item1.AngularSpeed;
			var acY = torque - segment.Item1.Torque;
			var z = abX * acY - abY * acX;
			return z.IsGreater(0);
		}

		/// <summary>
		/// Tests if current power request is on the left side of the shiftpolygon segment
		/// </summary>
		/// <param name="angularSpeed">The angular speed.</param>
		/// <param name="torque">The torque.</param>
		/// <param name="segment">Edge of the shift polygon</param>
		/// <returns><c>true</c> if current power request is on the left side of the shiftpolygon segment; otherwise, <c>false</c>.</returns>
		/// <remarks>Computes a simplified cross product for the vectors: from--X, from--to and checks
		/// if the z-component is negative (which means that X was on the left side of from--to).</remarks>
		public static bool IsRightOf(PerSecond angularSpeed, NewtonMeter torque,
			Tuple<ShiftPolygonEntry, ShiftPolygonEntry> segment)
		{
			var abX = segment.Item2.AngularSpeed - segment.Item1.AngularSpeed;
			var abY = segment.Item2.Torque - segment.Item1.Torque;
			var acX = angularSpeed - segment.Item1.AngularSpeed;
			var acY = torque - segment.Item1.Torque;
			var z = abX * acY - abY * acX;
			return z.IsSmaller(0);
		}

		public static ValidationResult ValidateShiftPolygon(ShiftPolygon shiftPolygon, ValidationContext validationContext)
		{
			return shiftPolygon.Downshift.Pairwise(Tuple.Create)
				.Any(
					downshiftLine =>
						shiftPolygon.Upshift.Any(upshiftEntry => IsLeftOf(upshiftEntry.AngularSpeed, upshiftEntry.Torque, downshiftLine)))
				? new ValidationResult("upshift line has to be right of the downshift line!")
				: ValidationResult.Success;
		}


		[DebuggerDisplay("{Torque}, {AngularSpeed}")]
		public class ShiftPolygonEntry
		{
			/// <summary>
			///		[Nm] engine torque
			/// </summary>
			public NewtonMeter Torque { get; set; }

			/// <summary>
			///		[1/s] angular velocity threshold
			/// </summary>
			public PerSecond AngularSpeed { get; set; }
		}
	}
}