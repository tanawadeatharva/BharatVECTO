namespace DownstreamModules.Electrics
{
	public class ElectricConstants
	{
		// Anticipated Min and Max Allowable values for Powernet, normally 26.3 volts but could be 48 in the future.
		public const double PowenetVoltageMin = 6;
		public const double PowenetVoltageMax = 50;

		// Duty Cycle IE Percentage of use
		public const double PhaseIdleTractionOnMin = 0;
		public const double PhaseIdleTractionMax = 1;

		// Max Min Expected Consumption for a Single Consumer, negative values allowed as bonuses.
		public const int NonminalConsumerConsumptionAmpsMin = -10;
		public const int NominalConsumptionAmpsMax = 100;


		// Alternator
		public const double AlternatorPulleyEfficiencyMin = 0.1;
		public const double AlternatorPulleyEfficiencyMax = 1;
	}
}
