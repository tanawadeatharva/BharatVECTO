using System;
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
											.Named(XMLEngineeringInputDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLEngineeringInputReader>().To<XMLEngineeringInputReaderV10>()
											.Named(XMLEngineeringInputReaderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringJobInputData>().To<XMLEngineeringJobInputDataProviderV10>()
												.Named(XMLEngineeringJobInputDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataReaderV10>()
									.Named(XMLJobDataReaderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringDriverData>().To<XMLEngineeringDriverDataProviderV10>()
											.Named(XMLEngineeringDriverDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLDriverDataReader>().To<XMLDriverDataReaderV10>()
										.Named(XMLDriverDataReaderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringGearshiftData>().To<XMLEngineeringGearshiftDataV10>()
												.Named(XMLEngineeringGearshiftDataV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLCyclesDataProvider>().To<XMLCyclesDataProviderV10>()
										.Named(XMLCyclesDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringVehicleData>().To<XMLEngineeringVehicleDataProviderV10>()
											.Named(XMLEngineeringVehicleDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLComponentsReader>().To<XMLComponentsEngineeringReaderV10>()
										.Named(XMLComponentsEngineeringReaderV10.QUALIFIED_XSD_TYPE_COMPONENTS);
			Bind<IXMLAxlesReader>().To<XMLComponentsEngineeringReaderV10>()
									.Named(XMLComponentsEngineeringReaderV10.QUALIFIED_XSD_TYPE_AXLES);
			Bind<IXMLAxleReader>().To<XMLComponentsEngineeringReaderV10>()
								.Named(XMLComponentsEngineeringReaderV10.QUALIFIED_XSD_TYPE_AXLE);
			Bind<IXMLGearboxReader>().To<XMLComponentsEngineeringReaderV10>()
									.Named(XMLComponentsEngineeringReaderV10.QUALIFIED_XSD_TYPE_GEARBOX);
			Bind<IXMLAuxiliaryReader>().To<XMLComponentsEngineeringReaderV10>()
										.Named(XMLComponentsEngineeringReaderV10.QUALIFIED_XSD_TYPE_AUXILIARY);


			Bind<IXMLEngineeringVehicleComponentsData>().To<XMLEngineeringVehicleComponentsDataProviderV10>()
														.Named(
															XMLEngineeringVehicleComponentsDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleEngineeringData>().To<XMLAxleEngineeringDataV10>().Named(
				XMLAxleEngineeringDataV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLGearData>().To<XMLGearDataV10>().Named(XMLGearDataV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLAuxiliaryData>().To<XMLAuxiliaryEngineeringDataV10>().Named(
				XMLAuxiliaryEngineeringDataV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLAxlegearData>().To<XMLEngineeringAxlegearDataProviderV10>()
									.Named(XMLEngineeringAxlegearDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLAngledriveData>().To<XMLEngineeringAngledriveDataProviderV10>()
									.Named(XMLEngineeringAngledriveDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLEngineData>().To<XMLEngineeringEngineDataProviderV10>()
								.Named(XMLEngineeringEngineDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLRetarderData>().To<XMLEngineeringRetarderDataProviderV10>()
									.Named(XMLEngineeringRetarderDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLAuxiliairesData>().To<XMLEngineeringAuxiliariesDataProviderV10>()
										.Named(XMLEngineeringAuxiliariesDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLGearboxData>().To<XMLEngineeringGearboxDataProviderV10>()
									.Named(XMLEngineeringGearboxDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLAirdragData>().To<XMLEngineeringAirdragDataProviderV10>()
									.Named(XMLEngineeringAirdragDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLTorqueconverterData>().To<XMLEngineeringTorqueConverterDataProviderV10>()
											.Named(XMLEngineeringTorqueConverterDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLAxlesData>().To<XMLEngineeringAxlesDataProviderV10>()
								.Named(XMLEngineeringAxlesDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLTyreData>().To<XMLTyreEngineeringDataProviderV10>().Named(
				XMLTyreEngineeringDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLLookaheadData>().To<XMLEngineeringDriverLookAheadV10>().Named(
				XMLEngineeringDriverLookAheadV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLOverspeedData>().To<XMLEngineeringOverspeedV10>().Named(
				XMLEngineeringOverspeedV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLDriverAcceleration>().To<XMLDriverAccelerationV10>()
										.Named(XMLDriverAccelerationV10.QUALIFIED_XSD_TYPE);
		}

		#endregion
	}

	public class XMLEngineeringReaderV11InjectModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IXMLEngineData>().To<XMLEngineeringEngineDataProviderV11>()
								.Named(XMLEngineeringEngineDataProviderV11.QUALIFIED_XSD_TYPE);

		}
	}
}
