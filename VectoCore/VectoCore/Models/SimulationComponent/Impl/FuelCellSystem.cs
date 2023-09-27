using System.Collections.Generic;
using System.Collections.ObjectModel;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class FuelCellSystem : StatefulVectoSimulationComponent<FuelCellSystem.State>, IElectricChargerPort
	{
		private readonly IList<FuelCell> _fuelCells;

		public class State
		{
			public Watt ActualPower { get; set; }
			public Watt TargetPower { get; set; }

		}
		private readonly IMileageCounter _mileageCounter;

		public FuelCellSystem(FuelCellSystemData fuelCellSystemData, IVehicleContainer databus) : base(databus)
		{
			_mileageCounter = databus.MileageCounter;
			_fuelCells = new List<FuelCell>();
			ModelData = fuelCellSystemData;
			Initialize();
		}

		public IReadOnlyCollection<FuelCell> FuelCells => new ReadOnlyCollection<FuelCell>(_fuelCells);

		private FuelCellSystemData ModelData { get; set; }

		#region Implementation of IElectricChargerPort

		public Watt Initialize()
		{
			var distance = _mileageCounter.Distance;
			var power = ModelData.ChargingPower(distance);
			PreviousState.TargetPower = power;
			PreviousState.ActualPower = power;
			return power;
		}





		public Watt PowerDemand(Second absTime, Second dt, Watt powerDemandEletricMotor, Watt auxPower, bool dryRun)
		{
			var targetPower = ModelData.ChargingPower(_mileageCounter.Distance);




			//Limit by gradient powerchange
			var limitedPower =
				GetLimitedPower(PreviousState.ActualPower, targetPower, dt, ModelData.GradientPowerChange);



			var fcCount = FuelCells.Count;

			var generatedPower = 0.SI<Watt>();
			foreach (var fc in FuelCells) {
				generatedPower += fc.Request(limitedPower / fcCount, dryRun);
			}

			if (!dryRun) {
				CurrentState.ActualPower = generatedPower;
				CurrentState.TargetPower = targetPower;
            }
			return targetPower;
		}


		public void AddFuelCell(FuelCell fuelCell)
		{
			_fuelCells.Add(fuelCell);

		}

		#endregion


		/// <summary>
		/// Returns the power wrt. to the GradientPowerChange
		/// </summary>
		/// <param name="previous"></param>
		/// <param name="current"></param>
		/// <param name="dt"></param>
		/// <param name="gradientPowerChange"></param>
		/// <returns></returns>
		public static Watt GetLimitedPower(Watt previous, Watt current, Second dt, WattPerSecond gradientPowerChange)
		{
			var delta = dt * gradientPowerChange;
			return current.LimitTo(previous - delta, previous + delta);
		} 

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.P_fuelCellSystem_target] = CurrentState.TargetPower;
			container[ModalResultField.P_fuelCellSystem_actual] = CurrentState.ActualPower;
			container[ModalResultField.Fc_fuelCellSystem_actual] = _fuelCells.Sum(x => x.PreviousState.FuelConsumption);


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