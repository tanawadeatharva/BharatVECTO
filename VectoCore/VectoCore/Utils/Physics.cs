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

using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Utils
{
	public static class Physics
	{
		/// <summary>
		/// The standard acceleration for gravity on earth.
		/// http://physics.nist.gov/Pubs/SP330/sp330.pdf (page 52)
		/// </summary>
		public static MeterPerSquareSecond GravityAccelleration = DeclarationData.GravityAccelleration;

		/// <summary>
		/// Density of air.
		/// </summary>
		public static KilogramPerCubicMeter AirDensity = DeclarationData.AirDensity; // 1.188.SI<KilogramPerCubicMeter>();


		public const double RollResistanceExponent = 0.9;

		/// <summary>
		/// Base Wind Speed.
		/// </summary>
		public static readonly MeterPerSecond BaseWindSpeed = 3.SI<MeterPerSecond>();

		/// <summary>
		/// Base Height for Wind Speed.
		/// </summary>
		public static readonly Meter BaseWindHeight = 4.SI<Meter>();

		/// <summary>
		/// Hellmann Exponent for modelling of wind speed in specific heights.
		/// </summary>
		public const double HellmannExponent = 0.2;
	}
}