using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
    internal class MRFTorqueLimitationsType : AbstractMrfXmlType, IMrfXmlType
    {


		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var torqueLimitsElement = new XElement(_mrf + XMLNames.Vehicle_TorqueLimits);

			var maxEngineTorque = inputData.JobInputData.Vehicle.Components.EngineInputData.MaxTorqueDeclared;
			foreach (var torqueLimitInputData in inputData.JobInputData.Vehicle.TorqueLimits) {
				
				torqueLimitsElement.Add(new XElement(_mrf + "EngineTorqueLimit", new XAttribute("Gear", torqueLimitInputData.Gear), torqueLimitInputData.MaxTorque/maxEngineTorque));

			}

			return torqueLimitsElement;
		}



		public MRFTorqueLimitationsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }
	}
}
