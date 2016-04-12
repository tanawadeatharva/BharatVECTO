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

using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockVairVehicleContainer : IVehicleContainer
	{
		// only CycleData Lookup is set / accessed...

		public uint Gear { get; private set; }
		public MeterPerSecond StartSpeed { get; private set; }
		public MeterPerSquareSecond StartAcceleration { get; private set; }
		public FullLoadCurve GearFullLoadCurve { get; private set; }
		public PerSecond EngineSpeed { get; private set; }

		public Watt EngineStationaryFullPower(PerSecond angularSpeed)
		{
			throw new System.NotImplementedException();
		}

		public Watt EngineDragPower(PerSecond angularSpeed)
		{
			throw new System.NotImplementedException();
		}

		public PerSecond EngineIdleSpeed { get; private set; }
		public PerSecond EngineRatedSpeed { get; private set; }
		public MeterPerSecond VehicleSpeed { get; private set; }
		public Kilogram VehicleMass { get; private set; }
		public Kilogram VehicleLoading { get; private set; }
		public Kilogram TotalMass { get; private set; }
		public Meter Distance { get; private set; }

		public bool ClutchClosed(Second absTime)
		{
			throw new System.NotImplementedException();
		}

		public Watt BrakePower { get; set; }
		public Meter CycleStartDistance { get; private set; }

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
		{
			throw new System.NotImplementedException();
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			throw new System.NotImplementedException();
		}

		public bool VehicleStopped { get; private set; }
		public DrivingBehavior DrivingBehavior { get; private set; }
		public CycleData CycleData { get; set; }
		public ExecutionMode ExecutionMode { get; set; }
		public IModalDataContainer ModalData { get; private set; }
		public VectoRunData RunData { get; private set; }

		public ISimulationOutPort GetCycleOutPort()
		{
			throw new System.NotImplementedException();
		}

		public VectoRun.Status RunStatus { get; set; }

		public void AddComponent(VectoSimulationComponent component)
		{
			throw new System.NotImplementedException();
		}

		public void CommitSimulationStep(Second time, Second simulationInterval)
		{
			throw new System.NotImplementedException();
		}

		public void FinishSimulation()
		{
			throw new System.NotImplementedException();
		}
	}
}