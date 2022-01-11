using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    public class MRFConventionalLorryAuxiliariesType : AbstractMrfXmlType
    {
		public MRFConventionalLorryAuxiliariesType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var auxData = inputData.JobInputData.Vehicle.Components.AuxiliaryInputData;
			var fanData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.Fan);
			var steeringPumpData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.SteeringPump);
			var electricSystemData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.ElectricSystem);
			var pneumaticSystemData = auxData.Auxiliaries.Single(aux => aux.Type == AuxiliaryType.PneumaticSystem);
			//var steering



			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				new XElement(_mrf + "CoolingFanTechnology",
					string.Join("\n", fanData.Technology)),
				new XElement(_mrf + "SteeringPumpTechnology", string.Join("\n", steeringPumpData.Technology)),
				new XElement(_mrf + XMLNames.BusAux_ElectricSystem, new XElement(_mrf + "LEDHeadLights", electricSystemData.Technology.Contains("Standard technology - LED headlights, all"))),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem, new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, string.Join("\n", pneumaticSystemData.Technology))));	
		}

		#endregion
	}
}
