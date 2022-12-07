using Microsoft.VisualBasic;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Auxiliaries;
using Constants = TUGraz.VectoCore.Configuration.Constants;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public interface IEPTO
	{
		bool EPTOOn(IDataBus dataBus);
	}

	public class EPTO : IAuxDemand, IEPTO
	{
		private readonly IPTOCycleController _ptoCycleController;

		public EPTO(IPTOCycleController cycleController)
		{
			_ptoCycleController = cycleController;
		}

		#region Implementation of IAuxDemand
		public Watt PowerDemand(IDataBus dataBus)
		{
			if (dataBus.DrivingCycleInfo.PTOActive) {
				return _ptoCycleController.CycleData.LeftSample.PTOElectricalPowerDemand ?? 0.SI<Watt>();
			}

			return 0.SI<Watt>();
		}

		public bool EPTOOn(IDataBus dataBus)
		{
			return !PowerDemand(dataBus).IsEqual(0);
		}


		public string AuxID => Constants.Auxiliaries.IDs.PTOConsumer;

		#endregion
	}
}