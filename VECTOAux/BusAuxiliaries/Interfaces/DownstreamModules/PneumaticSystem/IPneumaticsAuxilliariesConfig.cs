public interface IPneumaticsAuxilliariesConfig
{
	double OverrunUtilisationForCompressionFraction { get; set; }
	double BrakingWithRetarderNIperKG { get; set; }
	double BrakingNoRetarderNIperKG { get; set; }
	double BreakingPerKneelingNIperKGinMM { get; set; }
	double PerDoorOpeningNI { get; set; }
	double PerStopBrakeActuationNIperKG { get; set; }
	double AirControlledSuspensionNIperMinute { get; set; }
	double AdBlueNIperMinute { get; set; }
	double NonSmartRegenFractionTotalAirDemand { get; set; }
	double SmartRegenFractionTotalAirDemand { get; set; }
	double DeadVolumeLitres { get; set; }
	double DeadVolBlowOutsPerLitresperHour { get; set; }
}
