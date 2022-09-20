using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{


	public class ElectricAuxiliaries : IElectricAuxPort
	{
		private IEnumerable<VectoRunData.AuxData> _auxData;

		#region Implementation of IElectricAuxPort


		public ElectricAuxiliaries(IVehicleContainer container)
		{
			
			DataBus = container;

		}

		private IVehicleContainer DataBus { get; set; }

		public Watt Initialize()
		{
			_auxData = DataBus.RunData.Aux.Where(aux => aux.MissionType == DataBus.RunData.Mission.MissionType);
			foreach (var auxData in _auxData) {
				//auxData.
			}

			return 0.SI<Watt>();
		}

		public Watt PowerDemand(Second absTime, Second dt, bool dryRun)
		{
			var left = DataBus.DrivingCycleInfo.CycleData.LeftSample;
			var sum = 0.SI<Watt>();
			foreach (var auxData in _auxData) {
				if (auxData.DemandType == AuxiliaryDemandType.Constant) {
					sum += auxData.PowerDemand;
				} else {
					sum += auxData.PowerDemandFunc(left);
				}
			}

			return sum;
		}

		#endregion
	}
}

