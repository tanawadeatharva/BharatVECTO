using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Auxiliaries
{
	public interface IAuxDemand
	{
		Watt PowerDemand(IDataBus dataBus);

		string AuxID { get; }
	}

	public class Conditioning : IAuxDemand
	{
		private readonly Watt electricPowerDemand;

		#region Implementation of IAuxDemand

		public string AuxID { get; }

		#endregion

		public Conditioning(VectoRunData.AuxData condAuxData)
		{
			if (condAuxData.ID != Constants.Auxiliaries.IDs.Cond) {
				throw new VectoException($"Invalid {nameof(condAuxData)}: ID must be {Constants.Auxiliaries.IDs.Cond}");
			}

			if (condAuxData.PowerDemandElectric == null) {
				throw new VectoException($"No electric powerdemand set for {condAuxData.ID}");
			}
			electricPowerDemand = condAuxData.PowerDemandElectric;
			AuxID = condAuxData.ID;
		}




		public Watt PowerDemand(IDataBus dataBus)
        {
			switch (dataBus.PowertrainInfo.VehicleArchitecutre) {
				case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.SerialHybridVehicle:
					return GetPEV_SHEV_PowerDemand(dataBus);

				case VectoSimulationJobType.ParallelHybridVehicle:
					return GetP_HEV_PowerDemand(dataBus);

				case VectoSimulationJobType.EngineOnlySimulation:
				case VectoSimulationJobType.IEPC_E:
				case VectoSimulationJobType.IEPC_S:
				case VectoSimulationJobType.IHPC:
				case VectoSimulationJobType.ConventionalVehicle:
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public Watt GetPEV_SHEV_PowerDemand(IDataBus dataBus)
		{
			var elInfo = GetElectricMotorInfo(dataBus);
			if (elInfo.EmOff)
			{
				return 0.SI<Watt>();
			}
			else {
				return electricPowerDemand;
			}
		}

		public Watt GetP_HEV_PowerDemand(IDataBus dataBus)
		{
			double xFactor = 0;

			var elInfo = GetElectricMotorInfo(dataBus);
			if (!elInfo.EmOff)
			{
				var iceInfo = dataBus.EngineInfo;
				var emPower = elInfo.ElectricMotorSpeed * elInfo.ElectricMotorTorque;
				var icePower = iceInfo.EngineSpeed * iceInfo.EngineTorque;

				xFactor = emPower.Abs() / (emPower.Abs() + icePower.Abs());
			}

			return electricPowerDemand * xFactor;
		}

		private IElectricMotorInfo GetElectricMotorInfo(IDataBus dataBus)
		{
			try
			{
				return dataBus.ElectricMotorInfo(dataBus.PowertrainInfo.ElectricMotorPositions.Single());
			}
			catch (Exception ex)
			{
				throw new VectoException("Only one electric motor position supported");
			}

		}
	}


}
