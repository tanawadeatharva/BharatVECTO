using System.Dynamic;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public interface ICrossWindCorrection
	{
		//SquareMeter EffectiveAirDragArea(MeterPerSecond x, IDataBus dataBus);

		void SetDataBus(IDataBus dataBus);

		CrossWindCorrectionMode CorrectionMode { get; }

		Watt AverageAirDragPowerLoss(MeterPerSecond v1, MeterPerSecond v2, Second dt);
	}
}