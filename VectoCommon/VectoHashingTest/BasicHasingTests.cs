using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoHashing;
using TUGraz.VectoHashing.Impl;
using TUGraz.VectoHashing.Util;

namespace VectoHashingTest
{
	[TestClass]
	public class BasicHasingTests
	{
		public const string SimpleXML = @"Testdata\XML\simple_document.xml";
		public const string ReferenceXMLEngine = @"Testdata\XML\Reference\vecto_engine-sample.xml";
		public const string ReferenceXMLVehicle = @"Testdata\XML\Reference\vecto_vehicle-sample_FULL.xml";

		public const string UnorderedXMLVehicle = @"Testdata\XML\Variations\vecto_vehicle-sample_FULL_Entry_Order.xml";

		public const string HashSimpleXML = "U2zic7KOnKw60rzh+KKQ1lwZL6NmXju+DXG7cYYmlxo=";
		public const string HashEngineXML = "cfPKB2LkHIbznFA9aQwCNfNLSj9V7qNnSskyOxaXB+o=";
		public const string HashVehicleXML = "yZCH9sF1GUdawVOa1fKQ2zvuUHg5ZthmitTOcWg/s1Y=";

		[TestMethod]
		public void HashSimpleXml()
		{
			var elementToHash = "elemID";
			var doc = new XmlDocument();
			doc.Load(SimpleXML);
			var hasher = new XMLHashProvider();
			var hashed = XMLHashProvider.ComputeHash(doc, elementToHash);

			var hash = GetHashValue(hashed, elementToHash);
			WriteSignedXML(doc, "simple_document_hashed.xml");


			Assert.AreEqual(HashSimpleXML, hash);
		}

		[TestMethod]
		public void HashReferenceEngineXML()
		{
			var elementToHash = "ENG-gooZah3D";
			var doc = new XmlDocument();
			doc.Load(ReferenceXMLEngine);
			var hasher = new XMLHashProvider();
			var hashed = XMLHashProvider.ComputeHash(doc, elementToHash);

			var hash = GetHashValue(hashed, elementToHash);
			WriteSignedXML(doc, "reference_engine_hashed.xml");


			Assert.AreEqual(HashEngineXML, hash);
		}

		[TestMethod]
		public void HashReferenceVehicleXML()
		{
			var elementToHash = "VEH-1234567890";
			var doc = new XmlDocument();
			doc.Load(ReferenceXMLVehicle);
			var hasher = new XMLHashProvider();
			var hashed = XMLHashProvider.ComputeHash(doc, elementToHash);

			var hash = GetHashValue(hashed, elementToHash);
			WriteSignedXML(doc, "reference_vehicle_hashed.xml");


			Assert.AreEqual(HashVehicleXML, hash);
		}

		[TestMethod]
		public void HashUnorderedVehicleXML()
		{
			var elementToHash = "VEH-1234567890";
			var doc = new XmlDocument();
			doc.Load(UnorderedXMLVehicle);
			var hasher = new XMLHashProvider();
			var hashed = XMLHashProvider.ComputeHash(doc, elementToHash);

			var hash = GetHashValue(hashed, elementToHash);
			WriteSignedXML(doc, "reference_vehicle_hashed.xml");


			Assert.AreEqual(HashVehicleXML, hash);
		}

		private static string GetHashValue(XmlDocument hashed, string elementToHash)
		{
			var xdoc = hashed.ToXDocument();
			var hash = xdoc.XPathSelectElement("//*[@URI='#" + elementToHash + "']/*[local-name() = 'DigestValue']").Value;
			return hash;
		}

		private static void WriteSignedXML(XmlDocument doc, string filename)
		{
			var xmltw = new XmlTextWriter(filename, new UTF8Encoding(false));
			doc.WriteTo(xmltw);
			xmltw.Close();
		}
	}
}