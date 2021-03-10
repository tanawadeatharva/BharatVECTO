using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataMultistageV01InjectModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IXMLMultistageBusInputDataProvider>().To<XMLDeclarationInputDataProviderMultistageV01>()
				.Named(XMLDeclarationInputDataProviderMultistageV01.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationMultistageJobInputData>().To<XMLDeclarationMultistageJobInputDataV01>()
                .Named(XMLDeclarationMultistageJobInputDataV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationMultistageJobInputData>().To<XMLDeclarationMultistageJobInputDataV01>();

			Bind<IXMLDeclarationMultistageVehicleBusInputDataReader>().To<XMLDeclarationInputReaderMultistageV01>()
				.Named(XMLDeclarationInputReaderMultistageV01.QUALIFIED_XSD_TYPE);


			Bind<IXMLMultistageJobReader>().To<XMLMultistageJobReaderV01>()
				.Named(XMLMultistageJobReaderV01.QUALIFIED_XSD_TYPE);
		}
	}
}