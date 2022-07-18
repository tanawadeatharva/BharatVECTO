using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataMultistageV01InjectModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IXMLMultistageInputDataProvider>().To<XMLDeclarationInputDataProviderMultistageV01>()
				.Named(XMLDeclarationInputDataProviderMultistageV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationMultistageJobInputData>().To<XMLDeclarationMultistageJobInputDataV01>()
				.Named(XMLDeclarationMultistageJobInputDataV01.QUALIFIED_XSD_TYPE);

			// -----------------------------------

			Bind<IXMLDeclarationMultistageVehicleInputDataReader>().To<XMLDeclarationMultistageInputReaderV01>()
				.Named(XMLDeclarationMultistageInputReaderV01.QUALIFIED_XSD_TYPE);


			Bind<IXMLMultistageJobReader>().To<XMLMultistageJobReaderV01>()
				.Named(XMLMultistageJobReaderV01.QUALIFIED_XSD_TYPE);

			// -----------------------------------

			Bind<IXMLPrimaryVehicleBusInputData>().To<XMLDeclarationMultistagePrimaryVehicleInputDataV01>()
				.Named(XMLDeclarationMultistagePrimaryVehicleInputDataV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationPrimaryBusAuxiliariesDataProviderV01>()
				.Named(XMLDeclarationPrimaryBusAuxiliariesDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationPrimaryVehicleBusInputDataReader>().To<XMLMultistagePrimaryVehicleReaderV01>()
				.Named(XMLMultistagePrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_Conventional_PrimaryVehicleBusJobInputDataProviderV01>()
				.Named(XMLDeclarationMultistage_Conventional_PrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusJobInputDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_HEV_Sx_PrimaryVehicleBusJobInputDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_Sx_PrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_HEV_IEPC_S_PrimaryVehicleBusJobInputDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_IEPC_S_PrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_HEV_Ex_PrimaryVehicleBusJobInputDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_Ex_PrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistageExemptedPrimaryVehicleBusJobInputDataProviderV01>()
                .Named(XMLDeclarationMultistageExemptedPrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

            Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01>()
				.Named(XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_HEV_Px_PrimaryVehicleReaderV01>()
				.Named(XMLJobDataMultistage_HEV_Px_PrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_HEV_Sx_PrimaryVehicleReaderV01>()
				.Named(XMLJobDataMultistage_HEV_Sx_PrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_HEV_IEPC_S_PrimaryVehicleReaderV01>()
				.Named(XMLJobDataMultistage_HEV_IEPC_S_PrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_HEV_Ex_PrimaryVehicleReaderV01>()
				.Named(XMLJobDataMultistage_HEV_Ex_PrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataMultistageExemptedPrimaryVehicleReaderV01>()
				.Named(XMLJobDataMultistageExemptedPrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_Conventional_PrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationMultistage_Conventional_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_HEV_Sx_PrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_Sx_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_HEV_IEPC_S_PrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_IEPC_S_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_HEV_Ex_PrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_Ex_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);


			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistageExemptedPrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationMultistageExemptedPrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);
			// -----------------------------------

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_Px_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_HEV_Px_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_S2_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_HEV_S2_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_S3_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_HEV_S3_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_S4_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_HEV_S4_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_IEPC_S_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_HEV_IEPC_S_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_E2_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_HEV_E2_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_E3_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_HEV_E3_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_E4_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_HEV_E4_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxReader>().To<XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01.GEARBOX_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAxlesReader>().To<XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01.AXLES_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>()
				.To<XMLDeclarationComponentsMultistagePrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationComponentsMultistagePrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);

			// -----------------------------------

			Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusEngineDataProviderV01>()
				.Named(XMLDeclarationMultistagePrimaryVehicleBusEngineDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxDeclarationInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusGearboxDataProviderV01>()
				.Named(XMLDeclarationMultistagePrimaryVehicleBusGearboxDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearData>().To<XMLMultistagePrimaryVehicleBusTransmissionDataV01>()
				.Named(XMLMultistagePrimaryVehicleBusTransmissionDataV01.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLAngledriveInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusAngledriveDataProviderV01>()
				.Named(XMLDeclarationMultistagePrimaryVehicleBusAngledriveDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLTorqueConverterDeclarationInputData>()
				.To<XMLDeclarationMultistagePrimaryTorqueConverterDataProviderV01>()
				.Named(XMLDeclarationMultistagePrimaryTorqueConverterDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleGearInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusAxlegearDataProviderV01>().Named(
				XMLDeclarationMultistagePrimaryVehicleBusAxlegearDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxlesDeclarationInputData>().To<XMLDeclarationAxlesDataProviderV01>().Named(
				XMLDeclarationAxlesDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLApplicationInformationData>().To<XMLDeclarationMultistagePrimaryVehicleBusApplicationInformationDataProviderV01>()
				.Named(XMLDeclarationMultistagePrimaryVehicleBusApplicationInformationDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLResultsInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusResultsInputDataProviderV01>()
				.Named(XMLDeclarationMultistagePrimaryVehicleBusResultsInputDataProviderV01.QUALIFIED_XSD_TYPE);


			// -----------------------------------

			Bind<IXMLMultistageEntryInputDataProvider>().To<XMLDeclarationMultistageTypeInputDataV01>()
				.Named(XMLDeclarationMultistageTypeInputDataV01.QUALIFIED_XSD_TYPE);


			Bind<IXMLMultistageReader>().To<XMLMultistageEntryReaderV01>()
				.Named(XMLMultistageEntryReaderV01.QUALIFIED_XSD_TYPE);

		}
	}
}