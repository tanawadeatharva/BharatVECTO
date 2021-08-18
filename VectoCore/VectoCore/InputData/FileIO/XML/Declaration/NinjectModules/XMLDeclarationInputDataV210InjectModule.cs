using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV210InjectModule : NinjectModule
	{
		#region Overrides of NinjectModlue

		public override void Load()
		{

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationComponentsDataProviderV210_Lorry>().Named(
				XMLDeclarationComponentsDataProviderV210_Lorry.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationCompletedBusDataProviderV210>()
				.Named(XMLDeclarationCompletedBusDataProviderV210.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedCompletedBusDataProviderV210>()
				.Named(XMLDeclarationExemptedCompletedBusDataProviderV210.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationCompletedBusComponentsDataProviderV210>()
				.Named(XMLDeclarationCompletedBusComponentsDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLComponentReaderV210_CompletedBus>().Named(XMLComponentReaderV210_CompletedBus.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationCompletedBusAuxiliariesDataProviderV210>()
				.Named(XMLDeclarationCompletedBusAuxiliariesDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLAirdragDeclarationInputData>().To<XMLDeclarationAirdragDataProviderV210>().Named(
				XMLDeclarationAirdragDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV210_Lorry>().Named(
				XMLDeclarationAuxiliariesDataProviderV210_Lorry.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV210_Lorry>()
				.Named(XMLAuxiliaryDeclarationDataProviderV210_Lorry.QUALIFIED_XSD_TYPE);

			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataProviderV210>().Named(XMLDeclarationADASDataProviderV210.QUALIFIED_XSD_TYPE);

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


			Bind<IXMLComponentReader>().To<XMLComponentReaderV210_Lorry>()
				.Named(XMLComponentReaderV210_Lorry.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLComponentReaderV210_Lorry>()
				.Named(XMLComponentReaderV210_Lorry.AUXILIARIES_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLComponentReaderV210_PrimaryBus>().Named(XMLComponentReaderV210_PrimaryBus.QUALIFIED_XSD_TYPE);

			Bind<IXMLADASReader>()
				.To<XMLADASReaderV210>().Named(XMLADASReaderV210.QUALIFIED_XSD_TYPE_CONVENTIONAL);
			Bind<IXMLADASReader>()
				.To<XMLADASReaderV210>().Named(XMLADASReaderV210.QUALIFIED_XSD_TYPE_HEV);
			Bind<IXMLADASReader>()
				.To<XMLADASReaderV210>().Named(XMLADASReaderV210.QUALIFIED_XSD_TYPE_PEV);
			Bind<IXMLADASReader>()
				.To<XMLADASReaderV210>().Named(XMLADASReaderV210.QUALIFIED_XSD_TYPE_IEPC);

		}

		#endregion


	}
}
