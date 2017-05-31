using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringAngledriveDataProvider : AbstractEngineeringXMLComponentDataProvider, IAngledriveInputData
	{
		public XMLEngineeringAngledriveDataProvider(XMLEngineeringInputDataProvider jobInputData, XPathDocument xmlDocument,
			string xBasePath, string fsBasePath) : base(jobInputData, xmlDocument, xBasePath, fsBasePath) {}

		public AngledriveType Type
		{
			get { return InputData._vehicleInputData.AngledriveType; }
		}

		public double Ratio
		{
			get { return GetDoubleElementValue(XMLNames.AngleDrive_Ratio); }
		}

		public TableData LossMap
		{
			get {
				if (ElementExists(Helper.Query(XMLNames.AngleDrive_TorqueLossMap, XMLNames.Angledrive_LossMap_Entry))) {
					return ReadTableData(AttributeMappings.TransmissionLossmapMapping,
						Helper.Query(XMLNames.AngleDrive_TorqueLossMap, XMLNames.Angledrive_LossMap_Entry));
				}
				return ReadCSVResourceFile(XMLNames.AngleDrive_TorqueLossMap);
			}
		}

		public double Efficiency
		{
			get {
				return GetDoubleElementValue(Helper.Query(XMLNames.AngleDrive_TorqueLossMap, XMLNames.AngleDrive_Efficiency));
			}
		}
	}
}