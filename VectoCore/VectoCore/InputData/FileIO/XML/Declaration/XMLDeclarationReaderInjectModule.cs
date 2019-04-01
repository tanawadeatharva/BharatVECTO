using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationReaderInjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IDeclarationInjectFactory>().ToFactory(() => new UseFirstArgumentAsInstanceProvider());

			Bind<IXMLInputDataReader>().To<XMLInputDataFactory>();

			Kernel?.Load(new INinjectModule[] {
				new XMLDeclarationInputDataV10InjectModule(), 
			});

			// dummy 
			Bind<IEngineeringInjectFactory>().ToFactory(() => new UseFirstArgumentAsInstanceProvider());

			#endregion
		}
	}
}
