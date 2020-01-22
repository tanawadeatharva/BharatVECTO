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

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationCompletedBusDataProviderV26>()
												.Named(XMLDeclarationCompletedBusDataProviderV26.QUALIFIED_XSD_TYPE);


			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusComponentsDataProviderV26>()
													.Named(XMLDeclarationPrimaryBusComponentsDataProviderV26.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationCompleteBusComponentsDataProviderV26>()
													.Named(XMLDeclarationCompleteBusComponentsDataProviderV26.QUALIFIED_XSD_TYPE);


			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationBusAuxiliariesDataProviderV26>()
													.Named(XMLDeclarationBusAuxiliariesDataProviderV26.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationCompleteBusAuxiliariesDataProviderV26>()
													.Named(XMLDeclarationCompleteBusAuxiliariesDataProviderV26.QUALIFIED_XSD_TYPE);


			Bind<IXMLComponentReader>().To<XMLComponentReaderV26>().Named(XMLComponentReaderV26.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLComponentReaderV26>().Named(XMLComponentReaderV26.QUALIFIED_COMPLETE_XSD_TYPE);

		}

		#endregion
	}
}