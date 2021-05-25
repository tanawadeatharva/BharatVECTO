using System;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.MultiStage.Interfaces
{
	public interface IMultistageAirdragViewModel : IAirdragDeclarationInputData
	{
		EventHandler AirdragViewModelChanged { get; set; }
		IAirDragViewModel AirDragViewModel { get; set; }

		void SetAirdragInputData(IAirdragDeclarationInputData airdragInputData);
	}
}