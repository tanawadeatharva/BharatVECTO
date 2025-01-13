using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public abstract class PrimaryComponentVIFType : IXmlTypeWriter
	{
		protected readonly IVIFReportFactory _vifReportFactory;
		protected XNamespace _vif = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1";
		protected XNamespace _xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		protected PrimaryComponentVIFType(IVIFReportFactory vifReportFactory)
		{
			_vifReportFactory = vifReportFactory;
		}

		#region Implementation of IXmlTypeWriter

		public abstract XElement GetElement(IDeclarationInputDataProvider inputData);

		#endregion
	}
	

	public class ConventionalComponentVIFType : PrimaryComponentVIFType
	{
		public ConventionalComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
				return new XElement(_vif + XMLNames.Vehicle_Components,
					new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_Conventional_ComponentsVIFType"),
					_vifReportFactory.GetEngineType().GetElement(inputData),
					_vifReportFactory.GetTransmissionType().GetElement(inputData),
					_vifReportFactory.GetTorqueConvertType().GetElement(inputData),
					_vifReportFactory.GetAngelDriveType().GetElement(inputData),
					_vifReportFactory.GetRetarderType().GetElement(inputData), 
					_vifReportFactory.GetAxlegearType().GetElement(inputData),
					_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
					_vifReportFactory.GetAuxiliaryType().GetElement(inputData));
		}

		#endregion
	}


	public class HevIepcSComponentVIFType : PrimaryComponentVIFType
	{
		public HevIepcSComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-IEPC-S_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineGENType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetIepcType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryIEPC_SType().GetElement(inputData));
		}

		#endregion
	}

	public class HevPxComponentVIFType : PrimaryComponentVIFType
	{
		public HevPxComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-Px_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetTransmissionType().GetElement(inputData),
				_vifReportFactory.GetTorqueConvertType().GetElement(inputData),
				_vifReportFactory.GetAngelDriveType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryHevPType().GetElement(inputData));
		}

		#endregion
	}

	public class HevS2ComponentVIFType: PrimaryComponentVIFType
	{
		public HevS2ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-S2_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineGENType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetTransmissionType().GetElement(inputData),
				_vifReportFactory.GetTorqueConvertType().GetElement(inputData),
				_vifReportFactory.GetAngelDriveType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryHevSType().GetElement(inputData));
		}

		#endregion
	}


	public class HevS3ComponentVIFType : PrimaryComponentVIFType
	{
		public HevS3ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-S3_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineGENType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryHevSType().GetElement(inputData));
		}

		#endregion
	}


	public class HevS4ComponentVIFType : PrimaryComponentVIFType
	{
		public HevS4ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-S4_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineGENType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryHevSType().GetElement(inputData));
		}

		#endregion
	}

	public class HevF2ComponentVIFType : PrimaryComponentVIFType
	{
		public HevF2ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-F2_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineGENType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetTransmissionType().GetElement(inputData),
				_vifReportFactory.GetTorqueConvertType().GetElement(inputData),
				_vifReportFactory.GetAngelDriveType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData),
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryHevSType().GetElement(inputData));
		}

		#endregion
	}


	public class HevF3ComponentVIFType : PrimaryComponentVIFType
	{
		public HevF3ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-F3_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineGENType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData),
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryHevSType().GetElement(inputData));
		}

		#endregion
	}


	public class HevF4ComponentVIFType : PrimaryComponentVIFType
	{
		public HevF4ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-F4_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineGENType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryHevSType().GetElement(inputData));
		}

		#endregion
	}

	public class HevIepcFComponentVIFType : PrimaryComponentVIFType
	{
		public HevIepcFComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-IEPC-F_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineGENType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetIepcType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData),
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryIEPC_SType().GetElement(inputData));
		}

		#endregion
	}


	public class PevE2ComponentVIFType : PrimaryComponentVIFType
	{
		public PevE2ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_PEV-E2_ComponentsVIFType"),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetTransmissionType().GetElement(inputData),
				_vifReportFactory.GetTorqueConvertType().GetElement(inputData),
				_vifReportFactory.GetAngelDriveType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryPEVType().GetElement(inputData));
		}

		#endregion
	}


	public class PevE3ComponentVIFType : PrimaryComponentVIFType
	{
		public PevE3ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_PEV-E3_ComponentsVIFType"),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryPEVType().GetElement(inputData));
		}

		#endregion
	}


	public class PevE4ComponentVIFType : PrimaryComponentVIFType
	{
		public PevE4ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_PEV-E4_ComponentsVIFType"),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryPEVType().GetElement(inputData));
		}

		#endregion
	}


	public class IepcComponentVIFType : PrimaryComponentVIFType
	{
		public IepcComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_PEV-IEPC_ComponentsVIFType"),
				_vifReportFactory.GetIepcType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryIEPCType().GetElement(inputData));
		}

		#endregion
	}

}
