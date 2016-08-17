/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockGearbox : VectoSimulationComponent, IGearbox, ITnInPort, ITnOutPort, IClutchInfo
	{
		private ITnOutPort _outPort;

		public MockGearbox(IVehicleContainer cockpit) : base(cockpit) {}

		public ITnInPort InPort()
		{
			return this;
		}

		public ITnOutPort OutPort()
		{
			return this;
		}

		public uint Gear { get; set; }

		public MeterPerSecond StartSpeed
		{
			get { return 2.SI<MeterPerSecond>(); }
		}

		public MeterPerSquareSecond StartAcceleration
		{
			get { return 0.6.SI<MeterPerSquareSecond>(); }
		}

		public FullLoadCurve GearFullLoadCurve
		{
			get { return null; }
		}

		public Watt GearboxLoss()
		{
			return 0.SI<Watt>();
		}

		public void Connect(ITnOutPort other)
		{
			_outPort = other;
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity, bool dryRun = false)
		{
			if (_outPort != null) {
				if (Gear > 0) {
					return _outPort.Request(absTime, dt, torque, angularVelocity, dryRun);
				}
				return _outPort.Request(absTime, dt, 0.SI<NewtonMeter>(), null, dryRun);
			}
			throw new NotImplementedException();
		}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			if (_outPort != null) {
				return _outPort.Initialize(torque, angularVelocity);
			}
			throw new NotImplementedException();
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			// nothing to write
		}

		protected override void DoCommitSimulationStep() {}

		public bool ClutchClosed(Second absTime)
		{
			return true;
		}

		public void Connect(IAuxPort aux)
		{
			throw new NotImplementedException();
		}
	}
}