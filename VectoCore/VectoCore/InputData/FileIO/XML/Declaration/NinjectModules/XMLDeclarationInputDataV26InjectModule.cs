using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
    public class XMLDeclarationInputDataV26InjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IXMLAxleDeclarationInputData>().To<XMLDeclarationAxleDataProviderV26>().Named(
				XMLDeclarationAxleDataProviderV26.QUALIFIED_XSD_TYPE);

            Bind<IXMLAxleReader>().To<XMLComponentReaderV26>().Named(
                XMLComponentReaderV26.AXLE_READER_QUALIFIED_XSD_TYPE);

            Bind<IXMLAxlesDeclarationInputData>().To<XMLDeclarationAxlesDataProviderV26>().Named(
                XMLDeclarationAxlesDataProviderV26.QUALIFIED_XSD_TYPE);

            Bind<IXMLAxlesReader>().To<XMLComponentReaderV26>().Named(
                XMLComponentReaderV26.AXLES_READER_QUALIFIED_XSD_TYPE);
        }
    }
}
