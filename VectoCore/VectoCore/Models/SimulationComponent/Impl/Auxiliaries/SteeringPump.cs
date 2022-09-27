using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Auxiliaries
{
    public class SteeringPumpSystem : IAuxDemand
	{
		private bool ConnectedToREESS = false;

		private List<VectoRunData.AuxData> fullyElectricSp = new List<VectoRunData.AuxData>();
		private List<VectoRunData.AuxData> mechanicalSp = new List<VectoRunData.AuxData>();

		


		public SteeringPumpSystem(VectoRunData.AuxData[] spAuxData)
		{
			if (spAuxData.Any(aux => aux.ID != Constants.Auxiliaries.IDs.SteeringPump)) {
				throw new VectoException($"Only steering pumps can be used in {nameof(SteeringPumpSystem)}");
			}
			
			ConnectedToREESS = spAuxData.All(aux => aux.ConnectToREESS);
			if (spAuxData.Any(aux => aux.ConnectToREESS != ConnectedToREESS)) {
				throw new VectoException("All steering pumps must be connected to either REESS or Engine");
			}

			if (spAuxData.Any(auxData => auxData.DemandType != AuxiliaryDemandType.Constant)) {
				throw new VectoException("Only constant auxiliaries supported");
			}

			if (ConnectedToREESS && spAuxData.Any(aux => !aux.IsFullyElectric)) {
				throw new VectoException("Only fully electric steering pumps can be connected to REESS");
			}

			foreach (var aux in spAuxData) {
				if (aux.IsFullyElectric) {
					fullyElectricSp.Add(aux);
				} else {
					mechanicalSp.Add(aux);
				}
			}
		}


		#region Implementation of IAuxDemand
		/// <summary>
		/// Returns electrical power demand if the SteeringPumpSystem is connected to REESS
		/// </summary>
		/// <param name="dataBus"></param>
		/// <returns></returns>
		public Watt PowerDemand(IDataBus dataBus)
		{
			var powerDemand = 0.SI<Watt>();
			if (!dataBus.VehicleInfo.VehicleStopped) {
				powerDemand += fullyElectricSp
					.Sum(aux => ConnectedToREESS ? aux.PowerDemandElectric : aux.PowerDemandMech).DefaultIfNull(0);
			}

			powerDemand += mechanicalSp.Sum(aux => ConnectedToREESS ? aux.PowerDemandElectric : aux.PowerDemandMech).DefaultIfNull(0);
			return powerDemand;
		}

		public string AuxID => Constants.Auxiliaries.IDs.SteeringPump;

		#endregion
	}

	public interface IAuxDemand
	{
		Watt PowerDemand(IDataBus dataBus);

		string AuxID { get; }
	}
}
