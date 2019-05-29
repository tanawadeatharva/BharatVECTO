using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering;

namespace TUGraz.VectoCore.InputData.FileIO.XML
{
	public class XMLInputDataNinjectModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLInputDataReader>().To<XMLInputDataFactory>();

			LoadModule<XMLDeclarationReaderInjectModule>();

			LoadModule<XMLEngineeringReaderInjectModule>();
		}

		#endregion
	}
}
