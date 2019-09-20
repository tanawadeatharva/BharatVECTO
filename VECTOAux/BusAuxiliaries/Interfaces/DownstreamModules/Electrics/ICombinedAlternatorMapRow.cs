namespace DownstreamModules.Electrics
{
	// Reflects stored data in pesisted CombinedAlternator Map .AALT
	public interface ICombinedAlternatorMapRow
	{
		string AlternatorName { get; set; }
		double RPM { get; set; }
		double Amps { get; set; }
		double Efficiency { get; set; }
		double PulleyRatio { get; set; }
	}
}
