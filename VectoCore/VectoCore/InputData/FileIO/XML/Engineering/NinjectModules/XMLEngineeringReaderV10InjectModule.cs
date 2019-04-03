using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Impl;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.NinjectModules
{
	public class XMLEngineeringReaderV10InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLEngineeringInputData>().To<XMLEngineeringInputDataProviderV10>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringInputDataProviderV10.NAMESPACE_URI));
			Bind<IXMLEngineeringInputReader>().To<XMLEngineeringInputReaderV10>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringInputReaderV10.NAMESPACE_URI));

			Bind<IXMLEngineeringJobInputData>().To<XMLEngineeringJobInputDataProviderV10>()
												.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringJobInputDataProviderV10.NAMESPACE_URI));

			Bind<IXMLJobDataReader>().To<XMLJobDataReaderV10>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLJobDataReaderV10.NAMESPACE_URI));

			Bind<IXMLEngineeringDriverData>().To<XMLEngineeringDriverDataProviderV10>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringDriverDataProviderV10.NAMESPACE_URI));

			Bind<IXMLDriverDataReader>().To<XMLDriverDataReaderV10>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLDriverDataReaderV10.NAMESPACE_URI));

			Bind<IXMLEngineeringGearshiftData>().To<XMLEngineeringGearshiftDataV10>()
												.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringGearshiftDataV10.NAMESPACE_URI));

			Bind<IXMLCyclesDataProvider>().To<XMLCyclesDataProviderV10>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLCyclesDataProviderV10.NAMESPACE_URI));

			Bind<IXMLEngineeringVehicleData>().To<XMLEngineeringVehicleDataProviderV10>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringVehicleDataProviderV10.NAMESPACE_URI));
			Bind<IXMLComponentsReader>().To<XMLComponentsEngineeringReaderV10>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLComponentsEngineeringReaderV10.NAMESPACE_URI));

			Bind<IXMLEngineeringVehicleComponentsData>().To<XMLEngineeringVehicleComponentsDataProviderV10>()
														.Named(
															XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringVehicleComponentsDataProviderV10.NAMESPACE_URI));

			Bind<IXMLAxleEngineeringData>().To<XMLAxleEngineeringDataV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLAxleEngineeringDataV10.NAMESPACE_URI));
			Bind<IXMLGearData>().To<XMLGearDataV10>().Named(XMLHelper.GetVersionFromNamespaceUri(XMLGearDataV10.NAMESPACE_URI));
			Bind<IXMLAuxiliaryData>().To<XMLAuxiliaryEngineeringDataV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLAuxiliaryEngineeringDataV10.NAMESPACE_URI));
			Bind<IXMLAxlegearData>().To<XMLEngineeringAxlegearDataProviderV10>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAxlegearDataProviderV10.NAMESPACE_URI));
			Bind<IXMLAngledriveData>().To<XMLEngineeringAngledriveDataProviderV10>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAngledriveDataProviderV10.NAMESPACE_URI));
			Bind<IXMLEngineData>().To<XMLEngineeringEngineDataProviderV10>()
								.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringEngineDataProviderV10.NAMESPACE_URI));
			Bind<IXMLRetarderData>().To<XMLEngineeringRetarderDataProviderV10>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringRetarderDataProviderV10.NAMESPACE_URI));
			Bind<IXMLAuxiliairesData>().To<XMLEngineeringAuxiliariesDataProviderV10>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAuxiliariesDataProviderV10.NAMESPACE_URI));
			Bind<IXMLGearboxData>().To<XMLEngineeringGearboxDataProviderV10>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringGearboxDataProviderV10.NAMESPACE_URI));
			Bind<IXMLAirdragData>().To<XMLEngineeringAirdragDataProviderV10>()
									.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAirdragDataProviderV10.NAMESPACE_URI));
			Bind<IXMLTorqueconverterData>().To<XMLEngineeringTorqueConverterDataProviderV10>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringTorqueConverterDataProviderV10.NAMESPACE_URI));
			Bind<IXMLAxlesData>().To<XMLEngineeringAxlesDataProviderV10>()
								.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAxlesDataProviderV10.NAMESPACE_URI));
			Bind<IXMLTyreData>().To<XMLTyreEngineeringDataProviderV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLTyreEngineeringDataProviderV10.NAMESPACE_URI));

			Bind<IXMLLookaheadData>().To<XMLEngineeringDriverLookAheadV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringDriverLookAheadV10.NAMESPACE_URI));

			Bind<IXMLOverspeedData>().To<XMLEngineeringOverspeedV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringOverspeedV10.NAMESPACE_URI));

			Bind<IXMLDriverAcceleration>().To<XMLDriverAccelerationV10>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLDriverAccelerationV10.NAMESPACE_URI));
		}

		#endregion
	}
}
