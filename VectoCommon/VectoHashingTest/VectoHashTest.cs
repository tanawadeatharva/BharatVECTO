using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;
using TUGraz.VectoHashing;
using Assert = NUnit.Framework.Assert;

namespace VectoHashingTest
{
	[TestFixture]
	public class VectoHashTest
	{
		public const string ReferenceXMLEngine = @"Testdata\XML\Reference\vecto_engine-sample.xml";
		public const string ReferenceXMLVehicle = @"Testdata\XML\Reference\vecto_vehicle-sample_FULL.xml";

		[TestCase]
		public void TestComponentsEngineFile()
		{
			var h = VectoHash.Load(ReferenceXMLEngine);
			var components = h.GetContainigComponents().ToList();

			Assert.AreEqual(1, components.Count);
			Assert.AreEqual(VectoComponents.Engine, components[0]);
		}

		[TestCase]
		public void TestComponentsVehicleFile()
		{
			var h = VectoHash.Load(ReferenceXMLVehicle);
			var components = h.GetContainigComponents().ToList();

			Assert.AreEqual(10, components.Count);
		}

		[TestCase(ReferenceXMLEngine, VectoComponents.Engine, BasicHasingTests.HashEngineXML)]
		public void TestHashComputationSelected(string file, VectoComponents component, string expectedHash)
		{
			var h = VectoHash.Load(file);
			var hash = h.ComputeHash(component);

			Assert.AreEqual(expectedHash, hash);
		}

		[TestCase(ReferenceXMLVehicle, BasicHasingTests.HashVehicleXML),
		TestCase(ReferenceXMLEngine, BasicHasingTests.HashEngineXML)]
		public void TestHashComputation(string file, string expectedHash)
		{
			var h = VectoHash.Load(file);
			var hash = h.ComputeHash();

			Assert.AreEqual(expectedHash, hash);
		}

		[TestCase(ReferenceXMLVehicle, VectoComponents.Engine, "e0c253b643f7f8f09b963aca4a264d06fbfa599f"),
		TestCase(ReferenceXMLVehicle, VectoComponents.Gearbox, "d14189366134120e08fa3f2c6e3328dd13c08a23")]
		public void TestReadHash(string file, VectoComponents component, string expectedHash)
		{
			var h = VectoHash.Load(file);
			var existingHash = h.ReadHash(component);

			Assert.AreEqual(expectedHash, existingHash);
		}

		[TestCase(ReferenceXMLVehicle, VectoComponents.Tyre, 0, "5074334bb2c090c5e258e9a664f5d19689a3f13d"),
		TestCase(ReferenceXMLVehicle, VectoComponents.Tyre, 1, "6074334bb2c090c5e258e9a664f5d19689a3f13d")]
		public void TestReadHashIdx(string file, VectoComponents component, int index, string expectedHash)
		{
			var h = VectoHash.Load(file);
			var existingHash = h.ReadHash(component, index);

			Assert.AreEqual(expectedHash, existingHash);
		}

		[TestCase("vecto_vehicle-sample_FULL_Comments.xml", BasicHasingTests.HashVehicleXML),
		TestCase("vecto_vehicle-sample_FULL_Entry_Order.xml", BasicHasingTests.HashVehicleXML),
		TestCase("vecto_vehicle-sample_FULL_Newlines_Linux_LF.xml", BasicHasingTests.HashVehicleXML),
		TestCase("vecto_vehicle-sample_FULL_Newlines_Mac_CR.xml", BasicHasingTests.HashVehicleXML),
		TestCase("vecto_vehicle-sample_FULL_Newlines_Windows_CRLF.xml", BasicHasingTests.HashVehicleXML),
		TestCase("vecto_engine-sample Encoding ISO 8859-15.xml", BasicHasingTests.HashEngineXML),
		TestCase("vecto_engine-sample Encoding UTF-8 BOM.xml", BasicHasingTests.HashEngineXML),
		TestCase("vecto_engine-sample Encoding UTF-8.xml", BasicHasingTests.HashEngineXML),
		TestCase("vecto_engine-sample Encoding UTF-16 BE BOM.xml", BasicHasingTests.HashEngineXML),
		TestCase("vecto_engine-sample Encoding UTF-16 LE.xml", BasicHasingTests.HashEngineXML),
		TestCase("vecto_engine-sample Encoding windows-1292.xml", BasicHasingTests.HashEngineXML),
		TestCase("vecto_engine-sample_Whitespaces.xml", BasicHasingTests.HashEngineXML),
		]
		public void TestHashComputationVariations(string file, string expectedHash)
		{
			var h = VectoHash.Load(@"Testdata\XML\Variations\" + file);
			var hash = h.ComputeHash();

			Assert.AreEqual(expectedHash, hash);
		}


		[TestCase(@"Testdata\XML\ToHash\vecto_engine-input.xml"),
		TestCase(@"Testdata\XML\ToHash\vecto_gearbox-input.xml")]
		public void TestAddHash(string file)
		{
			var destination = Path.GetFileNameWithoutExtension(file) + "_hashed.xml";

			var h = VectoHash.Load(file);
			var r = h.AddHash();

			var writer = new XmlTextWriter(destination, Encoding.UTF8);
			r.WriteTo(writer);
			writer.Flush();
			writer.Close();

			var h2 = VectoHash.Load(destination);
			Assert.IsTrue(h2.ValidateHash());
		}
	}
}