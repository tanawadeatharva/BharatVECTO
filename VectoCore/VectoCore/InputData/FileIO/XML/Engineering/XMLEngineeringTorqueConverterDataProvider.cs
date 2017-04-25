using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringTorqueConverterDataProvider : AbstractEngineeringXMLComponentDataProvider,
		ITorqueConverterEngineeringInputData
	{
		public XMLEngineeringTorqueConverterDataProvider(XMLEngineeringInputDataProvider jobInputData,
			XPathDocument xmlDocument,
			string xBasePath, string fsBasePath)
			: base(jobInputData, xmlDocument, xBasePath, fsBasePath) {}

		public TableData TCData
		{
			get
			{
				if (
					ElementExists(Helper.Query(XMLNames.TorqueConverter_Characteristics, XMLNames.TorqueConverter_Characteristics_Entry))) {
					return ReadTableData(AttributeMappings.TorqueConverterDataMapping,
						Helper.Query(XMLNames.TorqueConverter_Characteristics, XMLNames.TorqueConverter_Characteristics_Entry));
				}
				return ReadCSVResourceFile(XMLNames.TorqueConverter_Characteristics);
			}
		}

		public PerSecond ReferenceRPM
		{
			get { return GetDoubleElementValue(XMLNames.TorqueConverter_ReferenceRPM).RPMtoRad(); }
		}

		public KilogramSquareMeter Inertia
		{
			get { return GetDoubleElementValue(XMLNames.TorqueConverter_Inertia).SI<KilogramSquareMeter>(); }
		}


		public TableData ShiftPolygon
		{
			get
			{
				if (ElementExists(Helper.Query(XMLNames.TorqueConverter_ShiftPolygon, XMLNames.TorqueConverter_ShiftPolygon_Entry))) {
					return ReadTableData(AttributeMappings.ShiftPolygonMapping,
						Helper.Query(XMLNames.TorqueConverter_ShiftPolygon, XMLNames.TorqueConverter_ShiftPolygon_Entry));
				}
				if (
					ElementExists(Helper.Query(XMLNames.TorqueConverter_ShiftPolygon, ExtCsvResourceTag))) {
					return ReadCSVResourceFile(XMLNames.TorqueConverter_ShiftPolygon);
				}
				return null;
			}
		}

		public PerSecond MaxInputSpeed
		{
			get
			{
				return ElementExists(Helper.Query("MaxInputSpeed"))
					? GetDoubleElementValue("MaxInputSpeed").RPMtoRad()
					: 5000.RPMtoRad();
			}
		}

		public MeterPerSquareSecond CLUpshiftMinAcceleration
		{
			get { return InputData.XMLEngineeringJobData.CLUpshiftMinAcceleration; }
		}

		public MeterPerSquareSecond CCUpshiftMinAcceleration
		{
			get
			{
				return InputData.XMLEngineeringJobData.CCUpshiftMinAcceleration;
			}
		}
	}
}