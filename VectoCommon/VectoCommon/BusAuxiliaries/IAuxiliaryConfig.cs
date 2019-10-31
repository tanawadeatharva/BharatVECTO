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

namespace TUGraz.VectoCommon.BusAuxiliaries {
	public interface IAuxiliaryConfig
	{
		// Electrical
		IElectricsUserInputsConfig ElectricalUserInputsConfig { get;  }


		// Pneumatics
		IPneumaticUserInputsConfig PneumaticUserInputsConfig { get;  }
		IPneumaticsConsumersDemand PneumaticAuxillariesConfig { get;  }

		ISSMInputs SSMInputs { get; }

		IActuationsMap ActuationsMap { get; }

		bool ConfigValuesAreTheSameAs(IAuxiliaryConfig other);


		string Cycle { get; }

		IVehicleData VehicleData { get; }

		IFuelConsumptionMap FuelMap { get; }

		ISignals Signals { get; }

		// Persistance Functions
			//bool Save(string filePath);
			//bool Load(string filePath);
		}
}
