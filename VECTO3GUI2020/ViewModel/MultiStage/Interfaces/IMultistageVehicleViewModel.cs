using System.Collections.Generic;
using System.ComponentModel;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.ViewModel.MultiStage.Interfaces
{
	public interface IMultistageVehicleViewModel : IVehicleViewModel, INotifyPropertyChanged
	{
		bool HasErrors { get; }
		Dictionary<string, string> Errors { get; }
		IMultistageAirdragViewModel MultistageAirdragViewModel { get; set; }
		IMultistageAuxiliariesViewModel MultistageAuxiliariesViewModel { get; set; }
		bool PrimaryVehicleHybridElectric { get; set; }
		bool ShowConsolidatedData { get; set; }
		StageInputViewModel.CompletedBusArchitecture Architecture { get; }
		void SetVehicleInputData(IVehicleDeclarationInputData vehicleInputData, bool checkExempted);
	}
}

