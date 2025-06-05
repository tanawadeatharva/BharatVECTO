using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
    public abstract class VehicleTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
    {
		

		public VehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public abstract XElement GetElement(IDeclarationInputDataProvider inputData);
	}


	public class ConventionalLorryVehicleTypeWriter : VehicleTypeWriter
	{
		public ConventionalLorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMRFComponentWriter

		#endregion

		#region Overrides of AbstractMRFComponentWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;

			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetConventionalLorryVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetEngineTorqueLimitationsType().GetElement(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetConventionalLorryComponentsType().GetElement(inputData)
			);
				
		}

		#endregion
	}

	public class HevPxIhpcLorryVehicleTypeWriter : VehicleTypeWriter
	{
		public HevPxIhpcLorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMRFComponentWriter
		#endregion

		#region Overrides of AbstractMRFComponentWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_lorryVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetEngineTorqueLimitationsType().GetElement(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetHEV_Px_IHCP_LorryComponentsType().GetElement(inputData)
			);
		}

		#endregion
	}

	public class HevS2LorryVehicleTypeWriter : VehicleTypeWriter
	{
		public HevS2LorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_lorryVehicleOutputGroup().GetElements(inputData),
                //_mrfFactory.GetEngineTorqueLimitationsType().GetElement(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetHEV_S2_LorryComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class HevS3LorryVehicleTypeWriter : VehicleTypeWriter
	{
		public HevS3LorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_lorryVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetHEV_S3_LorryComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class HevS4LorryVehicleTypeWriter : VehicleTypeWriter
	{
		public HevS4LorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_lorryVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetHEV_S4_LorryComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class HevIepcSLorryVehicleTypeWriter : VehicleTypeWriter
	{
		public HevIepcSLorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{

			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_lorryVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetHEV_IEPC_S_LorryComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class FCHV_F2_LorryVehicleTypeWriter : VehicleTypeWriter
	{
		public FCHV_F2_LorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
                _mrfFactory.GetFCHV_lorryVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetFCHV_F2_LorryComponentsType().GetElement(inputData));
		}
	}

	public class FCHV_F3_LorryVehicleTypeWriter : VehicleTypeWriter
	{
		public FCHV_F3_LorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetFCHV_lorryVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetFCHV_F3_LorryComponentsType().GetElement(inputData));
		}
	}

	public class FCHV_F4_LorryVehicleTypeWriter : VehicleTypeWriter
	{
		public FCHV_F4_LorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetFCHV_lorryVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetFCHV_F4_LorryComponentsType().GetElement(inputData));
		}
	}

	public class FCHV_IEPC_LorryVehicleTypeWriter : VehicleTypeWriter
	{
		public FCHV_IEPC_LorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{

			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetFCHV_lorryVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetFCHV_IEPC_LorryComponentsType().GetElement(inputData));
		}
	}

	public class PevE2LorryVehicleTypeWriter : VehicleTypeWriter
	{
		public PevE2LorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_lorryVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetPEV_E2_LorryComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class PevE3LorryVehicleTypeWriter : VehicleTypeWriter
	{
		public PevE3LorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_lorryVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetPEV_E3_LorryComponentsType().GetElement(inputData));
		}

		#endregion
	}
	public class PevE4LorryVehicleTypeWriter : VehicleTypeWriter
	{
		public PevE4LorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_lorryVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetPEV_E4_LorryComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class PevIEPCLorryVehicleTypeWriter : VehicleTypeWriter
	{
		public PevIEPCLorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_lorryVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetPEV_IEPC_S_LorryComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class ExemptedLorryVehicleTypeWriter : VehicleTypeWriter
	{
		public ExemptedLorryVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of VehicleTypeWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;
			var tech = new string(inputData.JobInputData.Vehicle.ExemptedTechnology
				.Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '.').ToArray());

            return new XElement(_mrf + XMLNames.Component_Vehicle,
				new XElement(_mrf + XMLNames.Component_Manufacturer, vehicle.Manufacturer),
				new XElement(_mrf + XMLNames.ManufacturerAddress, vehicle.ManufacturerAddress),
				_mrfFactory.GetGeneralVehicleOutputGroup().GetElements(vehicle),
				new XElement(_mrf + XMLNames.CorrectedActualMass, vehicle.CurbMassChassis.ValueAsUnit("kg")),
				new XElement(_mrf + XMLNames.Vehicle_ZeroEmissionVehicle, vehicle.ZeroEmissionVehicle),
                new XElement(_mrf + XMLNames.Vehicle_SleeperCab, vehicle.SleeperCab),
                new XElement(_mrf + "VehicleTechnologyExempted", tech),
				new XElement(_mrf + XMLNames.Exempted_SumNetPower, inputData.JobInputData.Vehicle.MaxNetPower1.ValueAsUnit(XMLNames.Unit_kW)),
				new XElement(_mrf + "SimulationToolLicenseNumber", vehicle.SimulationToolLicenseNumber ?? "N/A")
            );
		}

		#endregion
	}


	public class ConventionalPrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public ConventionalPrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;
			var dualFuel = vehicle.Components.EngineInputData.EngineModes.Any(x => x.Fuels.Count > 1);
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPrimaryBusGeneralVehicleOutputGroup().GetElements(inputData),
				new XElement(_mrf + XMLNames.Vehicle_DualFuelVehicle, dualFuel),
				_mrfFactory.GetConventionalADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
				_mrfFactory.GetEngineTorqueLimitationsType().GetElement(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetConventional_PrimaryBusComponentsType().GetElement(inputData)
			);
		}

		#endregion
	}


	public class HevPxIhpcPrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public HevPxIhpcPrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetEngineTorqueLimitationsType().GetElement(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetHEV_Px_IHPC_PrimaryBusComponentsType().GetElement(inputData)
			);
		}

		#endregion
	}

	public class HevS2PrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public HevS2PrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetHEV_S2_PrimaryBusComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class HevS3PrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public HevS3PrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetHEV_S3_PrimaryBusComponentsType().GetElement(inputData));
		}

		#endregion
	}
	public class HevS4PrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public HevS4PrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetHEV_S4_PrimaryBusComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class HevIepcSPrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public HevIepcSPrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetHEV_IEPC_S_PrimaryBusComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class FCHV_F2_PrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public FCHV_F2_PrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetFCHV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetFCHV_F2_PrimaryBusComponentsType().GetElement(inputData));
		}
	}

	public class FCHV_F3_PrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public FCHV_F3_PrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetFCHV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetFCHV_F3_PrimaryBusComponentsType().GetElement(inputData));
		}
	}

	public class FCHV_F4_PrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public FCHV_F4_PrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetFCHV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetFCHV_F4_PrimaryBusComponentsType().GetElement(inputData));
		}
	}

	public class FCHV_IEPC_PrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public FCHV_IEPC_PrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetFCHV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetFCHV_IEPC_PrimaryBusComponentsType().GetElement(inputData));
		}
	}

	public class PevE2PrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public PevE2PrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetPEV_E2_PrimaryBusComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class PevE3PrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public PevE3PrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetPEV_E3_PrimaryBusComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class PevE4PrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public PevE4PrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetPEV_E4_PrimaryBusComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class PevIepcPrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public PevIepcPrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetPEV_IEPC_PrimaryBusComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class ExemptedPrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public ExemptedPrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of VehicleTypeWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var tech = new string(inputData.JobInputData.Vehicle.ExemptedTechnology
				.Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '.').ToArray());

			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetExemptedPrimaryBusGeneralVehicleOutputGroup().GetElements(inputData),
				new XElement(_mrf + "VehicleTechnologyExempted", tech),
				new XElement(_mrf + XMLNames.Exempted_SumNetPower, inputData.JobInputData.Vehicle.MaxNetPower1.ValueAsUnit(XMLNames.Unit_kW)),
                new XElement(_mrf + "SimulationToolLicenseNumber", inputData.JobInputData.Vehicle.SimulationToolLicenseNumber ?? "N/A")
            );
		}

		#endregion
	}

	public class ConventionalCompletedBusVehicleTypeWriter : VehicleTypeWriter
	{
		public ConventionalCompletedBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var multistageInputdata = inputData as IMultistepBusInputDataProvider;
			if (multistageInputdata == null) {
				throw new ArgumentException($"inputdata must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			return new XElement(_mrf + XMLNames.Component_Vehicle, 
				_mrfFactory.GetConventionalCompletedBusGeneralVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetConventionalADASType().GetXmlType(multistageInputdata.JobInputData.ConsolidateManufacturingStage.Vehicle.ADAS),
                new XElement(_mrf + "SimulationToolLicenseNumber", multistageInputdata.JobInputData.ConsolidateManufacturingStage.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetConventional_CompletedBusComponentsType().GetElement(inputData)
				);

		}

		#endregion
	}

	public class HevCompletedBusVehicleTypeWriter : VehicleTypeWriter
	{
		public HevCompletedBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var multistageInputdata = inputData as IMultistepBusInputDataProvider;
			if (multistageInputdata == null) {
				throw new ArgumentException($"inputdata must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEVCompletedBusGeneralVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetHEVADASType().GetXmlType(multistageInputdata.JobInputData.ConsolidateManufacturingStage.Vehicle.ADAS),
                new XElement(_mrf + "SimulationToolLicenseNumber", multistageInputdata.JobInputData.ConsolidateManufacturingStage.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetHEV_CompletedBusComponentsType().GetElement(inputData)
			);
		}

		#endregion
	}

	public class PevCompletedBusVehicleTypeWriter : VehicleTypeWriter
	{
		public PevCompletedBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var multistageInputdata = inputData as IMultistepBusInputDataProvider;
			if (multistageInputdata == null) {
				throw new ArgumentException($"inputdata must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEVCompletedBusGeneralVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetPEVADASType().GetXmlType(multistageInputdata.JobInputData.ConsolidateManufacturingStage.Vehicle.ADAS),
                new XElement(_mrf + "SimulationToolLicenseNumber", multistageInputdata.JobInputData.ConsolidateManufacturingStage.Vehicle.SimulationToolLicenseNumber ?? "N/A"),
                _mrfFactory.GetPEV_CompletedBusComponentsType().GetElement(inputData)
			);
		}

		#endregion
	}

	public class ExemptedCompletedBusVehicleTypeWriter : VehicleTypeWriter
	{
		public ExemptedCompletedBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of VehicleTypeWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var multistageInputdata = inputData as IMultistepBusInputDataProvider;
			if (multistageInputdata == null)
			{
				throw new ArgumentException($"inputdata must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			var primaryVehicleData = multistageInputdata.JobInputData.PrimaryVehicle.Vehicle;
			var consolidatedVehicleData = multistageInputdata.JobInputData.ConsolidateManufacturingStage.Vehicle;
			var result = new XElement(_mrf + XMLNames.Component_Vehicle);
			var manufacturers = new XElement(_mrf + "Manufacturers");
			result.Add(manufacturers);
			manufacturers.Add(GetManufacturerAndAddress(primaryVehicleData.Manufacturer, primaryVehicleData.ManufacturerAddress, 1));
			foreach (var manufacturingStageInputData in multistageInputdata.JobInputData.ManufacturingStages) {
				manufacturers.Add(GetManufacturerAndAddress(manufacturingStageInputData.Vehicle.Manufacturer,
					manufacturingStageInputData.Vehicle.ManufacturerAddress,
					stepCount: manufacturingStageInputData.StepCount));
			}
			result.Add(_mrfFactory.GetGeneralVehicleOutputGroup().GetElements(multistageInputdata.JobInputData.ConsolidateManufacturingStage.Vehicle));
			result.Add(
				new XElement(_mrf + XMLNames.CorrectedActualMass,
					consolidatedVehicleData.CurbMassChassis.ValueAsUnit("kg")),
				new XElement(_mrf + XMLNames.Vehicle_ZeroEmissionVehicle, consolidatedVehicleData.ZeroEmissionVehicle),
				new XElement(_mrf + XMLNames.Vehicle_RegisteredClass,
					consolidatedVehicleData.RegisteredClass.ToXMLFormat()),
				new XElement(_mrf + XMLNames.Bus_NumberPassengersUpperDeck,
					consolidatedVehicleData.NumberPassengerSeatsUpperDeck +
					consolidatedVehicleData.NumberPassengersStandingUpperDeck),
				new XElement(_mrf + XMLNames.Bus_NumberPassengersLowerDeck,
					consolidatedVehicleData.NumberPassengerSeatsLowerDeck +
					consolidatedVehicleData.NumberPassengersStandingLowerDeck),
				new XElement(_mrf + XMLNames.Vehicle_BodyworkCode, consolidatedVehicleData.VehicleCode.ToXMLFormat()),
				new XElement(_mrf + XMLNames.Bus_LowEntry, consolidatedVehicleData.LowEntry),
                new XElement(_mrf + "SimulationToolLicenseNumber", multistageInputdata.JobInputData.PrimaryVehicle.Vehicle.SimulationToolLicenseNumber ?? "N/A")
                );
			//result.Add(_mrfFactory.GetCompletedBusSequenceGroup().GetElements(consolidatedVehicleData));
			return result;

			//return new XElement(_mrf + XMLNames.Component_Vehicle,
			//	_mrfFactory.GetCompletedBusGeneralVehicleOutputGroup().GetElements(inputData)
			//);
		}

		#endregion

		protected XElement GetManufacturerAndAddress(string manufacturer, string address, int stepCount)
		{
			return new XElement(_mrf + "Step",
				new XAttribute("Count", stepCount),
				new XElement(_mrf + XMLNames.Component_Manufacturer, manufacturer),
				new XElement(_mrf + XMLNames.Component_ManufacturerAddress, address));
		}
	}
}
