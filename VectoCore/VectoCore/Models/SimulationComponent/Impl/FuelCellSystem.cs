using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class FuelCellSystem : StatefulVectoSimulationComponent<FuelCellSystem.State>, IElectricChargerPort
	{
		public class State
		{
			public Watt Power { get; set; }

		}
		private readonly IMileageCounter _mileageCounter;

		public FuelCellSystem(FuelCellSystemData fuelCellSystemData, IVehicleContainer databus) : base(databus)
		{
			_mileageCounter = databus.MileageCounter;
			
			ModelData = fuelCellSystemData;
		}

		private FuelCellSystemData ModelData { get; set; }

		#region Implementation of IElectricChargerPort

		public Watt Initialize()
		{
			return 0.SI<Watt>();
		}

		public Watt PowerDemand(Second absTime, Second dt, Watt powerDemandEletricMotor, Watt auxPower, bool dryRun)
		{
			var power = ModelData.ChargingPower(_mileageCounter.Distance);
			CurrentState.Power = power;
			




			return power;
		}

		#endregion

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.P_fuelCellSystem] = CurrentState.Power;
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}

		protected override bool DoUpdateFrom(object other)
		{
			if (other is FuelCellSystem fc) {
				PreviousState = fc.PreviousState;
				return true;
			}

			return false;
		}

		#endregion
	}


}