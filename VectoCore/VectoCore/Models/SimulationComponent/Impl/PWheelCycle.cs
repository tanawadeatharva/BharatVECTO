using System.Collections.Generic;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// Driving Cycle for the PWheel driving cycle.
	/// </summary>
	public class PWheelCycle : PowertrainDrivingCycle, IDriverInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PWheelCycle"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="cycle">The cycle.</param>
		/// <param name="axleRatio">The axle ratio.</param>
		/// <param name="gearRatios"></param>
		public PWheelCycle(IVehicleContainer container, DrivingCycleData cycle, double axleRatio,
			IDictionary<uint, double> gearRatios) : base(container, cycle)
		{
			// just to ensure that null-gear has ratio 1
			gearRatios[0] = 1;

			foreach (var entry in Data.Entries) {
				entry.WheelAngularVelocity = entry.AngularVelocity / (axleRatio * gearRatios[entry.Gear]);
				entry.Torque = entry.PWheel / entry.WheelAngularVelocity;
			}
		}

		public override IResponse Request(Second absTime, Second dt)
		{
			if (RightSample.Current == null) {
				return new ResponseCycleFinished { Source = this };
			}

			// interval exceeded
			if ((absTime + dt).IsGreater(RightSample.Current.Time)) {
				return new ResponseFailTimeInterval {
					AbsTime = absTime,
					Source = this,
					DeltaT = RightSample.Current.Time - absTime
				};
			}

			return DoHandleRequest(absTime, dt, LeftSample.Current.WheelAngularVelocity);
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.P_wheel_in] = LeftSample.Current.PWheel;
			base.DoWriteModalResults(container);
		}

		#region IDriverInfo

		/// <summary>
		/// True if the angularVelocity at the wheels is 0.
		/// </summary>
		public bool VehicleStopped
		{
			get { return false; }
		}

		/// <summary>
		/// Always Driving.
		/// </summary>
		public DrivingBehavior DriverBehavior
		{
			get { return DrivingBehavior.Driving; }
		}

		public MeterPerSquareSecond DriverAcceleration
		{
			get { return 0.SI<MeterPerSquareSecond>(); }
		}

		#endregion
	}
}