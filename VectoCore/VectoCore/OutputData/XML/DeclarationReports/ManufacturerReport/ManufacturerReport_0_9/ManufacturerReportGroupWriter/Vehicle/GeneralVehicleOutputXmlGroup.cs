using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter
{
    public class GeneralVehicleOutputXmlGroup : AbstractMrfXmlGroup
    {
		public GeneralVehicleOutputXmlGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMRFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;
			return new List<XElement>() {
				new XElement(_mrf + XMLNames.Component_Model, vehicle.Model),
				new XElement(_mrf + XMLNames.Vehicle_VIN, vehicle.VIN),
				new XElement(_mrf + XMLNames.Vehicle_VehicleCategory, vehicle.VehicleCategory.ToXMLFormat()),
				new XElement(_mrf + XMLNames.Vehicle_AxleConfiguration, vehicle.AxleConfiguration.ToXMLFormat()),
				new XElement(_mrf + XMLNames.TPMLM, vehicle.GrossVehicleMassRating.ToXMLFormat()),
				new XElement(_mrf + XMLNames.Report_Vehicle_VehicleGroup, vehicle.LegislativeClass.ToXMLFormat()),
			};
		}

		#endregion
	}
}
