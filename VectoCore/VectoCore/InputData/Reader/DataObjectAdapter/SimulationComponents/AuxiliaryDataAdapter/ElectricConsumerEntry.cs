using System.Diagnostics;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter
{
	[DebuggerDisplay("{Current}  (Base: {BaseVehicle}, active ESS standstill: {ActiveDuringEngineStopStandstill} active ESS driving: {ActiveDuringEngineStopDriving}")]
	public class ElectricConsumerEntry
	{

		public ElectricConsumerEntry()
		{
			ActiveDuringEngineStopStandstill = true;
			ActiveDuringEngineStopDriving = true;
		}

		public bool ActiveDuringEngineStopDriving { get; set; }

		public bool ActiveDuringEngineStopStandstill { get; set; }

		public bool BaseVehicle { get; set; }
		public Ampere Current { get; set; }
	}
}