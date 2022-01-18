using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    internal class MRFPrimaryBusElectricSystemType : AbstractMrfXmlType
    {
		public MRFPrimaryBusElectricSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var busAux = inputData.JobInputData.Vehicle.Components.BusAuxiliaries;
			return new XElement(_mrf + XMLNames.BusAux_ElectricSystem,
				new XElement(_mrf + XMLNames.BusAux_ElectricSystem_AlternatorTechnology,
					busAux.ElectricSupply.AlternatorTechnology.ToXMLFormat()),
				busAux.ElectricSupply.AlternatorTechnology != AlternatorType.None ? "TODO" : null,
				busAux.ElectricSupply.ElectricStorage != null ? "TODO" : null);

		}

		#endregion
	}
}
