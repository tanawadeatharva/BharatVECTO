using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
    public class XMLMonitoringReaderV27 : AbstractComponentReader, IXMLMonitoringReader
    {
        public XMLMonitoringReaderV27(IXMLDeclarationVehicleData vehicle, XmlNode monitoringNode) : base(vehicle, monitoringNode)
        {}

        public static XNamespace NAMESPACE_URI => XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

        public static string QUALIFIED_XSD_TYPE_EXEMPTED => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "Exempted_DataType");
        public static string QUALIFIED_XSD_TYPE_CONVENTIONAL => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "Conventional_DataType");
        public static string QUALIFIED_XSD_TYPE_COMPLETED => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "Completed_DataType");
        public static string QUALIFIED_XSD_TYPE_PHEV => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "HEV-Px_IHPC_DataType");
        public static string QUALIFIED_XSD_TYPE_SHEV_S2 => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "HEV-S2_DataType");
        public static string QUALIFIED_XSD_TYPE_SHEV_S3 => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "HEV-S3_DataType");
        public static string QUALIFIED_XSD_TYPE_SHEV_S4 => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "HEV-S4_DataType");
        public static string QUALIFIED_XSD_TYPE_SHEV_IEPC => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "HEV-IEPC-S_DataType");
        public static string QUALIFIED_XSD_TYPE_PEV_E2 => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "PEV-E2_DataType");
        public static string QUALIFIED_XSD_TYPE_PEV_E3 => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "PEV-E3_DataType");
        public static string QUALIFIED_XSD_TYPE_PEV_E4 => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "PEV-E4_DataType");
        public static string QUALIFIED_XSD_TYPE_PEV_IEPC => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "PEV-IEPC_DataType");
        public static string QUALIFIED_XSD_TYPE_FCHV_F2 => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "FCHV_F2_DataType");
        public static string QUALIFIED_XSD_TYPE_FCHV_F3 => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "FCHV_F3_DataType");
        public static string QUALIFIED_XSD_TYPE_FCHV_F4 => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "FCHV_F4_DataType");
        public static string QUALIFIED_XSD_TYPE_FCHV_IEPC => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "FCHV_IEPC_DataType");
        public static string QUALIFIED_XSD_TYPE_MULTIPLE_PEV => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "Multiple_PEV_DataType");
        public static string QUALIFIED_XSD_TYPE_MULTIPLE_FCHV => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "Multiple_FCHV_DataType");
        public static string QUALIFIED_XSD_TYPE_MULTIPLE_SHEV => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "Multiple_SHEV_DataType");

        public string Data => BaseNode.OuterXml;
    }
}
