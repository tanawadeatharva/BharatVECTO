#if CERTIFICATION_RELEASE || RELEASE_CANDIDATE
#define PROHIBIT_V27_XML
#endif

//#define PROHIBIT_V27_XML

using System.Xml.Linq;
using System.Xml;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v27
{
    public abstract class AbstractXMLDeclarationExemptedVehicleDataProviderV27 : AbstractXMLDeclarationExemptedVehicleDataProviderV24
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        
        public AbstractXMLDeclarationExemptedVehicleDataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) :
            base(jobData, xmlNode, sourceFile)
        {
            CheckVehicleAllowed();
        }

        protected virtual void CheckVehicleAllowed()
        {
#if PROHIBIT_V27_XML
            throw new VectoException("this v2.7 vehicle is not supported yet.");
#endif
        }

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
}
