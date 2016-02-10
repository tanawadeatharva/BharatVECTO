/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

namespace TUGraz.VectoCore.Utils
{
	public class Physics
	{
		/// <summary>
		/// The standard acceleration for gravity on earth.
		/// http://physics.nist.gov/Pubs/SP330/sp330.pdf (page 52)
		/// </summary>
		public static readonly MeterPerSquareSecond GravityAccelleration = 9.80665.SI<MeterPerSquareSecond>();

		/// <summary>
		/// Density of air.
		/// </summary>
		public static readonly SI AirDensity = 1.188.SI().Kilo.Gramm.Per.Cubic.Meter;

		/// <summary>
		/// Density of fuel.
		/// </summary>
		public static readonly SI FuelDensity = 0.832.SI().Kilo.Gramm.Per.Cubic.Dezi.Meter;

		public static readonly double RollResistanceExponent = 0.9;

		public static readonly MeterPerSecond BaseWindSpeed = 3.SI<MeterPerSecond>();


		/// <summary>
		/// fuel[kg] => co2[kg]. Factor to convert from fuel weight to co2 weight.
		/// </summary>
		public static readonly double CO2PerFuelWeight = 3.16;
	}
}