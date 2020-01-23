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
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationPrimaryBusVehicleDataProviderV26>()
												.Named(XMLDeclarationPrimaryBusVehicleDataProviderV26.QUALIFIED_XSD_TYPE);


			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusComponentsDataProviderV26>()
													.Named(XMLDeclarationPrimaryBusComponentsDataProviderV26.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationPrimaryBusAuxiliariesDataProviderV26>()
													.Named(XMLDeclarationPrimaryBusAuxiliariesDataProviderV26.QUALIFIED_XSD_TYPE);


			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMediumLorryVehicleDataProviderV26>()
											.Named(XMLDeclarationMediumLorryVehicleDataProviderV26.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationComponentsDataProviderNoAxlegearV26>()
													.Named(XMLComponentReaderNoAxlegearV26.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxDeclarationInputData>().To<XMLDeclarationGearboxDataProviderV26>()
													.Named(XMLDeclarationGearboxDataProviderV26.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLComponentReaderV26>().Named(XMLComponentReaderV26.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLComponentReaderNoAxlegearV26>().Named(XMLComponentReaderNoAxlegearV26.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxReader>().To<XMLGearboxReaderV26>().Named(XMLGearboxReaderV26.QUALIFIED_XSD_TYPE);
		}

		#endregion
	}
}