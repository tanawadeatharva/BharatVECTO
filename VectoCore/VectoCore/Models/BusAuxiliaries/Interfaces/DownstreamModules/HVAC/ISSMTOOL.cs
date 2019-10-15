using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC {
	public interface ISSMTOOL
	{
		ISSMInputs SSMInputs { get; set; }
		ISSMTechList TechList { get; set; }
		ISSMCalculate Calculate { get; set; }
		bool SSMDisabled { get; set; }
		IHVACConstants HVACConstants { get; set; }

		Watt ElectricalWBase { get; } // Watt
		Watt MechanicalWBase { get; } // Watt
		KilogramPerSecond FuelPerHBase { get; } // LiterPerHour

		Watt ElectricalWAdjusted { get; } // Watt
		Watt MechanicalWBaseAdjusted { get; } // Watt
		KilogramPerSecond FuelPerHBaseAdjusted { get; } // LiterPerHour

		//void Clone(ISSMTOOL from);

		bool Load(string filePath);

		bool Save(string filePath);

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