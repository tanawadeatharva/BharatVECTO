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
	
	internal class MRFConventionalLorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFConventionalLorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of MRFComponentType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var addTorqueConverterData =
				inputData.JobInputData.Vehicle.Components.GearboxInputData.Type == GearboxType.ATPowerSplit ||
				inputData.JobInputData.Vehicle.Components.GearboxInputData.Type == GearboxType.ATSerial;
			var addRetarderInputData = inputData.JobInputData.Vehicle.Components.RetarderInputData != null;
			var addAngleDriveData = inputData.JobInputData.Vehicle.Components.AngledriveInputData != null;
			var addAirdragdata = inputData.JobInputData.Vehicle.Components.AirdragInputData != null;
			return new XElement(_mrf + XMLNames.Vehicle_Components, 
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				_mrfFactory.GetTransmissionType().GetXmlType(inputData),
				(addRetarderInputData ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null),
				(addTorqueConverterData ? _mrfFactory.GetTorqueConverterType().GetXmlType(inputData) : null),
				(addAngleDriveData ? _mrfFactory.GetAngleDriveType().GetXmlType(inputData) : null),
				(addAirdragdata ? _mrfFactory.GetAirdragType().GetXmlType(inputData) : null),
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				_mrfFactory.GetConventionalLorryAuxType().GetXmlType(inputData.JobInputData.Vehicle.Components.AuxiliaryInputData)
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

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var result = new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				_mrfFactory.GetTransmissionType().GetXmlType(inputData),
				_mrfFactory.GetRetarderType().GetXmlType(inputData),
				_mrfFactory.GetTorqueConverterType().GetXmlType(inputData),
				_mrfFactory.GetAngleDriveType().GetXmlType(inputData),
				_mrfFactory.GetAxleGearType().GetXmlType(inputData),
				_mrfFactory.GetHEV_LorryAuxiliariesType().GetXmlType(inputData.JobInputData.Vehicle.Components.AuxiliaryInputData)
			);
			return result;
		}

		#endregion
	}

	internal class MRFHEV_S2_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_S2_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				_mrfFactory.GetTransmissionType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				components.TorqueConverterInputData != null
					? _mrfFactory.GetTorqueConverterType().GetXmlType(inputData)
					: null,
				_mrfFactory.GetAxleGearType().GetXmlType(inputData),
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				components.AngledriveInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData) : null,
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}

	internal class MRFHEV_S3_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_S3_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleGearType().GetXmlType(inputData),
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData) : null,
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}

	internal class MRFHEV_S4_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_S4_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData) : null,
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}
		#endregion
	}

	internal class MRFHEV_IEPC_S_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_IEPC_S_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData) : null,
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetIEPCSpecifications().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}

	internal class MRFPEV_E2_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_E2_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetTransmissionType().GetXmlType(inputData),
				_mrfFactory.GetAxleGearType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				components.TorqueConverterInputData != null
					? _mrfFactory.GetTorqueConverterType().GetXmlType(inputData)
					: null,
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData) : null,
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData)
			);
		}
	}

	internal class MRFPEV_E3_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_E3_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetAxleGearType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData) : null,
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData)
			);
		}

	}

	internal class MRFPEV_E4_LorryComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_E4_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData) : null,
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData)
			);
		}
	}

	internal class MRFConventional_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFConventional_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				_mrfFactory.GetTransmissionType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetAxleGearType().GetXmlType(inputData) : null,
				components.TorqueConverterInputData != null ? _mrfFactory.GetTorqueConverterType().GetXmlType(inputData) : null,
				components.AngledriveInputData != null ? _mrfFactory.GetAngleDriveType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				_mrfFactory.GetPrimaryBusAuxType_Conventional().GetXmlType(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}


	internal class MRFHEV_Px_IHPC_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_Px_IHPC_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	internal class MRFHEV_S2_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_S2_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				_mrfFactory.GetTransmissionType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleGearType().GetXmlType(inputData),
				components.TorqueConverterInputData != null
					? _mrfFactory.GetTorqueConverterType().GetXmlType(inputData)
					: null,
				components.AngledriveInputData != null ? _mrfFactory.GetAngleDriveType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				_mrfFactory.GetPrimaryBusAuxType_HEV_S().GetXmlType(inputData.JobInputData.Vehicle.Components.BusAuxiliaries),
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}

	internal class MRFHEV_S3_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_S3_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleGearType().GetXmlType(inputData),
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				_mrfFactory.GetPrimaryBusAuxType_HEV_S().GetXmlType(inputData.JobInputData.Vehicle.Components.BusAuxiliaries),
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}

	internal class MRFHEV_S4_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_S4_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				_mrfFactory.GetPrimaryBusAuxType_HEV_S().GetXmlType(inputData.JobInputData.Vehicle.Components.BusAuxiliaries),
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}

	internal class MRFHEV_IEPC_S_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFHEV_IEPC_S_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleGearType().GetXmlType(inputData),
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				_mrfFactory.GetPrimaryBusAuxType_HEV_P().GetXmlType(inputData.JobInputData.Vehicle.Components.BusAuxiliaries),
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetIEPCSpecifications().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}

	internal class MRFPEV_E2_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_E2_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public  XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetTransmissionType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				components.TorqueConverterInputData != null ? _mrfFactory.GetTorqueConverterType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleGearType().GetXmlType(inputData),
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				_mrfFactory.GetPrimaryBusAuxType_PEV().GetXmlType(inputData.JobInputData.Vehicle.Components.BusAuxiliaries),
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}

	internal class MRFPEV_E3_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_E3_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public  XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleGearType().GetXmlType(inputData),
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				_mrfFactory.GetPrimaryBusAuxType_PEV().GetXmlType(inputData.JobInputData.Vehicle.Components.BusAuxiliaries),
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}


	internal class MRFPEV_E4_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_E4_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public  XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				_mrfFactory.GetPrimaryBusAuxType_PEV().GetXmlType(inputData.JobInputData.Vehicle.Components.BusAuxiliaries),
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}

	internal class MRFPEV_IEPC_PrimaryBusComponentsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFPEV_IEPC_PrimaryBusComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public  XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				_mrfFactory.GetPrimaryBusAuxType_PEV().GetXmlType(inputData.JobInputData.Vehicle.Components.BusAuxiliaries),
				_mrfFactory.GetIEPCSpecifications().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}
}
