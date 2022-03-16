using System.IO;
using NUnit.Framework;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoHashing;

namespace VectoHashingTest
{
    public class VectoJobHashSortTest
    {
        private const string UnsortedJobPath = @"Testdata\XML\Sort\Job\Unsorted\";
        private const string SortedJobPath = @"Testdata\XML\Sort\Job\Sorted\";

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [TestCase("HpFMjk3vmSp7FuZD6FEyBmDyrix7ifdcrKI26a5qEiw=", SortedJobPath + "HEV-S_heavyLorry_IEPC-S.xml")]
        [TestCase("HpFMjk3vmSp7FuZD6FEyBmDyrix7ifdcrKI26a5qEiw=", UnsortedJobPath + "HEV-S_heavyLorry_IEPC-S.xml")]
        public void TestJobHEV_S_HeavyLorryHashSort(string expectedJobHash, string filePath)
        {
            var loadedFile = VectoHash.Load(filePath);

            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Engine));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricMachineSystem));//ElectricMachineGEN
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.REESS, 0));//ElectricEnergyStorage
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.REESS, 1));//ElectricEnergyStorage
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.IEPC));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Retarder));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Axlegear));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 0));//AxelWheels
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 1));//AxelWheels
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Airdrag));

            Assert.AreEqual(expectedJobHash, loadedFile.ComputeHash());
        }

        [TestCase("gn9Zq6pNRbb+cwWh2m08uUDbWBUufMrQmdICqqWL/Y8=", SortedJobPath + "Conventional_primaryBus_AMT.xml")]
        [TestCase("gn9Zq6pNRbb+cwWh2m08uUDbWBUufMrQmdICqqWL/Y8=", UnsortedJobPath + "Conventional_primaryBus_AMT.xml")]
        public void TestJobPrimaryBusHashSort(string expectedJobHash, string filePath)
        {
            var loadedFile = VectoHash.Load(filePath);

            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Engine));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Gearbox));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Axlegear));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Retarder));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.TorqueConverter));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Angledrive));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 0));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 1));

            Assert.AreEqual(expectedJobHash, loadedFile.ComputeHash());

        }
    }
}
