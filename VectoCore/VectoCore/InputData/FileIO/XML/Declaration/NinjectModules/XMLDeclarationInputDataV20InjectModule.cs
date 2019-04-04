using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV20InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLDeclarationInputData>().To<XMLDeclarationInputDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationInputDataProviderV20.NAMESPACE_URI));

			Bind<IXMLDeclarationJobInputData>().To<XMLDeclarationJobInputDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationJobInputDataProviderV20.NAMESPACE_URI));

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationVehicleDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationVehicleDataProviderV20.NAMESPACE_URI));

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationComponentsDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationComponentsDataProviderV20.NAMESPACE_URI));

			Bind<IXMLAirdragDeclarationInputData>().To<XMLDeclarationAirdragDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationAirdragDataProviderV20.NAMESPACE_URI));

			Bind<IXMLAngledriveInputData>().To<XMLDeclarationAngledriveDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationAngledriveDataProviderV20.NAMESPACE_URI));

			Bind<IXMLAxleGearInputData>().To<XMLDeclarationAxlegearDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationAxlegearDataProviderV20.NAMESPACE_URI));

			Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationEngineDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationEngineDataProviderV20.NAMESPACE_URI));

			Bind<IXMLRetarderInputData>().To<XMLDeclarationRetarderDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationRetarderDataProviderV20.NAMESPACE_URI));

			Bind<IXMLGearboxDeclarationInputData>().To<XMLDeclarationGearboxDataProviderV20>()
													.Named(XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationGearboxDataProviderV20.NAMESPACE_URI));

			Bind<IXMLGearData>().To<XMLGearDataV20>().Named(XMLHelper.GetVersionFromNamespaceUri(XMLGearDataV20.NAMESPACE_URI));

			Bind<IXMLTorqueConverterDeclarationInputData>().To<XMLDeclarationTorqueConverterDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationTorqueConverterDataProviderV20.NAMESPACE_URI));

			Bind<IXMLAxlesDeclarationInputData>().To<XMLDeclarationAxlesDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationAxlesDataProviderV20.NAMESPACE_URI));

			Bind<IXMLAxleDeclarationInputData>().To<XMLDeclarationAxleDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationAxleDataProviderV20.NAMESPACE_URI));

			Bind<IXMLTyreDeclarationInputData>().To<XMLDeclarationTyreDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationTyreDataProviderV20.NAMESPACE_URI));

			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationAuxiliariesDataProviderV20.NAMESPACE_URI));

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLAuxiliaryDeclarationDataProviderV20.NAMESPACE_URI));

			Bind<IXMLPTOTransmissionInputData>().To<XMLDeclarationPTODataProviderV20>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationPTODataProviderV20.NAMESPACE_URI));


			// ---------------------------------------------------------------------------------------

			Bind<IXMLDeclarationInputDataReader>().To<XMLDeclarationInputReaderV20>()
												.Named(XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationInputReaderV20.NAMESPACE_URI));

			Bind<IXMLJobDataReader>().To<XMLJobDataReaderV20>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLJobDataReaderV20.NAMESPACE_URI));

			Bind<IXMLComponentReader>().To<XMLComponentReaderV20>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLComponentReaderV20.NAMESPACE_URI));

			Bind<IXMLPTOReader>().To<XMLPTOReaderV20>()
								.Named(XMLHelper.GetVersionFromNamespaceUri(XMLPTOReaderV20.NAMESPACE_URI));

			Bind<IXMLADASReader>().To<XMLADASReaderV20>()
								.Named(XMLHelper.GetVersionFromNamespaceUri(XMLADASReaderV20.NAMESPACE_URI));
		}

		#endregion
	}
}
