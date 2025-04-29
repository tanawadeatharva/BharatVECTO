using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Exceptions;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v27
{
    public class XMLDeclaration_Conventional_HeavyLorry_DataProviderV27 : AbstractXMLVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Conventional_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Conventional_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) { }

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.ConventionalVehicle;
    }

    public class XMLDeclaration_PHEV_HeavyLorry_DataProviderV27 : AbstractXMLVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_HEV-Px_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_PHEV_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) 
        {}

        public override string PowertrainPositionPrefix => "P";

        public override bool HybridElectricHDV => true;

        public override VectoSimulationJobType VehicleType => Components.ElectricMachines.Entries.Any(em => em.ElectricMachine.IsIHPC()) 
            ? VectoSimulationJobType.IHPC 
            : VectoSimulationJobType.ParallelHybridVehicle;
        
        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));

        public override XmlElement PTONode => (VehicleType == VectoSimulationJobType.IHPC) ? null : base.PTONode;

        public override IPTOTransmissionInputData PTOTransmissionInputData => (VehicleType == VectoSimulationJobType.IHPC) ? null : base.PTOTransmissionInputData;
    }

    public class XMLDeclaration_SHEV_HeavyLorry_DataProviderV27 : AbstractXMLVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_HEV-Sx_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_SHEV_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) { }

        public override IList<ITorqueLimitInputData> TorqueLimits => null;

        public override string PowertrainPositionPrefix => "E";
                
        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.SerialHybridVehicle;

        public override bool HybridElectricHDV => true;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));
    }

    public class XMLDeclaration_SHEV_IEPC_HeavyLorry_DataProviderV27 : XMLDeclaration_SHEV_HeavyLorry_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_HEV-IEPC-S_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_SHEV_IEPC_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) { }

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.IEPC_S;

        public override XmlElement PTONode => null;

        public override IPTOTransmissionInputData PTOTransmissionInputData => null;
    }

    public class XMLDeclaration_PEV_HeavyLorry_DataProviderV27 : AbstractXMLVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_PEV_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_PEV_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) 
        {
            if (!OVC)
            {
                throw new VectoException("OVC must be set to true for PEVs.");
            }
        
            if (!BatteryOnlyMode)
            {
                throw new VectoException("BatteryOnlyMode must be set to true for PEVs.");
            }
        }

        public override string PowertrainPositionPrefix => "E";

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));

        public override IList<ITorqueLimitInputData> TorqueLimits => null;

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.BatteryElectricVehicle;
    }

    public class XMLDeclaration_PEV_IEPC_HeavyLorry_DataProviderV27 : XMLDeclaration_PEV_HeavyLorry_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_IEPC_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_PEV_IEPC_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) 
        {}

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.IEPC_E;

        public override XmlElement PTONode => null;

        public override IPTOTransmissionInputData PTOTransmissionInputData => null;
    }

    public class XMLDeclaration_FCHV_HeavyLorry_DataProviderV27 : XMLDeclaration_PEV_HeavyLorry_DataProviderV27
    {
		public new const string XSD_TYPE = "Vehicle_FCHV_Fx_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_FCHV_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) { }

        public override string PowertrainPositionPrefix => "F";

		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.FCHV;

        public override bool HybridElectricHDV => true;

        public override ArchitectureID ArchitectureID => base.ArchitectureID;
    }

	public class XMLDeclaration_FCHV_IEPC_HeavyLorry_DataProviderV27 : XMLDeclaration_FCHV_HeavyLorry_DataProviderV27
    {
		public new const string XSD_TYPE = "Vehicle_FCHV_IEPC_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclaration_FCHV_IEPC_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }
		
		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.FCHV_IEPC;

        public override XmlElement PTONode => null;

        public override IPTOTransmissionInputData PTOTransmissionInputData => null;
    }
}
