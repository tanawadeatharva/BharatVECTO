using System.Collections.Generic;
using System.Linq;

namespace TUGraz.VectoCore.Models.Simulation.DataBus
{
	public static class DataBusExtensions
	{
		public static IList<IElectricMotorInfo> GetElectricMotors(this IDataBus db)
		{
			return db.ElectricMotorsInfo;
		}
	}
}