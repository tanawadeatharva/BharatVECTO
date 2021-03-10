using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataMultistageV01InjectModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IXMLDeclarationInputData>().To<XMLDeclarationInputDataProviderMultistageV01>()
				.Named(XMLDeclarationInputDataProviderMultistageV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationInputDataReader>().To<XMLDeclarationInputReaderMultistageV01>()
				.Named(XMLDeclarationInputReaderMultistageV01.QUALIFIED_XSD_TYPE);

		}
	}
}