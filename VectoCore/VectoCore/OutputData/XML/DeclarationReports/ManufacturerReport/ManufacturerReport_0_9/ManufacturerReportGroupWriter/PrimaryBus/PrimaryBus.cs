using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle
{
    internal class PrimaryBusGeneralVehicleOutputGroup : AbstractReportOutputGroup
    {
		public PrimaryBusGeneralVehicleOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var primaryBus = inputData.JobInputData.Vehicle;
			var result = new List<XElement>() {
				new XElement(_mrf + XMLNames.Component_Manufacturer, primaryBus.Manufacturer),
				new XElement(_mrf + XMLNames.Component_ManufacturerAddress, primaryBus.ManufacturerAddress)
			};
			result.AddRange(_mrfFactory.GetGeneralVehicleOutputGroup().GetElements(primaryBus));
			result.Add(new XElement(_mrf + XMLNames.Vehicle_ZeroEmissionVehicle, primaryBus.ZeroEmissionVehicle));
			result.Add(new XElement(_mrf + XMLNames.Vehicle_HybridElectricHDV, primaryBus.HybridElectricHDV));


			return result;
		}

		#endregion
	}

	internal class ExemptedPrimaryBusGeneralVehicleOutputGroup : AbstractReportOutputGroup
	{
		public ExemptedPrimaryBusGeneralVehicleOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var primaryBus = inputData.JobInputData.Vehicle;
			var result = new List<XElement>() {
				new XElement(_mrf + XMLNames.Component_Manufacturer, primaryBus.Manufacturer),
				new XElement(_mrf + XMLNames.Component_ManufacturerAddress, primaryBus.ManufacturerAddress)
			};
			result.AddRange(_mrfFactory.GetGeneralVehicleOutputGroup().GetElements(primaryBus));
			result.Add(new XElement(_mrf + XMLNames.Vehicle_ZeroEmissionVehicle, primaryBus.ZeroEmissionVehicle));
			//result.Add(new XElement(_mrf + XMLNames.Vehicle_HybridElectricHDV, primaryBus.HybridElectricHDV));


			return result;
		}

		#endregion
	}

	internal class HEVPrimaryBusVehicleOutputGroup : AbstractReportOutputGroup
	{
		public HEVPrimaryBusVehicleOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			result.AddRange(_mrfFactory.GetPrimaryBusGeneralVehicleOutputGroup().GetElements(inputData));
			result.AddRange(_mrfFactory.GetHEV_VehicleSequenceGroup().GetElements(inputData));

			return result;
		}

		#endregion
	}

	internal class FuelCellPrimaryBusVehicleOutputGroup : AbstractReportOutputGroup
	{
		public FuelCellPrimaryBusVehicleOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			result.AddRange(GetFuelCellPrimaryBusVehicleOutputGroup(inputData));
			result.AddRange(_mrfFactory.GetFuelCell_VehicleSequenceGroup().GetElements(inputData));

			return result;
		}

		#endregion

		private IList<XElement> GetFuelCellPrimaryBusVehicleOutputGroup(IDeclarationInputDataProvider inputData)
		{
			var primaryBus = inputData.JobInputData.Vehicle;
			var result = new List<XElement>() {
				new XElement(_mrf + XMLNames.Component_Manufacturer, primaryBus.Manufacturer),
				new XElement(_mrf + XMLNames.Component_ManufacturerAddress, primaryBus.ManufacturerAddress)
			};
			result.AddRange(_mrfFactory.GetGeneralVehicleOutputGroup().GetElements(primaryBus));
			result.Add(new XElement(_mrf + XMLNames.Vehicle_ZeroEmissionVehicle, primaryBus.ZeroEmissionVehicle));
			result.Add(new XElement(_mrf + XMLNames.Vehicle_HybridElectricHDV, primaryBus.HybridElectricHDV));


			return result;
		}
	}

	internal class PEVPrimaryBusVehicleOutputGroup : AbstractReportOutputGroup
	{
		public PEVPrimaryBusVehicleOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			result.AddRange(_mrfFactory.GetPrimaryBusGeneralVehicleOutputGroup().GetElements(inputData));
			result.AddRange(_mrfFactory.GetPEV_VehicleSequenceGroup().GetElements(inputData));

			return result;
		}

		#endregion
	}

}
