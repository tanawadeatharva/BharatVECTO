using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI.ViewModel.Interfaces;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Impl
{
	public class CompleteVehicleBusJobViewModel : AbstractJobViewModel, IJobEditViewModel
	{

		public CompleteVehicleBusJobViewModel(IKernel kernel, IDeclarationInputDataProvider inputData)
		{
			Kernel = kernel;
			InputDataProvider = inputData;
			JobViewModel = this;
			CreateComponentModel(Component.CompleteBusVehicle);
			CreateComponentModel(Component.Airdrag);
			CreateComponentModel(Component.Auxiliaries);
			CurrentComponent = GetComponentViewModel(Component.CompleteBusVehicle);
		}



		protected override void DoSaveJob()
		{
			throw new NotImplementedException();
		}

		public string JobFile { get; }
		public IInputDataProvider InputDataProvider { get; set; }
	}
}
