using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoMockup.Simulation.SimulatorFactory;

namespace TUGraz.VectoMockup.Ninject
{
    public class SimulatorFactoryModule : AbstractNinjectModule
    {
		#region Overrides of NinjectModule

		public override void Load()
		{
			Kernel.Rebind<ISimulatorFactory>().To<MockupDeclarationSimulatorFactory>().Named(ExecutionMode.Declaration.ToString());
			//Rebind clears all bindings for ISimulatorFactory
			Kernel.Bind<ISimulatorFactory>().To<MockupEngineeringSimulatorFactory>().Named(ExecutionMode.Engineering.ToString());
			//Bind<ISimulatorFactory>().To<SimulatorFactoryEngineering>().Named(ExecutionMode.Engineering.ToString());
		}

		#endregion
	}
}
