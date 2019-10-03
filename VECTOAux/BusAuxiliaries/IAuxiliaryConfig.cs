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

using TUGraz.VectoCore.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.PneumaticSystem;

namespace TUGraz.VectoCore.BusAuxiliaries {
	public interface IAuxiliaryConfig
	{
		// Vecto
		IVectoInputs VectoInputs { get; set; }

		// Electrical
		IElectricsUserInputsConfig ElectricalUserInputsConfig { get; set; }


		// Pneumatics
		IPneumaticUserInputsConfig PneumaticUserInputsConfig { get; set; }
		IPneumaticsAuxilliariesConfig PneumaticAuxillariesConfig { get; set; }

		// Hvac
		IHVACUserInputsConfig HvacUserInputsConfig { get; set; }

		bool ConfigValuesAreTheSameAs(AuxiliaryConfig other);


		// Persistance Functions
		bool Save(string filePath);
		bool Load(string filePath);
	}
}
