using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.VectoCore.Utils
{
    internal static class MockupResultReader
    {
        
		public static XElement GetMockupResult(string xmlName, XMLDeclarationReport.ResultEntry result, XName resultElementName)
		{
			var xDoc = XDocument.Load(RessourceHelper.ReadStream("TUGraz.VectoCore.Resources.MockupResults.xml"));
			var elements = xDoc.Root.Elements();

			var mockUpElement = elements.Single(x => x.Name == xmlName);
			var resultElement = new XElement(resultElementName, mockUpElement.Elements());
			resultElement.Elements().Single(x => x.Name.LocalName == XMLNames.Report_Result_Mission).Value =
				result.Mission.ToXMLFormat();


			return resultElement;
		}




    }
}
