using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter
{
    public class GeneralVehicleSequenceGroupCIF : AbstractCIFGroupWriter
	{
		public GeneralVehicleSequenceGroupCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			var result = new List<XElement>() {
				new XElement(_cif + XMLNames.Vehicle_VIN, vehicleData.VIN),
				new XElement(_cif + XMLNames.Vehicle_VehicleCategory, vehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(_cif + XMLNames.Vehicle_AxleConfiguration, vehicleData.AxleConfiguration.ToXMLFormat()),
				new XElement(_cif + XMLNames.Vehicle_TPMLM, XMLHelper.ValueAsUnit(vehicleData.GrossVehicleMassRating, "kg")),
				new XElement(_cif + XMLNames.Report_Vehicle_VehicleGroup, vehicleData.VehicleCategory.ToXMLFormat())

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
				new XElement(_cif + "VehicleGroupCO2", "todo"),
				new XElement(_cif + XMLNames.Component_Manufacturer, vehicleData.Manufacturer),
				new XElement(_cif + XMLNames.Component_ManufacturerAddress, vehicleData.ManufacturerAddress),
				new XElement(_cif + XMLNames.Component_Model, vehicleData.Model),
				new XElement(_cif + XMLNames.VehicleTypeApprovalNumber, vehicleData.VehicleTypeApprovalNumber),
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

	public class HEV_LorryVehicleTypeGroupCIF : AbstractCIFGroupWriter
	{
		public HEV_LorryVehicleTypeGroupCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			result.AddRange(_cifFactory.GetGeneralVehicleSequenceGroupWriter().GetElements(inputData));
			result.AddRange(_cifFactory.GetLorryGeneralVehicleSequenceGroupWriter().GetElements(inputData));
			result.AddRange(_cifFactory.GetHEV_VehicleSequenceGroupWriter().GetElements(inputData));

			return result;
		}

		#endregion
	}


	public class HEV_VehicleSequenceGroupWriter : AbstractCIFGroupWriter
	{
		public HEV_VehicleSequenceGroupWriter(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

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
				result.Add(new XElement(_cif + "OffVehicleChargingMaxPower", inputData.JobInputData.Vehicle.MaxChargingPower.ValueAsUnit("kW")));
			}
			return result;
		}

		#endregion
	}
}
