using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
	#region Lorry
	internal class MRFConventionalLorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFConventionalLorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of MRFComponentType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var addTorqueConverterData =
				inputData.JobInputData.Vehicle.Components.GearboxInputData.Type == GearboxType.ATPowerSplit ||
				inputData.JobInputData.Vehicle.Components.GearboxInputData.Type == GearboxType.ATSerial;
			var addRetarderInputData = inputData.JobInputData.Vehicle.Components.RetarderInputData != null;
			var addAngleDriveData = inputData.JobInputData.Vehicle.Components.AngledriveInputData != null;
			var addAirdragdata = inputData.JobInputData.Vehicle.Components.AirdragInputData != null;
			return new XElement(_mrf + XMLNames.Vehicle_Components, 
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetTransmissionType().GetElement(inputData),
				(addTorqueConverterData ? _mrfFactory.GetTorqueConverterType().GetElement(inputData) : null),
				(addAngleDriveData ? _mrfFactory.GetAngleDriveType().GetElement(inputData) : null),
				(addRetarderInputData ? _mrfFactory.GetRetarderType().GetElement(inputData) : null),
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetConventionalLorryAuxType().GetXmlType(inputData.JobInputData.Vehicle.Components.AuxiliaryInputData),
				(addAirdragdata ? _mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle.Components.AirdragInputData) : null)
			);
			//return new XElement(_mrf + XMLNames.Vehicle_Components, 
			//	_mrfFactory.GetEngineType())
		}

		#endregion
	}

	internal class MRFHEV_Px_IHPC_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_Px_IHPC_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var result = new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetTransmissionType().GetElement(inputData),
				_mrfFactory.GetTorqueConverterType().GetElement(inputData),
				_mrfFactory.GetAngleDriveType().GetElement(inputData),
				_mrfFactory.GetRetarderType().GetElement(inputData),
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetHEV_LorryAuxiliariesType().GetXmlType(inputData.JobInputData.Vehicle.Components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle.Components.AirdragInputData)
			);
			return result;
		}

		#endregion
	}

	internal class MRFHEV_S2_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_S2_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetTransmissionType().GetElement(inputData),
				
				components.TorqueConverterInputData != null
					? _mrfFactory.GetTorqueConverterType().GetElement(inputData)
					: null,
				components.AngledriveInputData != null ? _mrfFactory.GetAngleDriveType().GetElement(inputData) : null,
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,

				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetHEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle.Components.AirdragInputData) : null
				);
	
		}

		#endregion
	}

	internal class MRFHEV_S3_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_S3_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetHEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle.Components.AirdragInputData) : null);
		}

		#endregion
	}

	internal class MRFHEV_S4_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_S4_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetElement(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetHEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle.Components.AirdragInputData) : null);
		}
		#endregion
	}

	internal class MRFHEV_IEPC_S_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_IEPC_S_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetIEPCSpecifications().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetElement(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetHEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle.Components.AirdragInputData) : null);
		}

		#endregion
	}

	internal class MRFPEV_E2_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_E2_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetTransmissionType().GetElement(inputData),
				components.TorqueConverterInputData != null
					? _mrfFactory.GetTorqueConverterType().GetElement(inputData)
					: null,
				components.RetarderInputData != null 
					? _mrfFactory.GetRetarderType().GetElement(inputData) 
					: null,

				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle.Components.AirdragInputData) : null
				);
		}
	}

	internal class MRFPEV_E3_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_E3_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,

				
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPEV_LorryAuxiliariesType().GetXmlType(inputData.JobInputData.Vehicle.Components.AuxiliaryInputData),
				components.AirdragInputData != null
					? _mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle.Components.AirdragInputData)
					: null
			);
		}

	}

	internal class MRFPEV_E4_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_E4_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPEV_LorryAuxiliariesType().GetXmlType(inputData.JobInputData.Vehicle.Components.AuxiliaryInputData),
				components.AirdragInputData != null 
					? _mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle.Components.AirdragInputData) 
					: null
			);
		}
	}

	#endregion Lorry


	#region PrimaryBus
	internal class MRFConventional_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFConventional_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetTransmissionType().GetElement(inputData),
				components.TorqueConverterInputData != null ? _mrfFactory.GetTorqueConverterType().GetElement(inputData) : null,
				components.AngledriveInputData != null ? _mrfFactory.GetAngleDriveType().GetElement(inputData) : null,
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetElement(inputData) : null,

	
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_Conventional().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}


	internal class MRFHEV_Px_IHPC_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_Px_IHPC_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetTransmissionType().GetElement(inputData),
				_mrfFactory.GetTorqueConverterType().GetElement(inputData),
				_mrfFactory.GetAngleDriveType().GetElement(inputData),
				_mrfFactory.GetRetarderType().GetElement(inputData),
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_Conventional().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MRFHEV_S2_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_S2_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),

				_mrfFactory.GetTransmissionType().GetElement(inputData),

				components.TorqueConverterInputData != null
					? _mrfFactory.GetTorqueConverterType().GetElement(inputData)
					: null,

				components.AngledriveInputData != null 
					? _mrfFactory.GetAngleDriveType().GetElement(inputData)
					: null,

				components.RetarderInputData != null
					? _mrfFactory.GetRetarderType().GetElement(inputData)
					: null,

				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_HEV_S().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MRFHEV_S3_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_S3_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),


				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_HEV_S().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)

				
				);
		}

		#endregion
	}

	internal class MRFHEV_S4_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_S4_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_HEV_S().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)

				
				);
		}

		#endregion
	}

	internal class MRFHEV_IEPC_S_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_IEPC_S_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetIEPCSpecifications().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_HEV_P().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MRFPEV_E2_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_E2_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public  XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetTransmissionType().GetElement(inputData),
				components.TorqueConverterInputData != null ? _mrfFactory.GetTorqueConverterType().GetElement(inputData) : null,
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_PEV().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MRFPEV_E3_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_E3_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public  XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_PEV().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
				);
		}

		#endregion
	}


	internal class MRFPEV_E4_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_E4_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public  XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetElectricMachinesType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_PEV().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MRFPEV_IEPC_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_IEPC_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public  XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetElement(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_PEV().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries),
				_mrfFactory.GetIEPCSpecifications().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData));
		}

		#endregion
	}

	#endregion
	#region CompletedBus
	internal class MRFConventional_CompletedBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFConventional_CompletedBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var multistageInputData = inputData as IMultistageBusInputDataProvider;
			if (multistageInputData == null) {
				throw new ArgumentException($"inputData must implement {nameof(IMultistageBusInputDataProvider)}");
			}
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.AirdragInputData != null
					? _mrfFactory.GetAirdragType().GetXmlType(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.AirdragInputData) : null,
				_mrfFactory.GetConventionalCompletedBusAuxType().GetElement(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MRFHEV_CompletedBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_CompletedBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	internal class MRFPEV_CompletedBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_CompletedBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	#endregion
}
