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

using System;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class DriverData
	{
		public enum DriverMode
		{
			Off,
			Overspeed,
			EcoRoll,
		}


		public VectoRunData.StartStopData StartStop;
		public OverSpeedEcoRollData OverSpeedEcoRoll;
		public LACData LookAheadCoasting;
		public AccelerationCurveData AccelerationCurve;

		public static DriverMode ParseDriverMode(string mode)
		{
			return mode.Replace("-", "").Parse<DriverMode>();
		}

		public class OverSpeedEcoRollData
		{
			public DriverMode Mode;
			public MeterPerSecond MinSpeed;
			public MeterPerSecond OverSpeed;
			public MeterPerSecond UnderSpeed;
		}

		public class LACData
		{
			public bool Enabled;
			public MeterPerSquareSecond Deceleration;
			public MeterPerSecond MinSpeed;
		}
	}
}