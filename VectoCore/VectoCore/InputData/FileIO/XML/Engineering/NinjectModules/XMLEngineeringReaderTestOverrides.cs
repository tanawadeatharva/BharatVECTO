using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.NinjectModules
{
	public class XMLEngineeringReaderTestOverrides : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			// testing derived xml data types
			Bind<IXMLEngineData>().To<XMLEngineeringEngineDataProviderV10TEST>()
								.Named(XMLEngineeringEngineDataProviderV10TEST.QUALIFIED_XSD_TYPE);
			Bind<IXMLTyreData>().To<XMLTyreEngineeringDataProviderV10TEST>().Named(XMLTyreEngineeringDataProviderV10TEST.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleEngineeringData>().To<XMLAxleEngineeringDataV10TEST>()
											.Named(XMLAxleEngineeringDataV10TEST.QUALIFIED_XSD_TYPE);
			Bind<IXMLComponentsReader>().To<XMLComponentsEngineeringReaderV10TEST>()
										.Named(XMLComponentsEngineeringReaderV10TEST.QUALIFIED_XSD_TYPE_COMPONENTS);

			Bind<IXMLAxlesReader>().To<XMLComponentsEngineeringReaderV10TEST>()
									.Named(XMLComponentsEngineeringReaderV10TEST.QUALIFIED_XSD_TYPE_AXLES);
			Bind<IXMLAxleReader>().To<XMLComponentsEngineeringReaderV10TEST>()
								.Named(XMLComponentsEngineeringReaderV10TEST.QUALIFIED_XSD_TYPE_AXLE);
			Bind<IXMLGearboxReader>().To<XMLComponentsEngineeringReaderV10TEST>()
									.Named(XMLComponentsEngineeringReaderV10TEST.QUALIFIED_XSD_TYPE_GEARBOX);
			Bind<IXMLAuxiliaryReader>().To<XMLComponentsEngineeringReaderV10TEST>()
										.Named(XMLComponentsEngineeringReaderV10TEST.QUALIFIED_XSD_TYPE_AUXILIARY);

		}

		#endregion
	}
}
