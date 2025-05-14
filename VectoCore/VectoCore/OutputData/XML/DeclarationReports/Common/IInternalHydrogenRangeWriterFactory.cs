using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
	public interface IInternalHydrogenRangeWriterFactory
	{
		IHydrogenRangeWriter GetHydrogenRangeWriter(VectoSimulationJobType jobType, bool offVehicleCharging, IList<IFuelProperties> fuels);
	}
}