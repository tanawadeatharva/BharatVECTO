using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
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
	}
}