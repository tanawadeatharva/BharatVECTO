using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.CompletedBus
{
    internal class CompletedBusGeneralVehicleOutputGroup : AbstractReportOutputGroup
    {
		private XElement GetManufacturerAndAddress(string manufacturer, string address, int stepCount)
		{
			return new XElement(_mrf + "Step",
				new XAttribute("Count", stepCount),
				new XElement(_mrf + XMLNames.Component_Manufacturer, manufacturer),
				new XElement(_mrf + XMLNames.Component_ManufacturerAddress, address));
		}
		public CompletedBusGeneralVehicleOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var multiStageInputData = inputData as IMultistageBusInputDataProvider;
			if (multiStageInputData == null) {
				throw new ArgumentException($"inputdata must implement {nameof(IMultistageBusInputDataProvider)}");
			}
			var primaryVehicleData = multiStageInputData.JobInputData.PrimaryVehicle.Vehicle;
			var consolidatedVehicleData = multiStageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle;
			var result = new List<XElement>();
			var manufacturers = new XElement(_mrf + "Manufacturers");
			result.Add(manufacturers);
			manufacturers.Add(GetManufacturerAndAddress(primaryVehicleData.Manufacturer, primaryVehicleData.ManufacturerAddress, 1));
			foreach (var manufacturingStageInputData in multiStageInputData.JobInputData.ManufacturingStages) {
				manufacturers.Add(GetManufacturerAndAddress(manufacturingStageInputData.Vehicle.Manufacturer,
					manufacturingStageInputData.Vehicle.ManufacturerAddress,
					stepCount: manufacturingStageInputData.StepCount));
			}
			//result.AddRange(_mrfFactory.GetGeneralVehicleOutputGroup().GetElements(multiStageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle));
			result.AddRange(new List<XElement>() {
				new XElement(_mrf + XMLNames.Component_Model, consolidatedVehicleData.Model),
				new XElement(_mrf + XMLNames.Vehicle_VIN, consolidatedVehicleData.VIN),
				consolidatedVehicleData.VehicleTypeApprovalNumber != null ? new XElement(_mrf + XMLNames.Vehicle_TypeApprovalNumber, consolidatedVehicleData.VehicleTypeApprovalNumber) : null,
				new XElement(_mrf + XMLNames.Vehicle_VehicleCategory, consolidatedVehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(_mrf + XMLNames.Vehicle_AxleConfiguration, primaryVehicleData.AxleConfiguration.ToXMLFormat()),
				new XElement(_mrf + XMLNames.TPMLM, consolidatedVehicleData.GrossVehicleMassRating.ToXMLFormat(0)),
				new XElement(_mrf + XMLNames.Report_Vehicle_VehicleGroup, consolidatedVehicleData.VehicleCategory.ToXMLFormat()),
			});

			result.AddRange(_mrfFactory.GetCompletedBusSequenceGroup().GetElements(consolidatedVehicleData));
			result.AddRange(_mrfFactory.GetCompletedBusDimensionSequenceGroup().GetElements(consolidatedVehicleData));
			result.Add(new XElement(_mrf + XMLNames.Bus_DoorDriveTechnology, consolidatedVehicleData.DoorDriveTechnology.ToXMLFormat()));
			result.Add(new XElement(_mrf + XMLNames.Vehicle_NgTankSystem, consolidatedVehicleData.TankSystem));
			return result;
		}

		#endregion
	}

	internal class CompletedBusSequenceOutputGroup : AbstractReportOutputGroup, IReportVehicleOutputGroup
	{
		public CompletedBusSequenceOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IMrfVehicleGroup

		public IList<XElement> GetElements(IVehicleDeclarationInputData vehicleData)
		{
			var result = new List<XElement>() {
				new XElement(_mrf + XMLNames.CorrectedActualMass, vehicleData.CurbMassChassis.ToXMLFormat(0)),
				new XElement(_mrf + "ZeroEmissionHDV", vehicleData.ZeroEmissionVehicle),
				new XElement(_mrf + XMLNames.Vehicle_HybridElectricHDV, vehicleData.HybridElectricHDV),
				new XElement(_mrf + XMLNames.Vehicle_RegisteredClass, vehicleData.RegisteredClass.ToXMLFormat()),
				new XElement(_mrf + XMLNames.Bus_NumberPassengersUpperDeck, vehicleData.NumberPassengerSeatsUpperDeck + vehicleData.NumberPassengersStandingUpperDeck),
				new XElement(_mrf + XMLNames.Bus_NumberPassengersLowerDeck, vehicleData.NumberPassengerSeatsLowerDeck + vehicleData.NumberPassengersStandingLowerDeck),
				new XElement(_mrf + XMLNames.Vehicle_BodyworkCode, vehicleData.VehicleCode.ToXMLFormat()),
				new XElement(_mrf + XMLNames.Bus_LowEntry, vehicleData.LowEntry)
			};
			return result;
		}

		#endregion
	}

	internal class CompletedBusDimensionsSequenceOutputGroup : AbstractReportOutputGroup, IReportVehicleOutputGroup
	{
		public CompletedBusDimensionsSequenceOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IMrfVehicleGroup

		public IList<XElement> GetElements(IVehicleDeclarationInputData vehicleData)
		{
			var result = new List<XElement>() {
				new XElement(_mrf + XMLNames.Bus_HeightIntegratedBody, vehicleData.Height.ConvertToMilliMeter().ToXMLFormat(0)),
				new XElement(_mrf + XMLNames.Bus_VehicleLength, vehicleData.Length.ConvertToMilliMeter().ToXMLFormat(0)),
				new XElement(_mrf + XMLNames.Bus_VehicleWidth, vehicleData.Width.ConvertToMilliMeter().ToXMLFormat(0))
			};
			return result;
		}

		#endregion
	}

}
