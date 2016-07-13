using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class AngularGearData : SimulationComponentData
	{
		[ValidateObject] public TransmissionData AngularGear;

		public AngularGearType Type;
	}
}