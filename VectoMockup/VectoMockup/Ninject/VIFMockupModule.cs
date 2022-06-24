using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;
using TUGraz.VectoMockup.Reports;

namespace TUGraz.VectoMockup.Ninject
{
    internal class VIFMockupModule : AbstractNinjectModule
    {
		#region Overrides of NinjectModule

		public override void Load()
		{
			Kernel.Bind<IVIFReportFactory>().To<MockupVIFReportFactory>()
				.WhenInjectedExactlyInto<MockupReportFactory>().InSingletonScope();
		}

		#endregion

		public class MockupVIFReportFactory : IVIFReportFactory
		{
			private IVIFReportFactory _vifReportFactoryImplementation;


			public MockupVIFReportFactory(IVIFReportFactory vifReportFactoryImplementation)
			{
				_vifReportFactoryImplementation = vifReportFactoryImplementation;
			}


			#region Implementation of IVIFReportFactory

			public IXMLPrimaryVehicleReport GetVIFReport(VehicleCategory vehicleType, VectoSimulationJobType jobType, ArchitectureID archId,
				bool exempted, bool iepc, bool ihpc)
			{
				return new MockupVIFReport(
					_vifReportFactoryImplementation.GetVIFReport(vehicleType, jobType, archId, exempted, iepc, ihpc));
			}

			public IXmlTypeWriter GetConventionalLorryVehicleType()
			{
				return _vifReportFactoryImplementation.GetConventionalLorryVehicleType();
			}

			public IXmlTypeWriter GetTorqueConvertType()
			{
				return _vifReportFactoryImplementation.GetTorqueConvertType();
			}

			public IXmlTypeWriter GetTorqueLimitsType()
			{
				return _vifReportFactoryImplementation.GetTorqueLimitsType();
			}

			public IXmlTypeWriter GetTransmissionType()
			{
				return _vifReportFactoryImplementation.GetTransmissionType();
			}

			public IReportOutputGroup GetConventionalVehicleGroup()
			{
				return _vifReportFactoryImplementation.GetConventionalVehicleGroup();
			}

			public IReportOutputGroup GetPrimaryBusGeneralParameterGroup()
			{
				throw new NotImplementedException();
			}

			public IXmlTypeWriter GetAdasType()
			{
				return _vifReportFactoryImplementation.GetAdasType();
			}

			public IXmlTypeWriter GetAngelDriveType()
			{
				return _vifReportFactoryImplementation.GetAngelDriveType();
			}

			public IXmlTypeWriter GetAuxiliaryType()
			{
				return _vifReportFactoryImplementation.GetAuxiliaryType();
			}

			public IXmlTypeWriter GetAxlegearType()
			{
				return _vifReportFactoryImplementation.GetAxlegearType();
			}

			public IXmlTypeWriter GetAxleWheelsType()
			{
				return _vifReportFactoryImplementation.GetAxleWheelsType();
			}

			public IXmlTypeWriter GetEngineType()
			{
				return _vifReportFactoryImplementation.GetEngineType();
			}

			#endregion
		}

	}
}
