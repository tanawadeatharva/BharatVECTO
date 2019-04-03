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
								.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringEngineDataProviderV10TEST.NAMESPACE_URI));
			Bind<IXMLTyreData>().To<XMLTyreEngineeringDataProviderV10TEST>().Named(XMLHelper.GetVersionFromNamespaceUri(XMLTyreEngineeringDataProviderV10TEST.NAMESPACE_URI));

			Bind<IXMLAxleEngineeringData>().To<XMLAxleEngineeringDataV10TEST>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLAxleEngineeringDataV10TEST.NAMESPACE_URI));
			Bind<IXMLComponentsReader>().To<XMLComponentsEngineeringReaderV10TEST>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLComponentsEngineeringReaderV10TEST.NAMESPACE_URI));
		}

		#endregion
	}
}
