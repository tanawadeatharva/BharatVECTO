using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC {
	public interface ISSMTOOL
	{
		ISSMInputs SSMInputs { get;  }
		ISSMTechList TechList { get;  }
		ISSMCalculate Calculate { get;  }
		bool SSMDisabled { get; }
		IHVACConstants HVACConstants { get; set; }

		Watt ElectricalWBase { get; } // Watt
		Watt MechanicalWBase { get; } // Watt
		KilogramPerSecond FuelPerHBase { get; } // LiterPerHour

		Watt ElectricalWAdjusted { get; } // Watt
		Watt MechanicalWBaseAdjusted { get; } // Watt
		KilogramPerSecond FuelPerHBaseAdjusted { get; } // LiterPerHour

		Watt EngineWasteHeatkW { get; }

		
		bool IsEqualTo(ISSMTOOL source);

		/// <summary>
		/// This alters the waste heat and returns an adjusted fueling value
		/// </summary>
		/// <param name="AverageUseableEngineWasteHeatKW"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		KilogramPerSecond FuelPerHBaseAsjusted(Watt AverageUseableEngineWasteHeatKW);

		event MessageEventHandler Message;

	}

	public delegate void MessageEventHandler(ref object sender, string message, AdvancedAuxiliaryMessageType messageType);
}