using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{

	public class FuelCellString
	{
		private readonly FuelCellStringMassFlowMap _fcStringMap;


		public FuelCellString(FuelCell[] fuelCells)
		{
			_fcStringMap 
		}


		public Watt Request(Watt requestedPower, bool dryRun)
		{
			//Dont know how to handle
			//if (!requestedPower.IsBetween(MinPower, MaxPower)) {
			//	throw new VectoException(string.Format("Requested power {0} is outside of fuelcell limits ({1}, {2}",
			//		requestedPower, MinPower, MaxPower));
			//}

			var fcCount = _fcStringMap.GetActiveFuelCellCount(requestedPower);


			var generatedPower = requestedPower.LimitTo(_fcStringMap.MinPower, _fcStringMap.MaxPower);



			return generatedPower;
		}



    }

}