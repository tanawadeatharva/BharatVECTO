namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
{
	public interface IHVACSteadyStateModel
	{
		/// <summary>
		/// 	Initialised Values From Map
		/// 	</summary>
		/// 	<param name="filePath"></param>
		/// 	<param name="message"></param>
		/// 	<returns>True if successfull, and False if not.</returns>
		/// 	<remarks></remarks>
		bool SetValuesFromMap(string filePath, ref string message);

		/// <summary>
		/// 	HVAC Mechanical Load Power  (W)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		float HVACMechanicalLoadPowerWatts { get; set; }

		/// <summary>
		/// 	HVAC Electrical Load Power (W)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		float HVACElectricalLoadPowerWatts { get; set; }

		/// <summary>
		/// 	HVAC Fuelling (L/H)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns>Liters per hour</returns>
		/// 	<remarks></remarks>
		float HVACFuellingLitresPerHour { get; set; }
	}
}
