using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class FuelCell : StatefulVectoSimulationComponent<FuelCell.FuelCellState>
	{
		public class FuelCellState
		{
			public Watt Power { get; set; }
			public bool On { get; set; }
		}

		private FuelCellData ModelData;



		public FuelCellData.FuelCellId Id { get; private set; }


		public FuelCell(FuelCellData fcData, IVehicleContainer dataBus) : base(null) //provide null here, when registering the component the Id is accessed but is not set in the base constructor
		{
			DataBus = dataBus;
			ModelData = fcData;
			Id = fcData.Id;
			CurrentState.On = true;
			dataBus.AddComponent(this);
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			var power = CurrentState.Power;
			container[ModalResultField.P_fuelCell, Id.ToString()] = power;

			var h2 = ModelData.MassFlowMap.Lookup(CurrentState.Power);
			container[ModalResultField.H2, Id.ToString()] = h2;
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}

		public Watt Request(Watt requestedPower)
		{
			CurrentState.Power = requestedPower;


			return requestedPower;
		}

		protected override bool DoUpdateFrom(object other)
		{
			if (other is FuelCell fc) {
				PreviousState = fc.PreviousState;
				return true;
			}

			return false;
		}

		#endregion
	}
}