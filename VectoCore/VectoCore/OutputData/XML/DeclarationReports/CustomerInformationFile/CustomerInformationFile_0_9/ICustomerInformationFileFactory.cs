using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9
{
    public interface ICustomerInformationFileFactory
    {
		IXMLCustomerReport GetCustomerReport(VehicleCategory vehicleType, VectoSimulationJobType jobType,
			ArchitectureID archId, bool exempted, bool iepc, bool ihpc);

		IXmlTypeWriter GetConventionalLorryVehicleType();
		IXmlTypeWriter GetHEV_PxLorryVehicleType();
		IXmlTypeWriter GetHEV_S2_LorryVehicleType();
		IXmlTypeWriter GetHEV_S3_LorryVehicleType();
		IXmlTypeWriter GetHEV_S4_LorryVehicleType();
		IXmlTypeWriter GetHEV_IEPC_LorryVehicleType();

		IXmlTypeWriter GetPEV_E2_LorryVehicleType();
		IXmlTypeWriter GetPEV_E3_LorryVehicleType();
		IXmlTypeWriter GetPEV_E4_LorryVehicleType();
		IXmlTypeWriter GetPEV_IEPC_LorryVehicleType();


		IXmlTypeWriter GetConventional_CompletedBusVehicleType();
		IXmlTypeWriter GetHEV_CompletedBusVehicleType();
		IXmlTypeWriter GetPEV_CompletedBusVehicleType();


		IReportVehicleOutputGroup GetGeneralVehicleSequenceGroupWriter();
		IReportOutputGroup GetLorryGeneralVehicleSequenceGroupWriter();
		IReportOutputGroup GetConventionalLorryVehicleSequenceGroupWriter();
		IReportOutputGroup GetEngineGroup();
		IReportOutputGroup GetTransmissionGroup();
		IReportOutputGroup GetAxleWheelsGroup();
		IReportOutputGroup GetLorryAuxGroup();
		IReportOutputGroup GetHEV_VehicleSequenceGroupWriter();
		IReportOutputGroup GetHEV_LorryVehicleTypeGroup();
		IReportOutputGroup GetElectricMachineGroup();
		IReportOutputGroup GetREESSGroup();
		IReportOutputGroup GetPEV_LorryVehicleTypeGroup();
		IReportOutputGroup GetPEV_VehicleSequenceGroupWriter();
		IReportOutputGroup GetCompletedBusVehicleTypeGroup();
	}
}
