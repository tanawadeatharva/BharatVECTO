using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.LorryManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9
{
    internal class MRFNinjectModule : AbstractNinjectModule
    {
		#region Overrides of NinjectModule


		//IXMLManufacturerReport GetManufacturerReport(string vehicleType, VectoSimulationJobType jobType,
		//	ArchitectureID archId, bool exempted);
		private CombineArgumentsToNameInstanceProvider.CombineToName nameCombinationMethod = arguments => {
			string result = "";
			result += (bool)arguments[3] ? "exempted" : "";
			result += arguments[1].ToString();
			result += (ArchitectureID)arguments[2] == ArchitectureID.UNKNOWN ? "" : arguments[2].ToString();
			result += arguments[0].ToString();
			return result;
		};

		public override void Load()
		{
			Bind<IManufacturerReportFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(nameCombinationMethod, 
				4, 4, typeof(IManufacturerReportFactory).GetMethod(nameof(IManufacturerReportFactory.GetManufacturerReport)))).InSingletonScope();
			Bind<IXMLManufacturerReport>().To<ConventionalLorryManufacturerReport>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryManufacturerReport());
			Bind<IXMLManufacturerReport>().To<ConventionalLorryManufacturerReport>().Named(nameCombinationMethod.Invoke(
				VehicleCategoryHelper.Lorry, 
				VectoSimulationJobType.ConventionalVehicle, 
				ArchitectureID.UNKNOWN, 
				false));
			Bind<IXMLManufacturerReport>().To<ConventionalLorryManufacturerReport>().Named(nameCombinationMethod.Invoke(
				VehicleCategoryHelper.Lorry,
				VectoSimulationJobType.ConventionalVehicle,
				ArchitectureID.UNKNOWN,
				false));
			Bind<IXMLManufacturerReport>().To<ConventionalLorryManufacturerReport>().Named(nameCombinationMethod.Invoke(
				VehicleCategoryHelper.Lorry,
				VectoSimulationJobType.ConventionalVehicle,
				ArchitectureID.UNKNOWN,
				false));
			Bind<IXMLManufacturerReport>().To<ConventionalLorryManufacturerReport>().Named(nameCombinationMethod.Invoke(
				VehicleCategoryHelper.Lorry,
				VectoSimulationJobType.ConventionalVehicle,
				ArchitectureID.UNKNOWN,
				false));


			Bind<IMrfXmlType>().To<MRF_ConventionalLorryVehicleType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryVehicleType());

			Bind<IMrfXmlType>().To<MRFConventionalAdasType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalADASType());

			Bind<IMrfXmlType>().To<MRFTorqueLimitationsType>().
				NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetEngineTorqueLimitationsType());

			Bind<IMrfXmlType>().To<MRFEngineType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetEngineType());

			Bind<IMrfXmlType>().To<MRFConventionalLorryComponentsType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryComponentsType());

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

			Bind<IMrfXmlType>().To<MRFConventionalLorryAuxiliariesType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryAuxType());

			#region XMLGroups

			Bind<IMrfXmlGroup>().To<GeneralVehicleOutputXmlGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetGeneralVehicleOutputGroup());

			Bind<IMrfXmlGroup>().To<LorryGeneralVehicleOutputXmlGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetGeneralLorryVehicleOutputGroup());
			Bind<IMrfXmlGroup>().To<ConventionalLorryVehicleXmlGroup>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryVehicleOutputGroup());


			#endregion
		}

		#endregion
	}
}
