using System.Collections.Generic;

namespace TUGraz.VectoCore.Models.Simulation.DataBus
{
	public static class DataBusExtensions
	{
		public static IList<IElectricMotorInfo> GetElectricMotors(this IDataBus db)
		{
			var result = new List<IElectricMotorInfo>();

			foreach (var pos in db.PowertrainInfo.ElectricMotorPositions) {
				result.Add(db.ElectricMotorInfo(pos));
			}

			return result;
		}
	}
}