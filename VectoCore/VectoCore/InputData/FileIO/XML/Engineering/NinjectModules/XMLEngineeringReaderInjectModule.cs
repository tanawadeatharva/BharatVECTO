using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.NinjectModules;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringReaderInjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IEngineeringInjectFactory>().ToFactory(() => new UseFirstArgumentAsInstanceProvider());

			Kernel?.Load(new INinjectModule[] {
				new XMLEngineeringReaderV07InjectModule(),
				new XMLEngineeringReaderV10InjectModule(),
				new XMLEngineeringReaderV11InjectModule()
				}
			);

		}

		#endregion

		


	}
}
