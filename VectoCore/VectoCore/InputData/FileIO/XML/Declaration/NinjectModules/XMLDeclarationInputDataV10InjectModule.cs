using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV10InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLDeclarationInputData>().To<XMLDeclarationInputDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationInputDataProviderV10.NAMESPACE_URI));

			Bind<IXMLDeclarationJobInputData>().To<XMLDeclarationJobInputDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationJobInputDataProviderV10.NAMESPACE_URI));

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationVehicleDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationVehicleDataProviderV10.NAMESPACE_URI));

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationComponentsDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationComponentsDataProviderV10.NAMESPACE_URI));

			Bind<IXMLAirdragDeclarationInputData>().To<XMLDeclarationAirdragDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationAirdragDataProviderV10.NAMESPACE_URI));

			Bind<IXMLAngledriveInputData>().To<XMLDeclarationAngledriveDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationAngledriveDataProviderV10.NAMESPACE_URI));

			Bind<IXMLAxleGearInputData>().To<XMLDeclarationAxlegearDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationAxlegearDataProviderV10.NAMESPACE_URI));

			Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationEngineDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationEngineDataProviderV10.NAMESPACE_URI));

			Bind<IXMLRetarderInputData>().To<XMLDeclarationRetarderDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationRetarderDataProviderV10.NAMESPACE_URI));

			Bind<IXMLGearboxDeclarationInputData>().To<XMLDeclarationGearboxDataProviderV10>()
													.Named(XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationGearboxDataProviderV10.NAMESPACE_URI));

			Bind<IXMLGearData>().To<XMLGearDataV10>().Named(XMLHelper.GetVersionFromNamespaceUri(XMLGearDataV10.NAMESPACE_URI));

			Bind<IXMLTorqueConverterDeclarationInputData>().To<XMLDeclarationTorqueConverterDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationTorqueConverterDataProviderV10.NAMESPACE_URI));

			Bind<IXMLAxlesDeclarationInputData>().To<XMLDeclarationAxlesDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationAxlesDataProviderV10.NAMESPACE_URI));

			Bind<IXMLAxleDeclarationInputData>().To<XMLDeclarationAxleDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationAxleDataProviderV10.NAMESPACE_URI));

			Bind<IXMLTyreDeclarationInputData>().To<XMLDeclarationTyreDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationTyreDataProviderV10.NAMESPACE_URI));

			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationAuxiliariesDataProviderV10.NAMESPACE_URI));

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLAuxiliaryDeclarationDataProviderV10.NAMESPACE_URI));

			Bind<IXMLPTOTransmissionInputData>().To<XMLDeclarationPTODataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationPTODataProviderV10.NAMESPACE_URI));

			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataProviderV10>().Named(
					XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationADASDataProviderV10.NAMESPACE_URI));

			// ---------------------------------------------------------------------------------------

			Bind<IXMLDeclarationInputDataReader>().To<XMLDeclarationInputReaderV10>()
												.Named(XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationInputReaderV10.NAMESPACE_URI));

			Bind<IXMLJobDataReader>().To<XMLJobDataReaderV10>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLJobDataReaderV10.NAMESPACE_URI));

			Bind<IXMLComponentReader>().To<XMLComponentReaderV10>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLComponentReaderV10.NAMESPACE_URI));

			Bind<IXMLPTOReader>().To<XMLPTOReaderV10>()
								.Named(XMLHelper.GetVersionFromNamespaceUri(XMLPTOReaderV10.NAMESPACE_URI));

			Bind<IXMLADASReader>().To<XMLADASReaderV10>()
								.Named(XMLHelper.GetVersionFromNamespaceUri(XMLADASReaderV10.NAMESPACE_URI));
		}

		#endregion
	}
}
