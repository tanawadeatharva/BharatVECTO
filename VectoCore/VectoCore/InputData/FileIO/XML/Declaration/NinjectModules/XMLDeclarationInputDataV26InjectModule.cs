using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
    public class XMLDeclarationInputDataV26InjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IXMLBatteryPackDeclarationInputData>().To<XMLBatteryPackDeclarationInputDataStandardV26>()
                .Named(XMLBatteryPackDeclarationInputDataStandardV26.QUALIFIED_XSD_TYPE);

            Bind<IXMLSuperCapDeclarationInputData>().To<XMLSuperCapDeclarationInputDataV26>()
                .Named(XMLSuperCapDeclarationInputDataV26.QUALIFIED_XSD_TYPE);
        }
    }
}
