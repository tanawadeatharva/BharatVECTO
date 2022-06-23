using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter
{
    public class LorryGeneralVehicleOutputXmlGroup : AbstractReportOutputGroup
    {
		#region Implementation of IMRFGroupWriterb

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;
			var elements = new List<XElement>();
            elements.AddRange(new List<XElement>() {
                new XElement(_mrf + XMLNames.Component_Manufacturer, vehicle.Manufacturer),
                new XElement(_mrf + XMLNames.Component_ManufacturerAddress, vehicle.ManufacturerAddress),
            });
            elements.AddRange(_mrfFactory.GetGeneralVehicleOutputGroup().GetElements(vehicle));
            elements.AddRange(new List<XElement>() {
				// new XElement(mrf + "CO2StandardGroup", vehicle.) //CO2 Standardgroup
				new XElement(_mrf + XMLNames.CorrectedActualMass, vehicle.CurbMassChassis.ToXMLFormat(0)),
                new XElement(_mrf + XMLNames.Vehicle_VocationalVehicle, vehicle.VocationalVehicle),
                new XElement(_mrf + "ZeroEmissionHDV", vehicle.ZeroEmissionVehicle),
                new XElement(_mrf + XMLNames.Vehicle_HybridElectricHDV, vehicle.HybridElectricHDV),
            });

            return elements;
		}

		#endregion

		public LorryGeneralVehicleOutputXmlGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }
	}
}
