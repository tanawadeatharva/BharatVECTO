namespace TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
{
	public interface ISSMRun
	{
		double HVACOperation { get; }
		double TCalc { get; }
		double TemperatureDelta { get; }
		double QWall { get; }
		double WattsPerPass { get; }
		double Solar { get; }
		double TotalW { get; }
		double TotalKW { get; }
		double FuelW { get; }
		double TechListAmendedFuelW { get; }
	}
}
