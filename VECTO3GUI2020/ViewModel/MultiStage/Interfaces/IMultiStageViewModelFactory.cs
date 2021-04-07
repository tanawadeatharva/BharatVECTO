using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.ViewModel.MultiStage.Interfaces
{
    public interface IMultiStageViewModelFactory
	{
		IViewModelBase GetNewMultistageJobViewModel();

		IMultiStageJobViewModel GetMultiStageJobViewModel(IMultistageBusInputDataProvider inputData);

		IVehicleViewModel GetInterimStageVehicleViewModel();

		IVehicleViewModel GetInterimStageVehicleViewModel(IVehicleDeclarationInputData consolidatedVehicleData);

		IManufacturingStageViewModel GetManufacturingStageViewModel(IManufacturingStageInputData consolidatedManufacturingStageInputData);
		IMultistageAirdragViewModel GetMultistageAirdragViewModel();

		IMultistageAirdragViewModel getMultistageAirdragViewModel(
			IAirdragDeclarationInputData consolidatedAirdragInputData);

		//IViewModelBase CreateNewMultiStageJobViewModel();

		//IMultiStageJobViewModel CreateMultiStageJobViewModel(string inputProviderType, IMultistageBusInputDataProvider inputData);

		//IVehicleViewModel CreateInterimStageVehicleViewModel(string inputProviderType);

		//IVehicleViewModel CreateInterimStageVehicleViewModel(string inputProviderType, IVehicleDeclarationInputData prevStageInputData);

		//IManufacturingStageViewModel CreateManufacturingStageViewModel(string inputProviderType, IManufacturingStageInputData consolidatedManufacturingStageInputData);

		//IMultistageAirdragViewModel CreateMultistageAirdragViewModel();
	}
}
