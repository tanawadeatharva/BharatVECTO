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
		private readonly Watt _electricPowerDemand;
		private readonly IEPTO _epto;


		private bool EPTOOn(IDataBus dataBus)
		{
			return _epto?.EPTOOn(dataBus) ?? false;
		}

		#region Implementation of IAuxDemand

		public string AuxID { get; }

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="condAuxData"></param>
		/// <param name="eptoCycleController">needed in case an epto is present in the vehicle</param>
		/// <exception cref="VectoException"></exception>

		public Conditioning(VectoRunData.AuxData condAuxData, IEPTO epto = null)
		{
			if (condAuxData.ID != Constants.Auxiliaries.IDs.Cond) {
				throw new VectoException($"Invalid {nameof(condAuxData)}: ID must be {Constants.Auxiliaries.IDs.Cond}");
			}

			if (condAuxData.PowerDemandElectric == null) {
				throw new VectoException($"No electric powerdemand set for {condAuxData.ID}");
			}
			_electricPowerDemand = condAuxData.PowerDemandElectric;
			_epto = epto;
			AuxID = condAuxData.ID;
		}

		public Watt PowerDemand(IDataBus dataBus)
        {
			switch (dataBus.PowertrainInfo.VehicleArchitecutre) {
				case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.IEPC_E:
				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.IEPC_S:
				case VectoSimulationJobType.FCHV:
				case VectoSimulationJobType.FCHV_IEPC:
					return GetPEV_SHEV_PowerDemand(dataBus);
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.IHPC:
					return GetP_HEV_PowerDemand(dataBus);
				case VectoSimulationJobType.EngineOnlySimulation:
				case VectoSimulationJobType.ConventionalVehicle:
				default:
					throw new ArgumentOutOfRangeException($"{nameof(dataBus)}");
			}
		}

		public Watt GetPEV_SHEV_PowerDemand(IDataBus dataBus)
		{
			var oneEmOn = dataBus.GetElectricMotors().Any(elInfo => !elInfo.EmOff);
			if (oneEmOn || EPTOOn(dataBus)) {
				return _electricPowerDemand;
			} else {
				return 0.SI<Watt>();
			}
		}

		public Watt GetP_HEV_PowerDemand(IDataBus dataBus)
		{
			double xFactor = 0;

			var elInfo = dataBus.GetElectricMotors().Single();
			if (!elInfo.EmOff)
			{
				var iceInfo = dataBus.EngineInfo;
				var emPower = elInfo.ElectricMotorSpeed * elInfo.ElectricMotorTorque;
				var icePower = iceInfo.EngineSpeed * iceInfo.EngineTorque;
				if (!(emPower + icePower).IsEqual(0)) {
					xFactor = emPower.Abs() / (emPower.Abs() + icePower.Abs());
				}
			}

			return _electricPowerDemand * xFactor;
		}
	}


}
