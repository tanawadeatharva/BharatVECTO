namespace TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC {
	public interface ISSMTOOL
	{
		ISSMGenInputs GenInputs { get; set; }
		ISSMTechList TechList { get; set; }
		ISSMCalculate Calculate { get; set; }
		bool SSMDisabled { get; set; }
		IHVACConstants HVACConstants { get; set; }

		double ElectricalWBase { get; } // Watt
		double MechanicalWBase { get; } // Watt
		double FuelPerHBase { get; } // LiterPerHour

		double ElectricalWAdjusted { get; } // Watt
		double MechanicalWBaseAdjusted { get; } // Watt
		double FuelPerHBaseAdjusted { get; } // LiterPerHour

		void Clone(ISSMTOOL from);

		bool Load(string filePath);

		bool Save(string filePath);

		bool IsEqualTo(ISSMTOOL source);

		/// <summary>
		/// This alters the waste heat and returns an adjusted fueling value
		/// </summary>
		/// <param name="AverageUseableEngineWasteHeatKW"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		double FuelPerHBaseAsjusted(double AverageUseableEngineWasteHeatKW);

		event MessageEventHandler Message;

	}

	public delegate void MessageEventHandler(ref object sender, string message, AdvancedAuxiliaryMessageType messageType);
}