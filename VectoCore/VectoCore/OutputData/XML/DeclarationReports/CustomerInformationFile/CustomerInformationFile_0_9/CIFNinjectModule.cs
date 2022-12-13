using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ninject.Activation;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
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
				new CombineArgumentsToNameInstanceProvider.MethodSettings()
				{
					combineToNameDelegate = VehicleTypeAndArchitectureStringHelper.CreateName,
					skipArguments = 6,
					takeArguments = 6,
					methods = new[] {
						typeof(ICustomerInformationFileFactory).GetMethod(
							nameof(ICustomerInformationFileFactory.GetCustomerReport))
					}
				})).InSingletonScope();

			#region Lorry CIF
			Bind<IXMLCustomerReport>().To<ConventionalLorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_PxLorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_S2_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_S3_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_S4_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_IEPC_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S_IEPC, false,true, false)));

			Bind<IXMLCustomerReport>().To<PEV_E2_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E3_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E4_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_IEPC_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E_IEPC,
					false,
					true,
					false)));

			Bind<IXMLCustomerReport>().To<Exempted_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					true,
					false,
					false)));

			#region MediumLorryCIF

			Bind<IXMLCustomerReport>().To<ConventionalLorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Van, VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_PxLorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Van, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_S2_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Van, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_S3_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Van, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_S4_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Van, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4, false, false, false)));

			Bind<IXMLCustomerReport>().To<HEV_IEPC_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Van, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S_IEPC, false, true, false)));

			Bind<IXMLCustomerReport>().To<PEV_E2_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E3_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E4_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_IEPC_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E_IEPC,
					false,
					true,
					false)));

			Bind<IXMLCustomerReport>().To<Exempted_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Van,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					true,
					false,
					false)));

			#endregion

			#endregion

			#region CompletedBUsCIF

			Bind<IXMLCustomerReport>().To<Conventional_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<HEV_Px_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<HEV_S2_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<HEV_S3_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<HEV_S4_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4,
					false,
					false,
					false)));


			Bind<IXMLCustomerReport>().To<HEV_IEPC_S_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S_IEPC,
					false,
					true,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E2_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E3_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E4_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_IEPC_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E_IEPC,
					false,
					true,
					false)));

			Bind<IXMLCustomerReport>().To<Exempted_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					true,
					false,
					false)));
			#endregion

			#region VehicleTypes
			Bind<IXmlTypeWriter>().To<CIFConventionalLorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventionalLorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEVPx_LorryVehicleWriter>().When(AccessedViaCIFFactory);

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

			Bind<IXmlTypeWriter>().To<CIF_Conventional_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventional_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_Px_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_Px_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_IHPC_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_IHPC_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_S2_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_S2_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_S3_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_S3_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_S4_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_S4_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_IEPC_S_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_IEPC_S_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_E2_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_E2_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_E3_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_E3_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_E4_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_E4_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_IEPC_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_IEPC_CompletedBusVehicleType());

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
			Bind<IReportOutputGroup>().To<HEV_LorryVehicleSequenceGroupWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_LorryVehicleSequenceGroupWriter());
			Bind<IReportOutputGroup>().To<PEV_LorryVehicleTypeGroupCIF>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_LorryVehicleTypeGroup());
			Bind<IReportOutputGroup>().To<PEV_LorryVehicleSequenceGroupWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_LorryVehicleSequenceGroupWriter());
			Bind<IReportOutputGroup>().To<CompletedBusVehicleTypeGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetCompletedBusVehicleTypeGroup());
			Bind<IReportOutputGroup>().To<PEVCompletedBusVehicleTypeGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEVCompletedBusVehicleTypeGroup());
			Bind<IReportOutputGroup>().To<ExemptedCompletedBusVehicleTypeGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetExemptedCompletedBusVehicleTypeGroup());

			Bind<IReportOutputGroup>().To<HEV_CompletedBusVehicleSequenceGroupWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_CompletedBusVehicleSequenceGroupWriter());
			Bind<IReportOutputGroup>().To<PEV_CompletedBusVehicleSequenceGroupWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_CompletedBusVehicleSequenceGroupWriter());
			Bind<IReportOutputGroup>().To<ConventionalCompletedBusVehicleSequenceGroupCIF>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventionalCompletedBusVehicleSequenceGroupWriter());

			#endregion
			#region ComponentGroups
			Bind<IReportOutputGroup>().To<EngineGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetEngineGroup());
			Bind<IReportOutputGroup>().To<TransmissionGroupWithGearbox>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetTransmissionGroup());
			Bind<IReportOutputGroup>().To<TransmissionGroupWithoutGearbox>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetTransmissionGroupNoGearbox());
			Bind<IReportOutputGroup>().To<IEPCTransmissionGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetIEPCTransmissionGroup());
			Bind<IReportOutputGroup>().To<AxleWheelsGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetAxleWheelsGroup());
			Bind<IReportOutputGroup>().To<ElectricMachineGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetElectricMachineGroup());
			Bind<IReportOutputGroup>().To<REESSGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetREESSGroup());
			Bind<IReportOutputGroup>().To<LorryAuxGroup>().When(AccessedViaCIFFactory).
				NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetLorryAuxGroup());
			Bind<IReportOutputGroup>().To<ConventionalCompletedBusAuxGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventionalCompletedBusAuxGroup());
			Bind<IReportOutputGroup>().To<HEV_Px_IHPCompletedBusAuxGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_Px_IHPC_CompletedBusAuxGroup());
			Bind<IReportOutputGroup>().To<HEV_Sx_CompletedBusAuxGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_Sx_CompletedBusAuxGroup());
			Bind<IReportOutputGroup>().To<PEVCompletedBusAuxGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_CompletedBusAuxGroup());


			#endregion

			#region ADAS
			Bind<ICIFAdasType>().To<CIFConventionalAdasType>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventionalADASType());
			Bind<ICIFAdasType>().To<CIFHevAdasType>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEVADASType());
			Bind<ICIFAdasType>().To<CIFPevAdasType>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEVADASType());

			#endregion ADAS


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


	internal class CIFResultsNinjectModule : NinjectModule
	{
		private VehicleTypeAndArchitectureStringHelperResults _namingHelper =
			new VehicleTypeAndArchitectureStringHelperResults();

		public override void Load()
		{
			Bind<IResultsWriterFactory>().To<ResultWriterFactory>().InSingletonScope();

			Bind<IInternalResultWriterFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = _namingHelper.CreateName,
					skipArguments = 1,
					takeArguments = 1,
					methods = new[] {
						typeof(IInternalResultWriterFactory).GetMethod(nameof(IInternalResultWriterFactory.GetCIFResultsWriter))
					}
				})).InSingletonScope();

			Bind<IResultsWriter>().To<CIFResultsWriter.ConventionalLorry>().Named(
				_namingHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.Conventional, false));
			Bind<IResultsWriter>().To<CIFResultsWriter.HEVNonOVCLorry>().Named(
				_namingHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.Hybrid, false));
			Bind<IResultsWriter>().To<CIFResultsWriter.HEVOVCLorry>().Named(
				_namingHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.Hybrid, true));
			Bind<IResultsWriter>().To<CIFResultsWriter.PEVLorry>().Named(
				_namingHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.PureElectric, true));
			Bind<IResultsWriter>().To<CIFResultsWriter.ExemptedResultsWriter>().Named(
				_namingHelper.GetName(VehicleCategoryHelper.Lorry, true));

			Bind<IResultsWriter>().To<CIFResultsWriter.ConventionalBus>().Named(
				_namingHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.Conventional, false));
			Bind<IResultsWriter>().To<CIFResultsWriter.HEVNonOVCBus>().Named(
				_namingHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.Hybrid, false));
			Bind<IResultsWriter>().To<CIFResultsWriter.HEVOVCBus>().Named(
				_namingHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.Hybrid, true));
			Bind<IResultsWriter>().To<CIFResultsWriter.PEVBus>().Named(
				_namingHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.PureElectric, true));
			Bind<IResultsWriter>().To<CIFResultsWriter.ExemptedResultsWriter>().Named(
				_namingHelper.GetName(VehicleCategoryHelper.CompletedBus, true));

			Bind<ICifResultsWriterFactory>().ToFactory().InSingletonScope();

			Bind<IElectricRangeWriter>().To<ElectricRangeWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetElectricRangeWriter());

			// -- Lorry

			Bind<IResultGroupWriter>().To<LorryConvResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryConvSuccessResultWriter());
			Bind<IResultGroupWriter>().To<LorryHEVNonOVCResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryHEVNonOVCSuccessResultWriter());
			Bind<IResultGroupWriter>().To<LorryHEVOVCResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryHEVOVCSuccessResultWriter());
			Bind<IResultGroupWriter>().To<LorryPEVResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryPEVSuccessResultWriter());
			Bind<IResultGroupWriter>().To<ErrorResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryErrorResultWriter());

			Bind<IResultGroupWriter>().To<ResultMissionWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetMissionWriter());
			Bind<IResultGroupWriter>().To<ResultSimulationParameterLorryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorrySimulationParameterWriter());

			Bind<IResultGroupWriter>().To<LorryConvTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryConvTotalWriter());
			Bind<IResultGroupWriter>().To<LorryHEVNonOVCTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryHEVNonOVCTotalWriter());
			Bind<IResultGroupWriter>().To<LorryHEVOVCChargeDepletingWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryHEVOVCResultWriterChargeDepleting());
			Bind<IResultGroupWriter>().To<LorryHEVOVCChargeSustainingWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryHEVOVCResultWriterChargeSustaining());
			Bind<IResultGroupWriter>().To<LorryHEVOVCTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryHEVOVCTotalWriter());
			Bind<IResultGroupWriter>().To<LorryPEVTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryPEVTotalWriter());


			Bind<IFuelConsumptionWriter>().To<LorryFuelConsumptionWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetFuelConsumptionLorry());
			Bind<IElectricEnergyConsumptionWriter>().To<LorryElectricEnergyConsumptionWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetElectricEnergyConsumptionLorry());
			Bind<ICO2Writer>().To<LorryCO2Writer>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetCO2ResultLorry());

			Bind<ICifSummaryWriter>().To<LorryConvSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryConvSummaryWriter());
			Bind<ICifSummaryWriter>().To<LorryHEVNonOVCSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryHEVNonOVCSummaryWriter());
			Bind<ICifSummaryWriter>().To<LorryHEVOVCSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryHEVOVCSummaryWriter());
			Bind<ICifSummaryWriter>().To<LorryPEVSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetLorryPEVSummaryWriter());


			// -- Bus

			Bind<IResultGroupWriter>().To<BusConvResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusConvSuccessResultWriter());
			Bind<IResultGroupWriter>().To<BusHEVNonOVCResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusHEVNonOVCSuccessResultWriter());
			Bind<IResultGroupWriter>().To<BusHEVOVCResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusHEVOVCSuccessResultWriter());
			Bind<IResultGroupWriter>().To<BusPEVResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusPEVSuccessResultWriter());
			Bind<IResultGroupWriter>().To<ErrorResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusErrorResultWriter());

			Bind<IResultGroupWriter>().To<ResultSimulationParameterBusWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusSimulationParameterWriter());

			Bind<IResultGroupWriter>().To<BusConvTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusConvTotalWriter());
			Bind<IResultGroupWriter>().To<BusHEVNonOVCTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusHEVNonOVCTotalWriter());
			Bind<IResultGroupWriter>().To<BusOVCChargeDepletingWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusHEVOVCResultWriterChargeDepleting());
			Bind<IResultGroupWriter>().To<BusOVCChargeSustainingWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusHEVOVCResultWriterChargeSustaining());
			Bind<IResultGroupWriter>().To<BusOVCTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusHEVOVCTotalWriter());
			Bind<IResultGroupWriter>().To<BusPEVTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusPEVTotalWriter());
			


			Bind<IFuelConsumptionWriter>().To<BusFuelConsumptionWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetFuelConsumptionBus());
			Bind<IElectricEnergyConsumptionWriter>().To<BusElectricEnergyConsumptionWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetElectricEnergyConsumptionBus());
			Bind<ICO2Writer>().To<BusCO2Writer>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetCO2ResultBus());
			Bind<ICO2Writer>().To<BusPEVCO2Writer>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetCO2ResultPEVBus());

			Bind<ICifSummaryWriter>().To<BusConvSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusConvSummaryWriter());
			Bind<ICifSummaryWriter>().To<BusHEVNonOVCSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusHEVNonOVCSummaryWriter());
			Bind<ICifSummaryWriter>().To<BusHEVOVCSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusHEVOVCSummaryWriter());
			Bind<ICifSummaryWriter>().To<BusPEVSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICifResultsWriterFactory c) => c.GetBusPEVSummaryWriter());

		}

		private bool AccessedViaCIFResultsWriterFactory(IRequest request)
		{
			if (request.ParentRequest == null) {
				return false;
			}
			return typeof(ICifResultsWriterFactory).IsAssignableFrom(request.ParentRequest.Service);
		}
	}
}
