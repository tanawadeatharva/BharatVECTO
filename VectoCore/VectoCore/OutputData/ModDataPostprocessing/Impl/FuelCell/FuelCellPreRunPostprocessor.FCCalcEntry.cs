using System;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl.FuelCell
{
	public partial class FuelCellPreRunPostprocessor
	{
		public class FCCalcEntry
		{
			public FCCalcEntry(Watt p_max_charging, Watt p_max_discharge, PreRunEntry preRunEntry)
			{
				this.P_max_charging = p_max_charging;
				this.P_max_discharge = p_max_discharge;
				this.preRunEntry = preRunEntry;
			}
			/// <summary>
			/// Limits the Battery Charging Power
			/// </summary>
			public Watt P_max_charging { get; private set; }

			/// <summary>
			/// Limits the Battery Discharing Power
			/// </summary>
			public Watt P_max_discharge { get; private set; }

            public PreRunEntry preRunEntry { get; private set; }

			public Second t => preRunEntry.t; 

			public Second dt => preRunEntry.dt;

			public Meter s => preRunEntry.s;

			/// <summary>
			/// P_ES_t, electric demand from powertrain + aux.
			/// </summary>
			public Watt P_el_dem { get; set; }

			/// <summary>
			/// P_ES_t, electric demand from powertrain + aux, without recuperation if battery is full
			/// </summary>
			public Watt P_el_dem_corr { get; set; }

			/// <summary>
			/// Fuel Cell power without any corrections
			/// </summary>
			public Watt P_FC_raw { get; set; }

			/// <summary>
			/// Fuel Cell power calculated based on <see cref="P_el_dem_corr"/>
			/// </summary>
			public Watt P_FC_raw_corr { get; set; }

			/// <summary>
            /// Fuel Cell power limited to Min/Max power, zero => fuel cell is off
            /// </summary>
            public Watt P_FC { get; set; }

			/// <summary>
			/// Remaining power that should be provided by battery
			/// </summary>
			public Watt P_Bat_T => P_FC + P_el_dem.LimitTo(P_max_discharge, P_max_charging);

			/// <summary>
			/// Battery losses from P_Bat_T
			/// </summary>
			public Watt P_Bat_loss { get; set; }
			
			/// <summary>
			/// Power provided by fuel cell including battery losses
			/// </summary>
			public Watt P_FC_corr => P_FC.IsEqual(0) ? P_FC : P_FC + P_Bat_loss;

			/// <summary>
			/// Fuel Cell power demand
			/// </summary>
			public Watt delta_Power_corr => P_FC_corr + P_el_dem;

			/// <summary>
			/// SoC of the tracing infinity battery
			/// </summary>
			public double SoC { get; set; } = -1;

			/// <summary>
			/// Soc of the "real" battery
			/// </summary>
			public double Real_SoC { get; set; } = -1;

			public bool CanChangeFCPower { get; set; }

			public Watt FCPowerFinal { get; set; }

			/// <summary>
			/// Resulting power provided by battery considering <see cref="P_Bat_T_Final"/>
			/// </summary>
			public Watt P_Bat_T_Final => (FCPowerFinal + P_el_dem).LimitTo(P_max_discharge, P_max_charging);

			///// <summary>
			///// <see cref="P_Bat_T_Final"/> limited by MaxCharge and MaxDischargePower of the infinity battery, not considering SoC/>
			///// </summary>
			//public Watt P_Bat_T_Final_limited { get; set; }

			public Watt P_REESS_int { get; set; } = 0.SI<Watt>();

			/// <summary>
			/// 
			/// </summary>
			public Watt delta_P_FCS { get; set; } = 0.SI<Watt>();

			public static string Header => "Time [s]," +
											" dt [s], " +
											"Distance [m], " +
											"P_el_dem [kW]," +
											"P_el_dem_corr [kW], " +
											"P_FC_raw [kW], " +
											"P_FC [kW], " +
											"P_Bat_T [kW], " +
											"P_Bat_loss [kW], " +
											"P_FC_corr [kW], " +
											"delta_Power_corr [kW], " +
											"P_REESS_int [kW], " +
											"SoC_inf [%], " +
											"SoC_real [%], " +
											"CanChangeFCPower, " +
											"FCPowerFinal [kW]," +
											"delta_P_FCS [kW],";
			#region Overrides of Object
			public override string ToString()
			{
				return $"{t.ToXMLFormat()}," +
						$"{dt.ToXMLFormat()}, " +
						$"{s.ToXMLFormat()}, " +
						$"{P_el_dem?.ConvertToKiloWatt()?.ToXMLFormat() ?? "-"}, " +
						$"{P_el_dem_corr?.ConvertToKiloWatt()?.ToXMLFormat() ?? "-"}, " +
						$"{P_FC_raw?.ConvertToKiloWatt()?.ToXMLFormat() ?? "-"}, " +
						$"{P_FC?.ConvertToKiloWatt()?.ToXMLFormat() ?? "-"}," +
						$"{P_Bat_T?.ConvertToKiloWatt()?.ToXMLFormat() ?? "-"}, " +
						$"{P_Bat_loss?.ConvertToKiloWatt()?.ToXMLFormat() ?? "-"}, " +
						$"{P_FC_corr?.ConvertToKiloWatt()?.ToXMLFormat() ?? "-"}, " +
						$"{delta_Power_corr?.ConvertToKiloWatt()?.ToXMLFormat() ?? "-"}, " +
						$"{P_REESS_int?.ConvertToKiloWatt()?.ToXMLFormat() ?? "-"}, " +
						$"{(SoC * 100).ToXMLFormat() ?? "-"}, " +
						$"{(Real_SoC * 100).ToXMLFormat() ?? "-"}, " +
                        (CanChangeFCPower ? "1" : "0") + "," +
						$"{FCPowerFinal?.ConvertToKiloWatt()?.ToXMLFormat() ?? "-"}, " +
						$"{delta_P_FCS?.ConvertToKiloWatt()?.ToXMLFormat() ?? "-"},";
			}
			#endregion
			/// <summary>
			/// Windowsize that was used to create this entry
			/// </summary>
			public Meter WindowSize { get; set; }


		}





		public class PreRunEntry
		{
			/// <summary>
			/// P_Terminal_ES
			/// </summary>
			public Watt P_es_T { get; set; }

			public Meter s { get; set; }

			public Meter ds { get; set; }

			public Second dt { get; set; }

			public Second t { get; set; }

			public PreRunEntry()
			{

			}
			public PreRunEntry(PreRunEntry s)
			{
				this.dt = s.dt;
				this.ds = s.ds;
				this.s = s.s;
				this.t = s.t;
				this.P_es_T = s.P_es_T;
			}
		}
	}
}