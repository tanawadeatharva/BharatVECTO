using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Common
{
    public class GeneralVehicleOutputGroup : AbstractReportOutputGroup, IReportVehicleOutputGroup
    {
		public GeneralVehicleOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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
				new XElement(_mrf + XMLNames.TPMLM, vehicleData.GrossVehicleMassRating.ValueAsUnit("kg")),
				new XElement(_mrf + XMLNames.Report_Vehicle_VehicleGroup, DeclarationData.GetVehicleGroupGroup(vehicleData).Item1.GetClassNumber()),
				new XElement(_mrf + XMLNames.VehicleGroupCO2, string.Empty), //the value of this element will be replaced later, write empty string to let the validation fail if the value is not replaced
				//new XElement(_mrf + "VehicleGroupCO2", DeclarationData.GetVehicleGroupCO2StandardsGroup(vehicleData).ToXMLFormat()),

			};
		}

		#endregion
	}
}
