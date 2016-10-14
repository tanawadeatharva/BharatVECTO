using System;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class IdleControllerSwitcher : IIdleController
	{
		private readonly IIdleController _idleController;
		private readonly PTOCycleController _ptoController;
		private IIdleController _currentController;

		public IdleControllerSwitcher(IIdleController idleController, PTOCycleController ptoController)
		{
			_idleController = idleController;
			_ptoController = ptoController;

			// default state is idleController
			_currentController = _idleController;
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			return _currentController.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			throw new InvalidOperationException(string.Format("{0} cannot initialize.", GetType().FullName));
		}

		public ITnOutPort RequestPort
		{
			set
			{
				_idleController.RequestPort = value;
				_ptoController.RequestPort = value;
			}
		}

		public void Reset()
		{
			_idleController.Reset();
			_ptoController.Reset();
			_currentController = _idleController;
		}

		public void ActivatePTO()
		{
			_currentController = _ptoController;
		}

		public void ActivateIdle()
		{
			_currentController = _idleController;
		}

		public Second GetNextCycleTime()
		{
			return _ptoController.GetNextCycleTime();
		}

		public void CommitSimulationStep(IModalDataContainer container)
		{
			_ptoController.CommitSimulationStep(container);
		}

		public Second Duration
		{
			get
			{
				if (_ptoController != null)
					return _ptoController.Duration;
				else {
					return 0.SI<Second>();
				}
			}
		}
	}
}