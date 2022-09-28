using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Auxiliaries;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{

	/// <summary>
	/// Container Class for Auxiliaries which are connected to the DCDC system.
	/// </summary>
	public class ElectricAuxiliaries : VectoSimulationComponent, IElectricAuxPort
	{
		private IDictionary<string, Func<IDataBus, Watt>> _auxData = new Dictionary<string, Func<IDataBus, Watt>>();
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
			
			foreach (var auxId in _auxData.Keys) {
				var name = $"P_{auxId}_el";
				_auxColumnName.Add(auxId, name); //use column name as ID
				VehicleContainer.AddAuxiliary(name, name);
			}


			return 0.SI<Watt>();
		}

		public void AddAuxiliary(VectoRunData.AuxData aux)
		{
			if (aux.DemandType == AuxiliaryDemandType.Constant) {
				_auxData.Add(aux.ID, (dataBus) => aux.PowerDemandElectric);
			}else if (aux.DemandType == AuxiliaryDemandType.Dynamic) {
				_auxData.Add(aux.ID, (dataBus) => aux.PowerDemandDataBusFunc(dataBus, false));
			}
		}

		public void AddAuxiliary(IAuxDemand aux)
		{
			_auxData.Add(aux.AuxID, aux.PowerDemand);
		}

		public void AddAuxiliaries(IEnumerable<VectoRunData.AuxData> auxData)
		{
			foreach (var aux in auxData) {
				AddAuxiliary(aux);
			}
		}

		public Watt PowerDemand(Second absTime, Second dt, bool dryRun)
		{

			var auxiliarieIgnoredDuringVehicleStop = new[] {
				Constants.Auxiliaries.IDs.Fan,
			};
			var sum = 0.SI<Watt>();
			
			foreach (var aux in _auxData) {

				var powerDemand = 0.SI<Watt>();
				if (DataBus.VehicleInfo.VehicleStopped) {
					powerDemand += auxiliarieIgnoredDuringVehicleStop.Contains(aux.Key)
						? aux.Value(DataBus)
						: 0.SI<Watt>();
				} else {
					powerDemand += aux.Value(DataBus);
				}
				


				if (!dryRun) {
					_powerDemands[aux.Key] = powerDemand;
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
				
				container[_auxColumnName[aux.Key]] = _powerDemands[aux.Key];
				
			}
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			_powerDemands.Clear();
		}

		#endregion
	}
}

