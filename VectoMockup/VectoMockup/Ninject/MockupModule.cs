using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using TUGraz.VectoMockup.Factories;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.VectoMockup.Ninject
{
    public class MockupModule : AbstractNinjectModule
    {
		#region Overrides of NinjectModule

		public override void Load()
		{
			Rebind<IVectoRunDataFactoryFactory>().To<VectoMockUpRunDataFactoryFactory>();
			Rebind<IXMLDeclarationReportFactory>().To<XMLDeclarationMockupReportFactory>();
		}

		#endregion
	}
}
