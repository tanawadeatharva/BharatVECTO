using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent;

namespace TUGraz.VectoCore.Models.Connector.Ports.Impl
{
	public interface IBatteryProvider
	{
		IBatteryPort MainBatteryPort { get; }
	}

	public interface IBatteryPort
	{

		void Initialize(double initialSoC);

		/// <summary>
		/// Convention: positive powerdemand charges the battery, 
		///             negative powerdemand discharges
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="powerDemand"></param>
		/// <param name="dryRun"></param>
		/// <returns></returns>
		IBatteryResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun = false);

	}

	public interface IElectricAuxConnecor
	{
		void Connect(IBatteryAuxPort aux);
	}

	public interface IElectricChargerConnector
	{
		void Connect(IBatteryChargePort charger);
	}

	public interface IBatteryConnector
	{
		void Connect(IBattery battery);
	}

	public interface IBatteryAuxPort
	{
		Watt Initialize();

		Watt PowerDemand(Second absTime, Second dt, bool dryRun);
	}

	public interface IBatteryChargePort
	{
		Watt Initialize();

		Watt PowerDemand(Second absTime, Second dt, Watt powerDemandEletricMotor, Watt auxPower, bool dryRun);
	}
}