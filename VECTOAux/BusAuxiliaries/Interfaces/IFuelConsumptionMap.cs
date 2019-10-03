using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.BusAuxiliaries.Interfaces {
	public interface IFuelConsumptionMap
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="torque"></param>
		/// <param name="angularVelocity"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		KilogramPerSecond GetFuelConsumption(NewtonMeter torque, PerSecond angularVelocity);
	}
}
