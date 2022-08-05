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
	internal class ConventionalLorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public ConventionalLorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfhevPxIhpcLorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevPxIhpcLorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfhevS2LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevS2LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfhevS3LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevS3LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfhevS4LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevS4LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfhevIepcSLorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevIepcSLorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfpevE2LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevE2LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfpevE3LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevE3LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfpevE4LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevE4LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfPevIEPCLorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfPevIEPCLorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetIEPCSpecifications().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetElement(inputData) : null,
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
	internal class ConventionalPrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public ConventionalPrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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


	internal class MrfhevPxIhpcPrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevPxIhpcPrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfhevS2PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevS2PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfhevS3PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevS3PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfhevS4PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevS4PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfhevIepcSPrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevIepcSPrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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
				_mrfFactory.GetPrimaryBusAuxType_HEV_S().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MrfpevE2PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevE2PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfpevE3PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevE3PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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


	internal class MrfpevE4PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevE4PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

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

	internal class MrfpevIepcPrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevIepcPrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public  XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetIEPCSpecifications().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetElement(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_PEV()
					.GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries));
		}

		#endregion
	}

	#endregion
	#region CompletedBus
	internal class ConventionalCompletedBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public ConventionalCompletedBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var multistageInputData = inputData as IMultistepBusInputDataProvider;
			if (multistageInputData == null) {
				throw new ArgumentException($"inputData must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.AirdragInputData != null
					? _mrfFactory.GetAirdragType().GetXmlType(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.AirdragInputData) : null,
				_mrfFactory.GetConventionalCompletedBusAuxType().GetElement(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MrfhevCompletedBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevCompletedBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var multistageInputData = inputData as IMultistepBusInputDataProvider;
			if (multistageInputData == null) {
				throw new ArgumentException($"inputData must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.AirdragInputData != null
					? _mrfFactory.GetAirdragType().GetXmlType(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.AirdragInputData) : null,
				_mrfFactory.GetHEVCompletedBusAuxType().GetElement(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MrfpevCompletedBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevCompletedBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var multistageInputData = inputData as IMultistepBusInputDataProvider;
			if (multistageInputData == null) {
				throw new ArgumentException($"inputData must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.AirdragInputData != null
					? _mrfFactory.GetAirdragType().GetXmlType(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.AirdragInputData) : null,
				_mrfFactory.GetPEVCompletedBusAuxType().GetElement(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	#endregion
}
