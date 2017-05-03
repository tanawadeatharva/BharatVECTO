using System;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoHashing;

namespace VectoHashingTest
{
	[TestClass]
	public class UnitTest1
	{
		public const string SimpleXML = @"Testdata\XML\simple_document.xml";

		[TestMethod]
		public void TestCompareHashing()
		{
			var hasher1 = new XmlHashProvider(SimpleXML);
			var hash1 = hasher1.ComputeHash("//*[@id='elemID']");

			var doc = new XmlDocument();
			doc.Load(SimpleXML);
			var hasher2 = new XmlHashTest();
			var hash2 = hasher2.ComputeHash(doc);

			Assert.AreEqual(hash1, hash2);
		}
	}
}