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

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
{
	public class GearData
	{
		public ShiftPolygon ShiftPolygon { get; internal set; }

		public TransmissionLossMap LossMap { get; internal set; }

		public FullLoadCurve FullLoadCurve { get; internal set; }

		public double Ratio { get; internal set; }

		public bool TorqueConverterActive { get; internal set; } // TODO: think about refactoring...

		// public double AverageEfficiency { get; internal set; }
	}
}