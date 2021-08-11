using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV28InjectModule : NinjectModule
	{
		#region Overrides of NinjectModlue

		public override void Load()
		{
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationInterimStageBusDataProviderV28>()
				.Named(XMLDeclarationInterimStageBusDataProviderV28.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedInterimStageBusDataProviderV28>()
				.Named(XMLDeclarationExemptedInterimStageBusDataProviderV28.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationInterimStageBusComponentsDataProviderV28>()
				.Named(XMLDeclarationInterimStageBusComponentsDataProviderV28.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLComponentReaderV28>().Named(XMLComponentReaderV28.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationCompleteBusAuxiliariesDataProviderV28>()
				.Named(XMLDeclarationCompleteBusAuxiliariesDataProviderV28.QUALIFIED_XSD_TYPE);

			Bind<IXMLAirdragDeclarationInputData>().To<XMLDeclarationAirdragDataProviderV28>().Named(
				XMLDeclarationAirdragDataProviderV28.QUALIFIED_XSD_TYPE);


			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataProviderV210>().Named(XMLDeclarationADASDataProviderV210.QUALIFIED_XSD_TYPE);



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
