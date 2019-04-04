using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV21InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationVehicleDataProviderV21>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationVehicleDataProviderV21.NAMESPACE_URI));

			Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationEngineDataProviderV21>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationEngineDataProviderV21.NAMESPACE_URI));

			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataProviderV21>().Named(
					XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationADASDataProviderV21.NAMESPACE_URI));

			Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationEngineDataProviderV21>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationEngineDataProviderV21.NAMESPACE_URI));

			// ---------------------------------------------------------------------------------------

			Bind<IXMLJobDataReader>().To<XMLJobDataReaderV21>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLJobDataReaderV21.NAMESPACE_URI));

			Bind<IXMLADASReader>().To<XMLADASReaderV21>()
								.Named(XMLHelper.GetVersionFromNamespaceUri(XMLADASReaderV21.NAMESPACE_URI));
		}

		#endregion
	}
}
