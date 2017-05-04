using System.Linq;
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

			Assert.AreEqual(9, components.Count);
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
	}
}