using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {


	public class SimplePowertrainContainer : VehicleContainer, IDriverInfo, ISimpleVehicleContainer
	{
		public SimplePowertrainContainer(VectoRunData runData, ISimplePowertrainBuilder ptBuilder, int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN) : base(runData, null, null, ptBuilder)
		{
			RunData = runData;
			AxleNumber = axleNumber;
		}

		//public IDriverDemandOutPort VehiclePort => (VehicleInfo as Vehicle)?.OutPort();
		public int AxleNumber { get; private set; }

		public ITnOutPort GearboxOutPort => (GearboxesInfo.FirstOrDefault(x => x.AxleNumber == AxleNumber) as IGearbox)?.OutPort();

		public IGearbox GearboxCtlTest => GearboxesInfo.First(x => x.AxleNumber == AxleNumber) as IGearbox;

		public override Second AbsTime => 0.SI<Second>();

		public override IDriverInfo DriverInfo => base.DriverInfo ?? this;

		public override bool IsTestPowertrain => true;

		#region Implementation of IDriverInfo

		public DrivingBehavior DriverBehavior => DrivingBehavior.Driving;

		public DrivingAction DrivingAction => DrivingAction.Accelerate;

		public MeterPerSquareSecond DriverAcceleration => 0.SI<MeterPerSquareSecond>();
		public PCCStates PCCState => PCCStates.OutsideSegment;
		public MeterPerSecond NextBrakeTriggerSpeed => 0.SI<MeterPerSecond>();
		public MeterPerSecond ApplyOverspeed(MeterPerSecond targetSpeed) => targetSpeed;

		#endregion

		public Dictionary<PowertrainPosition, IElectricMotorInfo> ElectricMotors => base.EMs.ToDictionary(x => x.Position, x => x);

		public void UpdateComponents(IDataBus realContainer) => UpdateComponentsInternal(realContainer);
	}
}