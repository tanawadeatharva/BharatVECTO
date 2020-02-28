using System.Collections.Generic;
using System.Collections.ObjectModel;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces {
	public interface IAuxiliariesViewModel : IComponentViewModel
	{
		IAuxiliariesDeclarationInputData ModelData { get; }

		string PneumaticSystemTechnology { get; set; }
		AllowedEntry<string>[] AllowedPneumaticSystemTechnologies { get; }

		string FanTechnology { get; set; }
		AllowedEntry<string>[] AllowedFanTechnologies { get; }

		ObservableCollection<SteeringPumpEntry> SteeringPumpTechnologies { get; }
		AllowedEntry<string>[] AllowedSteeringPumpTechnologies { get; }

		string ElectricSystemTechnology { get; set; }
		AllowedEntry<string>[] AllowedElectricSystemTechnologies { get; }

		string HVACTechnology { get; set; }
		AllowedEntry<string>[] AllowedHVACTechnologies { get; }

		#region Bus Auxiliaries

		#region Electric System
		
		ObservableCollection<IAlternatorDeclarationInputData> Alternators { get; set; }
		bool DayrunninglightsLED { get; set; }
		bool HeadlightsLED { get; set; }
		bool PositionlightsLED { get; set; }
		bool BrakelightsLED { get; set; }
		bool InteriorLightsLED { get; set; }

		#endregion

		ConsumerTechnology DoorDriveTechnology { get; set; }

		#region Havac

		BusHVACSystemConfiguration SystemConfiguration { get; set; }
		ACCompressorType CompressorTypeDriver { get; set; }
		ACCompressorType CompressorTypePassenger { get; set; }
		Watt AuxHeaterPower { get; set; }
		bool DoubleGlasing { get; set; }
		bool HeatPump { get; set; }
		bool AdjustableAuxiliaryHeater { get; set; }
		bool SeparateAirDistributionDucts { get; set; }

		#endregion

		AllowedEntry<BusHVACSystemConfiguration>[] AllowedSystemConfigurations { get; }
		AllowedEntry<ACCompressorType>[] AllowedDriverACCompressorTypes { get; }
		AllowedEntry<ACCompressorType>[] AllowedPassengerACCompressorTypes { get; }
		AllowedEntry<ConsumerTechnology>[] AllowedConsumerTechnologies { get; }

		#endregion
	}
}