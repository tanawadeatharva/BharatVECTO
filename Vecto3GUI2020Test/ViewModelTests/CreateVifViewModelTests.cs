using System.Diagnostics;
using Ninject;
using NUnit.Framework;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class CreateVifViewModelTests : ViewModelTestBase
	{
		private ICreateVifViewModel _createVifViewModel;

		private const string testdata_2_6 = "XML\\XMLReaderDeclaration\\SchemaVersion2.6_Buses\\";
		private const string testdata_2_8 = "XML\\XMLReaderDeclaration\\SchemaVersion2.8\\";

		private const string vecto_vehicle_primary_heavyBusSample =
			testdata_2_6 + "vecto_vehicle-primary_heavyBus-sample.xml";

		private const string vecto_vehicle_exempted_input_only_certain_entries =
			"vecto_vehicle-exempted_input_only_certain_entries01-sample.xml";

		private const string vecto_vehicle_primary_heavyBusExempted = testdata_2_6 + "exempted_primary_heavyBus.xml";

		[SetUp]
		public void SetUpCreateVif()
		{
			_createVifViewModel = _kernel.Get<ICreateVifViewModel>();
		}

		[TestCase(stageInputFullSample, TestName="InvalidPrimaryFile_StageInput")]
		[TestCase(airdragLoadTestFile, TestName="InvalidPrimaryFile_Airdrag")]
		[TestCase(consolidated_multiple_stages_airdrag, TestName = "InvalidPrimaryFile_VIF")]
		[TestCase(vecto_vehicle_exempted_input_only_certain_entries, TestName="InvalidPrimaryFile_ExemptedStageInput")]
		public void LoadInvalidPrimaryFile(string fileName)
		{
			var filePath = GetTestDataPath(fileName);
			Assert.IsFalse(_createVifViewModel.LoadPrimaryInput(filePath));
			Assert.IsNull(_createVifViewModel.PrimaryInputPath);
			Assert.IsNull(_createVifViewModel.ExemptedPrimary);
		}


		[TestCase(stageInputFullSample, TestName = "InvalidPrimaryFile_StageInput")]
		[TestCase(airdragLoadTestFile, TestName = "InvalidPrimaryFile_Airdrag")]
		[TestCase(consolidated_multiple_stages_airdrag, TestName = "InvalidPrimaryFile_VIF")]
		public void LoadInvalidCompletedFile(string fileName)
		{
			var filePath = GetTestDataPath(fileName);
			Assert.IsFalse(_createVifViewModel.LoadStageInput(filePath));
			Assert.IsNull(_createVifViewModel.StageInputExempted);
			Assert.IsNull(_createVifViewModel.StageInputPath);
		}

		[Ignore("incomplete")]
		[Test]
		public void LoadExemptedCompletedAndNonExemptedPrimary()
		{

		}

		[Test]
		public void LoadNonExemptedCompletedAndExemptedPrimary()
		{
			var exemptedPrimaryPath = GetTestDataPath(vecto_vehicle_primary_heavyBusExempted);
			var stageInputPath = GetTestDataPath(stageInputFullSample);

			Assert.IsTrue(_createVifViewModel.LoadPrimaryInput(exemptedPrimaryPath));
			Assert.IsFalse(_createVifViewModel.LoadStageInput(stageInputPath));


		}

		

		[Test]
		public void LoadValidPrimaryFile()
		{
			var filePath = GetTestDataPath(vecto_vehicle_primary_heavyBusSample);
			Assert.IsTrue(_createVifViewModel.LoadPrimaryInput(filePath));
			Assert.AreEqual(filePath, _createVifViewModel.PrimaryInputPath);

		}

		[TestCase(stageInputFullSample, TestName = "ValidStageInput_fullStageInput")]
		[TestCase(vecto_vehicle_exempted_input_only_certain_entries, TestName = "ValidStageInput_exemptedStageInput")]
		public void LoadValidStageInputFile(string fileName)
		{
			var filePath = GetTestDataPath(fileName);
			Assert.IsTrue(_createVifViewModel.LoadStageInput(filePath));
			Assert.AreEqual(filePath, _createVifViewModel.StageInputPath);
		}

		[Test]
		public void LoadValidNonExemptedFiles()
		{
			var primaryPath = GetTestDataPath(vecto_vehicle_primary_heavyBusSample);
			var stageInputPath = GetTestDataPath(stageInputFullSample);

			Assert.IsTrue(_createVifViewModel.LoadPrimaryInput(primaryPath));
			Assert.IsTrue(_createVifViewModel.LoadStageInput(stageInputPath));


		}

		[Test]
		public void LoadValidExemptedFiles()
		{


		}


		//[TestCase(null, true, true, TestName="StageInputExempted_PrimaryNotSet1")]
		//[TestCase(null, false, true, TestName = "StageInputExempted_PrimaryNotSet2")]
		//[TestCase(true, true, true, TestName = "StageInputExempted_PrimaryExempted")]
		//[TestCase(false, false, true, TestName = "StageInputExempted_PrimaryExempted2")]
		//[TestCase(true, false, false, TestName = "StageInputExempted_PrimaryNonExempted")]
		//[TestCase(false, true, false, TestName = "StageInputExempted_PrimaryNonExempted1")]
		//public void SetStageInputExempted(bool? primaryExpected, bool stageInputExempted, bool expectedResult)
		//{

		//}

		//[TestCase(null, true, TestName = "PrimaryNotSet")]
		//[TestCase(null, true, TestName = "PrimaryNotSet")]
		//[TestCase(null, true, TestName = "PrimaryNotSet")]
		//public void SetPrimaryInputExempted(bool? primaryExpected, bool stageInputExempted, bool expectedResult)
		//{

		//}





	}
}