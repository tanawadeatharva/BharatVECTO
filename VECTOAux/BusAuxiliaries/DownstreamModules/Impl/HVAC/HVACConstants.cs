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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class HVACConstants : IHVACConstants
	{
		[JsonProperty("FuelDensity")]
		private readonly double _fuelDensity;
		[JsonProperty("DieselGCVJperGram")]
		private readonly double _dieselGcvJperGram = 44800;

		public HVACConstants()
		{
			_fuelDensity = 835; // .SI(Of KilogramPerCubicMeter)()
		}

		public HVACConstants(KilogramPerCubicMeter fuelDensitySingle)
		{
			_fuelDensity = fuelDensitySingle.Value();
		}


		[JsonIgnore]
		public JoulePerKilogramm DieselGCVJperGram
		{
			get {
				return _dieselGcvJperGram.SI(Unit.SI.Joule.Per.Gramm).Cast<JoulePerKilogramm>();
			}
		}

		[JsonIgnore()]
		public KilogramPerCubicMeter FuelDensity
		{
			get {
				return _fuelDensity.SI<KilogramPerCubicMeter>();
			}
		}

		[JsonIgnore()]
		public double FuelDensityAsGramPerLiter
		{
			get {
				return _fuelDensity * 1000;
			}
		}
	}
}
