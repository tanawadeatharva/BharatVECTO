using System.Xml.Linq;
using System.Xml;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCommon.Exceptions;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v27
{
    public abstract class AbstractXMLDeclarationExemptedVehicleDataProviderV27 : AbstractXMLDeclarationExemptedVehicleDataProviderV24
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        
        public AbstractXMLDeclarationExemptedVehicleDataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) :
            base(jobData, xmlNode, sourceFile, true)
        {}

        public override string SimulationToolLicenseNumber => GetString("SimulationToolLicenseNumber");

        public override XmlElement MonitoringNode => _monitoringNode ?? (_monitoringNode = GetNode("MonitoringData", required: false) as XmlElement);

        public override string VehicleMonitoringData => MonitoringReader?.Data;
    }

    public class XMLDeclaration_Exempted_HeavyLorry_DataProviderV27 : AbstractXMLDeclarationExemptedVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Exempted_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Exempted_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) { }   
    }

    public class XMLDeclaration_Exempted_MediumLorry_DataProviderV27 : AbstractXMLDeclarationExemptedVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Exempted_MediumLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Exempted_MediumLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) 
            : base(jobData, xmlNode, sourceFile) { }
    }

    public class XMLDeclaration_Exempted_PrimaryBus_DataProviderV27 : AbstractXMLDeclarationExemptedVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Exempted_PrimaryBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Exempted_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) 
        { }

        public override bool Articulated => GetBool("Articulated");
    }

    public class XMLDeclaration_Exempted_CompletedBus_DataProviderV27 : AbstractXMLDeclarationExemptedVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Exempted_CompletedBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Exempted_CompletedBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) { }

        public override string SimulationToolLicenseNumber => ElementExists("SimulationToolLicenseNumber")
            ? GetString("SimulationToolLicenseNumber")
            : null;

        public override RegistrationClass? RegisteredClass => ElementExists(XMLNames.Vehicle_RegisteredClass)
            ? RegistrationClassHelper.Parse(GetString(XMLNames.Vehicle_RegisteredClass))[0]
            : null;

        public override int? NumberPassengerSeatsLowerDeck => ElementExists(XMLNames.Bus_NumberPassengerSeatsLowerDeck)
            ? XmlConvert.ToInt32(GetNode(XMLNames.Bus_NumberPassengerSeatsLowerDeck).InnerText)
            : (int?)null;
            
        public override int? NumberPassengerSeatsUpperDeck => ElementExists(XMLNames.Bus_NumberPassengerSeatsUpperDeck)
            ? XmlConvert.ToInt32(GetNode(XMLNames.Bus_NumberPassengerSeatsUpperDeck).InnerText)
            : (int?)null;
        
        public override int? NumberPassengersStandingLowerDeck => ElementExists(XMLNames.Bus_NumberPassengersStandingLowerDeck)
            ? XmlConvert.ToInt32(GetNode(XMLNames.Bus_NumberPassengersStandingLowerDeck).InnerText)
            : (int?)null;
        
        public override int? NumberPassengersStandingUpperDeck => ElementExists(XMLNames.Bus_NumberPassengersStandingUpperDeck)
            ? XmlConvert.ToInt32(GetNode(XMLNames.Bus_NumberPassengersStandingUpperDeck).InnerText)
            : (int?)null;

        public override VehicleCode? VehicleCode => ElementExists(XMLNames.Vehicle_BodyworkCode)
            ? GetString(XMLNames.Vehicle_BodyworkCode).ParseEnum<VehicleCode>()
            : (VehicleCode?)null;

        public override bool? LowEntry => ElementExists(XMLNames.Bus_LowEntry)
            ? GetBool(XMLNames.Bus_LowEntry)
            : (bool?)null;

        public override Meter Height => ElementExists(XMLNames.Bus_HeightIntegratedBody)
            ? GetDouble(XMLNames.Bus_HeightIntegratedBody).SI(Unit.SI.Milli.Meter).Cast<Meter>()
            : null;

        public override string Model => ElementExists(XMLNames.Component_Model) ? GetString(XMLNames.Component_Model) : null;

        public override LegislativeClass? LegislativeClass => ElementExists(XMLNames.Vehicle_LegislativeCategory)
            ? GetString(XMLNames.Vehicle_LegislativeCategory)?.ParseEnum<LegislativeClass>()
            : null;

        public override Kilogram CurbMassChassis => ElementExists(XMLNames.CorrectedActualMass)
            ? GetDouble(XMLNames.CorrectedActualMass).SI<Kilogram>()
            : null;

        public override Kilogram GrossVehicleMassRating => ElementExists(XMLNames.Vehicle_TPMLM) 
            ? GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>()
            : null;
    }
}
