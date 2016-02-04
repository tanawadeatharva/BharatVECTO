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
using System.Diagnostics;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockDriver : VectoSimulationComponent, IDriver, IDrivingCycleOutPort, IDriverDemandInPort, IDriverInfo
	{
		private IDriverDemandOutPort _next;

		public RequestData LastRequest;

		public MockDriver(IVehicleContainer container) : base(container) {}

		protected override void DoWriteModalResults(IModalDataContainer container) {}

		protected override void DoCommitSimulationStep() {}


		public IDrivingCycleOutPort OutPort()
		{
			return this;
		}

		public IDriverDemandInPort InPort()
		{
			return this;
		}

		public IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
		{
			LastRequest = new RequestData { AbsTime = absTime, ds = ds, Gradient = gradient, TargetVelocity = targetVelocity };
			var acc = 0.SI<MeterPerSquareSecond>();
			var dt = 1.SI<Second>();
			return new ResponseSuccess { SimulationInterval = dt, Source = this };
		}

		public IResponse Request(Second absTime, Second dt, MeterPerSecond targetVelocity, Radian gradient)
		{
			LastRequest = new RequestData { AbsTime = absTime, dt = dt, Gradient = gradient, TargetVelocity = targetVelocity };
			var acc = 0.SI<MeterPerSquareSecond>();
			return new ResponseSuccess { SimulationInterval = dt, Source = this };
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient)
		{
			if (_next != null) {
				return _next.Initialize(vehicleSpeed, roadGradient);
			}

			return new ResponseSuccess { Source = this };
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient, MeterPerSquareSecond startAcceleration)
		{
			if (_next != null) {
				return _next.Initialize(vehicleSpeed, startAcceleration, roadGradient);
			}

			return new ResponseSuccess { Source = this };
		}

		public void Connect(IDriverDemandOutPort other)
		{
			_next = other;
		}

		public class RequestData
		{
			public Second AbsTime;
			public Meter ds;
			public Second dt;
			public MeterPerSecond TargetVelocity;
			public Radian Gradient;
		}

		//public MeterPerSquareSecond DriverAcceleration { get; set; }

		public bool VehicleStopped { get; set; }

		public DrivingBehavior DrivingBehavior
		{
			get { return DrivingBehavior.Accelerating; }
		}
	}
}