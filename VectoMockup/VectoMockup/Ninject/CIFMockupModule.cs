using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoMockup.Factories;
using TUGraz.VectoMockup.Reports;

namespace TUGraz.VectoMockup.Ninject
{
	// ReSharper disable once InconsistentNaming
	public class CIFMockupModule : AbstractNinjectModule
    {
        #region Overrides of NinjectModule

        public override void Load()
        {
			Kernel.Bind<ICustomerInformationFileFactory>().To<MockupCustomerInformationFileFactory>()
                .WhenInjectedExactlyInto<XMLDeclarationMockupReportFactory>().InSingletonScope();

        }

        #endregion
    }

    public class MockupCustomerReport : IXMLCustomerReport, IXMLMockupReport
    {
        private readonly AbstractCustomerReport _originalCustomerReport;
		private XNamespace Cif = AbstractCustomerReport.Cif;
        public MockupCustomerReport(IXMLCustomerReport originalReport)
        {
            _originalCustomerReport = originalReport as AbstractCustomerReport;
			_outputData = _originalCustomerReport.OutputDataType;
            Results	= new XElement(Cif + XMLNames.Report_Results);
        }

		private XElement Results;
		private readonly string _outputData;
		private VectoRunData _modelData;

		#region Implementation of IXMLCustomerReport

        public void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			_modelData = modelData;
            _originalCustomerReport.Initialize(modelData, fuelModes);
        }

        public XDocument Report
		{
			get
			{
				var report = _originalCustomerReport.Report;
				report.XPathSelectElements($"//*[name()='{XMLNames.Report_Results}']").Single().ReplaceWith(Results);

				return report;


            }
		}

		public void WriteResult(XMLDeclarationReport.ResultEntry resultValue)
        {
            _originalCustomerReport.WriteResult(resultValue);
        }

        public void GenerateReport(XElement resultSignature)
        {
            _originalCustomerReport.GenerateReport(resultSignature);
        }

        #endregion

		#region Implementation of IXMLMockupReport

		public void WriteMockupResult(XMLDeclarationReport.ResultEntry resultValue)
		{
			Results.Add(MockupResultReader.GetCIFMockupResult(_outputData, resultValue, Cif + "Result", _modelData));
		}

		public void WriteMockupSummary(XMLDeclarationReport.ResultEntry resultValue)
		{
			Results.AddFirst(new XElement(Cif + "Status", "success"));
			Results.AddFirst(new XComment("Always prints success at the moment"));
			Results.Add(MockupResultReader.GetCIFMockupResult(_outputData, resultValue, Cif + "Summary", _modelData));
		}

		#endregion
    }

    public class MockupCustomerInformationFileFactory : ICustomerInformationFileFactory
    {
        private readonly ICustomerInformationFileFactory _cifFactory;

        public MockupCustomerInformationFileFactory(ICustomerInformationFileFactory cifFactory)
        {
            _cifFactory = cifFactory;
        }

        #region Implementation of ICustomerInformationFileFactory

        public IXMLCustomerReport GetCustomerReport(VehicleCategory vehicleType, VectoSimulationJobType jobType, ArchitectureID archId,
            bool exempted, bool iepc, bool ihpc)
        {
            return new MockupCustomerReport(_cifFactory.GetCustomerReport(vehicleType, jobType, archId, exempted, iepc, ihpc));
        }

        public IXmlTypeWriter GetConventionalLorryVehicleType()
        {
            return _cifFactory.GetConventionalLorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_PxLorryVehicleType()
        {
            return _cifFactory.GetHEV_PxLorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_S2_LorryVehicleType()
        {
            return _cifFactory.GetHEV_S2_LorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_S3_LorryVehicleType()
        {
            return _cifFactory.GetHEV_S3_LorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_S4_LorryVehicleType()
        {
            return _cifFactory.GetHEV_S4_LorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_IEPC_LorryVehicleType()
        {
            return _cifFactory.GetHEV_IEPC_LorryVehicleType();
        }

        public IXmlTypeWriter GetPEV_E2_LorryVehicleType()
        {
            return _cifFactory.GetPEV_E2_LorryVehicleType();
        }

        public IXmlTypeWriter GetPEV_E3_LorryVehicleType()
        {
            return _cifFactory.GetPEV_E3_LorryVehicleType();
        }

        public IXmlTypeWriter GetPEV_E4_LorryVehicleType()
        {
            return _cifFactory.GetPEV_E4_LorryVehicleType();
        }

        public IXmlTypeWriter GetPEV_IEPC_LorryVehicleType()
        {
            return _cifFactory.GetPEV_IEPC_LorryVehicleType();
        }

        public IXmlTypeWriter GetConventional_CompletedBusVehicleType()
        {
            return _cifFactory.GetConventional_CompletedBusVehicleType();
        }

        public IXmlTypeWriter GetHEV_CompletedBusVehicleType()
        {
            return _cifFactory.GetHEV_CompletedBusVehicleType();
        }

        public IXmlTypeWriter GetPEV_CompletedBusVehicleType()
        {
            return _cifFactory.GetPEV_CompletedBusVehicleType();
        }

        public IReportVehicleOutputGroup GetGeneralVehicleSequenceGroupWriter()
        {
            return _cifFactory.GetGeneralVehicleSequenceGroupWriter();
        }

        public IReportOutputGroup GetLorryGeneralVehicleSequenceGroupWriter()
        {
            return _cifFactory.GetLorryGeneralVehicleSequenceGroupWriter();
        }

        public IReportOutputGroup GetConventionalLorryVehicleSequenceGroupWriter()
        {
            return _cifFactory.GetConventionalLorryVehicleSequenceGroupWriter();
        }

        public IReportOutputGroup GetEngineGroup()
        {
            return _cifFactory.GetEngineGroup();
        }

        public IReportOutputGroup GetTransmissionGroup()
        {
            return _cifFactory.GetTransmissionGroup();
        }

        public IReportOutputGroup GetAxleWheelsGroup()
        {
            return _cifFactory.GetAxleWheelsGroup();
        }

        public IReportOutputGroup GetLorryAuxGroup()
        {
            return _cifFactory.GetLorryAuxGroup();
        }

        public IReportOutputGroup GetCompletedBusAuxGroup()
        {
            return _cifFactory.GetCompletedBusAuxGroup();
        }

        public IReportOutputGroup GetHEV_VehicleSequenceGroupWriter()
        {
            return _cifFactory.GetHEV_VehicleSequenceGroupWriter();
        }

        public IReportOutputGroup GetHEV_LorryVehicleTypeGroup()
        {
            return _cifFactory.GetHEV_LorryVehicleTypeGroup();
        }

        public IReportOutputGroup GetElectricMachineGroup()
        {
            return _cifFactory.GetElectricMachineGroup();
        }

        public IReportOutputGroup GetREESSGroup()
        {
            return _cifFactory.GetREESSGroup();
        }

        public IReportOutputGroup GetPEV_LorryVehicleTypeGroup()
        {
            return _cifFactory.GetPEV_LorryVehicleTypeGroup();
        }

        public IReportOutputGroup GetPEV_VehicleSequenceGroupWriter()
        {
            return _cifFactory.GetPEV_VehicleSequenceGroupWriter();
        }

        public IReportOutputGroup GetCompletedBusVehicleTypeGroup()
        {
            return _cifFactory.GetCompletedBusVehicleTypeGroup();
        }

        public IReportCompletedBusOutputGroup GetGeneralVehicleSequenceGroupWriterCompletedBus()
        {
            return _cifFactory.GetGeneralVehicleSequenceGroupWriterCompletedBus();
        }

        #endregion
    }
}