using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
    public class XMLDeclarationInputDataV27InjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IXMLAxleDeclarationInputData>().To<XMLDeclarationAxleDataProviderV27>().Named(
                XMLDeclarationAxleDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLAxleReader>().To<XMLComponentReaderV27>().Named(
                XMLComponentReaderV27.AXLE_READER_QUALIFIED_XSD_TYPE);

            Bind<IXMLAxlesDeclarationInputData>().To<XMLDeclarationAxlesDataProviderV27>().Named(
                XMLDeclarationAxlesDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLAxlesReader>().To<XMLComponentReaderV27>().Named(
                XMLComponentReaderV27.AXLES_READER_QUALIFIED_XSD_TYPE);
        }
    }
}
