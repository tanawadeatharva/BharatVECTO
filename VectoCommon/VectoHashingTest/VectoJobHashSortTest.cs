using System.IO;
using NUnit.Framework;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoHashing;

namespace VectoHashingTest
{
    public class VectoJobHashSortTest
    {

		private const string UnsortedComponentPath = @"Testdata\XML\Sort\Job\Unsorted\";
		private const string SortedComponentPath = @"Testdata\XML\Sort\Job\Sorted\";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase("HpFMjk3vmSp7FuZD6FEyBmDyrix7ifdcrKI26a5qEiw=", SortedComponentPath + "HEV-S_heavyLorry_IEPC-S.xml")]
		[TestCase("HpFMjk3vmSp7FuZD6FEyBmDyrix7ifdcrKI26a5qEiw=", UnsortedComponentPath + "HEV-S_heavyLorry_IEPC-S.xml")]
		public void TestJobHashSort(string expectedJobHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);

			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Engine));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricMachineSystem));//ElectricMachineGEN
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.REESS,0));//ElectricEnergyStorage
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.REESS,1));//ElectricEnergyStorage
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.IEPC));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Retarder));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Axlegear));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre,0));//AxelWheels
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre,1));//AxelWheels
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Airdrag));

			Assert.AreEqual(expectedJobHash, loadedFile.ComputeHash());
		}

	}
}
