using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.CompletedBus
{
    internal class CompletedBusGeneralVehicleOutputGroup : AbstractMrfXmlGroup
    {
		public CompletedBusGeneralVehicleOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var multiStageInputData = inputData as IMultistageBusInputDataProvider;
			if (multiStageInputData == null) {
				throw new ArgumentException($"inputdata must implement {nameof(IMultistageBusInputDataProvider)}");
			}
			var result = new List<XElement>();
			var manufacturers = new XElement(_mrf + "Manufacturers");
			result.Add(manufacturers);
			foreach (var manufacturingStageInputData in multiStageInputData.JobInputData.ManufacturingStages) {
				manufacturers.Add(new XElement(_mrf + "Step",
					new XAttribute("Count", manufacturingStageInputData.StageCount), 
					new XElement(_mrf + XMLNames.Component_Manufacturer, manufacturingStageInputData.Vehicle.Manufacturer),
					new XElement(_mrf + XMLNames.Component_ManufacturerAddress, manufacturingStageInputData.Vehicle.ManufacturerAddress)));
			}
			result.AddRange(_mrfFactory.GetGeneralVehicleOutputGroup().GetElements(multiStageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle));
			result.AddRange(_mrfFactory.GetCompletedBusSequenceGroup().GetElements(multiStageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle));
			result.AddRange(_mrfFactory.GetCompletedBusDimensionSequenceGroup().GetElements(multiStageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle));
			return result;
		}

		#endregion
	}

	internal class CompletedBusSequenceGroup : AbstractMrfXmlGroup, IMrfVehicleGroup
	{
		public CompletedBusSequenceGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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
				new XElement(_mrf + XMLNames.CorrectedActualMass, vehicleData.CurbMassChassis.ToXMLFormat()),
				new XElement(_mrf + "ZeroEmissionHDV", vehicleData.ZeroEmissionVehicle),
				new XElement(_mrf + XMLNames.Vehicle_HybridElectricHDV, vehicleData.HybridElectricHDV),
				new XElement(_mrf + XMLNames.Vehicle_RegisteredClass, vehicleData.RegisteredClass),
				new XElement(_mrf + XMLNames.Bus_NumberPassengersUpperDeck, vehicleData.NumberPassengerSeatsUpperDeck + vehicleData.NumberPassengersStandingUpperDeck),
				new XElement(_mrf + XMLNames.Bus_NumberPassengersLowerDeck, vehicleData.NumberPassengerSeatsLowerDeck + vehicleData.NumberPassengersStandingLowerDeck),
				new XElement(_mrf + XMLNames.Vehicle_BodyworkCode, vehicleData.VehicleCode),
				new XElement(_mrf + XMLNames.Bus_LowEntry, vehicleData.LowEntry)
			};
			return result;
		}

		#endregion
	}

	internal class CompletedBusDimensionsSequenceGroup : AbstractMrfXmlGroup, IMrfVehicleGroup
	{
		public CompletedBusDimensionsSequenceGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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
				new XElement(_mrf + XMLNames.Bus_HeighIntegratedBody, vehicleData.Height),
				new XElement(_mrf + XMLNames.Bus_VehicleLength, vehicleData.Length),
				new XElement(_mrf + XMLNames.Bus_VehicleWidth, vehicleData.Width)
			};
			return result;
		}

		#endregion
	}

}
