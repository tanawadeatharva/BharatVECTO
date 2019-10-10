namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public interface IBus
	{
		int Id { get; }

		string Model { get; set; }
		string FloorType { get; set; }
		string EngineType { get; set; }
		double LengthInMetres { get; set; }
		double WidthInMetres { get; set; }
		double HeightInMetres { get; set; }
		int RegisteredPassengers { get; set; }
		bool IsDoubleDecker { get; set; }
	}
}
