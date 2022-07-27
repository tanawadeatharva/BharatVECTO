using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter
{
    public class GeneralVehicleOutputOutputXmlGroup : AbstractReportOutputGroup, IReportVehicleOutputGroup
    {
		public GeneralVehicleOutputOutputXmlGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMRFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;
			return GetElements(vehicle);

		}

		#endregion

		#region Implementation of IMrfVehicleGroup

		public IList<XElement> GetElements(IVehicleDeclarationInputData vehicleData)
		{
			return new List<XElement>() {
				new XElement(_mrf + XMLNames.Component_Model, vehicleData.Model),
				new XElement(_mrf + XMLNames.Vehicle_VIN, vehicleData.VIN),
				vehicleData.VehicleTypeApprovalNumber.IsNullOrEmpty()
					? null 
					: new XElement(_mrf + XMLNames.Vehicle_TypeApprovalNumber, vehicleData.VehicleTypeApprovalNumber),
				new XElement(_mrf + XMLNames.Vehicle_VehicleCategory, vehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(_mrf + XMLNames.Vehicle_AxleConfiguration, vehicleData.AxleConfiguration.ToXMLFormat()),
				new XElement(_mrf + XMLNames.TPMLM, vehicleData.GrossVehicleMassRating.ToXMLFormat(0)),
				new XElement(_mrf + XMLNames.Report_Vehicle_VehicleGroup, DeclarationData.GetVehicleGroupGroup(vehicleData)),
			};
		}

		#endregion
	}
}
