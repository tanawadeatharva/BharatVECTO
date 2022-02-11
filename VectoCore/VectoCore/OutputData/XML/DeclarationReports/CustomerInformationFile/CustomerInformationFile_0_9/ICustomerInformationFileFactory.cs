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


		IMrfXmlGroup GetGeneralVehicleSequenceGroupWriter();
		IMrfXmlGroup GetLorryGeneralVehicleSequenceGroupWriter();
		IMrfXmlGroup GetConventionalLorryVehicleSequenceGroupWriter();
		IMrfXmlGroup GetEngineGroup();
		IMrfXmlGroup GetTransmissionGroup();
		IMrfXmlGroup GetAxleWheelsGroup();
		IMrfXmlGroup GetLorryAuxGroup();
		IMrfXmlGroup GetHEV_VehicleSequenceGroupWriter();
		IMrfXmlGroup GetHEV_LorryVehicleTypeGroup();
		IMrfXmlGroup GetElectricMachineGroup();
		IMrfXmlGroup GetREESSGroup();
	}
}
