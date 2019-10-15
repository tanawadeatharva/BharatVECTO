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

using Newtonsoft.Json;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces {
	public class VectoInputs : IVectoInputs
	{

		/// <summary>
		/// Name of the Cycle ( Urban, Interurban etc )
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public string Cycle { get; set; }

		/// <summary>
		/// Vehicle Mass (KG)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		[JsonIgnore]
		public Kilogram VehicleWeightKG
		{
			get {
				return _vehicleWeight.SI<Kilogram>();
			}
			set {
				_vehicleWeight = value.Value();
			}
		}

		[JsonProperty("VehicleWeightKG")]
		private double _vehicleWeight;

		/// <summary>
		/// Powernet Voltage (V)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>This is the power voltage available in the bus - usually 26.3 Volts</remarks>
		[JsonIgnore]
		public Volt PowerNetVoltage
		{
			get {
				return _powerNetVoltage.SI<Volt>();
			}
			set {
				_powerNetVoltage = value.Value();
			}
		}

		[JsonProperty("PowerNetVoltage")]
		private double _powerNetVoltage;

		/// <summary>
		/// Fuel Map Same One as used in Vecto.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		[JsonIgnore]
		public IFuelConsumptionMap FuelMap { get; set; }

		[JsonProperty("FuelMap")]
		public string FuelMapFile { get; set; }

		/// <summary>
		/// Fuel Density as used in Vecto.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		[JsonIgnore]
		public KilogramPerCubicMeter FuelDensity
		{
			get {
				return _fuelDensity.SI<KilogramPerCubicMeter>();
			}
			set {
				_fuelDensity = value.Value();
			}
		}

		[JsonProperty("FuelDensity")]
		private double _fuelDensity;
	}
}
