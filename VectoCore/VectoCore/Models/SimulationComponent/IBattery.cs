using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.DataBus;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IElectricSystemInfo : IBatteryInfo
	{


		Watt ElectricAuxPower { get; }

		Watt ChargePower { get; }

		Watt BatteryPower { get; }

		Watt ConsumerPower { get; }
	}

	public interface IElectricSystem : IElectricSystemInfo
	{
		IElectricSystemResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun = false);
	}

	public interface IBattery : IBatteryProvider, IBatteryInfo
	{

	}
}