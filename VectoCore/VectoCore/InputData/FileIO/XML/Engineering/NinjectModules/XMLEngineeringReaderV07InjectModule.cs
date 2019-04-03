using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Impl;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.NinjectModules
{
	public class XMLEngineeringReaderV07InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLEngineeringInputData>().To<XMLEngineeringInputDataProviderV07>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringInputDataProviderV07.NAMESPACE_URI));
			Bind<IXMLEngineeringInputReader>().To<XMLEngineeringInputReaderV07>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringInputReaderV07.NAMESPACE_URI));

			Bind<IXMLEngineeringJobInputData>().To<XMLEngineeringJobInputDataProviderV07>()
												.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringJobInputDataProviderV07.NAMESPACE_URI));

			Bind<IXMLJobDataReader>().To<XMLJobDataReaderV07>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLJobDataReaderV07.NAMESPACE_URI));

			Bind<IXMLEngineeringDriverData>().To<XMLEngineeringDriverDataProviderV07>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringDriverDataProviderV07.NAMESPACE_URI));

			Bind<IXMLDriverDataReader>().To<XMLDriverDataReaderV07>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLDriverDataReaderV07.NAMESPACE_URI));

			Bind<IXMLEngineeringGearshiftData>().To<XMLEngineeringGearshiftDataV07>()
												.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringGearshiftDataV07.NAMESPACE_URI));

			Bind<IXMLCyclesDataProvider>().To<XMLCyclesDataProviderV07>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLCyclesDataProviderV07.NAMESPACE_URI));

			Bind<IXMLEngineeringVehicleData>().To<XMLEngineeringVehicleDataProviderV07>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringVehicleDataProviderV07.NAMESPACE_URI));
			Bind<IXMLComponentsReader>().To<XMLComponentsEngineeringReaderV07>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLComponentsEngineeringReaderV07.NAMESPACE_URI));

			Bind<IXMLEngineeringVehicleComponentsData>().To<XMLEngineeringVehicleComponentsDataProviderV07>()
														.Named(
															XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringVehicleComponentsDataProviderV07.NAMESPACE_URI));

			Bind<IXMLAxleEngineeringData>().To<XMLAxleEngineeringDataV07>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLAxleEngineeringDataV07.NAMESPACE_URI));
			Bind<IXMLGearData>().To<XMLGearDataV07>().Named(XMLHelper.GetVersionFromNamespaceUri(XMLGearDataV07.NAMESPACE_URI));
			Bind<IXMLAuxiliaryData>().To<XMLAuxiliaryEngineeringDataV07>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLAuxiliaryEngineeringDataV07.NAMESPACE_URI));
			Bind<IXMLAxlegearData>().To<XMLEngineeringAxlegearDataProviderV07>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAxlegearDataProviderV07.NAMESPACE_URI));
			Bind<IXMLAngledriveData>().To<XMLEngineeringAngledriveDataProviderV07>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAngledriveDataProviderV07.NAMESPACE_URI));
			Bind<IXMLEngineData>().To<XMLEngineeringEngineDataProviderV07>()
								.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringEngineDataProviderV07.NAMESPACE_URI));
			Bind<IXMLRetarderData>().To<XMLEngineeringRetarderDataProviderV07>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringRetarderDataProviderV07.NAMESPACE_URI));
			Bind<IXMLAuxiliairesData>().To<XMLEngineeringAuxiliariesDataProviderV07>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAuxiliariesDataProviderV07.NAMESPACE_URI));
			Bind<IXMLGearboxData>().To<XMLEngineeringGearboxDataProviderV07>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringGearboxDataProviderV07.NAMESPACE_URI));
			Bind<IXMLAirdragData>().To<XMLEngineeringAirdragDataProviderV07>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAirdragDataProviderV07.NAMESPACE_URI));
			Bind<IXMLTorqueconverterData>().To<XMLEngineeringTorqueConverterDataProviderV07>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringTorqueConverterDataProviderV07.NAMESPACE_URI));
			Bind<IXMLAxlesData>().To<XMLEngineeringAxlesDataProviderV07>()
								.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAxlesDataProviderV07.NAMESPACE_URI));

			Bind<IXMLLookaheadData>().To<XMLEngineeringDriverLookAheadV07>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringDriverLookAheadV07.NAMESPACE_URI));

			Bind<IXMLOverspeedData>().To<XMLEngineeringOverspeedV07>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringOverspeedV07.NAMESPACE_URI));

			Bind<IXMLDriverAcceleration>().To<XMLDriverAccelerationV07>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLDriverAccelerationV07.NAMESPACE_URI));
		}

		#endregion
	}
}
