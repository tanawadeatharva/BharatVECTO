using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.ViewModel.MultiStage.Interfaces
{
    public interface IViewModelFactory
	{
		IMultiStageEditViewModel createManufacturingStageEditViewModel();
	}
}
