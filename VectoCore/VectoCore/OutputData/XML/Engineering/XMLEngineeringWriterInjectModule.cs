using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering;
using TUGraz.VectoCore.OutputData.XML.Engineering;
using TUGraz.VectoCore.OutputData.XML.Engineering.Factory;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.OutputData.XML.Engineering.Writer;
using TUGraz.VectoCore.OutputData.XML.Factory;
using TUGraz.VectoCore.OutputData.XML.NinjectModules;
using TUGraz.VectoCore.OutputData.XML.Writer;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLEngineeringWriterInjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IEngineeringWriterInjectFactory>().ToFactory(() => new UseFirstArgumentAsInstanceProvider());

			Bind<IEngineeringWriterFactory>().To<EngineeringWriterFactory>();

			Bind<IXMLEngineeringWriter>().To<XMLEngineeringWriter>();

			
			Kernel?.Load(new [] {
				new XMLEngineeringWriterV10InjectModule()
			});
		}

		#endregion
	}
}
