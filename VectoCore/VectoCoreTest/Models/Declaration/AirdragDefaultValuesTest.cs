using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestClass]
	public class AirdragDefaultValuesTest
	{
		[TestMethod]
		public void TestClass2()
		{
			var file = @"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto";
			var inputData = (JSONInputDataV3)JSONInputDataFactory.ReadJsonJob(file);
			inputData.AirdragData = null; // force use of standard values

			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();
			var runDataOrig = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			Assert.AreEqual(4.83, runDataOrig[0].AirdragData.DeclaredAirdragArea.Value());

			Assert.AreEqual(7.2, runData[0].AirdragData.DeclaredAirdragArea.Value());
		}

		[TestMethod]
		public void TestClass5()
		{
			var file = @"TestData\Integration\DeclarationMode\Class5_Tractor_4x2\Class5_Tractor_DECL.vecto";
			var inputData = (JSONInputDataV3)JSONInputDataFactory.ReadJsonJob(file);
			inputData.AirdragData = null; // force use of standard values

			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();
			var runDataOrig = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			Assert.AreEqual(5.3, runDataOrig[0].AirdragData.DeclaredAirdragArea.Value());
			Assert.AreEqual(8.7, runData[0].AirdragData.DeclaredAirdragArea.Value());
		}

		[TestMethod]
		public void TestClass9()
		{
			var file = @"TestData\Integration\DeclarationMode\Class9_RigidTruck_6x2\Class9_RigidTruck_DECL.vecto";
			var inputData = (JSONInputDataV3)JSONInputDataFactory.ReadJsonJob(file);
			inputData.AirdragData = null; // force use of standard values

			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();
			var runDataOrig = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			Assert.AreEqual(5.2, runDataOrig[0].AirdragData.DeclaredAirdragArea.Value());
			Assert.AreEqual(8.5, runData[0].AirdragData.DeclaredAirdragArea.Value());
		}
	}
}