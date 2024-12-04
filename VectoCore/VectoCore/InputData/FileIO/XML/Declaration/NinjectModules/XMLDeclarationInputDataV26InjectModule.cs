using Ninject.Modules;
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
                
            Bind<IXMLBatteryPackDeclarationInputData>().To<XMLBatteryPackDeclarationInputDataStandardV26>()
                .Named(XMLBatteryPackDeclarationInputDataStandardV26.QUALIFIED_XSD_TYPE);

            Bind<IXMLSuperCapDeclarationInputData>().To<XMLSuperCapDeclarationInputDataV26>()
                .Named(XMLSuperCapDeclarationInputDataV26.QUALIFIED_XSD_TYPE);

            Bind<IXMLIEPCInputData>().To<XMLElectricMotorIEPCIInputDataProviderV26>().Named(XMLElectricMotorIEPCIInputDataProviderV26.QUALIFIED_XSD_TYPE);

            Bind<IXMLIEPCInputData>().To<XMLElectricMotorIepciStandardInputDataProviderV26>().Named(XMLElectricMotorIepciStandardInputDataProviderV26.QUALIFIED_XSD_TYPE);
        }
    }
}
