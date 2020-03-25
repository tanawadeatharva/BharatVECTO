using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class HybridController :
		StatefulProviderComponent<HybridController.HybridControllerState, ITnOutPort, ITnInPort, ITnOutPort>,
		IHybridController, ITnInPort, ITnOutPort
	{
		private Dictionary<PowertrainPosition, ElectricMotorController> _electricMotorCtl;
		private HybridCtlShiftStrategy _shiftStrategy;

		public HybridController(IVehicleContainer container, IHybridControlStrategy strategy, IElectricSystem es,
			SwitchableClutch clutch) : base(container)
		{
			_electricMotorCtl = new Dictionary<PowertrainPosition, ElectricMotorController>();
			_shiftStrategy = new HybridCtlShiftStrategy(this);
		}

		public virtual void AddElectricMotor(PowertrainPosition pos)
		{
			if (_electricMotorCtl.ContainsKey(pos)) {
				throw new VectoException("Electric motor already registered as position {0}", pos);
			}
			_electricMotorCtl[pos] = new ElectricMotorController(this);
		}

		public virtual IElectricMotorControl ElectricMotorControl(PowertrainPosition pos)
		{
			return _electricMotorCtl[pos];
		}

		public virtual IShiftStrategy ShiftStrategy
		{
			get { return _shiftStrategy; }
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			return NextComponent.Request(absTime, dt, outTorque, outAngularVelocity);
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			return NextComponent.Initialize(outTorque, outAngularVelocity);
		}


		protected override void DoWriteModalResults(Second time, Second simulationInterval,
			IModalDataContainer container) { }

		///=======================================================================================

		public class HybridControllerState { }

		///=======================================================================================
		public class ElectricMotorController : IElectricMotorControl
		{
			protected HybridController _controller;

			public ElectricMotorController(HybridController hybridController)
			{
				_controller = hybridController;
			}

			public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque,
				PerSecond prevOutAngularVelocity,
				PerSecond currOutAngularVelocity, bool dryRun)
			{
				throw new System.NotImplementedException();
			}

			public NewtonMeter MaxDriveTorque(PerSecond avgSpeed, Second dt)
			{
				throw new System.NotImplementedException();
			}

			public NewtonMeter MaxDragTorque(PerSecond avgSpeed, Second dt)
			{
				throw new System.NotImplementedException();
			}
		}

		///=======================================================================================
		public class HybridCtlShiftStrategy : IShiftStrategy
		{
			protected HybridController _controller;

			public HybridCtlShiftStrategy(HybridController hybridController)
			{
				_controller = hybridController;
			}

			public ShiftPolygon ComputeDeclarationShiftPolygon(GearboxType gearboxType, int i,
				EngineFullLoadCurve engineDataFullLoadCurve,
				IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio,
				Meter dynamicTyreRadius)
			{
				throw new System.NotImplementedException();
			}

			public bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
				NewtonMeter inTorque,
				PerSecond inAngularVelocity, uint gear, Second lastShiftTime, IResponse response)
			{
				throw new System.NotImplementedException();
			}

			public uint InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
			{
				throw new System.NotImplementedException();
			}

			public uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				throw new System.NotImplementedException();
			}

			public void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
			{
				throw new System.NotImplementedException();
			}

			public IGearbox Gearbox { get; set; }
			public GearInfo NextGear { get; }
			public bool CheckGearshiftRequired { get; }

			public void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				throw new System.NotImplementedException();
			}

			public void WriteModalResults(IModalDataContainer container)
			{
				throw new System.NotImplementedException();
			}
		}
	}
}