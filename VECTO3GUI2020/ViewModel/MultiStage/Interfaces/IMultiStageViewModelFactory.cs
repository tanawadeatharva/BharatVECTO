using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Ninject.Factories;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.ViewModel.MultiStage.Interfaces
{

	public interface IMultiStageViewModelFactory : 
		IMultiStageViewModelFactoryDefaultInstanceProvider,		IDocumentViewModelFactory, 
IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider
    {
		//IVehicleViewModel GetInterimStageVehicleViewModel(StageInputViewModel.CompletedBusArchitecture arch);
		IMultistageVehicleViewModel GetInterimStageVehicleViewModel(StageInputViewModel.CompletedBusArchitecture arch);
		IMultistageVehicleViewModel GetInterimStageVehicleViewModel(IVehicleDeclarationInputData inputData);
	}



	public interface IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider
	{
		IVehicleViewModel CreateStageInputVehicleViewModel(string inputProviderType);
	}

    public interface IMultiStageViewModelFactoryDefaultInstanceProvider
	{
		IViewModelBase GetNewMultistageJobViewModel();

		IMultiStageJobViewModel GetMultiStageJobViewModel(IMultistepBusInputDataProvider inputData);

		//IVehicleViewModel GetInterimStageVehicleViewModel();

		IVehicleViewModel GetInterimStageVehicleViewModel(IVehicleDeclarationInputData consolidatedVehicleData, bool exempted);

		IManufacturingStageViewModel GetManufacturingStageViewModel(IManufacturingStageInputData consolidatedManufacturingStageInputData, bool exempted);
		IMultistageAirdragViewModel GetMultistageAirdragViewModel();

		IMultistageAirdragViewModel GetMultistageAirdragViewModel(
			IAirdragDeclarationInputData consolidatedAirdragInputData);

		IMultistageAuxiliariesViewModel GetAuxiliariesViewModel(StageInputViewModel.CompletedBusArchitecture arch);
		IMultistageAuxiliariesViewModel GetAuxiliariesViewModel(StageInputViewModel.CompletedBusArchitecture arch,
			IBusAuxiliariesDeclarationData consolidatedAuxiliariesInputData);
	}
}
