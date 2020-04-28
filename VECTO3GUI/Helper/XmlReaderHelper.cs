using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Castle.Core.Internal;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using TUGraz.VectoCommon.Resources;

namespace VECTO3GUI.Helper
{
	public static class XmlReaderHelper
	{
		public static XmlDocument ReadXmlDocument(string filePath)
		{
			if (filePath.IsNullOrEmpty())
				return null;

			var xmlDocument = new XmlDocument();
			
			using (var reader = new XmlTextReader(filePath)) {
				xmlDocument.Load(reader);	
			}

			return xmlDocument;
		}


		public static XmlNodeList GetComponentNodes(XmlDocument xmlDocument, string parentNode, string nodeName)
		{
			if (xmlDocument == null || parentNode.IsNullOrEmpty() || nodeName.IsNullOrEmpty())
				return null;

			return xmlDocument.SelectNodes($"//*[local-name()='{parentNode}']//*[local-name()='{nodeName}']");
		}

		

	}
}
