using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IPneumaticUserInputsConfig
	{
		ICompressorMap CompressorMap { get;  }
		double CompressorGearEfficiency { get;  }
		double CompressorGearRatio { get;  }
		bool SmartAirCompression { get;  }
		bool SmartRegeneration { get;  }
		//bool RetarderBrake { get;  }
		Meter KneelingHeightMillimeters { get;  }
		ConsumerTechnology AirSuspensionControl { get;  } // mechanical or electrical
		ConsumerTechnology AdBlueDosing { get;  } // pnmeumatic or electric
		ConsumerTechnology Doors { get;  } // pneumatic or electric
	}

	public enum ConsumerTechnology
	{
		Unknown,
		Mechanically,
		Electrically,
		Pneumatic,
	}
}
