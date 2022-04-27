using System;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

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
				_mrfFactory.GetEngineTorqueLimitationsType().GetElement(inputData),
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
				_mrfFactory.GetHEV_IEPC_S_LorryComponentsType().GetElement(inputData));
		}

		#endregion
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
				_mrfFactory.GetPEV_IEPC_S_LorryComponentsType().GetElement(inputData));
		}

		#endregion
	}




	public class ConventionalPrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public ConventionalPrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPrimaryBusGeneralVehicleOutputGroup().GetElements(inputData),
				new XElement(_mrf + XMLNames.Vehicle_DualFuelVehicle, inputData.JobInputData.Vehicle.DualFuelVehicle),
				_mrfFactory.GetConventionalADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
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
				_mrfFactory.GetHEV_IEPC_S_PrimaryBusComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class PevE2PrimaryBusVehicleTypeWriter : VehicleTypeWriter
	{
		public PevE2PrimaryBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
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
				_mrfFactory.GetPEV_IEPC_PrimaryBusComponentsType().GetElement(inputData));
		}

		#endregion
	}

	public class ConventionalCompletedBusVehicleTypeWriter : VehicleTypeWriter
	{
		public ConventionalCompletedBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var multistageInputdata = inputData as IMultistageBusInputDataProvider;
			if (multistageInputdata == null) {
				throw new ArgumentException($"inputdata must implement {nameof(IMultistageBusInputDataProvider)}");
			}
			return new XElement(_mrf + XMLNames.Component_Vehicle, 
				_mrfFactory.GetCompletedBusGeneralVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetConventionalADASType().GetXmlType(multistageInputdata.JobInputData.ConsolidateManufacturingStage.Vehicle.ADAS),
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
			throw new NotImplementedException();
		}

		#endregion
	}

	public class PevCompletedBusVehicleTypeWriter : VehicleTypeWriter
	{
		public PevCompletedBusVehicleTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}





}
