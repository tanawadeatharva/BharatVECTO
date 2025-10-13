using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
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

			var vehicleData = inputData.JobInputData.Vehicle;
            var dualFuel = vehicleData.Components?.EngineInputData.EngineModes.Any(x => x.Fuels.Count > 1) ?? false;
            result.Add(new XElement(_mrf + XMLNames.Vehicle_DualFuelVehicle, dualFuel));

            result.AddRange(_mrfFactory.GetHEV_VehicleSequenceGroup().GetElements(inputData));

            var tankSystem = vehicleData.TankSystem.HasValue
                ? vehicleData.TankSystem.Value.ToString()
                : (vehicleData.HydrogenStorageTechnology.HasValue
                    ? vehicleData.HydrogenStorageTechnology.Value.ToString()
                    : null);

            if (tankSystem != null)
            {
                result.Add(new XElement(_mrf + "TankSystem", tankSystem));
            }

            return result;
		}

		#endregion
	}

	internal class FCHVPrimaryBusVehicleOutputGroup : AbstractReportOutputGroup
	{
		public FCHVPrimaryBusVehicleOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			result.AddRange(GetFCHVPrimaryBusVehicleOutputGroup(inputData));
			result.AddRange(_mrfFactory.GetFCHV_VehicleSequenceGroup().GetElements(inputData));

			return result;
		}

		private IList<XElement> GetFCHVPrimaryBusVehicleOutputGroup(IDeclarationInputDataProvider inputData)
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
