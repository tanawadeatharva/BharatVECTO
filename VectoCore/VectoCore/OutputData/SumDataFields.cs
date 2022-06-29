using System.Diagnostics.CodeAnalysis;

namespace TUGraz.VectoCore.OutputData
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "IdentifierTypo")]
	public static class SumDataFields
	{
		public const string INTERNAL_PREFIX = "INTERNAL";

		public const string SORT = INTERNAL_PREFIX + " Sorting";
		public const string JOB = "Job [-]";
		public const string INPUTFILE = "Input File [-]";
		public const string CYCLE = "Cycle [-]";
		public const string STATUS = "Status";
		public const string CURB_MASS = "Corrected Actual Curb Mass [kg]";
		public const string LOADING = "Loading [kg]";
		public const string PassengerCount = "Passenger count [-]";

		public const string VEHICLE_MANUFACTURER = "Vehicle manufacturer [-]";
		public const string VIN_NUMBER = "VIN number";
		public const string VEHICLE_MODEL = "Vehicle model [-]";

		public const string ENGINE_MANUFACTURER = "Engine manufacturer [-]";
		public const string ENGINE_MODEL = "Engine model [-]";
		public const string ENGINE_FUEL_TYPE = "Engine fuel type [-]";
		public const string ENGINE_WHTC_URBAN = "Engine WHTCUrban";
		public const string ENGINE_WHTC_RURAL = "Engine WHTCRural";
		public const string ENGINE_WHTC_MOTORWAY = "Engine WHTCMotorway";
		public const string ENGINE_BF_COLD_HOT = "Engine BFColdHot";
		public const string ENGINE_CF_REG_PER = "Engine CFRegPer";
		public const string ENGINE_ACTUAL_CORRECTION_FACTOR = "Engine actual CF";
		public const string ENGINE_RATED_POWER = "Engine rated power [kW]";
		public const string ENGINE_IDLING_SPEED = "Engine idling speed [rpm]";
		public const string ENGINE_RATED_SPEED = "Engine rated speed [rpm]";
		public const string ENGINE_DISPLACEMENT = "Engine displacement [ccm]";

		public const string ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER = "total RRC [-]";
		public const string ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER = "weighted RRC w/o trailer [-]";

		public const string GEARBOX_MANUFACTURER = "Gearbox manufacturer [-]";
		public const string GEARBOX_MODEL = "Gearbox model [-]";
		public const string GEARBOX_TYPE = "Gearbox type [-]";
		public const string GEAR_RATIO_FIRST_GEAR = "Gear ratio first gear [-]";
		public const string GEAR_RATIO_LAST_GEAR = "Gear ratio last gear [-]";

		public const string TORQUECONVERTER_MANUFACTURER = "Torque converter manufacturer [-]";
		public const string TORQUECONVERTER_MODEL = "Torque converter model [-]";

		public const string RETARDER_MANUFACTURER = "Retarder manufacturer [-]";
		public const string RETARDER_MODEL = "Retarder model [-]";
		public const string RETARDER_TYPE = "Retarder type [-]";

		public const string ANGLEDRIVE_MANUFACTURER = "Angledrive manufacturer [-]";
		public const string ANGLEDRIVE_MODEL = "Angledrive model [-]";
		public const string ANGLEDRIVE_RATIO = "Angledrive ratio [-]";

		public const string AXLE_MANUFACTURER = "Axle manufacturer [-]";
		public const string AXLE_MODEL = "Axle model [-]";
		public const string AXLE_RATIO = "Axle gear ratio [-]";

		public const string AUX_TECH_FORMAT = "Auxiliary technology {0} [-]";

		public const string HDV_CO2_VEHICLE_CLASS = "HDV CO2 vehicle class [-]";
		public const string TOTAL_VEHICLE_MASS = "Total vehicle mass [kg]";
		public const string CD_x_A_DECLARED = "Declared CdxA [m²]";

		public const string CD_x_A = "CdxA [m²]";

		public const string R_DYN = "r_dyn [m]";

		public const string CARGO_VOLUME = "Cargo Volume [m³]";
		public const string TIME = "time [s]";
		public const string DISTANCE = "distance [km]";
		public const string SPEED = "speed [km/h]";
		public const string ALTITUDE_DELTA = "altitudeDelta [m]";

		public const string FCMAP_H = "FC-Map{0} [g/h]";
		public const string FCMAP_KM = "FC-Map{0} [g/km]";
		public const string FCNCVC_H = "FC-NCVc{0} [g/h]";
		public const string FCNCVC_KM = "FC-NCVc{0} [g/km]";
		public const string FCWHTCC_H = "FC-WHTCc{0} [g/h]";
		public const string FCWHTCC_KM = "FC-WHTCc{0} [g/km]";

		public const string FCESS_H = "FC-ESS{0} [g/h]";
		public const string FCESS_KM = "FC-ESS{0} [g/km]";
		public const string FCESS_H_CORR = "FC-ESS_Corr{0} [g/h]";
		public const string FCESS_KM_CORR = "FC-ESS_Corr{0} [g/km]";
		public const string FCWHR_H_CORR = "FC-WHR_Corr{0} [g/h]";
		public const string FCWHR_KM_CORR = "FC-WHR_Corr{0} [g/km]";
		public const string FC_HEV_SOC_H = "FC-SoC{0} [g/h]";
		public const string FC_HEV_SOC_KM = "FC-SoC{0} [g/km]";
		public const string FC_HEV_SOC_CORR_H = "FC-SoC_Corr{0} [g/h]";
		public const string FC_HEV_SOC_CORR_KM = "FC-SoC_Corr{0} [g/km]";


		public const string FC_BusAux_PS_CORR_H = "FC-BusAux_PS_Corr{0} [g/h]";
		public const string FC_BusAux_PS_CORR_KM = "FC-BusAux_PS_Corr{0} [g/km]";
		public const string FC_BusAux_ES_CORR_H = "FC-BusAux_ES_Corr{0} [g/h]";
		public const string FC_BusAux_ES_CORR_KM = "FC-BusAux_ES_Corr{0} [g/km]";
		public const string FC_AUXHTR_H = "FC-BusAux_AuxHeater{0} [g/h]";
		public const string FC_AUXHTR_KM = "FC-BusAux_AuxHeater{0} [g/km]";
		public const string FC_AUXHTR_H_CORR = "FC-BusAux_AuxHeater_Corr{0} [g/h]";
		public const string FC_AUXHTR_KM_CORR = "FC-BusAux_AuxHeater_Corr{0} [g/km]";


		public const string FCFINAL_H = "FC-Final{0} [g/h]";
		public const string FCFINAL_KM = "FC-Final{0} [g/km]";
		public const string FCFINAL_LITERPER100KM = "FC-Final{0} [l/100km]";
		public const string FCFINAL_LITERPER100TKM = "FC-Final{0} [l/100tkm]";
		public const string FCFINAL_LiterPer100M3KM = "FC-Final{0} [l/100m³km]";
		public const string FCFINAL_LiterPer100PassengerKM = "FC-Final{0} [l/100Pkm]";

		public const string ElectricEnergyConsumptionPerKm = "EC_el_final [kWh/km]";

		public const string CO2_KM = "CO2 [g/km]";
		public const string CO2_TKM = "CO2 [g/tkm]";
		public const string CO2_M3KM = "CO2 [g/m³km]";
		public const string CO2_PKM = "CO2 [g/Pkm]";

		public const string P_WHEEL_POS = "P_wheel_in_pos [kW]";
		public const string P_WHEEL = "P_wheel_in [kW]";
		public const string P_FCMAP_POS = "P_fcmap_pos [kW]";
		public const string P_FCMAP = "P_fcmap [kW]";

		public const string E_FORMAT = "E_{0} [kWh]";
		public const string E_AUX_FORMAT = "E_aux_{0} [kWh]";
		public const string E_AUX = "E_aux_sum [kWh]";

		public const string E_AUX_EL_HV = "E_aux_el(HV) [kWh]";

		public const string E_ICE_START = "E_ice_start [kWh]";
		public const string NUM_ICE_STARTS = "ice_starts [-]";
		public const string K_ENGLINE = "k_engline{0} [g/kWh]";
		public const string K_VEHLINE = "k_vehline{0} [g/kWh]";

		public const string E_WHR_EL = "E_WHR_el [kWh]";
		public const string E_WHR_MECH = "E_WHR_mech [kWh]";

		public const string E_BusAux_PS_corr = "E_BusAux_PS_corr [kWh]";
		public const string E_BusAux_ES_mech_corr = "E_BusAux_ES_mech_corr [kWh]";
		public const string E_BusAux_AuxHeater = "E_BusAux_AuxhHeater [kWh]";

		public const string E_AIR = "E_air [kWh]";
		public const string E_ROLL = "E_roll [kWh]";
		public const string E_GRAD = "E_grad [kWh]";
		public const string E_VEHICLE_INERTIA = "E_vehi_inertia [kWh]";
		public const string E_POWERTRAIN_INERTIA = "E_powertrain_inertia [kWh]";
		public const string E_WHEEL = "E_wheel [kWh]";
		public const string E_BRAKE = "E_brake [kWh]";
		public const string E_GBX_LOSS = "E_gbx_loss [kWh]";
		public const string E_SHIFT_LOSS = "E_shift_loss [kWh]";
		public const string E_AXL_LOSS = "E_axl_loss [kWh]";
		public const string E_RET_LOSS = "E_ret_loss [kWh]";
		public const string E_TC_LOSS = "E_tc_loss [kWh]";
		public const string E_ANGLE_LOSS = "E_angle_loss [kWh]";
		public const string E_CLUTCH_LOSS = "E_clutch_loss [kWh]";
		public const string E_FCMAP_POS = "E_fcmap_pos [kWh]";
		public const string E_FCMAP_NEG = "E_fcmap_neg [kWh]";

		public const string AirGenerated = "BusAux PS air generated [Nl]";
		public const string AirConsumed = "BusAux PS air consumed [Nl]";
		public const string E_PS_CompressorOff = "E_PS_compressorOff [kWh]";
		public const string E_PS_CompressorOn = "E_PS_compressorOn [kWh]";

		public const string E_BusAux_ES_generated = "E_BusAux_ES_generated [kWh]";
		public const string E_BusAux_ES_consumed = "E_BusAux_ES_consumed [kWh]";
		public const string Delta_E_BusAux_Battery = "ΔE_BusAux_Bat [kWh]";

		public const string E_BusAux_HVAC_Mech = "E_BusAux_HVAC_mech [kWh]";
		public const string E_BusAux_HVAC_El = "E_BusAux_HVAC_el [kWh]";

		public const string E_AUX_ESS_missing = "E_aux_ESS_missing [kWh]";

		public const string SPECIFIC_FC = "Specific FC{0} [g/kWh] wheel pos.";

		public const string ACC = "a [m/s^2]";
		public const string ACC_POS = "a_pos [m/s^2]";
		public const string ACC_NEG = "a_neg [m/s^2]";

		public const string ACC_TIMESHARE = "AccelerationTimeShare [%]";
		public const string DEC_TIMESHARE = "DecelerationTimeShare [%]";
		public const string CRUISE_TIMESHARE = "CruiseTimeShare [%]";
		public const string STOP_TIMESHARE = "StopTimeShare [%]";

		public const string MAX_SPEED = "max. speed [km/h]";
		public const string MAX_ACCELERATION = "max. acc [m/s²]";
		public const string MAX_DECELERATION = "max. dec [m/s²]";
		public const string AVG_ENGINE_SPEED = "n_eng_avg [rpm]";
		public const string MAX_ENGINE_SPEED = "n_eng_max [rpm]";
		public const string NUM_GEARSHIFTS = "gear shifts [-]";
		public const string ICE_FULL_LOAD_TIME_SHARE = "ICE max. Load time share [%]";
		public const string ICE_OFF_TIME_SHARE = "ICE off time share [%]";
		public const string COASTING_TIME_SHARE = "CoastingTimeShare [%]";
		public const string BRAKING_TIME_SHARE = "BrakingTimeShare [%]";

		public const string TIME_SHARE_PER_GEAR_FORMAT = "Gear {0} TimeShare [%]";

		public const string NUM_AXLES_DRIVEN = "Number axles vehicle driven [-]";
		public const string NUM_AXLES_NON_DRIVEN = "Number axles vehicle non-driven [-]";
		public const string NUM_AXLES_TRAILER = "Number axles trailer [-]";

		public const string TCU_MODEL = "ShiftStrategy";

		public const string VEHICLE_FUEL_TYPE = "Vehicle fuel type [-]";
		public const string AIRDRAG_MODEL = "AirDrag model [-]";
		public const string SLEEPER_CAB = "Sleeper cab [-]";
		public const string DECLARED_RRC_AXLE1 = "Declared RRC axle 1 [-]";
		public const string DECLARED_FZISO_AXLE1 = "Declared FzISO axle 1 [N]";
		public const string DECLARED_RRC_AXLE2 = "Declared RRC axle 2 [-]";
		public const string DECLARED_FZISO_AXLE2 = "Declared FzISO axle 2 [N]";
		public const string DECLARED_RRC_AXLE3 = "Declared RRC axle 3 [-]";
		public const string DECLARED_FZISO_AXLE3 = "Declared FzISO axle 3 [N]";
		public const string DECLARED_RRC_AXLE4 = "Declared RRC axle 4 [-]";
		public const string DECLARED_FZISO_AXLE4 = "Declared FzISO axle 4 [N]";
		public const string ADAS_TECHNOLOGY_COMBINATION = "ADAS technology combination [-]";

		public const string PTO_TECHNOLOGY = "PTOShaftsGearWheels";

		//public const string PTO_OTHER_ELEMENTS = "PTOOtherElements";

		public const string ENGINE_CERTIFICATION_NUMBER = "Engine certification number";
		public const string AVERAGE_ENGINE_EFFICIENCY = "Average engine efficiency [-]";
		public const string TORQUE_CONVERTER_CERTIFICATION_NUMBER = "TorqueConverter certification number";
		public const string TORQUE_CONVERTER_CERTIFICATION_METHOD = "Torque converter certification option";

		public const string AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITH_LOCKUP =
			"Average torque converter efficiency with lockup [-]";

		public const string AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITHOUT_LOCKUP =
			"Average torque converter efficiency w/o lockup [-]";

		public const string GEARBOX_CERTIFICATION_NUMBER = "Gearbox certification number";
		public const string GEARBOX_CERTIFICATION_METHOD = "Gearbox certification option";
		public const string AVERAGE_GEARBOX_EFFICIENCY = "Average gearbox efficiency [-]";
		public const string RETARDER_CERTIFICATION_NUMBER = "Retarder certification number";
		public const string RETARDER_CERTIFICATION_METHOD = "Retarder certification option";
		public const string ANGLEDRIVE_CERTIFICATION_NUMBER = "Angledrive certification number";
		public const string ANGLEDRIVE_CERTIFICATION_METHOD = "Angledrive certification option";
		public const string AVERAGE_ANGLEDRIVE_EFFICIENCY = "Average angledrive efficiency [-]";
		public const string AXLEGEAR_CERTIFICATION_NUMBER = "Axlegear certification number";
		public const string AXLEGEAR_CERTIFICATION_METHOD = "Axlegear certification method";
		public const string AVERAGE_AXLEGEAR_EFFICIENCY = "Average axlegear efficiency [-]";
		public const string AIRDRAG_CERTIFICATION_NUMBER = "AirDrag certification number";
		public const string AIRDRAG_CERTIFICATION_METHOD = "AirDrag certification option";

		public const string AVERAGE_POS_ACC = "a_avg_acc";

		public const string E_EM_DRIVE_FORMAT = "E_EM_{0}_drive [kWh]";
		public const string E_EM_GENERATE_FORMAT = "E_EM_{0}_gen [kWh]";
		public const string ETA_EM_DRIVE_FORMAT = "η_EM_{0}_drive";
		public const string ETA_EM_GEN_FORMAT = "η_EM_{0}_gen";

		public const string E_EM_Mot_DRIVE_FORMAT = "E_EM_{0}-em_drive [kWh]";
		public const string E_EM_Mot_GENERATE_FORMAT = "E_EM_{0}-em_gen [kWh]";
		public const string ETA_EM_Mot_DRIVE_FORMAT = "η_EM_{0}-em_drive";
		public const string ETA_EM_Mot_GEN_FORMAT = "η_EM_{0}-em_gen";

		public const string EM_AVG_SPEED_FORMAT = "n_EM_{0}-em_avg [rpm]";

		public const string E_EM_OFF_Loss_Format = "E_EM_{0}_off_loss [kWh]";
		public const string E_EM_LOSS_TRANSM_FORMAT = "E_EM_{0}_transm_loss [kWh]";
		public const string E_EM_Mot_LOSS_FORMAT = "E_EM_{0}-em_loss [kWh]";
		public const string E_EM_LOSS_FORMAT = "E_EM_{0}_loss [kWh]";
		public const string E_EM_OFF_TIME_SHARE = "EM {0} off time share [%]";

		public const string REESS_CAPACITY = "REESS Capacity";
		public const string REESS_StartSoC = "REESS Start SoC [%]";
		public const string REESS_EndSoC = "REESS End SoC [%]";
		public const string REESS_DeltaEnergy = "ΔE_REESS [kWh]";

		public const string E_REESS_LOSS = "E_REESS_loss [kWh]";
		public const string E_REESS_T_chg = "E_REESS_T_chg [kWh]";
		public const string E_REESS_T_dischg = "E_REESS_T_dischg [kWh]";
		public const string E_REESS_int_chg = "E_REESS_int_chg [kWh]";
		public const string E_REESS_int_dischg = "E_REESS_int_dischg [kWh]";

		public const string IEPC_AVG_SPEED_FORMAT = "n_{0}-em_avg [rpm]";
		public const string E_IEPC_DRIVE_FORMAT = "E_{0}_drive [kWh]";
		public const string E_IEPC_GENERATE_FORMAT = "E_{0}_gen [kWh]";

		public const string ETA_IEPC_DRIVE_FORMAT = "η_{0}_drive";
		public const string ETA_IEPC_GEN_FORMAT = "η_{0}_gen";
		public const string E_IEPC_OFF_Loss_Format = "E_{0}_off_loss [kWh]";
		public const string E_IEPC_LOSS_FORMAT = "E_{0}_loss [kWh]";
		public const string E_IEPC_OFF_TIME_SHARE = "{0} off time share [%]";


	}
}