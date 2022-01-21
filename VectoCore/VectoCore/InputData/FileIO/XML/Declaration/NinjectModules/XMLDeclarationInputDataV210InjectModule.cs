using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v210;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV210InjectModule : NinjectModule
	{
		#region Overrides of NinjectModlue

		public override void Load()
		{

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationComponentsDataProviderV210_Lorry>().Named(
				XMLDeclarationComponentsDataProviderV210_Lorry.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationConventionalCompletedBusDataProviderV210>()
				.Named(XMLDeclarationConventionalCompletedBusDataProviderV210.QUALIFIED_XSD_TYPE);
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationConventionalCompletedBusDataProviderV210>()
				.Named(XMLDeclarationHEVCompletedBusDataProviderV210.QUALIFIED_XSD_TYPE);
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationConventionalCompletedBusDataProviderV210>()
				.Named(XMLDeclarationPEVompletedBusDataProviderV210.QUALIFIED_XSD_TYPE);
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationConventionalCompletedBusDataProviderV210>()
				.Named(XMLDeclarationIEPCCompletedBusDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedCompletedBusDataProviderV210>()
				.Named(XMLDeclarationExemptedCompletedBusDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationConventionalHeavyLorryDataProviderV210>()
				.Named(XMLDeclarationConventionalHeavyLorryDataProviderV210.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHEVPxHeavyLorryDataProviderV210>()
				.Named(XMLDeclarationHEVPxHeavyLorryDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHEVSxHeavyLorryDataProviderV210>()
				.Named(XMLDeclarationHEVSxHeavyLorryDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHEVSxMediumLorryDataProviderV210>()
				.Named(XMLDeclarationHEVSxMediumLorryDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHEVPxMediumLorryDataProviderV210>()
				.Named(XMLDeclarationHEVPxMediumLorryDataProviderV210.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHEVPxPrimaryBusDataProviderV210>()
				.Named(XMLDeclarationHEVPxPrimaryBusDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHEVSxPrimaryBusDataProviderV210>()
				.Named(XMLDeclarationHEVSxPrimaryBusDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHEVIEPCSHeavyLorryDataProviderV210>()
				.Named(XMLDeclarationHEVIEPCSHeavyLorryDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHEVIEPCSMediumLorryDataProviderV210>()
				.Named(XMLDeclarationHEVIEPCSMediumLorryDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHEVIEPCSPrimaryBusDataProviderV210>()
				.Named(XMLDeclarationHEVIEPCSPrimaryBusDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationPEVHeavyLorryE2DataProviderV210>()
				.Named(XMLDeclarationPEVHeavyLorryE2DataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationPEVMediumLorryExDataProviderV210>()
				.Named(XMLDeclarationPEVMediumLorryExDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationPEVPrimaryBusDataProviderV210>()
				.Named(XMLDeclarationPEVPrimaryBusDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationIEPCHeavyLorryDataProviderV210>()
				.Named(XMLDeclarationIEPCHeavyLorryDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationIEPCMediumLorryDataProviderV210>()
				.Named(XMLDeclarationIEPCMediumLorryDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationIEPCPrimaryBusDataProviderV210>()
				.Named(XMLDeclarationIEPCPrimaryBusDataProviderV210.QUALIFIED_XSD_TYPE);


			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationCompletedBusComponentsDataProviderV210>()
				.Named(XMLDeclarationCompletedBusComponentsDataProviderV210.QUALIFIED_XSD_TYPE_CONVENTIONAL);
			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationCompletedBusComponentsDataProviderV210>()
				.Named(XMLDeclarationCompletedBusComponentsDataProviderV210.QUALIFIED_XSD_TYPE_xEV);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHEVPxLorryComponentsDataProviderV210>()
				.Named(XMLDeclarationHEVPxLorryComponentsDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHEVS2LorryComponentsDataProviderV210>()
				.Named(XMLDeclarationHEVS2LorryComponentsDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHEVSXLorryComponentsDataProviderV210>()
				.Named(XMLDeclarationHEVSXLorryComponentsDataProviderV210.QUALIFIED_HEV_S3_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHEVSXLorryComponentsDataProviderV210>()
				.Named(XMLDeclarationHEVSXLorryComponentsDataProviderV210.QUALIFIED_HEV_S4_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusHEVPxComponentsDataProviderV210>()
				.Named(XMLDeclarationPrimaryBusHEVPxComponentsDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusHEVS2ComponentDataProviderV210>()
				.Named(XMLDeclarationPrimaryBusHEVS2ComponentDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusHEVSxComponentDataProviderV210>()
				.Named(XMLDeclarationPrimaryBusHEVSxComponentDataProviderV210.QUALIFIED_HEV_S3_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusHEVSxComponentDataProviderV210>()
				.Named(XMLDeclarationPrimaryBusHEVSxComponentDataProviderV210.QUALIFIED_HEV_S4_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHeavyLorryHEVIEPCSComponentDataV210>()
				.Named(XMLDeclarationHeavyLorryHEVIEPCSComponentDataV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusHEVIEPCSComponentDataV210>()
				.Named(XMLDeclarationPrimaryBusHEVIEPCSComponentDataV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHeavyLorryPEVE2ComponentDataV210>()
				.Named(XMLDeclarationHeavyLorryPEVE2ComponentDataV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHeavyLorryPEVExComponentDataV210>()
				.Named(XMLDeclarationHeavyLorryPEVExComponentDataV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHeavyLorryPEVExComponentDataV210>()
				.Named(XMLDeclarationHeavyLorryPEVExComponentDataV210.QUALIFIED_XSD_PEV_E4_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusPEVE2ComponentDataV210>()
				.Named(XMLDeclarationPrimaryBusPEVE2ComponentDataV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusPEVExComponentDataV210>()
				.Named(XMLDeclarationPrimaryBusPEVExComponentDataV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusPEVExComponentDataV210>()
				.Named(XMLDeclarationPrimaryBusPEVExComponentDataV210.QUALIFIED_XSD_PEV_E4_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationIEPCHeavyLorryComponentDataV210>()
				.Named(XMLDeclarationIEPCHeavyLorryComponentDataV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationIEPCPrimaryBusComponentDataV210>()
				.Named(XMLDeclarationIEPCPrimaryBusComponentDataV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLComponentReaderV210_CompletedBus>().Named(XMLComponentReaderV210_CompletedBus.QUALIFIED_XSD_TYPE_CONVENTIONAL);
			Bind<IXMLComponentReader>().To<XMLComponentReaderV210_CompletedBus>().Named(XMLComponentReaderV210_CompletedBus.QUALIFIED_XSD_TYPE_xEV);

			Bind<IXMLComponentReader>().To<XMLComponentReaderV210_HEV_PxHeavyLorry>()
				.Named(XMLComponentReaderV210_HEV_PxHeavyLorry.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLComponentReaderV210_HEV_PxHeavyLorry>()
				.Named(XMLComponentReaderV210_HEV_PxHeavyLorry.QUALIFIED_XSD_HEV_Px_TYPE);

			Bind<IXMLComponentReader>().To<XMLPrimaryBusHEVPxDeclarationComponentReaderV201>()
				.Named(XMLPrimaryBusHEVPxDeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPrimaryBusHEVS2DeclarationComponentReaderV201>()
				.Named(XMLPrimaryBusHEVS2DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPrimaryBusHEVS3DeclarationComponentReaderV201>()
				.Named(XMLPrimaryBusHEVS3DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPrimaryBusHEVS4DeclarationComponentReaderV201>()
				.Named(XMLPrimaryBusHEVS4DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLHeavyLorryHEVS2DeclartionComponentReaderV201>().Named(XMLHeavyLorryHEVS2DeclartionComponentReaderV201.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLComponentReader>().To<XMLHeavyLorryHEVS3DeclarationComponentReaderV201>().Named(XMLHeavyLorryHEVS3DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLHeavyLorryHEVS4DeclarationComponentReaderV201>().Named(XMLHeavyLorryHEVS4DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLComponentReader>().To<XMLHeavyLorryHEVIEPCSDeclarationComponentReaderV201>()
				.Named(XMLHeavyLorryHEVIEPCSDeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPrimaryBusHEVIEPCSDeclarationComponentReaderV201>()
				.Named(XMLPrimaryBusHEVIEPCSDeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPEVHeavyLorryE2DeclarationComponentReaderV201>()
				.Named(XMLPEVHeavyLorryE2DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPEVHeavyLorryE3DeclarationComponentReaderV201>()
				.Named(XMLPEVHeavyLorryE3DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPEVHeavyLorryE4DeclarationComponentReaderV201>()
				.Named(XMLPEVHeavyLorryE4DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPEVPrimaryBusE2DeclarationComponentReaderV201>()
				.Named(XMLPEVPrimaryBusE2DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPEVPrimaryBusE3DeclarationComponentReaderV201>()
				.Named(XMLPEVPrimaryBusE3DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPEVPrimaryBusE4DeclarationComponentReaderV201>()
				.Named(XMLPEVPrimaryBusE4DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLIEPCHeavyLorryComponentReaderV201>()
				.Named(XMLIEPCHeavyLorryComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLIEPCPrimaryBusComponentReaderV201>()
				.Named(XMLIEPCPrimaryBusComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxReader>().To<XMLGearboxDeclarationComponentReaderV201>()
				.Named(XMLGearboxDeclarationComponentReaderV201.GEARBOX_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLAuxiliaryDeclarationComponentReaderV201>()
				.Named(XMLAuxiliaryDeclarationComponentReaderV201.AUXILIARIES_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLAuxiliaryDeclarationComponentReaderV201>()
				.Named(XMLAuxiliaryDeclarationComponentReaderV201.AUXILIARIES_READER_IEPC_PRIMARY_QUALIFIED_XSD_TYPE);

			Bind<IXMLElectricMachineSystemReader>().To<XMLElectricMachineSystemReaderV210>().Named(XMLElectricMachineSystemReaderV210.QUALIFIED_XSD_TYPE);
			Bind<IXMLElectricMachineSystemReader>().To<XMLElectricMachineSystemReaderV210>().Named(XMLElectricMachineSystemReaderV210.QUALIFIED_GEN_XSD_TYPE);

			Bind<IXMLREESSReader>().To<XMLREESSReaderV210>().Named(XMLREESSReaderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLPTOReader>().To<XMLPTOReaderV210>().Named(XMLPTOReaderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationCompletedBusAuxiliariesDataProviderV210>()
				.Named(XMLDeclarationCompletedBusAuxiliariesDataProviderV210.QUALIFIED_XSD_TYPE_CONVENTIONAL);
			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationCompletedBusAuxiliariesDataProviderV210>()
				.Named(XMLDeclarationCompletedBusAuxiliariesDataProviderV210.QUALIFIED_XSD_TYPE_xEV);

			Bind<IXMLAirdragDeclarationInputData>().To<XMLDeclarationAirdragDataProviderV210>().Named(
				XMLDeclarationAirdragDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV210_Lorry>().Named(
				XMLDeclarationAuxiliariesDataProviderV210_Lorry.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV210_Lorry>().Named(
				XMLDeclarationAuxiliariesDataProviderV210_Lorry.QUALIFIED_XSD_HEV_P_TYPE);

			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV210_Lorry>().Named(
				XMLDeclarationAuxiliariesDataProviderV210_Lorry.QUALIFIED_XSD_HEV_S_TYPE);

			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV210_Lorry>().Named(
				XMLDeclarationAuxiliariesDataProviderV210_Lorry.QUALIFIED_XSD_PEV_TYPE);
			
			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV210_Lorry>().Named(
				XMLDeclarationAuxiliariesDataProviderV210_Lorry.QUALIFIED_XSD_IEPC_TYPE);

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV210_Lorry>()
				.Named(XMLAuxiliaryDeclarationDataProviderV210_Lorry.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV210_Lorry>()
				.Named(XMLAuxiliaryDeclarationDataProviderV210_Lorry.QUALIFIED_XSD_HEV_P_TYPE);
			
			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV210_Lorry>()
				.Named(XMLAuxiliaryDeclarationDataProviderV210_Lorry.QUALIFIED_XSD_HEV_S_TYPE);

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV210_Lorry>()
				.Named(XMLAuxiliaryDeclarationDataProviderV210_Lorry.QUALIFIED_XSD_PEV_E2_TYPE);

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV210_Lorry>()
				.Named(XMLAuxiliaryDeclarationDataProviderV210_Lorry.QUALIFIED_XSD_IEPC_TYPE);

			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataProviderV210>().Named(XMLDeclarationADASDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataProviderV210>().Named(XMLDeclarationADASDataProviderV210.QUALIFIED_XSD_IEPC_TYPE);

			Bind<IXMLPTOTransmissionInputData>()
				.To<XMLDeclarationPTODataProviderV210>().Named(XMLDeclarationPTODataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationConventionalPrimaryBusVehicleDataProviderV210>()
				.Named(XMLDeclarationConventionalPrimaryBusVehicleDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedHeavyLorryDataProviderV210>()
				.Named(XMLDeclarationExemptedHeavyLorryDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedMediumLorryDataProviderV210>()
				.Named(XMLDeclarationExemptedMediumLorryDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedPrimaryBusDataProviderV210>()
				.Named(XMLDeclarationExemptedPrimaryBusDataProviderV210.QUALIFIED_XSD_TYPE);


			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusComponentsDataProviderV210>()
				.Named(XMLDeclarationPrimaryBusComponentsDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationPrimaryBusAuxiliariesDataProviderV210>()
				.Named(XMLDeclarationPrimaryBusAuxiliariesDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationPrimaryBusAuxiliariesDataProviderV210>()
				.Named(XMLDeclarationPrimaryBusAuxiliariesDataProviderV210.QUALIFIED_XSD_HEV_P_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationPrimaryBusAuxiliariesDataProviderV210>()
				.Named(XMLDeclarationPrimaryBusAuxiliariesDataProviderV210.QUALIFIED_XSD_HEV_S_TYPE);
			
			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationPrimaryBusAuxiliariesDataProviderV210>()
				.Named(XMLDeclarationPrimaryBusAuxiliariesDataProviderV210.QUALIFIED_XSD_PEV_E2_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationPrimaryBusAuxiliariesDataProviderV210>()
				.Named(XMLDeclarationPrimaryBusAuxiliariesDataProviderV210.QUALIFIED_XSD_IEPC_PRIMARY_BUS_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationConventionalMediumLorryVehicleDataProviderV210>()
				.Named(XMLDeclarationConventionalMediumLorryVehicleDataProviderV210.QUALIFIED_XSD_TYPE);

			Bind<IXMLElectricMachinesDeclarationInputData>().To<XMLElectricMachinesDeclarationInputDataProvider>()
				.Named(XMLElectricMachinesDeclarationInputDataProvider.QUALIFIED_XSD_TYPE);

			Bind<IXMLElectricMachinesDeclarationInputData>().To<XMLElectricMachinesDeclarationInputDataProvider>()
				.Named(XMLElectricMachinesDeclarationInputDataProvider.QUALIFIED_GEN_XSD_TYPE);

			Bind<IXMLElectricMotorDeclarationInputData>().To<XMLElectricMotorDeclarationInputDataProviderV2101>()
				.Named(XMLElectricMotorDeclarationInputDataProviderV2101.QUALIFIED_XSD_TYPE);

			Bind<IXMLElectricMotorDeclarationInputData>().To<XMLElectricMotorSystemStandardDeclarationInputDataProviderV2101>()
				.Named(XMLElectricMotorSystemStandardDeclarationInputDataProviderV2101.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLElectricMotorDeclarationInputData>().To<XMLElectricMotorIHPCDeclarationInputDataProviderV2101>()
				.Named(XMLElectricMotorIHPCDeclarationInputDataProviderV2101.QUALIFIED_XSD_TYPE);

			Bind<IXMLIEPCInputData>().To<XMLElectricMotorIEPCIInputDataProviderV2101>()
				.Named(XMLElectricMotorIEPCIInputDataProviderV2101.QUALIFIED_XSD_TYPE);

			Bind<IXMLIEPCInputData>().To<XMLElectricMotorIEPCIStandardInputDataProviderV2101>()
				.Named(XMLElectricMotorIEPCIStandardInputDataProviderV2101.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLElectricStorageSystemDeclarationInputData>().To<XMLElectricStorageSystemDeclarationInputData>()
				.Named(XMLElectricStorageSystemDeclarationInputData.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLBatteryPackDeclarationInputData>().To<XMLBatteryPackDeclarationDeclarationInputData>()
				.Named(XMLBatteryPackDeclarationDeclarationInputData.QUALIFIED_XSD_TYPE);

			Bind<IXMLSuperCapDeclarationInputData>().To<XMLSuperCapDeclarationInputData>()
				.Named(XMLSuperCapDeclarationInputData.QUALIFIED_XSD_TYPE);

			Bind<IXMLADCDeclarationInputData>().To<XMLADCDeclarationInputDataV2101>()
				.Named(XMLADCDeclarationInputDataV2101.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLAuxiliaryReader>().To<XMLComponentReaderV210_Lorry>()
				.Named(XMLComponentReaderV210_Lorry.AUXILIARIES_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLComponentReaderV210_Lorry>()
				.Named(XMLComponentReaderV210_Lorry.AUXILIARIES_READER_HEV_P_QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLComponentReaderV210_Lorry>()
				.Named(XMLComponentReaderV210_Lorry.AUXILIARIES_READER_HEV_S2_QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLComponentReaderV210_Lorry>()
				.Named(XMLComponentReaderV210_Lorry.AUXILIARIES_READER_PEV_E2_QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxDeclarationInputData>().To<XMLDeclarationGearboxDataProviderV2101>()
				.Named(XMLDeclarationGearboxDataProviderV2101.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>()
				.To<XMLComponentReaderV210_PrimaryBus>().Named(XMLComponentReaderV210_PrimaryBus.QUALIFIED_XSD_TYPE);

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
