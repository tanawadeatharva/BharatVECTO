using System.Collections.Generic;
using System.Collections.ObjectModel;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class FuelCellSystem : StatefulVectoSimulationComponent<FuelCellSystem.State>, IFuelCellPort
	{
		private readonly IList<FuelCellString> _fuelCellStrings;

		public class State
		{
			public Watt Power { get; set; }

			public FuelCellSystemShareMap.FuelCellShare Share { get; set; }
		}
		private readonly IMileageCounter _mileageCounter;
		private readonly FuelCellSystemShareMap _fuelCellShareMap;

		public FuelCellSystem(FuelCellSystemData fuelCellSystemData, IVehicleContainer databus) : base(databus)
		{
			_fuelCellShareMap = fuelCellSystemData.FuelCellShareMap;
			_mileageCounter = databus.MileageCounter;
			_fuelCellStrings = new List<FuelCellString>();
			ModelData = fuelCellSystemData;
			Initialize();
		}

		public IReadOnlyCollection<FuelCellString> FuelCellStrings => new ReadOnlyCollection<FuelCellString>(_fuelCellStrings);

		private FuelCellSystemData ModelData { get; set; }

		#region Implementation of IElectricChargerPort

		public Watt Initialize()
		{

			//var distance = _mileageCounter.Distance;
			var power = ModelData.FuelCellPowerMap.InitPower;
			PreviousState.Power = power;
			return power;
		}

		public Watt PowerDemand(Second absTime, Second dt, Watt maxPower, bool dryRun)
		{
			var targetPower = ModelData.ChargingPower(_mileageCounter.Distance).LimitTo(0.SI<Watt>(), maxPower);

			var shareResult = _fuelCellShareMap.Lookup(targetPower, PreviousState?.Share);

			var generatedPower = 0.SI<Watt>();

			generatedPower += _fuelCellStrings[0].Request(targetPower * shareResult.Share.ShareA, dryRun, dt);

			if (_fuelCellStrings.Count > 1) {
				generatedPower += _fuelCellStrings[1].Request(targetPower * shareResult.Share.ShareB, dryRun, dt);
			}

			if (!dryRun) {
				CurrentState.Share = shareResult.Share;
				CurrentState.Power = generatedPower;
			}

			return generatedPower;
        }


		public void AddFuelCellString(FuelCellString fuelCellString)
		{
			_fuelCellStrings.Add(fuelCellString);
		}

		#endregion


		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.P_FCSystem] = CurrentState.Power;
			container[ModalResultField.FC_FCSystem] = _fuelCellStrings.Sum(x => x.PreviousState.FuelConsumption);
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