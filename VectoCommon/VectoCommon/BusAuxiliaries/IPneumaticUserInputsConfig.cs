namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IPneumaticUserInputsConfig
	{
		string CompressorMap { get; set; }
		double CompressorGearEfficiency { get; set; }
		double CompressorGearRatio { get; set; }
		string ActuationsMap { get; set; }
		bool SmartAirCompression { get; set; }
		bool SmartRegeneration { get; set; }
		bool RetarderBrake { get; set; }
		double KneelingHeightMillimeters { get; set; }
		string AirSuspensionControl { get; set; } // mechanical or electrical
		string AdBlueDosing { get; set; } // pnmeumatic or electric
		string Doors { get; set; } // pneumatic or electric
	}
}
