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
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
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
				if (vehicleType == VehicleCategoryHelper.Lorry) {
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
				}else if (vehicleType == VehicleCategoryHelper.PrimaryBus) {



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



			Bind<IMrfXmlType>().To<MRF_ConventionalLorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryVehicleType());

			Bind<IMrfXmlType>().To<MRF_HEV_Px_IHPC_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_Px_IHCP_LorryVehicleType());

			Bind<IMrfXmlType>().To<MRF_HEV_S2_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S2_VehicleType());

			Bind<IMrfXmlType>().To<MRF_HEV_S3_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S3_VehicleType());

			Bind<IMrfXmlType>().To<MRF_HEV_S4_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S4_VehicleType());


			Bind<IMrfXmlType>().To<MRF_PEV_E2_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E2_VehicleType());

			Bind<IMrfXmlType>().To<MRF_PEV_E3_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E3_VehicleType());

			Bind<IMrfXmlType>().To<MRF_PEV_E4_LorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E4_VehicleType());



			Bind<IMrfXmlGroup>().To<HEV_VehicleSequenceGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEVVehicleSequenceGroup());
			#region ADAS
			Bind<IMrfXmlType>().To<MRFConventionalAdasType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalADASType());
			Bind<IMrfXmlType>().To<MRFHevAdasType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEVADASType());


			#endregion ADAS
			Bind<IMrfXmlType>().To<MRFTorqueLimitationsType>().
				NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetEngineTorqueLimitationsType());

			Bind<IMrfXmlType>().To<MRFEngineType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetEngineType());

			#region Components
			Bind<IMrfXmlType>().To<MRFConventionalLorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryComponentsType());

			Bind<IMrfXmlType>().To<MRFHEV_Px_IHPC_LorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_Px_IHCP_ComponentsType());

			Bind<IMrfXmlType>().To<MRFHEV_S2_LorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S2_ComponentsType());

			Bind<IMrfXmlType>().To<MRFHEV_S3_LorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S3_ComponentsType());

			Bind<IMrfXmlType>().To<MRFHEV_S4_LorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S4_ComponentsType());

			Bind<IMrfXmlType>().To<MRFHEV_IEPC_S_LorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_IEPC_S_ComponentsType());
			#endregion

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

			Bind<IMrfXmlType>().To<MRFElectricMachinesType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetElectricMachinesType());

			Bind<IMrfXmlType>().To<MRFIepcSpecificationsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetIEPCSpecifications());


			Bind<IMrfXmlType>().To<MRFREESSSpecificationsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetREESSSpecificationsType());

			#region Auxiliaries
			Bind<IMrfXmlType>().To<MRFConventionalLorryAuxiliariesType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryAuxType());



			#region Auxiliaries

			Bind<IMrfXmlType>().To<MRFHevLorryAuxiliariesType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_LorryAuxiliariesType());

			Bind<IMrfXmlType>().To<MRFAxleGearType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAxleGearType());
#endregion


			#region XMLGroups

			Bind<IMrfXmlGroup>().To<GeneralVehicleOutputXmlGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetGeneralVehicleOutputGroup());

			Bind<IMrfXmlGroup>().To<LorryGeneralVehicleOutputXmlGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetGeneralLorryVehicleOutputGroup());
			Bind<IMrfXmlGroup>().To<ConventionalLorryVehicleXmlGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryVehicleOutputGroup());

			Bind<IMrfXmlGroup>().To<HEV_LorryVehicleOutputTypeGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_lorryVehicleOutputGroup());
			Bind<IMrfXmlGroup>().To<HEV_LorryVehicleOutputTypeSequenceGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_lorryVehicleOutputSequenceGroup());

			#endregion
		}

		#endregion
	}
}
