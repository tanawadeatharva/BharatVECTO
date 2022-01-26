using System.IO;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoHashing;

namespace VectoHashingTest
{
    public class VectoHashSortTest
    {
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase(@"Testdata\XML\Sort\Axlegear.xml")]
		public void TestHash(string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			var hash = loadedFile.ComputeHash(VectoComponents.Axlegear);
		}

		[TestCase("ttvMnXYdQrEAu47QEO2AKyfzOdPSlcdsR/MrmH/mX+k=", @"Testdata\XML\Sort\Engine.xml"),
		TestCase("ttvMnXYdQrEAu47QEO2AKyfzOdPSlcdsR/MrmH/mX+k=", @"Testdata\XML\Sort\Engine_unsorted.xml")]
		public void TestEngineHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Engine));
		}


		[TestCase("hRzWfx3/El/LwEtP86Utm3dgBAF6CagrpEREsca7+/0=", @"Testdata\XML\Sort\Gearbox_APT-N_unsorted.xml"),
		TestCase("XZCluPiG05mOAj5rTjTllCWbhCTEYVxCbE940ck3XsA=", @"Testdata\XML\Sort\Gearbox_IHPC_unsorted.xml"),
		TestCase("LrsR3WfAkFia53dMiwGEIeAiA+9bdWvaS6x7dIL9yiQ=", @"Testdata\XML\Sort\Gearbox_FWD.xml"),
		TestCase("LrsR3WfAkFia53dMiwGEIeAiA+9bdWvaS6x7dIL9yiQ=", @"Testdata\XML\Sort\Gearbox_FWD_unsorted.xml")]
		public void TestGearboxComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Gearbox));
		}
		
		[TestCase("og41nROicUT1OoesUTZ9/uoroCUbqlTo7nKmCRYQap4=", @"Testdata\XML\Sort\Axlegear.xml"),
		TestCase("og41nROicUT1OoesUTZ9/uoroCUbqlTo7nKmCRYQap4=", @"Testdata\XML\Sort\Axlegear_unsorted.xml")]
		public void TestAxlegearComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Axlegear));
		}


		[TestCase("uMH8HJlAtm/SNaj8QOhuX/cBNXTHAZ1la3oEXI42bls=", @"Testdata\XML\Sort\Retarder.xml"),
		TestCase("uMH8HJlAtm/SNaj8QOhuX/cBNXTHAZ1la3oEXI42bls=", @"Testdata\XML\Sort\Retarder_unsorted.xml")]
		public void TestRetarderComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Retarder));
		}


		[TestCase("o5/ZtRQ8nAui3rhM6adKiHVU9jRync6f1cDlBcnyOg4=", @"Testdata\XML\Sort\TorqueConverter.xml"),
		TestCase("o5/ZtRQ8nAui3rhM6adKiHVU9jRync6f1cDlBcnyOg4=", @"Testdata\XML\Sort\TorqueConverter_unsorted.xml")]
		public void TestTorqueConverterComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.TorqueConverter));
		}

		
		[TestCase("NiyH2Xp0rQswwXIOf52Jm0wvK4Yc2/PL/T+zQCWQGFo=", @"Testdata\XML\Sort\ADC_unsorted.xml")]
		public void TestADCHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.ADC));
		}

		[TestCase("tam1LGpdznHGFGo+rp0WVr0/6+F2yU2Kv4G4tYvAe+Y=", @"Testdata\XML\Sort\BatterySystem_1_unsorted.xml")]
		public void TestBatterySystemHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.BatterySystem));
		}

		[TestCase("dBadIN60l8Iqcanj/nrx1EbD+KixtDxLAusUcutITk8=", @"Testdata\XML\Sort\CapacitorSystem_1.xml")]
		public void TestCapacitorHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.CapacitorSystem));
		}
		
		[TestCase("X5dgavua/V/jzBQeJ6SxZXsXm3i1jruL48LedzZ5IxU=", @"Testdata\XML\Sort\ElectricMachineSystem-IHPC_1_unsorted.xml"),
		TestCase("wLFLpJxFZ6mDXeqdlZCGVOLCoXTCf7XTL0q9ZKkmt7o=", @"Testdata\XML\Sort\ElectricMachineSystem_1_unsorted.xml"),
		TestCase("CunnDxsiE9kciX+v9oeEGADZpEc88NtfMtmrHyJkCQ0=", @"Testdata\XML\Sort\ElectricMachineSystem_StdValues_unsorted.xml")]
		public void TestElectricMachineHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.ElectricMachineSystem));
		}


		[TestCase("3L/fYxKTdIwzADHQMnUBPxcNwZNEHM+sKEC2M32UnEA=", @"Testdata\XML\Sort\IEPC_1_unsorted.xml"),
		TestCase("BTHs/Hh2SgycIwU5OSuTgU/2SptMvmRFvPXr2X1Y7XQ=", @"Testdata\XML\Sort\IEPC_StdValues_unsorted.xml")]
		public void TestIEPCHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.IEPC));
		}
	}
}
