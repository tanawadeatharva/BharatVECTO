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


			Bind<IXmlTypeWriter>().To<VIFConventionalVehicle>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetConventionalLorryVehicleType());


			#endregion

			#region Vehicle Group

			Bind<IReportOutputGroup>().To<ConventionalVIFVehicleGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetConventionalVehicleGroup());

			Bind<IReportOutputGroup>().To<PrimaryBusGeneralParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPrimaryBusGeneralParameterGroup());

			Bind<IReportOutputGroup>().To<PrimaryBusChassisParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPrimaryBusChassisParameterGroup());

			Bind<IReportOutputGroup>().To<PrimaryBusRetarderParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPrimaryBusRetarderParameterGroup());

			Bind<IReportOutputGroup>().To<PrimaryBusXeVParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPrimaryBusXeVParameterGroup());

			Bind<IReportOutputGroup>().To<HevIepcSVehicleGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevIepcSVehicleGroup());

			Bind<IReportOutputGroup>().To<HevSxVehicleGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevSxVehicleGroup());

			Bind<IReportOutputGroup>().To<IepcVehicleGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetIepcVehicleGroup());

			Bind<IReportOutputGroup>().To<PEVVehicleGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPEVVehicleGroup());

			#endregion



			#region Components

			Bind<IXmlTypeWriter>().To<VIFAdasType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAdasType());
			Bind<IXmlTypeWriter>().To<VIFAngleDriveType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAngelDriveType());
			Bind<IXmlTypeWriter>().To<VIFAuxiliaryType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAuxiliaryType());
			Bind<IXmlTypeWriter>().To<VIFAxlegearType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAxlegearType());
			Bind<IXmlTypeWriter>().To<VIFAxleWheelsType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAxleWheelsType());
			Bind<IXmlTypeWriter>().To<VIFBoostingLimitationsType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetBoostingLimitationsType());
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
