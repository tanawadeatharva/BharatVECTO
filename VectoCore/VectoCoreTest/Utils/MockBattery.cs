using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;

namespace TUGraz.VectoCore.Tests.Utils {
	public class MockBattery : IBattery, IBatteryPort, IElectricAuxConnecor
	{
		public Volt InternalCellVoltage
		{
			get { return 640.SI<Volt>(); }
		}

		public IBatteryResponse Request(Second absTime, Second dt, Watt powerdemand, bool dryRun = false)
		{
			return new BatteryResponseSuccess(this)
			{
				MaxBatteryLoadDischarge = InternalCellVoltage * MaxCurrent,
				AbsTime = absTime,
				BatteryLoss = 0.SI<Watt>(),
				MaxBatteryLoadCharge = -InternalCellVoltage * MaxCurrent,
				BatteryPower = powerdemand,
				SimulationInterval = dt,
			};
		}

		public double StateOfCharge { get; set; }

		public Ampere MaxCurrent
		{
			get { return 375.SI<Ampere>(); }
		}

		public Watt MaxChargePower(Second dt)
		{
			throw new System.NotImplementedException();
		}

		public Watt MaxDischargePower(Second dt)
		{
			throw new System.NotImplementedException();
		}

		public IBatteryPort MainBatteryPort
		{
			get { return this; }
		}

		public IElectricAuxConnecor AuxBatteryPort()
		{
			return this;
		}

		public void Initialize(double initialSoC)
		{
			StateOfCharge = initialSoC;
		}

		public void Connect(IBatteryAuxPort aux)
		{
			throw new System.NotImplementedException();
		}

		#region Implementation of IBatteryChargeProvider

		public void Connect(IBatteryChargePort charger)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}