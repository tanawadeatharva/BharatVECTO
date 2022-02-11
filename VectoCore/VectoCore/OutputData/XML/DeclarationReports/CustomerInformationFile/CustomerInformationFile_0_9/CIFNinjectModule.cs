using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ninject.Activation;
using Ninject.Extensions.Factory;
using Ninject.Extensions.NamedScope;
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
			Bind<ICustomerInformationFileFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				nameCombinationMethod, 6, 6, typeof(ICustomerInformationFileFactory).GetMethod(nameof(ICustomerInformationFileFactory
					.GetCustomerReport)))).InSingletonScope();

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
					ArchitectureID.UNKNOWN, false,true, false)));




			#region VehicleTypes
			Bind<IXmlTypeWriter>().To<CIFConventionalLorryVehicleWriter>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventionalLorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEVPx_LorryVehicleWriter>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_PxLorryVehicleType());

			#endregion
			#region VehicleGroups
			Bind<IMrfXmlGroup>().To<GeneralVehicleSequenceGroupCIF>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetGeneralVehicleSequenceGroupWriter());
			Bind<IMrfXmlGroup>().To<LorryGeneralVehicleSequenceGroupCIF>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetLorryGeneralVehicleSequenceGroupWriter());
			Bind<IMrfXmlGroup>().To<ConventionalLorryVehicleSequenceGroupCIF>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventionalLorryVehicleSequenceGroupWriter());


			#endregion
			#region ComponentGroups
			Bind<IMrfXmlGroup>().To<ComponentGroupWriters>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetEngineGroup());
			Bind<IMrfXmlGroup>().To<TransmissionGroup>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetTransmissionGroup());
			Bind<IMrfXmlGroup>().To<AxleWheelsGroup>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetAxleWheelsGroup());
			Bind<IMrfXmlGroup>().To<ElectricMachineGroup>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetElectricMachineGroup());
			Bind<IMrfXmlGroup>().To<REESSGroup>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetREESSGroup());
			#endregion

			
			

			
		}



		#endregion
	}
}
