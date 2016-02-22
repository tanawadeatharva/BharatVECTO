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

using System.Linq;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Connector.Ports.Impl
{
	public abstract class AbstractResponse : IResponse
	{
		public Second SimulationInterval { get; set; }

		public Meter SimulationDistance { get; set; }

		public MeterPerSquareSecond Acceleration { get; set; }

		public Watt EnginePowerRequest { get; set; }

		public Watt AuxiliariesPowerDemand { get; set; }

		public Watt ClutchPowerRequest { get; set; }

		public Watt GearboxPowerRequest { get; set; }

		public Watt AxlegearPowerRequest { get; set; }

		public Watt WheelsPowerRequest { get; set; }

		public Watt VehiclePowerRequest { get; set; }

		public Watt BrakePower { get; set; }

		public Second AbsTime { get; set; }

		public PerSecond EngineSpeed { get; set; }

		public object Source { get; set; }

		public override string ToString()
		{
			var t = GetType();
			return string.Format("{0}{{{1}}}", t.Name,
				", ".Join(t.GetProperties().Select(p => string.Format("{0}: {1}", p.Name, p.GetValue(this)))));
		}
	}

	/// <summary>
	/// Response when the Cycle is finished.
	/// </summary>
	public class ResponseCycleFinished : AbstractResponse {}

	/// <summary>
	/// Response when a request was successful.
	/// </summary>
	public class ResponseSuccess : AbstractResponse {}

	/// <summary>
	/// Response when the request resulted in an engine or gearbox overload. 
	/// </summary>
	public class ResponseOverload : AbstractResponse
	{
		public ResponseOverload() {}
		public Watt Delta { get; set; }
		public double Gradient { get; set; }
	}

	/// <summary>
	/// Response when the request resulted in an engine under-load. 
	/// </summary>
	public class ResponseUnderload : AbstractResponse
	{
		public ResponseUnderload() {}
		public Watt Delta { get; set; }
		public double Gradient { get; set; }
	}

	/// <summary>
	/// Response when the Speed Limit was exceeded.
	/// </summary>
	public class ResponseSpeedLimitExceeded : AbstractResponse {}

	/// <summary>
	/// Response when the request should have another time interval.
	/// </summary>
	public class ResponseFailTimeInterval : AbstractResponse
	{
		public Second DeltaT { get; set; }
	}

	public class ResponseDrivingCycleDistanceExceeded : AbstractResponse
	{
		public Meter MaxDistance { get; set; }
	}

	internal class ResponseDryRun : AbstractResponse
	{
		public Watt DeltaFullLoad { get; set; }
		public Watt DeltaDragLoad { get; set; }
	}

	internal class ResponseGearShift : AbstractResponse {}

	internal class ResponseEngineSpeedTooLow : ResponseDryRun {}
}