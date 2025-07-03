using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class WheelEndData
	{
		public NewtonMeter DeltaFrictionTorque { get; internal set; }

		public bool DisallowedVehicleClassHasMeasuredData { get; internal set; }
	}
}
