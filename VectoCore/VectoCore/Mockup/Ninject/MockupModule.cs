using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoMockup.Reports;
using TUGraz.VectoMockup.Simulation.RundataFactories;

namespace TUGraz.VectoMockup.Ninject
{
    public class MockupModule : AbstractNinjectModule
    {
		#region Overrides of NinjectModule

		public override void Load()
		{
			LoadModule<CIFMockupModule>();
			LoadModule<MRFMockupModule>();
			LoadModule<SimulatorFactoryModule>();
			LoadModule<VIFMockupModule>();

			Rebind<IVectoRunDataFactoryFactory>().To<VectoMockUpRunDataFactoryFactory>();
			Rebind<IXMLDeclarationReportFactory>().To<MockupReportFactory>();
			Rebind<IXMLInputDataReader>().To<MockupXMLInputDataFactory>();
		}

		#endregion
	}
}
