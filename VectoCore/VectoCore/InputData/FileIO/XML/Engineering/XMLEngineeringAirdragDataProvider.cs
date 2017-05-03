using System;
using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringAirdragDataProvider : AbstractEngineeringXMLComponentDataProvider,
		IAirdragEngineeringInputData
	{
		public XMLEngineeringAirdragDataProvider(XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XPathDocument axlegearDocument, string xmlBasePath, string fsBasePath)
			: base(xmlEngineeringJobInputDataProvider, axlegearDocument, xmlBasePath, fsBasePath) {}

		public SquareMeter AirDragArea
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_AirDragArea).SI<SquareMeter>(); }
		}

		public CrossWindCorrectionMode CrossWindCorrectionMode
		{
			get { return GetElementValue(XMLNames.Vehicle_CrossWindCorrectionMode).ParseEnum<CrossWindCorrectionMode>(); }
		}

		public TableData CrosswindCorrectionMap
		{
			get {
				return ReadTableData(AttributeMappings.CrossWindCorrectionMapping,
					Helper.Query(XMLNames.Vehicle_CrosswindCorrectionData, XMLNames.Vehicle_CrosswindCorrectionData_Entry));
			}
		}
	}
}