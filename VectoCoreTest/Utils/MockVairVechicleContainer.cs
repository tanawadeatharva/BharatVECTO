using System.Collections.Generic;
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
	public class MockVairVechicleContainer : IVehicleContainer
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