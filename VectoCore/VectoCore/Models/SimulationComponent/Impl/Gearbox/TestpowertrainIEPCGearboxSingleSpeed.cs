using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox
{
	// this class is intended to be used only in a testpowertrain and exposes certain internal properties
	// so that these can be set from outside without interfering with the class in the real implementation
    public class TestpowertrainIEPCGearboxSingleSpeed : IEPCGearboxSingleSpeed, ITestPowertrainTransmission
	{
		public TestpowertrainIEPCGearboxSingleSpeed(IVehicleContainer container, GearboxData modelData) : base(
			container, modelData, false)
		{
			if (!container.IsTestPowertrain) {
				throw new VectoException("This class shall not be used in a real powertrain!");
			}
		}

		#region Implementation of ITestPowertrainTransmission

		public GearshiftPosition SetGear
		{
			set => throw new NotImplementedException();
		}
		public GearshiftPosition SetNextGear {
			set => throw new NotImplementedException();
		}

		public bool SetDisengaged {
			set => throw new NotImplementedException();
		}

		public bool SetDisengageGearbox {
			set => throw new NotImplementedException();
		}

		public Second SetEngageTime {
			set => throw new NotImplementedException();
		}

		#endregion
	}
}