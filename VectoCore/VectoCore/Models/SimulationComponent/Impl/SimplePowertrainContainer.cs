using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class SimplePowertrainContainer : VehicleContainer, IDriverInfo
	{
		public SimplePowertrainContainer(VectoRunData runData, IModalDataContainer modData = null) : base(runData.ExecutionMode, modData)
		{
			RunData = runData;
		}

		public IDriverDemandOutPort VehiclePort
		{
			get { return (VehicleInfo as Vehicle)?.OutPort(); }
		}

		public ITnOutPort GearboxOutPort
		{
			get { return (GearboxInfo as IGearbox)?.OutPort(); }
		}

		public IGearbox GearboxCtlTest
		{
			get { return GearboxInfo as IGearbox; }
		}

		public override Second AbsTime { get { return 0.SI<Second>(); } }

		public override IDriverInfo DriverInfo { get { return base.DriverInfo ?? this; } }

		public override bool IsTestPowertrain
		{
			get { return true; }
		}

		#region Implementation of IDriverInfo

		public DrivingBehavior DriverBehavior
		{
			get { return  DrivingBehavior.Driving; }
		}

		public DrivingAction DrivingAction
		{
			get { return DrivingAction.Accelerate; }
		}

		public MeterPerSquareSecond DriverAcceleration
		{
			get { return 0.SI<MeterPerSquareSecond>(); }
		}

		#endregion
	}
}