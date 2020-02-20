using System;
using System.Collections.Generic;
using Ninject;
using VECTO3.ViewModel.Impl;
using VECTO3.ViewModel.Interfaces;

namespace VECTO3.Util
{
	public static class ViewModelFactory
	{

		public static Dictionary<Component, Type> ComponentViewModelMapping = new Dictionary<Component, Type>()
		{
			{ Component.Vehicle, typeof(IVehicleViewModel)},
			{ Component.Engine ,typeof(IEngineViewModel) },
			{ Component.Gearbox ,typeof(IGearboxViewModel) },
			{ Component.TorqueConverter ,typeof(ITorqueConverterViewModel) },
			{ Component.Retarder ,typeof(IRetarderViewModel) },
			{ Component.Angledrive ,typeof(IAngledriveViewModel) },
			{ Component.Axlegear ,typeof(IAxlegearViewModel) },
			{ Component.Auxiliaries, typeof(IAuxiliariesViewModel) },
			{ Component.Airdrag ,typeof(IAirdragViewModel) },
			{ Component.Cycle ,typeof(ICyclesViewModel) },
			{ Component.Axles, typeof(IAxlesViewModel) }

		};

		//public static IComponentViewModel CreateViewModel(IKernel kernel, Component component, DeclarationJobViewModel jobEditViewModel)
		//{
		//	throw new System.NotImplementedException();
		//}
	}
}
