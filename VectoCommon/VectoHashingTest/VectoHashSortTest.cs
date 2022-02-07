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
		private IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;
		private const string UnsortedComponentPath = @"Testdata\XML\Sort\Component\Unsorted\";
		private const  string SortedComponentPath = @"Testdata\XML\Sort\Component\Sorted\";

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

		[TestCase("ttvMnXYdQrEAu47QEO2AKyfzOdPSlcdsR/MrmH/mX+k=", SortedComponentPath + "Engine.xml"),
		TestCase("ttvMnXYdQrEAu47QEO2AKyfzOdPSlcdsR/MrmH/mX+k=",  UnsortedComponentPath + "Engine.xml")]
		public void TestEngineHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Engine));
		}


		[TestCase("LrsR3WfAkFia53dMiwGEIeAiA+9bdWvaS6x7dIL9yiQ=", SortedComponentPath + "Gearbox_FWD.xml"),
		 TestCase("LrsR3WfAkFia53dMiwGEIeAiA+9bdWvaS6x7dIL9yiQ=", UnsortedComponentPath + "Gearbox_FWD.xml"),
		 TestCase("hRzWfx3/El/LwEtP86Utm3dgBAF6CagrpEREsca7+/0=", SortedComponentPath + "Gearbox_APT-N.xml"),
		 TestCase("hRzWfx3/El/LwEtP86Utm3dgBAF6CagrpEREsca7+/0=", UnsortedComponentPath + "Gearbox_APT-N.xml"),
		 TestCase("XZCluPiG05mOAj5rTjTllCWbhCTEYVxCbE940ck3XsA=", SortedComponentPath + "Gearbox_IHPC.xml"),
		 TestCase("XZCluPiG05mOAj5rTjTllCWbhCTEYVxCbE940ck3XsA=", UnsortedComponentPath + "Gearbox_IHPC.xml")]
		public void TestGearboxComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Gearbox));
		}
		
		[TestCase("og41nROicUT1OoesUTZ9/uoroCUbqlTo7nKmCRYQap4=", SortedComponentPath + "Axlegear.xml"),
		TestCase("og41nROicUT1OoesUTZ9/uoroCUbqlTo7nKmCRYQap4=", UnsortedComponentPath + "Axlegear.xml")]
		public void TestAxlegearComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Axlegear));
		}


		[TestCase("uMH8HJlAtm/SNaj8QOhuX/cBNXTHAZ1la3oEXI42bls=", SortedComponentPath + "Retarder.xml"),
		TestCase("uMH8HJlAtm/SNaj8QOhuX/cBNXTHAZ1la3oEXI42bls=", UnsortedComponentPath + "Retarder.xml")]
		public void TestRetarderComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Retarder));
		}


		[TestCase("o5/ZtRQ8nAui3rhM6adKiHVU9jRync6f1cDlBcnyOg4=", SortedComponentPath + "TorqueConverter.xml"),
		TestCase("o5/ZtRQ8nAui3rhM6adKiHVU9jRync6f1cDlBcnyOg4=", UnsortedComponentPath + "TorqueConverter.xml")]
		public void TestTorqueConverterComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.TorqueConverter));
		}


		[TestCase("FGaItzPcx1qGytVlroFp+PU9rzsaQWRWnxmIspkqCRM=", SortedComponentPath + "Angledrive.xml"),
		TestCase("FGaItzPcx1qGytVlroFp+PU9rzsaQWRWnxmIspkqCRM=", UnsortedComponentPath + "Angledrive.xml")]
		public void TestAngledriveComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Angledrive));
		}


		[TestCase("NiyH2Xp0rQswwXIOf52Jm0wvK4Yc2/PL/T+zQCWQGFo=", SortedComponentPath + "ADC.xml"),
		 TestCase("NiyH2Xp0rQswwXIOf52Jm0wvK4Yc2/PL/T+zQCWQGFo=", UnsortedComponentPath + "ADC.xml")]
		public void TestADCHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.ADC));
		}

		[TestCase("tam1LGpdznHGFGo+rp0WVr0/6+F2yU2Kv4G4tYvAe+Y=", SortedComponentPath + "BatterySystem_1.xml"),
		TestCase("tam1LGpdznHGFGo+rp0WVr0/6+F2yU2Kv4G4tYvAe+Y=", UnsortedComponentPath + "BatterySystem_1.xml")]
		public void TestBatterySystemHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.BatterySystem));
		}

		[TestCase("dBadIN60l8Iqcanj/nrx1EbD+KixtDxLAusUcutITk8=", SortedComponentPath + "CapacitorSystem_1.xml"),
		 TestCase("dBadIN60l8Iqcanj/nrx1EbD+KixtDxLAusUcutITk8=", UnsortedComponentPath + "CapacitorSystem_1.xml")]
		public void TestCapacitorHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.CapacitorSystem));
		}

		[TestCase("X5dgavua/V/jzBQeJ6SxZXsXm3i1jruL48LedzZ5IxU=", SortedComponentPath + "ElectricMachineSystem-IHPC_1.xml"),
		 TestCase("X5dgavua/V/jzBQeJ6SxZXsXm3i1jruL48LedzZ5IxU=", UnsortedComponentPath + "ElectricMachineSystem-IHPC_1.xml"),
		 TestCase("wLFLpJxFZ6mDXeqdlZCGVOLCoXTCf7XTL0q9ZKkmt7o=", SortedComponentPath + "ElectricMachineSystem_1.xml"),
		 TestCase("wLFLpJxFZ6mDXeqdlZCGVOLCoXTCf7XTL0q9ZKkmt7o=", UnsortedComponentPath + "ElectricMachineSystem_1.xml"),
		 TestCase("CunnDxsiE9kciX+v9oeEGADZpEc88NtfMtmrHyJkCQ0=", SortedComponentPath + "ElectricMachineSystem_StdValues.xml"),
		 TestCase("CunnDxsiE9kciX+v9oeEGADZpEc88NtfMtmrHyJkCQ0=", UnsortedComponentPath + "ElectricMachineSystem_StdValues.xml")]
		public void TestElectricMachineHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.ElectricMachineSystem));
		}


		[TestCase("3L/fYxKTdIwzADHQMnUBPxcNwZNEHM+sKEC2M32UnEA=", SortedComponentPath + "IEPC_1.xml"),
		 TestCase("3L/fYxKTdIwzADHQMnUBPxcNwZNEHM+sKEC2M32UnEA=", UnsortedComponentPath + "IEPC_1.xml"),
		 TestCase("BTHs/Hh2SgycIwU5OSuTgU/2SptMvmRFvPXr2X1Y7XQ=", SortedComponentPath + "IEPC_StdValues.xml"),
		 TestCase("BTHs/Hh2SgycIwU5OSuTgU/2SptMvmRFvPXr2X1Y7XQ=", UnsortedComponentPath + "IEPC_StdValues.xml")]
		public void TestIEPCHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.IEPC));
		}
	}
}
