using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Connector.Ports.Impl
{
	public abstract class AbstractRESSResponse : IRESSResponse
	{
		protected AbstractRESSResponse(object source)
		{
			Source = source;
		}

		public Second AbsTime { get; set; }

		public Second SimulationInterval { get; set; }

		public Watt MaxChargePower { get; set; }

		public Watt MaxDischargePower { get; set; }

		public Watt PowerDemand { get; set; }

		public Watt LossPower { get; set; }

		public double StateOfCharge { get; set; }
		public Volt InternalVoltage { get; set; }

		public object Source { get; }
	}


	public class RESSResponseSuccess : AbstractRESSResponse
	{
		public RESSResponseSuccess(object source) : base(source) { }

		
	}

	public class RESSOverloadResponse : AbstractRESSResponse
	{
		public RESSOverloadResponse(object source) : base(source) { }
	}

	public class RESSUnderloadResponse : AbstractRESSResponse
	{
		public RESSUnderloadResponse(object source) : base(source) { }
	}

	public class RESSDryRunResponse : AbstractRESSResponse
	{
		public RESSDryRunResponse(object source) : base(source) { }
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

		public IRESSResponse RESSResponse { get; set; }

		public Watt MaxPowerDrive =>
			CalculateMaxEMPower(RESSResponse != null && RESSResponse.MaxDischargePower != null
				? RESSResponse.MaxDischargePower
				: 0.SI<Watt>(), RESSResponse.InternalVoltage);

		public Watt MaxPowerDrag =>
			CalculateMaxEMPower(RESSResponse != null && RESSResponse.MaxChargePower != null
				? RESSResponse.MaxChargePower
				: 0.SI<Watt>(), RESSResponse.InternalVoltage);

		protected Watt CalculateMaxEMPower(Watt P_batMax, Volt U_int)
		{
            //Note: consider losses of electric system (cables, junction box) when calculating max drive power
            //      see ElectricSystem implementation...
			// P_Bat = P_EM + P_Chg - P_aux - P_Conn
            // I_est = P_EM / U_Int
            // P_Conn = I_est ^ 2 * R_Conn = (P_EM / U_Int) ^ 2 * R_Conn
            // P_EM_max = P_Bat_max - P_Chg + P_aux + P_Conn(P_EM)
            // P_EM_max - P_Conn = P_Bat_max - P_Chg + P_aux
            // P_EM_max - ((P_EM_max + P_Chg - P_aux) / U_Int) ^ 2 * R_Conn = P_Bat_max - P_Chg + P_aux
            // -P_EM_max^2 * R_Conn / U_Int^2 + P_EM_max - (P_Bat_max - P_Chg + P_aux) = 0
            // Px = P_Bat_max - P_Chg + P_aux

            var Px = P_batMax -
					(ChargingPower != null ? ChargingPower : 0.SI<Watt>()) +
					(AuxPower != null ? AuxPower : 0.SI<Watt>());
			
			if (ConnectionSystemResistance.IsEqual(0)) {
				return Px;
			}

            var a = -(ConnectionSystemResistance / U_int / U_int).Value();
			var b = 1.0;
			var c = -Px.Value();
			var solutions = VectoMath.QuadraticEquationSolver(a, b, c);
			return solutions.First().SI<Watt>();
        }

		public Watt RESSPowerDemand { get; set; }

		public object Source { get; }
		public Ohm ConnectionSystemResistance { get; set; }

		public override string ToString()
		{
			var t = GetType();
			return $"{t.Name}{{{t.GetProperties().Select(p => $"{p.Name}: {p.GetValue(this)}").Join()}}}";
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