using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Connector.Ports.Impl
{
	public abstract class AbstractBatteryResponse : IBatteryResponse
	{
		protected AbstractBatteryResponse(object source)
		{
			Source = source;
		}

		public Second AbsTime { get; set; }

		public Second SimulationInterval { get; set; }

		public Watt MaxBatteryLoadCharge { get; set; }

		public Watt MaxBatteryLoadDischarge { get; set; }

		public Watt BatteryPower { get; set; }

		public Watt BatteryLoss { get; set; }

		public double StateOfCharge { get; set; }

		public object Source { get; }
	}


	public class BatteryResponseSuccess : AbstractBatteryResponse
	{
		public BatteryResponseSuccess(object source) : base(source) { }

		
	}

	public class BatteryOverloadResponse : AbstractBatteryResponse
	{
		public BatteryOverloadResponse(object source) : base(source) { }
	}

	public class BatteryUnderloadResponse : AbstractBatteryResponse
	{
		public BatteryUnderloadResponse(object source) : base(source) { }
	}

	public class BatteryDryRunResponse : AbstractBatteryResponse
	{
		public BatteryDryRunResponse(object source) : base(source) { }
	}



	public abstract class AbstractElectricSystemResponse : IElectricSystemResponse
	{
		protected AbstractElectricSystemResponse(object source)
		{
			Source = source;
		}

		public Second AbsTime { get; set; }

		public Second SimulationInterval { get; set; }

		public Watt AuxPower { get; set; }

		public Watt ConsumerPower { get; set; }

		public Watt ChargingPower { get; set; }

		public IBatteryResponse BatteryResponse { get; set; }

		public Watt MaxPowerDrive
		{
			get
			{
				return (BatteryResponse != null && BatteryResponse.MaxBatteryLoadDischarge != null ? BatteryResponse.MaxBatteryLoadDischarge : 0.SI<Watt>()) -
						(ChargingPower != null ? ChargingPower : 0.SI<Watt>()) +
						(AuxPower != null ? AuxPower : 0.SI<Watt>());
			}
		}

		public Watt MaxPowerDrag
		{
			get
			{
				return (BatteryResponse != null && BatteryResponse.MaxBatteryLoadCharge != null ? BatteryResponse.MaxBatteryLoadCharge : 0.SI<Watt>()) -
						(ChargingPower != null ? ChargingPower : 0.SI<Watt>()) +
						(AuxPower != null ? AuxPower : 0.SI<Watt>());
			}
		}

		public object Source { get; }

		public override string ToString()
		{
			var t = GetType();
			return string.Format("{0}{{{1}}}", t.Name,
				string.Join(", ", t.GetProperties().Select(p => string.Format("{0}: {1}", p.Name, p.GetValue(this)))));
		}
	}

	public class ElectricSystemResponseSuccess : AbstractElectricSystemResponse
	{
		public ElectricSystemResponseSuccess(object source) : base(source) { }
	}

	public class ElectricSystemOverloadResponse : AbstractElectricSystemResponse
	{
		public ElectricSystemOverloadResponse(object source) : base(source) { }
	}

	public class ElectricSystemUnderloadResponse : AbstractElectricSystemResponse
	{
		public ElectricSystemUnderloadResponse(object source) : base(source) { }
	}

	public class ElectricSystemDryRunResponse : AbstractElectricSystemResponse
	{
		public ElectricSystemDryRunResponse(object source) : base(source) { }

	}
}