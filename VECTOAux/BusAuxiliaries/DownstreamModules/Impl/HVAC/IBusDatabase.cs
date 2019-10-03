using System.Collections.Generic;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public interface IBusDatabase
	{
		bool AddBus(IBus bus);

		List<IBus> GetBuses(string busModel, bool AsSelectList = false);

		bool Initialise(string busFileCSV);

		bool UpdateBus(int id, IBus bus);

		bool Save(string filepath);
	}
}
