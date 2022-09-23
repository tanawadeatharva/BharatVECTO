using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{

	/// <summary>
	/// Container Class for Auxiliaries which are connected to the DCDC system.
	/// </summary>
	public class ElectricAuxiliaries : VectoSimulationComponent, IElectricAuxPort
	{
		private IEnumerable<VectoRunData.AuxData> _auxData;

		private IDictionary<string, string> _auxColumnName = new Dictionary<string, string>();

		#region Implementation of IElectricAuxPort


		public ElectricAuxiliaries(IVehicleContainer container) : base(container)// : base(container)
		{
			
			VehicleContainer = container;

		}

		private IVehicleContainer VehicleContainer { get; set; }

		public Watt Initialize()
		{
			
			_auxData = VehicleContainer.RunData.Aux.Where(aux => aux.MissionType == VehicleContainer.RunData.Mission.MissionType);
			foreach (var auxData in _auxData) {
				var name = $"P_{auxData.ID}_el";
				_auxColumnName.Add(auxData.ID, name); //use column name as ID
				VehicleContainer.AddAuxiliary(name, name);
			}

			return 0.SI<Watt>();
		}

		public Watt PowerDemand(Second absTime, Second dt, bool dryRun)
		{
			var left = VehicleContainer.DrivingCycleInfo.CycleData.LeftSample;
			
			var sum = 0.SI<Watt>();
			foreach (var auxData in _auxData) {
				if (auxData.DemandType == AuxiliaryDemandType.Constant) {
					sum += auxData.PowerDemandElectric;
				} else {
					sum += auxData.PowerDemandMechFunc(left);
					throw new NotImplementedException("only constant electric auxiliaries implemented");
				}
			}

			return sum;

			//VehicleContainer.EngineInfo.EngineOn
			VehicleContainer.ElectricMotorInfo(PowertrainPosition.BatteryElectricE3)
		}

		#endregion

		

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
            foreach (var aux in _auxData)
            {
				if (aux.DemandType == AuxiliaryDemandType.Constant) {
					container[_auxColumnName[aux.ID]] = aux.PowerDemandElectric;
				}
			}
        }

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}

		#endregion
	}
}

