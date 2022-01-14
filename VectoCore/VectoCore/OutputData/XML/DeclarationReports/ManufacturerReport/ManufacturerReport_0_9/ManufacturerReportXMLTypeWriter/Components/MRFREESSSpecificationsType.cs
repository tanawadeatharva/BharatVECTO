using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    internal class MRFREESSSpecificationsType : AbstractMrfXmlType
    {
		public MRFREESSSpecificationsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var reessElements = inputData.JobInputData.Vehicle.Components.ElectricStorage.ElectricStorageElements;

			var result = new XElement(_mrf + "REESSSpecifications");
			foreach (var element in reessElements) {
				var reessElement = new XElement(_mrf + "REESS",
					new XElement(_mrf + XMLNames.Battery_StringID, element.StringId));
				//reessElement.Add(new XElement(_mrf + "REESSSystem", element.REESSPack.));
				result.Add();
			}



			return result;
		}

		#endregion
	}
}
