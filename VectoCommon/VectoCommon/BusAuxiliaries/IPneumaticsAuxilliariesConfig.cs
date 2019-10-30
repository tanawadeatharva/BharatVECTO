using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries {
	public interface IPneumaticsAuxilliariesConfig
	{
		double OverrunUtilisationForCompressionFraction { get;  }
		NormLiterPerKilogram BrakingWithRetarderNIperKG { get;  }
		NormLiterPerKilogram BrakingNoRetarderNIperKG { get;  }
		NormLiterPerKilogramMeter BreakingPerKneelingNIperKGinMM { get;  }
		NormLiter PerDoorOpeningNI { get;  }
		NormLiterPerKilogram PerStopBrakeActuationNIperKG { get;  }
		NormLiterPerSecond AirControlledSuspensionNIperMinute { get;  }
		NormLiterPerSecond AdBlueNIperMinute { get;  }
		double NonSmartRegenFractionTotalAirDemand { get;  }
		double SmartRegenFractionTotalAirDemand { get;  }
		NormLiter DeadVolumeLitres { get;  }
		// Nl / Nl / h => 1/h
		PerSecond DeadVolBlowOutsPerLitresperHour { get;  }
	}
}
