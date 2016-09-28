using System;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class GearboxAuxiliary : EngineAuxiliary
	{
		public GearboxAuxiliary(IVehicleContainer container) : base(container) {}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			if (CurrentState.PowerDemands != null) {
				if (CurrentState.PowerDemands.ContainsKey("PTO_TRANSM"))
					container[ModalResultField.P_PTO_transm] = CurrentState.PowerDemands["PTO_TRANSM"];

				if (CurrentState.PowerDemands.ContainsKey("PTO_CONSUMER"))
					container[ModalResultField.P_PTO_consum] = CurrentState.PowerDemands["PTO_CONSUMER"];
			} else {
				container[ModalResultField.P_PTO_transm] = 0.SI<Watt>();

				if (container[ModalResultField.P_PTO_consum] == null || container[ModalResultField.P_PTO_consum] == DBNull.Value)
					container[ModalResultField.P_PTO_consum] = 0.SI<Watt>();
			}
		}
	}
}