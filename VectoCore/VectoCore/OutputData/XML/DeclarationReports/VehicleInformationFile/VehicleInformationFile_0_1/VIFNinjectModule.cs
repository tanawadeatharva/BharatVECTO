using Ninject.Activation;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	internal class VIFNinjectModule : MRFNinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			LoadModule<ContextPreservationModule>();

			Bind<IVIFReportFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(nameCombinationMethod,
				6, 6, typeof(IVIFReportFactory).GetMethod(nameof(IVIFReportFactory.GetVIFReport)))).InSingletonScope();

			
			#region VehicleTypes
			
			Bind<IXmlTypeWriter>().To<ConventionalVehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetConventionalVehicleType());

			Bind<IXmlTypeWriter>().To<HevIepcSVehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevIepcSVehicleType());

			Bind<IXmlTypeWriter>().To<HevPxVehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevPxVehicleType());

			Bind<IXmlTypeWriter>().To<HevS2VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevS2VehicleType());

			Bind<IXmlTypeWriter>().To<HevS3VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevS3VehicleType());

			Bind<IXmlTypeWriter>().To<HevS4VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevS4VehicleType());

			Bind<IXmlTypeWriter>().To<IepcVehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetIepcVehicleType());

			Bind<IXmlTypeWriter>().To<PevE2VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevE2VehicleType());

			Bind<IXmlTypeWriter>().To<PevE3VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevE3VehicleType());

			Bind<IXmlTypeWriter>().To<PevE4VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevE4VehicleType());

			#endregion


			#region Component Group

			Bind<IXmlTypeWriter>().To<ConventionalComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetConventionalComponentType());

			Bind<IXmlTypeWriter>().To<HevIepcSComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevIepcSComponentVIFType());

			Bind<IXmlTypeWriter>().To<HevPxComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevPxComponentVIFType());

			Bind<IXmlTypeWriter>().To<HevS2ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevS2ComponentVIFType());

			Bind<IXmlTypeWriter>().To<HevS3ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevS3ComponentVIFType());

			Bind<IXmlTypeWriter>().To<HevS4ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevS4ComponentVIFType());

			Bind<IXmlTypeWriter>().To<PevE2ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevE2ComponentVIFType());

			Bind<IXmlTypeWriter>().To<PevE3ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevE3ComponentVIFType());

			Bind<IXmlTypeWriter>().To<PevE4ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevE4ComponentVIFType());

			Bind<IXmlTypeWriter>().To<IepcComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetIepcComponentVIFType());

			#endregion

			#region Vehicle Group

			Bind<IReportOutputGroup>().To<ConventionalVIFVehicleParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetConventionalVehicleGroup());

			Bind<IReportOutputGroup>().To<PrimaryBusGeneralParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPrimaryBusGeneralParameterGroup());

			Bind<IReportOutputGroup>().To<PrimaryBusChassisParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPrimaryBusChassisParameterGroup());

			Bind<IReportOutputGroup>().To<PrimaryBusRetarderParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPrimaryBusRetarderParameterGroup());

			Bind<IReportOutputGroup>().To<PrimaryBusXeVParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPrimaryBusXevParameterGroup());

			Bind<IReportOutputGroup>().To<HevIepcSVehicleParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevIepcSVehicleParameterGroup());

			Bind<IReportOutputGroup>().To<HevSxVehicleParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevSxVehicleParameterGroup());

			Bind<IReportOutputGroup>().To<IepcVehicleParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetIepcVehicleParameterGroup());

			Bind<IReportOutputGroup>().To<PevExVehicleParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevExVehicleParmeterGroup());

			Bind<IReportOutputGroup>().To<HevPxVehicleParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevPxVehicleParameterGroup());

			#endregion
			

			#region Components

			Bind<IXmlTypeWriter>().To<VIFAdasType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAdasType());

			Bind<IXmlTypeWriter>().To<VIFAngleDriveType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAngelDriveType());

			Bind<IXmlTypeWriter>().To<VIFAuxiliaryType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAuxiliaryType());

			Bind<IXmlTypeWriter>().To<VIFAuxiliaryHevSType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAuxiliaryHevSType());

			Bind<IXmlTypeWriter>().To<VIFAuxiliaryHevPType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAuxiliaryHevPType());

			Bind<IXmlTypeWriter>().To<VIFAuxiliaryIEPCType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAuxiliaryIEPCType());

			Bind<IXmlTypeWriter>().To<VIFAuxiliaryPEVType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAuxiliaryPEVType());

			Bind<IXmlTypeWriter>().To<VIFAxlegearType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAxlegearType());

			Bind<IXmlTypeWriter>().To<VIFAxleWheelsType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAxleWheelsType());

			Bind<IXmlTypeWriter>().To<VIFBoostingLimitationsType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetBoostingLimitationsType());

			Bind<IXmlTypeWriter>().To<VIFElectricEnergyStorageType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetElectricEnergyStorageType());

			Bind<IXmlTypeWriter>().To<VIFElectricMachineGENType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetElectricMachineGENType());

			Bind<IXmlElectricMachineSystemType>().To<XmlElectricMachineSystemMeasuredType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetElectricMachineSystemType());

			Bind<IXmlTypeWriter>().To<VIFElectricMachineType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetElectricMachineType());
			
			Bind<IXmlTypeWriter>().To<VIFElectricMotorTorqueLimitsType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetElectricMotorTorqueLimitsType());

			Bind<IXmlTypeWriter>().To<VIFEngineType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetEngineType());

			Bind<IXmlTypeWriter>().To<VIFTorqueConverterType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetTorqueConvertType());

			Bind<IXmlTypeWriter>().To<VIFIepcType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetIepcType());

			Bind<IXmlTypeWriter>().To<VIFTorqueLimitsType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetTorqueLimitsType());

			Bind<IXmlTypeWriter>().To<VIFTransmissionType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetTransmissionType());

			#endregion
		}


		private bool AccessedViaVIFFactory(IRequest request)
		{
			if (request.ParentRequest == null)
			{
				return false;
			}
			return typeof(IVIFReportFactory).IsAssignableFrom(request.ParentRequest.Service);
		}


		#endregion
	}
}
