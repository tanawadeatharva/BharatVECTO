using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.LorryManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.CompletedBus;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Lorry;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.CompletedBus;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.Lorry;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9
{
    internal class MRFNinjectModule : AbstractNinjectModule
    {


		//IXMLManufacturerReport GetManufacturerReport(string vehicleType, VectoSimulationJobType jobType,
		//	ArchitectureID archId, bool exempted, bool iepc, bool ihpc);
		private object[] ToParams(string vehicleType, VectoSimulationJobType jobType, ArchitectureID archId,
			bool exempted, bool iepc, bool ihpc)
		{
			return new[] { (object)vehicleType, jobType, archId, exempted, iepc, ihpc};
		}
		private CombineArgumentsToNameInstanceProvider.CombineToName nameCombinationMethod = arguments => {
			string vehicleType = (string)arguments[0];
			VectoSimulationJobType jobType = (VectoSimulationJobType)arguments[1];
			ArchitectureID archId = (ArchitectureID)arguments[2];
			bool exempted = (bool)arguments[3];
			bool iepc = (bool)arguments[4];
			bool ihpc = (bool)arguments[5];



			string result = "";
			if (exempted) {
				result += exempted + vehicleType;
			} else {
				if (vehicleType == VehicleCategoryHelper.Lorry || vehicleType == VehicleCategoryHelper.PrimaryBus) {
					if (jobType == VectoSimulationJobType.ParallelHybridVehicle || ihpc) {
						result += "HEV-Px/IHPC";
					}else if (jobType == VectoSimulationJobType.SerialHybridVehicle) {
						result += "HEV-" + archId;
					}else if (jobType == VectoSimulationJobType.BatteryElectricVehicle) {
						if (iepc) {
							result += "PEV-IEPC";
						} else {
							result += "PEV" + archId;
						}
					}else if (jobType == VectoSimulationJobType.ConventionalVehicle) {
						result += "Conventional";
					}
				}else if (vehicleType == VehicleCategoryHelper.CompletedBus) {
					result += jobType;
				}

				result += vehicleType;
			}


			return result;
		};

		public override void Load()
		{
			Bind<IManufacturerReportFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(nameCombinationMethod, 
				6, 6, typeof(IManufacturerReportFactory).GetMethod(nameof(IManufacturerReportFactory.GetManufacturerReport)))).InSingletonScope();
			Bind<IXMLManufacturerReport>().To<ConventionalLorryManufacturerReport>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryManufacturerReport());
			Bind<IXMLManufacturerReport>().To<ConventionalLorryManufacturerReport>().Named(
				nameCombinationMethod.Invoke(ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.ConventionalVehicle, 
					ArchitectureID.UNKNOWN, 
					false, 
					false, 
					false)));
			Bind<IXMLManufacturerReport>().To<HEV_Px_IHPC_LorryManufacturerReport>()
				.Named(nameCombinationMethod.Invoke(ToParams(
					VehicleCategoryHelper.Lorry, 
					VectoSimulationJobType.ParallelHybridVehicle, 
					ArchitectureID.UNKNOWN, 
					false, 
					false, 
					true)));

			Bind<IXMLManufacturerReport>().To<HEV_S2_LorryManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2,
					false,
					false,
					false)));


			Bind<IXMLManufacturerReport>().To<HEV_S3_LorryManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3,
					false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_S4_LorryManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4,
					false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_IEPC_S_LorryManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S_IEPC,
					false,
					true,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_E2_LorryManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
					false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_E3_LorryManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
					false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_E4_LorryManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
					false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<Conventional_PrimaryBus_ManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_Px_IHPC_PrimaryBus_ManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN,
					false,
					false,
					true)));

			Bind<IXMLManufacturerReport>().To<HEV_S2_PrimaryBus_ManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2,
					false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_S3_PrimaryBus_ManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3,
					false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_S4_PrimaryBus_ManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4,
					false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_E2_PrimaryBus_ManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
					false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_E3_PrimaryBus_ManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
					false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_E4_PrimaryBus_ManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
					false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_IEPC_PrimaryBus_ManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.UNKNOWN,
					false,
					true,
					false)));

			Bind<IXMLManufacturerReport>().To<Conventional_CompletedBusManufacturerReport>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					false,
					true,
					false)));
			#region Vehicle

			Bind<IMrfXmlType>().To<MRF_ConventionalLorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryVehicleType());
			Bind<IMrfXmlType>().To<MRF_HEV_Px_IHPC_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_Px_IHCP_LorryVehicleType());
			Bind<IMrfXmlType>().To<MRF_HEV_S2_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S2_LorryVehicleType());
			Bind<IMrfXmlType>().To<MRF_HEV_S3_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S3_LorryVehicleType());
			Bind<IMrfXmlType>().To<MRF_HEV_S4_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S4_LorryVehicleType());
			Bind<IMrfXmlType>().To<MRF_PEV_E2_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E2_LorryVehicleType());
			Bind<IMrfXmlType>().To<MRF_PEV_E3_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E3_LorryVehicleType());
			Bind<IMrfXmlType>().To<MRF_PEV_E4_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E4_LorryVehicleType());

			Bind<IMrfXmlType>().To<MRF_Conventional_PrimaryBusVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventional_PrimaryBusVehicleType());
			Bind<IMrfXmlType>().To<MRF_HEV_Px_IHPC_PrimaryBusVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_Px_IHPC_PrimaryBusVehicleType());
			Bind<IMrfXmlType>().To<MRF_HEV_S2_PrimaryBusVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S2_PrimaryBusVehicleType());
			Bind<IMrfXmlType>().To<MRF_HEV_S3_PrimaryBusVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S3_PrimaryBusVehicleType());
			Bind<IMrfXmlType>().To<MRF_HEV_S4_PrimaryBusVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S4_PrimaryBusVehicleType());
			Bind<IMrfXmlType>().To<MRF_HEV_IEPC_S_PrimaryBusVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_IEPC_S_PrimaryBusVehicleType());
			Bind<IMrfXmlType>().To<MRF_PEV_E2_PrimaryBusVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E2_PrimaryBusVehicleType());
			Bind<IMrfXmlType>().To<MRF_PEV_E3_PrimaryBusVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E3_PrimaryBusVehicleType());
			Bind<IMrfXmlType>().To<MRF_PEV_E4_PrimaryBusVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E4_PrimaryBusVehicleType());
			Bind<IMrfXmlType>().To<MRF_PEV_IEPC_PrimaryBusVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_IEPC_PrimaryBusVehicleType());

			Bind<IMrfXmlType>().To<MRF_Conventional_CompletedBusVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventional_CompletedBusVehicleType());
			Bind<IMrfXmlType>().To<MRF_HEV_CompletedBusVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_CompletedBusVehicleType());
			Bind<IMrfXmlType>().To<MRF_PEV_CompletedBusVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_CompletedBusVehicleType());
			#endregion
			#region Components
			Bind<IMrfXmlType>().To<MRFConventionalLorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryComponentsType());
			Bind<IMrfXmlType>().To<MRFHEV_Px_IHPC_LorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_Px_IHCP_LorryComponentsType());
			Bind<IMrfXmlType>().To<MRFHEV_S2_LorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S2_LorryComponentsType());
			Bind<IMrfXmlType>().To<MRFHEV_S3_LorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S3_LorryComponentsType());
			Bind<IMrfXmlType>().To<MRFHEV_S4_LorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S4_LorryComponentsType());
			Bind<IMrfXmlType>().To<MRFHEV_IEPC_S_LorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_IEPC_S_LorryComponentsType());
			Bind<IMrfXmlType>().To<MRFPEV_E2_LorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E2_LorryComponentsType());

			Bind<IMrfXmlType>().To<MRFPEV_E3_LorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E3_LorryComponentsType());
			Bind<IMrfXmlType>().To<MRFPEV_E4_LorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E4_LorryComponentsType());

			Bind<IMrfXmlType>().To<MRFConventional_PrimaryBusComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventional_PrimaryBusComponentsType());
			Bind<IMrfXmlType>().To<MRFHEV_IEPC_S_PrimaryBusComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_IEPC_S_PrimaryBusComponentsType());
			Bind<IMrfXmlType>().To<MRFHEV_S2_PrimaryBusComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S2_PrimaryBusComponentsType());
			Bind<IMrfXmlType>().To<MRFHEV_S3_PrimaryBusComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S3_PrimaryBusComponentsType());
			Bind<IMrfXmlType>().To<MRFHEV_S4_PrimaryBusComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S4_PrimaryBusComponentsType());
			Bind<IMrfXmlType>().To<MRFHEV_IEPC_S_PrimaryBusComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_IEPC_S_PrimaryBusComponentsType());
			Bind<IMrfXmlType>().To<MRFPEV_E2_PrimaryBusComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E2_PrimaryBusComponentsType());
			Bind<IMrfXmlType>().To<MRFPEV_E3_PrimaryBusComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E3_PrimaryBusComponentsType());
			Bind<IMrfXmlType>().To<MRFPEV_E4_PrimaryBusComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E4_PrimaryBusComponentsType());

			#endregion


			Bind<IMrfXmlGroup>().To<HEV_VehicleSequenceGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_VehicleSequenceGroup());

			Bind<IMrfXmlGroup>().To<PEV_VehicleSequenceGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_VehicleSequenceGroup());
			#region ADAS
			Bind<IMRFAdasType>().To<MRFConventionalAdasType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalADASType());
			Bind<IMRFAdasType>().To<MRFHevAdasType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEVADASType());
			Bind<IMRFAdasType>().To<MRFPevAdasType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEVADASType());

			#endregion ADAS
			Bind<IMrfXmlType>().To<MRFTorqueLimitationsType>().
				NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetEngineTorqueLimitationsType());

			Bind<IMrfXmlType>().To<MRFEngineType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetEngineType());



			Bind<IMrfXmlType>().To<MRFTransmissionType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetTransmissionType());

			Bind<IMrfXmlType>().To<MRFRetarderType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetRetarderType());

			Bind<IMrfXmlType>().To<MRFTorqueConverterType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetTorqueConverterType());

			Bind<IMrfXmlType>().To<MRFAngleDriveType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAngleDriveType());

			Bind<IMrfXmlType>().To<MRFAirdragType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAirdragType());

			Bind<IMrfXmlType>().To<MRFAxleWheelsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAxleWheelsType());

			Bind<IMrfXmlType>().To<MRFAxleGearType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAxleGearType());

			Bind<IMrfXmlType>().To<MRFElectricMachinesType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetElectricMachinesType());

			Bind<IMrfXmlType>().To<MRFIepcSpecificationsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetIEPCSpecifications());

			Bind<IMrfXmlType>().To<MRFREESSSpecificationsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetREESSSpecificationsType());

			#region Auxiliaries
			Bind<IMRFLorryAuxiliariesType>().To<MRFConventionalLorryAuxiliariesType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryAuxType());
			Bind<IMRFLorryAuxiliariesType>().To<MRFHEV_LorryAuxiliariesType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_LorryAuxiliariesType());
			
			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusAuxType_Conventional>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusAuxType_Conventional());

			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusAuxType_HEV_P>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusAuxType_HEV_P());

			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusAuxType_HEV_S>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusAuxType_HEV_S());
			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusAuxType_PEV>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusAuxType_PEV());

			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusPneumaticSystemType_Conventional_Hev_Px>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusPneumaticSystemType_Conventional_HEV_Px());
			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusPneumaticSystemType_HEV_S>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusPneumaticSystemType_HEV_S());
			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusPneumaticSystemType_PEV_IEPC>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusPneumaticSystemType_PEV_IEPC());



			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusElectricSystemType_Conventional_HEV>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusElectricSystemType_Conventional_HEV());

			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusElectricSystemType_PEV>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusElectricSystemType_PEV());
			
			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusHVACSystemType_Conventional_HEV>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusHVACSystemType_Conventional_HEV());

			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusHVACSystemType_PEV>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusHVACSystemType_PEV());

			Bind<IMRFBusAuxiliariesType>().To<MRFConventionalCompletedBusAuxType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalCompletedBusAuxType());

			Bind<IMRFBusAuxiliariesType>().To<MRFConventionalCompletedBus_HVACSystemType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalCompletedBus_HVACSystemType());

			Bind<IMRFBusAuxiliariesType>().To<MRFCompletedBusElectricSystemType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetCompletedBusElectricSystemType());

			Bind<IMRFBusAuxiliariesType>().To<MRFConventionalCompletedBusAuxType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalCompletedBusAuxType());
			#region Groups

			Bind<IMrfVehicleGroup>().To<GeneralVehicleOutputXmlGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetGeneralVehicleOutputGroup());

			Bind<IMrfXmlGroup>().To<LorryGeneralVehicleOutputXmlGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetGeneralLorryVehicleOutputGroup());

			Bind<IMrfXmlGroup>().To<ConventionalLorryVehicleXmlGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryVehicleOutputGroup());
			
			Bind<IMrfXmlGroup>().To<PrimaryBusGeneralVehicleOutputGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusGeneralVehicleOutputGroup());

			Bind<IMrfXmlGroup>().To<HEVPrimaryBusVehicleOutputGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_PrimaryBusVehicleOutputGroup());

			Bind<IMrfXmlGroup>().To<PEVPrimaryBusVehicleOutputGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_PrimaryBusVehicleOutputGroup());

			Bind<IMrfXmlGroup>().To<HEV_LorryVehicleOutputTypeGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_lorryVehicleOutputGroup());

			Bind<IMrfXmlGroup>().To<PEV_LorryVehicleOutputTypeGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_lorryVehicleOutputGroup());
			Bind<IMrfXmlGroup>().To<HEV_LorryVehicleOutputTypeSequenceGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_lorryVehicleOutputSequenceGroup());

			Bind<IMrfVehicleGroup>().To<CompletedBusSequenceGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetCompletedBusSequenceGroup());

			Bind<IMrfVehicleGroup>().To<CompletedBusDimensionsSequenceGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetCompletedBusDimensionSequenceGroup());

			Bind<IMrfXmlGroup>().To<CompletedBusGeneralVehicleOutputGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetCompletedBusGeneralVehicleOutputGroup());


			Bind<IMrfBusAuxGroup>().To<CompletedBus_HVACSystem_Group>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetCompletedBus_HVACSystemGroup());

			#endregion
		}

		#endregion
	}
}
