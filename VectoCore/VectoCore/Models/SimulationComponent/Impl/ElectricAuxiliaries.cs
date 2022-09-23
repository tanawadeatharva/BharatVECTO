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
		private IDictionary<string, Watt> _powerDemands = new Dictionary<string, Watt>();

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
			
			
			var sum = 0.SI<Watt>();
			foreach (var aux in _auxData) {
				var powerDemand = 0.SI<Watt>(); 
				if (aux.DemandType == AuxiliaryDemandType.Constant) {
					powerDemand = aux.PowerDemandElectric;
				} else if(aux.DemandType == AuxiliaryDemandType.Dynamic) {
					powerDemand = aux.PowerDemandDataBusFunc(DataBus);
				}


				if (!dryRun) {
					_powerDemands[aux.ID] = powerDemand;
				}

				sum += powerDemand;
			}

			return sum;
		}

		#endregion

		

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
            foreach (var aux in _auxData)
            {
				
				container[_auxColumnName[aux.ID]] = _powerDemands[aux.ID];
				
			}
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			_powerDemands.Clear();
		}

		#endregion
	}
}

