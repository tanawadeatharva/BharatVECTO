using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter
{
    public class GeneralVehicleOutputSequenceGroupCif : AbstractCIFGroupWriter, IReportVehicleOutputGroup, IReportCompletedBusOutputGroup
	{
		public GeneralVehicleOutputSequenceGroupCif(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			return GetElements(inputData.JobInputData.Vehicle);
		}

		#endregion

		#region Implementation of IReportVehicleOutputGroup

		public IList<XElement> GetElements(IVehicleDeclarationInputData vehicleData)
		{
			var result = new List<XElement>() {
				new XElement(_cif + XMLNames.Vehicle_VIN, vehicleData.VIN),
				new XElement(_cif + XMLNames.Vehicle_VehicleCategory, vehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(_cif + XMLNames.Vehicle_AxleConfiguration, vehicleData.AxleConfiguration.ToXMLFormat()),
				new XElement(_cif + XMLNames.Vehicle_TPMLM, XMLHelper.ValueAsUnit(vehicleData.GrossVehicleMassRating, "kg")),
				new XElement(_cif + XMLNames.Report_Vehicle_VehicleGroup, DeclarationData.GetVehicleGroupGroup(vehicleData).GetClassNumber()),
				new XElement(_cif + "VehicleGroupCO2", DeclarationData.GetVehicleGroupCO2StandardsGroup(vehicleData).ToXMLFormat()),

			};
			return result;
		}

		#endregion

		#region Implementation of IReportCompletedBusOutputGroup

		public IList<XElement> GetElements(IMultistageBusInputDataProvider multiStageInputDataProvider)
		{
			var consolidatedVehicleData = multiStageInputDataProvider.JobInputData.ConsolidateManufacturingStage.Vehicle;
			var primary = multiStageInputDataProvider.JobInputData.PrimaryVehicle.Vehicle;
			var result = new List<XElement>() {
				new XElement(_cif + XMLNames.Vehicle_VIN, consolidatedVehicleData.VIN),
				new XElement(_cif + XMLNames.Vehicle_VehicleCategory, consolidatedVehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(_cif + XMLNames.Vehicle_AxleConfiguration, primary.AxleConfiguration.ToXMLFormat()),
				new XElement(_cif + XMLNames.Vehicle_TPMLM, XMLHelper.ValueAsUnit(consolidatedVehicleData.GrossVehicleMassRating, "kg")),
				new XElement(_cif + XMLNames.Report_Vehicle_VehicleGroup, DeclarationData.GetVehicleGroupGroup(consolidatedVehicleData).GetClassNumber()),
				new XElement(_cif + "VehicleGroupCO2", DeclarationData.GetVehicleGroupCO2StandardsGroup(multiStageInputDataProvider).ToXMLFormat()),
			};
			return result;
		}

		#endregion
	}


	public class LorryGeneralVehicleSequenceGroupCIF : AbstractCIFGroupWriter
	{
		public LorryGeneralVehicleSequenceGroupCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			var result = new List<XElement>() {
				new XElement(_cif + XMLNames.Component_Manufacturer, vehicleData.Manufacturer),
				new XElement(_cif + XMLNames.Component_ManufacturerAddress, vehicleData.ManufacturerAddress),
				new XElement(_cif + XMLNames.Component_Model, vehicleData.Model),
				!vehicleData.VehicleTypeApprovalNumber.IsNullOrEmpty() 
					? new XElement(_cif + XMLNames.VehicleTypeApprovalNumber, vehicleData.VehicleTypeApprovalNumber)
					: null,
				//new XElement(_cif + XMLNames.VehicleTypeApprovalNumber, !vehicleData.VehicleTypeApprovalNumber.IsNullOrEmpty() 
				//	? vehicleData.VehicleTypeApprovalNumber 
				//	: null),
				new XElement(_cif + XMLNames.CorrectedActualMass, vehicleData.CurbMassChassis.ValueAsUnit("kg")),
				new XElement(_cif + XMLNames.Vehicle_VocationalVehicle, vehicleData.VocationalVehicle),
				new XElement(_cif + XMLNames.Vehicle_SleeperCab, vehicleData.SleeperCab),
				new XElement(_cif + XMLNames.Vehicle_ZeroEmissionVehicle, vehicleData.ZeroEmissionVehicle),
				new XElement(_cif + XMLNames.Vehicle_HybridElectricHDV, vehicleData.HybridElectricHDV)
			};
			return result;
		}

		#endregion
	}

	public class ConventionalLorryVehicleSequenceGroupCIF : AbstractCIFGroupWriter
	{
		public ConventionalLorryVehicleSequenceGroupCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			return new List<XElement>() {
				new XElement(_cif + "WasteHeatRecovery",
					vehicleData.Components.EngineInputData.WHRType != WHRType.None),
				new XElement(_cif + XMLNames.Vehicle_DualFuelVehicle, vehicleData.DualFuelVehicle)
			};
		}

		#endregion
	}

	public class ConventionalCompletedBusVehicleSequenceGroupCIF : AbstractCIFGroupWriter
	{
		public ConventionalCompletedBusVehicleSequenceGroupCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var multistep = inputData as IMultistageBusInputDataProvider;
			if (multistep == null) {
				throw new VectoException("Completed Bus CIF requires bus input data");
			}
			var vehicleData = multistep.JobInputData.PrimaryVehicle.Vehicle;
			return new List<XElement>() {
				new XElement(_cif + "WasteHeatRecovery",
					vehicleData.Components.EngineInputData.WHRType != WHRType.None),
				new XElement(_cif + XMLNames.Vehicle_DualFuelVehicle, vehicleData.DualFuelVehicle)
			};
		}

		#endregion
	}

	public class HEV_LorryVehicleTypeGroupCIF : AbstractCIFGroupWriter
	{
		public HEV_LorryVehicleTypeGroupCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			result.AddRange(_cifFactory.GetGeneralVehicleSequenceGroupWriter().GetElements(inputData.JobInputData.Vehicle));
			result.AddRange(_cifFactory.GetLorryGeneralVehicleSequenceGroupWriter().GetElements(inputData));
			result.AddRange(_cifFactory.GetHEV_LorryVehicleSequenceGroupWriter().GetElements(inputData));

			return result;
		}

		#endregion
	}
	public class PEV_LorryVehicleTypeGroupCIF : AbstractCIFGroupWriter
	{
		public PEV_LorryVehicleTypeGroupCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			result.AddRange(_cifFactory.GetGeneralVehicleSequenceGroupWriter().GetElements(inputData.JobInputData.Vehicle));
			result.AddRange(_cifFactory.GetLorryGeneralVehicleSequenceGroupWriter().GetElements(inputData));
			result.AddRange(_cifFactory.GetPEV_LorryVehicleSequenceGroupWriter().GetElements(inputData));

			return result;
		}

		#endregion
	}


	public class HEV_LorryVehicleSequenceGroupWriter : AbstractCIFGroupWriter
	{
		public HEV_LorryVehicleSequenceGroupWriter(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			var ovCc = inputData.JobInputData.Vehicle.OvcHev;
			result.AddRange(_cifFactory.GetConventionalLorryVehicleSequenceGroupWriter().GetElements(inputData));
			result.AddRange(new List<XElement>() {
				new XElement(_cif + "HEVArchitecture", inputData.JobInputData.Vehicle.ArchitectureID.GetLabel()),
				new XElement(_cif + "OffVehicleChargingCapability", ovCc)
			});
			if (ovCc) {
				result.Add(new XElement(_cif + "OffVehicleChargingMaxPower", inputData.JobInputData.Vehicle.MaxChargingPower.ValueAsUnit("kW", 1)));
			}
			return result;
		}

		#endregion
	}


	public class PEV_LorryVehicleSequenceGroupWriter : AbstractCIFGroupWriter
	{
		public PEV_LorryVehicleSequenceGroupWriter(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			var vehicle = GetVehicle(inputData);
			var ovCc = vehicle.OvcHev;
			result.AddRange(new List<XElement>() {
				new XElement(_cif + "PEVArchitecture", vehicle.ArchitectureID.GetLabel()),
				new XElement(_cif + "OffVehicleChargingCapability", ovCc)
			});
			if (ovCc)
			{
				result.Add(new XElement(_cif + "OffVehicleChargingMaxPower", vehicle.MaxChargingPower.ValueAsUnit("kW", 1)));
			}
			return result;
		}

		#endregion
	}


	public class HEV_CompletedBusVehicleSequenceGroupWriter : AbstractCIFGroupWriter
	{
		public HEV_CompletedBusVehicleSequenceGroupWriter(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var multistep = inputData as IMultistageBusInputDataProvider;
			if (multistep == null) {
				throw new VectoException("Completed Bus CIF requires bus input data");
			}
			var result = new List<XElement>();
			var ovCc = multistep.JobInputData.PrimaryVehicle.Vehicle.OvcHev;
			//result.AddRange(_cifFactory.GetConventionalCompletedBusVehicleSequenceGroupWriter().GetElements(inputData));
			result.AddRange(new List<XElement>() {
				new XElement(_cif + "HEVArchitecture", multistep.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID.GetLabel()),
				new XElement(_cif + "OffVehicleChargingCapability", ovCc)
			});
			if (ovCc) {
				result.Add(new XElement(_cif + "OffVehicleChargingMaxPower", multistep.JobInputData.PrimaryVehicle.Vehicle.MaxChargingPower.ValueAsUnit("kW", 1)));
			}
			return result;
		}

		#endregion
	}


	public class PEV_CompletedBusVehicleSequenceGroupWriter : AbstractCIFGroupWriter
	{
		public PEV_CompletedBusVehicleSequenceGroupWriter(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var multistep = inputData as IMultistageBusInputDataProvider;
			if (multistep == null) {
				throw new VectoException("Completed Bus CIF requires bus input data");
			}
			var result = new List<XElement>();
			var ovCc = multistep.JobInputData.PrimaryVehicle.Vehicle.OvcHev;
			result.AddRange(new List<XElement>() {
				new XElement(_cif + "PEVArchitecture",  multistep.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID.GetLabel()),
				new XElement(_cif + "OffVehicleChargingCapability", ovCc)
			});
			if (ovCc) {
				result.Add(new XElement(_cif + "OffVehicleChargingMaxPower", multistep.JobInputData.PrimaryVehicle.Vehicle.MaxChargingPower.ValueAsUnit("kW", 1)));
			}
			return result;
		}

		#endregion
	}

	public class CompletedBusVehicleTypeGroup : AbstractCIFGroupWriter
	{
		private XElement GetManufacturerAndAddress(string manufacturer, string manufacturerAddress, int stepCount)
		{
			return new XElement(_cif + "Step",
				new XAttribute(XMLNames.ManufacturingStep_StepCount, stepCount),
				new XElement(_cif + XMLNames.Component_Manufacturer, manufacturer),
				new XElement(_cif + XMLNames.Component_ManufacturerAddress, manufacturerAddress));
		}
		public CompletedBusVehicleTypeGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var completedBusData = inputData as IMultistageBusInputDataProvider;
			if (completedBusData == null) {
				throw new ArgumentException(
					$"{nameof(inputData)} must implement {nameof(IMultistageBusInputDataProvider)}");
			}
			var result = new List<XElement>();
			result.AddRange(
				_cifFactory.GetGeneralVehicleSequenceGroupWriterCompletedBus().GetElements(completedBusData));
			result.Add(GetManufacturers(completedBusData));


	
			var consolidatedVehicle = completedBusData.JobInputData.ConsolidateManufacturingStage.Vehicle;
			result.AddRange(new List<XElement>() {
				new XElement(_cif + XMLNames.Component_Model, consolidatedVehicle.Model),
				new XElement(_cif + XMLNames.CorrectedActualMass, consolidatedVehicle.CurbMassChassis.ValueAsUnit("kg", 0)),
				new XElement(_cif + XMLNames.Vehicle_ZeroEmissionVehicle, consolidatedVehicle.ZeroEmissionVehicle),
				new XElement(_cif + XMLNames.Vehicle_HybridElectricHDV, consolidatedVehicle.HybridElectricHDV),
				new XElement(_cif + "WasteHeatRecovery", completedBusData.JobInputData.PrimaryVehicle.Vehicle.Components.EngineInputData.WHRType != WHRType.None),
				new XElement(_cif + XMLNames.Vehicle_DualFuelVehicle, consolidatedVehicle.DualFuelVehicle),
				new XElement(_cif + XMLNames.Vehicle_RegisteredClass, consolidatedVehicle.RegisteredClass.ToXMLFormat()),
				new XElement(_cif + "TotalNumberOfPassengers", consolidatedVehicle.NumberPassengerSeatsLowerDeck 
																+ consolidatedVehicle.NumberPassengerSeatsUpperDeck 
																+ consolidatedVehicle.NumberPassengersStandingLowerDeck 
																+ consolidatedVehicle.NumberPassengersStandingUpperDeck),
				!consolidatedVehicle.VehicleTypeApprovalNumber.IsNullOrEmpty() ? new XElement(_cif + XMLNames.VehicleTypeApprovalNumber, consolidatedVehicle.VehicleTypeApprovalNumber) : null
			});

			return result;
		}

		protected XElement GetManufacturers(IMultistageBusInputDataProvider completedBusData)
		{
			var manufacturers = new XElement(_cif + "Manufacturers",
				GetManufacturerAndAddress(completedBusData.JobInputData.PrimaryVehicle.Vehicle.Manufacturer,
					completedBusData.JobInputData.PrimaryVehicle.Vehicle.ManufacturerAddress, 1));
			foreach (var step in completedBusData.JobInputData.ManufacturingStages) {
				manufacturers.Add(GetManufacturerAndAddress(step.Vehicle.Manufacturer, step.Vehicle.ManufacturerAddress,
					step.StepCount));
			}

			return manufacturers;
		}

		#endregion
	}

	public class PEVCompletedBusVehicleTypeGroup : AbstractCIFGroupWriter
	{
		private XElement GetManufacturerAndAddress(string manufacturer, string manufacturerAddress, int stepCount)
		{
			return new XElement(_cif + "Step",
				new XAttribute(XMLNames.ManufacturingStep_StepCount, stepCount),
				new XElement(_cif + XMLNames.Component_Manufacturer, manufacturer),
				new XElement(_cif + XMLNames.Component_ManufacturerAddress, manufacturerAddress));
		}
		public PEVCompletedBusVehicleTypeGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var completedBusData = inputData as IMultistageBusInputDataProvider;
			if (completedBusData == null) {
				throw new ArgumentException(
					$"{nameof(inputData)} must implement {nameof(IMultistageBusInputDataProvider)}");
			}
			var result = new List<XElement>();
			result.AddRange(
				_cifFactory.GetGeneralVehicleSequenceGroupWriterCompletedBus().GetElements(completedBusData));
			result.Add(GetManufacturers(completedBusData));



			var consolidatedVehicle = completedBusData.JobInputData.ConsolidateManufacturingStage.Vehicle;
			result.AddRange(new List<XElement>() {
				new XElement(_cif + XMLNames.Component_Model, consolidatedVehicle.Model),
				new XElement(_cif + XMLNames.CorrectedActualMass, consolidatedVehicle.CurbMassChassis.ValueAsUnit("kg", 0)),
				new XElement(_cif + XMLNames.Vehicle_ZeroEmissionVehicle, consolidatedVehicle.ZeroEmissionVehicle),
				new XElement(_cif + XMLNames.Vehicle_HybridElectricHDV, consolidatedVehicle.HybridElectricHDV),
				new XElement(_cif + XMLNames.Vehicle_RegisteredClass, consolidatedVehicle.RegisteredClass.ToXMLFormat()),
				new XElement(_cif + "TotalNumberOfPassengers", consolidatedVehicle.NumberPassengerSeatsLowerDeck
																+ consolidatedVehicle.NumberPassengerSeatsUpperDeck
																+ consolidatedVehicle.NumberPassengersStandingLowerDeck
																+ consolidatedVehicle.NumberPassengersStandingUpperDeck),
				!consolidatedVehicle.VehicleTypeApprovalNumber.IsNullOrEmpty() ? new XElement(_cif + XMLNames.VehicleTypeApprovalNumber, consolidatedVehicle.VehicleTypeApprovalNumber) : null
			});

			return result;
		}

		private XElement GetManufacturers(IMultistageBusInputDataProvider completedBusData)
		{
			var manufacturers = new XElement(_cif + "Manufacturers",
				GetManufacturerAndAddress(completedBusData.JobInputData.PrimaryVehicle.Vehicle.Manufacturer,
					completedBusData.JobInputData.PrimaryVehicle.Vehicle.ManufacturerAddress, 1));
			foreach (var step in completedBusData.JobInputData.ManufacturingStages) {
				manufacturers.Add(GetManufacturerAndAddress(step.Vehicle.Manufacturer, step.Vehicle.ManufacturerAddress,
					step.StepCount));
			}

			return manufacturers;
		}

		#endregion
	}


	public class ExemptedCompletedBusVehicleTypeGroup : CompletedBusVehicleTypeGroup
	{
		public ExemptedCompletedBusVehicleTypeGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var completedBusData = inputData as IMultistageBusInputDataProvider;
			if (completedBusData == null) {
				throw new ArgumentException(
					$"{nameof(inputData)} must implement {nameof(IMultistageBusInputDataProvider)}");
			}
			var result = new List<XElement>();
			result.AddRange(
				_cifFactory.GetGeneralVehicleSequenceGroupWriterCompletedBus().GetElements(completedBusData));
			result.Add(GetManufacturers(completedBusData));



			var consolidatedVehicle = completedBusData.JobInputData.ConsolidateManufacturingStage.Vehicle;
			result.AddRange(new List<XElement>() {
				new XElement(_cif + XMLNames.Component_Model, consolidatedVehicle.Model),
				new XElement(_cif + XMLNames.CorrectedActualMass, consolidatedVehicle.CurbMassChassis.ValueAsUnit("kg", 0)),
				new XElement(_cif + XMLNames.Vehicle_ZeroEmissionVehicle, consolidatedVehicle.ZeroEmissionVehicle),
				new XElement(_cif + XMLNames.Vehicle_HybridElectricHDV, consolidatedVehicle.HybridElectricHDV),
				new XElement(_cif + XMLNames.Vehicle_RegisteredClass, consolidatedVehicle.RegisteredClass.ToXMLFormat()),
				new XElement(_cif + "TotalNumberOfPassengers", consolidatedVehicle.NumberPassengerSeatsLowerDeck
																+ consolidatedVehicle.NumberPassengerSeatsUpperDeck
																+ consolidatedVehicle.NumberPassengersStandingLowerDeck
																+ consolidatedVehicle.NumberPassengersStandingUpperDeck),
				!consolidatedVehicle.VehicleTypeApprovalNumber.IsNullOrEmpty() ? new XElement(_cif + XMLNames.VehicleTypeApprovalNumber, consolidatedVehicle.VehicleTypeApprovalNumber) : null
			});

			return result;
		}

	}
}
