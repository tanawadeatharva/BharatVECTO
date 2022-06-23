using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ninject.Activation;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9
{
    internal class CIFNinjectModule : MRFNinjectModule
    {
		#region Overrides of VectoNinjectModule

		public override void Load()
		{
			LoadModule<ContextPreservationModule>();
			Bind<ICustomerInformationFileFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				nameCombinationMethod, 6, 6, typeof(ICustomerInformationFileFactory).GetMethod(nameof(ICustomerInformationFileFactory
					.GetCustomerReport)))).InSingletonScope();

			#region Lorry CIF
			Bind<IXMLCustomerReport>().To<ConventionalLorry_CIF>().Named(nameCombinationMethod(
				ToParams(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_PxLorry_CIF>().Named(nameCombinationMethod(
				ToParams(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_S2_Lorry_CIF>().Named(nameCombinationMethod(
				ToParams(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_S3_Lorry_CIF>().Named(nameCombinationMethod(
				ToParams(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_S4_Lorry_CIF>().Named(nameCombinationMethod(
				ToParams(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_IEPC_Lorry_CIF>().Named(nameCombinationMethod(
				ToParams(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S_IEPC, false,true, false)));

			Bind<IXMLCustomerReport>().To<PEV_E2_Lorry_CIF>().Named(nameCombinationMethod.Invoke(
				ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E3_Lorry_CIF>().Named(nameCombinationMethod.Invoke(
				ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E4_Lorry_CIF>().Named(nameCombinationMethod.Invoke(
				ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_IEPC_Lorry_CIF>().Named(nameCombinationMethod.Invoke(
				ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E_IEPC,
					false,
					true,
					false)));

			Bind<IXMLCustomerReport>().To<Exempted_Lorry_CIF>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					true,
					false,
					false)));
			#endregion
			#region CompletedBUsCIF

			Bind<IXMLCustomerReport>().To<Conventional_CompletedBusCIF>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<HEV_CompletedBusCIF>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.UNKNOWN,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<HEV_CompletedBusCIF>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_CompletedBusCIF>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.UNKNOWN,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<Exempted_CompletedBusCIF>().Named(nameCombinationMethod.Invoke(
				ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					true,
					false,
					false)));
			#endregion

			#region VehicleTypes
			Bind<IXmlTypeWriter>().To<CIFConventionalLorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventionalLorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEVPx_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_PxLorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_S2_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_S2_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_S3_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_S3_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_S4_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_S4_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_IEPC_S_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_IEPC_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_E2_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_E2_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_E3_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_E3_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_E4_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_E4_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_IEPC_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_IEPC_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_Exempted_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetExempted_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_ConventionalCompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventional_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_ExemptedCompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetExemptedCompletedBusVehicleType());
			#endregion
			#region VehicleGroups
			Bind<IReportVehicleOutputGroup>().To<GeneralVehicleOutputSequenceGroupCif>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetGeneralVehicleSequenceGroupWriter());
			Bind<IReportCompletedBusOutputGroup>().To<GeneralVehicleOutputSequenceGroupCif>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetGeneralVehicleSequenceGroupWriterCompletedBus());

			Bind<IReportOutputGroup>().To<LorryGeneralVehicleSequenceGroupCIF>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetLorryGeneralVehicleSequenceGroupWriter());
			Bind<IReportOutputGroup>().To<ConventionalLorryVehicleSequenceGroupCIF>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventionalLorryVehicleSequenceGroupWriter());
			Bind<IReportOutputGroup>().To<HEV_LorryVehicleTypeGroupCIF>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_LorryVehicleTypeGroup());
			Bind<IReportOutputGroup>().To<HEV_VehicleSequenceGroupWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_VehicleSequenceGroupWriter());
			Bind<IReportOutputGroup>().To<PEV_LorryVehicleTypeGroupCIF>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_LorryVehicleTypeGroup());
			Bind<IReportOutputGroup>().To<PEV_VehicleSequenceGroupWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_VehicleSequenceGroupWriter());
			Bind<IReportOutputGroup>().To<CompletedBusVehicleTypeGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetCompletedBusVehicleTypeGroup());
			
			#endregion
			#region ComponentGroups
			Bind<IReportOutputGroup>().To<EngineGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetEngineGroup());
			Bind<IReportOutputGroup>().To<TransmissionGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetTransmissionGroup());
			Bind<IReportOutputGroup>().To<AxleWheelsGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetAxleWheelsGroup());
			Bind<IReportOutputGroup>().To<ElectricMachineGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetElectricMachineGroup());
			Bind<IReportOutputGroup>().To<REESSGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetREESSGroup());
			Bind<IReportOutputGroup>().To<LorryAuxGroup>().When(AccessedViaCIFFactory).
				NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetLorryAuxGroup());
			Bind<IReportOutputGroup>().To<CompletedBusAuxGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetCompletedBusAuxGroup());

			#endregion




		}

		private bool AccessedViaCIFFactory(IRequest request)
		{
			if (request.ParentRequest == null) {
				return false;
			}
			return typeof(ICustomerInformationFileFactory).IsAssignableFrom(request.ParentRequest.Service);
		}

		#endregion
	}
}
