using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationPrimaryVehicleBusApplicationInformationDataProviderV01 : AbstractXMLType, IXMLApplicationInformationData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_PRIMARY_BUS_VEHICLE_URI_V01;

		public const string XSD_TYPE = "ApplicationInformationPIFType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationPrimaryVehicleBusApplicationInformationDataProviderV01(XmlNode applicationNode) 
			: base(applicationNode) { }

		public string SimulationToolVersion
		{
			get { return GetString(XMLNames.Report_ApplicationInfo_SimulationToolVersion); }
		}

		public DateTime Date
		{
			get { return XmlConvert.ToDateTime(GetString(XMLNames.Report_ApplicationInfo_Date), XmlDateTimeSerializationMode.Utc); }
		}
	}
}