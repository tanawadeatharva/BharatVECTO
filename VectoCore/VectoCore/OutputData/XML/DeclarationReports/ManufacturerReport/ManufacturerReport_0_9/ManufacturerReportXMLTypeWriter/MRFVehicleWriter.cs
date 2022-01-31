using System;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
    public abstract class MRFVehicleType : AbstractMrfXmlType, IMrfXmlType
    {
		

		public MRFVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public abstract XElement GetXmlType(IDeclarationInputDataProvider inputData);
	}


	public class MRF_ConventionalLorryVehicleType : MRFVehicleType
	{
		public MRF_ConventionalLorryVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMRFComponentWriter

		#endregion

		#region Overrides of AbstractMRFComponentWriter

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetConventionalLorryVehicleOutputGroup().GetElements(inputData),

				_mrfFactory.GetEngineTorqueLimitationsType().GetXmlType(inputData),
				_mrfFactory.GetConventionalLorryComponentsType().GetXmlType(inputData)




				);
				
		}

		#endregion
	}

	public class MRF_HEV_Px_IHPC_LorryVehicleType : MRFVehicleType
	{
		public MRF_HEV_Px_IHPC_LorryVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMRFComponentWriter
		#endregion

		#region Overrides of AbstractMRFComponentWriter

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_lorryVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetEngineTorqueLimitationsType().GetXmlType(inputData),
				_mrfFactory.GetHEV_Px_IHCP_LorryComponentsType().GetXmlType(inputData)
				
				
				);
		}

		#endregion
	}

	public class MRF_HEV_S2_LorryVehicleType : MRFVehicleType
	{
		public MRF_HEV_S2_LorryVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_lorryVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetEngineTorqueLimitationsType().GetXmlType(inputData),
				_mrfFactory.GetHEV_S2_LorryComponentsType().GetXmlType(inputData));
		}

		#endregion
	}

	public class MRF_HEV_S3_LorryVehicleType : MRFVehicleType
	{
		public MRF_HEV_S3_LorryVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_lorryVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetHEV_S3_LorryComponentsType().GetXmlType(inputData));
		}

		#endregion
	}

	public class MRF_HEV_S4_LorryVehicleType : MRFVehicleType
	{
		public MRF_HEV_S4_LorryVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_lorryVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetHEV_S4_LorryComponentsType().GetXmlType(inputData));
		}

		#endregion
	}

	public class MRF_HEV_IEPC_S_LorryVehicleType : MRFVehicleType
	{
		public MRF_HEV_IEPC_S_LorryVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
			//return new XElement()
		}

		#endregion
	}

	public class MRF_PEV_E2_LorryVehicleType : MRFVehicleType
	{
		public MRF_PEV_E2_LorryVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_lorryVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetPEV_E2_LorryComponentsType().GetXmlType(inputData));
		}

		#endregion
	}

	public class MRF_PEV_E3_LorryVehicleType : MRFVehicleType
	{
		public MRF_PEV_E3_LorryVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_lorryVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetPEV_E3_LorryComponentsType().GetXmlType(inputData));
		}

		#endregion
	}
	public class MRF_PEV_E4_LorryVehicleType : MRFVehicleType
	{
		public MRF_PEV_E4_LorryVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_lorryVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetPEV_E4_LorryComponentsType().GetXmlType(inputData));
		}

		#endregion
	}




	public class MRF_Conventional_PrimaryBusVehicleType : MRFVehicleType
	{
		public MRF_Conventional_PrimaryBusVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPrimaryBusGeneralVehicleOutputGroup().GetElements(inputData),
				new XElement(_mrf + XMLNames.Vehicle_DualFuelVehicle, inputData.JobInputData.Vehicle.DualFuelVehicle),
				_mrfFactory.GetConventionalADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
				_mrfFactory.GetConventional_PrimaryBusComponentsType().GetXmlType(inputData)
			);
		}

		#endregion
	}


	public class MRF_HEV_Px_IHPC_PrimaryBusVehicleType : MRFVehicleType
	{
		public MRF_HEV_Px_IHPC_PrimaryBusVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetHEV_Px_IHPC_PrimaryBusComponentsType().GetXmlType(inputData)
			);
		}

		#endregion
	}

	public class MRF_HEV_S2_PrimaryBusVehicleType : MRFVehicleType
	{
		public MRF_HEV_S2_PrimaryBusVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetHEV_S2_PrimaryBusComponentsType().GetXmlType(inputData));
		}

		#endregion
	}

	public class MRF_HEV_S3_PrimaryBusVehicleType : MRFVehicleType
	{
		public MRF_HEV_S3_PrimaryBusVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetHEV_S3_PrimaryBusComponentsType().GetXmlType(inputData));
		}

		#endregion
	}
	public class MRF_HEV_S4_PrimaryBusVehicleType : MRFVehicleType
	{
		public MRF_HEV_S4_PrimaryBusVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetHEV_S4_PrimaryBusComponentsType().GetXmlType(inputData));
		}

		#endregion
	}

	public class MRF_HEV_IEPC_S_PrimaryBusVehicleType : MRFVehicleType
	{
		public MRF_HEV_IEPC_S_PrimaryBusVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetHEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetHEV_IEPC_S_PrimaryBusComponentsType().GetXmlType(inputData));
		}

		#endregion
	}

	public class MRF_PEV_E2_PrimaryBusVehicleType : MRFVehicleType
	{
		public MRF_PEV_E2_PrimaryBusVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetPEV_E2_PrimaryBusComponentsType().GetXmlType(inputData));
		}

		#endregion
	}

	public class MRF_PEV_E3_PrimaryBusVehicleType : MRFVehicleType
	{
		public MRF_PEV_E3_PrimaryBusVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetPEV_E3_PrimaryBusComponentsType().GetXmlType(inputData));
		}

		#endregion
	}

	public class MRF_PEV_E4_PrimaryBusVehicleType : MRFVehicleType
	{
		public MRF_PEV_E4_PrimaryBusVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetPEV_E4_PrimaryBusComponentsType().GetXmlType(inputData));
		}

		#endregion
	}

	public class MRF_PEV_IEPC_PrimaryBusVehicleType : MRFVehicleType
	{
		public MRF_PEV_IEPC_PrimaryBusVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetPEV_PrimaryBusVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetPEV_IEPC_PrimaryBusComponentsType().GetXmlType(inputData));
		}

		#endregion
	}

	public class MRF_Conventional_CompletedBusVehicleType : MRFVehicleType
	{
		public MRF_Conventional_CompletedBusVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var multistageInputdata = inputData as IMultistageBusInputDataProvider;
			if (multistageInputdata == null) {
				throw new ArgumentException($"inputdata must implement {nameof(IMultistageBusInputDataProvider)}");
			}
			return new XElement(_mrf + XMLNames.Component_Vehicle, 
				_mrfFactory.GetCompletedBusGeneralVehicleOutputGroup().GetElements(inputData),
				_mrfFactory.GetConventionalADASType().GetXmlType(multistageInputdata.JobInputData.ConsolidateManufacturingStage.Vehicle.ADAS),
				_mrfFactory.GetConventional_CompletedBusComponentsType().GetXmlType(inputData)
				);

		}

		#endregion
	}

	public class MRF_HEV_CompletedBusVehicleType : MRFVehicleType
	{
		public MRF_HEV_CompletedBusVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class MRF_PEV_CompletedBusVehicleType : MRFVehicleType
	{
		public MRF_PEV_CompletedBusVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}





}
