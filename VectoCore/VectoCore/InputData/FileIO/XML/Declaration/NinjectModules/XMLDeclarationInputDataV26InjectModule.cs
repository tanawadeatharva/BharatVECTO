using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules {
	public class XMLDeclarationInputDataV26InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationPrimaryBusVehicleDataProviderV210>()
												.Named(XMLDeclarationPrimaryBusVehicleDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedPrimaryBusDataProviderV210>()
				.Named(XMLDeclarationExemptedPrimaryBusDataProviderV210.QUALIFIED_XSD_TYPE);


			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusComponentsDataProviderV210>()
													.Named(XMLDeclarationPrimaryBusComponentsDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationPrimaryBusAuxiliariesDataProviderV210>()
													.Named(XMLDeclarationPrimaryBusAuxiliariesDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMediumLorryVehicleDataProviderV210>()
											.Named(XMLDeclarationMediumLorryVehicleDataProviderV210.QUALIFIED_XSD_TYPE);


            Bind<IXMLComponentReader>().To<XMLComponentReaderV210_PrimaryBus>().Named(XMLComponentReaderV210_PrimaryBus.QUALIFIED_XSD_TYPE);

		}

		#endregion
	}
}