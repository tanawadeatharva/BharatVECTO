// Copyright 2017 European Union.
// Licensed under the EUPL (the 'Licence');
// 
// * You may not use this work except in compliance with the Licence.
// * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
// * Unless required by applicable law or agreed to in writing,
// software distributed under the Licence is distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the LICENSE.txt for the specific language governing permissions and limitations.

using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries {
	public interface IVectoInputs
	{
		/// <summary>
		/// Vehicle Mass (KG)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Kilogram VehicleWeightKG { get; set; }

		/// <summary>
		/// Cycle ( Urban, Interurban etc )
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		string Cycle { get; set; }

		/// <summary>
		/// PowerNet Voltage (V) Volts available on the bus by Batteries
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Volt PowerNetVoltage { get; set; }

		/// <summary>
		/// Fuel Map Used in Vecto.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		IFuelConsumptionMap FuelMap { get; set; }

		string FuelMapFile { get; set; }

		/// <summary>
		/// Fuel density used in Vecto.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		KilogramPerCubicMeter FuelDensity { get; set; }
	}
}
