using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl.FuelCell
{
	public partial class FuelCellPreRunPostprocessor
	{
		public class FCCalcEntry
		{

			public Second Time { get; set; }
			public Second dt { get; set; }
			public Meter Distance { get; set; }


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
			/// Fuel Cell power limited to Min/Max power, zero => fuel cell is off
			/// </summary>
			public Watt P_FC { get; set; }


			/// <summary>
			/// Remaining power that should be provided by battery
			/// </summary>
			public Watt P_Bat_T => P_FC + P_el_dem;


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

			public double SoC { get; set; }

			public bool CanChangeFCPower { get; set; }



			public Watt FCPowerFinal { get; set; }

			/// <summary>
			/// Resulting power provided by battery considering <see cref="P_Bat_T_Final"/>
			/// </summary>
			public Watt P_Bat_T_Final => FCPowerFinal + P_el_dem;


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
											"P_el_dem_corr [kW] " +
											"P_FC_raw [kW], " +
											"P_FC [kW], " +
											"P_Bat_T [kW], " +
											"P_Bat_loss [kW], " +
											"P_FC_corr [kW], " +
											"delta_Power_corr [kW], " +
											"P_REESS_int [kW], " +
											"SoC [%], " +
											"CanChangeFCPower, " +
											"FCPowerFinal [kW]," +
											"delta_P_FCS [kW]";
			#region Overrides of Object

			public override string ToString()
			{
				return $"{Time.ToXMLFormat()}," +
						$"{dt.ToXMLFormat()}, " +
						$"{Distance.ToXMLFormat()}, " +
						$"{P_el_dem?.ConvertToKiloWatt()?.ToXMLFormat()}, " +
						$"{P_el_dem_corr?.ConvertToKiloWatt()?.ToXMLFormat()}" +
						$"{P_FC_raw?.ConvertToKiloWatt()?.ToXMLFormat()}, " +
						$"{P_FC?.ConvertToKiloWatt()?.ToXMLFormat()}," +
						$"{P_Bat_T?.ConvertToKiloWatt()?.ToXMLFormat()}, " +
						$"{P_Bat_loss?.ConvertToKiloWatt()?.ToXMLFormat()}, " +
						$"{P_FC_corr?.ConvertToKiloWatt()?.ToXMLFormat()}, " +
						$"{delta_Power_corr?.ConvertToKiloWatt()?.ToXMLFormat()}, " +
						$"{P_REESS_int?.ConvertToKiloWatt()?.ToXMLFormat()}, " +
						$"{SoC.ToXMLFormat()}, " +
						$"{CanChangeFCPower}, " +
						$"{FCPowerFinal?.ConvertToKiloWatt()?.ToXMLFormat()}, " +
						$"{delta_P_FCS?.ConvertToKiloWatt()?.ToXMLFormat()}";
			}

			#endregion
		}
	}
}