using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Simulation.DataBus
{
	public interface IBatteryInfo
	{
		Volt InternalCellVoltage { get; }

		double StateOfCharge { get; }

		WattSecond StoredEnergy { get; }

		//Ampere MaxCurrent { get; }

		Watt MaxChargePower(Second dt);

		Watt MaxDischargePower(Second dt);
	}
}