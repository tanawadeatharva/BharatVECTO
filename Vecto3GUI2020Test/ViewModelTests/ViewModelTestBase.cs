using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.Ninject.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace Vecto3GUI2020Test
{
	public class ViewModelTestBase
	{
		protected const string TestDataDirPath = @"Testdata\";
		protected const string consolidated_multiple_stages = "vecto_multistage_consolidated_multiple_stages.xml";

		protected const string consolidated_multiple_stages_airdrag =
			"vecto_multistage_consolidated_multiple_stages_airdrag.xml";

		protected const string consolidated_multiple_stages_hev =
			"vecto_multistage_consolidated_multiple_stages_hev.xml";

		protected const string consolidated_one_stage = "vecto_multistage_consolidated_one_stage.xml";
		protected const string primary_vehicle_only = "vecto_multistage_primary_vehicle_only.xml";

		

		protected const string stageInputFullSample = "vecto_vehicle-stage_input_full-sample.xml";

		protected const string airdragLoadTestFile = "AirdragLoadTestFile.xml";

		//protected IXMLInputDataReader xmlInputReader;
		protected IKernel _kernel;
		private Mock<IDialogHelper> _mockDialogHelper;


		[SetUp]
		public void OneTimeSetUp()
		{
			_kernel = new StandardKernel(
				new VectoNinjectModule(),
				new JobEditModule(),
				new ComponentModule(),
				new DocumentModule(),
				new XMLWriterFactoryModule(),
				new FactoryModule(),
				new MultistageModule(),
				new Vecto3GUI2020Module()
			);
			//xmlInputReader = _kernel.Get<IXMLInputDataReader>();
			_kernel.Rebind<IDialogHelper>().ToConstant(setMockDialogHelper().Object);

		}

		[TearDown]
		public void TearDown()
		{
			_kernel.Dispose();
			_kernel = null;
		}

		public bool checkFileNameExists(string fileName)
		{
			var filePath = Path.GetFullPath(TestDataDirPath + fileName);
			return checkFilePathExists(filePath);
		}

		public bool checkFilePathExists(string filePath)
		{
			var exists = File.Exists(filePath);
			if (exists)
			{
				Console.WriteLine(filePath + @" exists");
			}
			else
			{
				Console.WriteLine(filePath + @" not existing");
			}

			return exists;
		}

		public void deleteFile(string fileName)
		{
			var filePath = Path.GetFullPath(TestDataDirPath + fileName);
			File.Delete(fileName);
		}

		public virtual NewMultiStageJobViewModel loadFile(string fileName)
		{
			var mockDialogHelper = setMockDialogHelper(fileName);

			var newMultistageJobViewModel = _kernel.Get<NewMultiStageJobViewModel>();
			newMultistageJobViewModel.AddVifFile.Execute(null);

			Assert.NotNull(newMultistageJobViewModel.MultiStageJobViewModel);

			var manstageVehicleViewModel = newMultistageJobViewModel.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as DeclarationInterimStageBusVehicleViewModel_v2_8;
			Assert.NotNull(manstageVehicleViewModel);

			Assert.AreEqual(mockDialogHelper.Object.OpenXMLFileDialog(), newMultistageJobViewModel.VifPath);

			if (!manstageVehicleViewModel.ExemptedVehicle) {
				var auxiliariesViewModel = manstageVehicleViewModel.MultistageAuxiliariesViewModel;
				Assert.NotNull(auxiliariesViewModel);

				var airdragViewModel = (manstageVehicleViewModel as DeclarationInterimStageBusVehicleViewModel_v2_8)?.MultistageAirdragViewModel;
				Assert.NotNull(airdragViewModel);
			}
			





			return newMultistageJobViewModel;
		}

		protected virtual Mock<IDialogHelper> setMockDialogHelper(string fileToLoad = null, string fileToSave = null)
		{
			if (_mockDialogHelper == null) {
				_mockDialogHelper = new Mock<IDialogHelper>();
				_mockDialogHelper.Setup(dialogHelper => dialogHelper.ShowMessageBox(It.IsAny<string>(),
						It.IsAny<string>(),
						It.IsAny<MessageBoxButton>(),
						It.IsAny<MessageBoxImage>())).Returns(MessageBoxResult.OK)
					.Callback<string, string, MessageBoxButton, MessageBoxImage>((
						(message, caption, button, image) => {
							TestContext.WriteLine($"{caption}\n {message}");
						}));

				_mockDialogHelper.Setup(dialogHelper =>
						dialogHelper.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>()))
					.Callback<string, string>((message, caption) => 
						TestContext.WriteLine($"{{caption}}\n {message}"));
			}
			if (fileToLoad != null) {
				var filePath = Path.GetFullPath(TestDataDirPath + fileToLoad);

				Assert.NotNull(filePath);
				_mockDialogHelper.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog(It.IsAny<string>())).Returns(filePath);
				_mockDialogHelper.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog()).Returns(filePath);

				TestContext.WriteLine($"Created MOCKDIALOGHELPER, returns {filePath} for OpenXMLFileDialog()");
			}

			if (fileToSave != null) {
				var filePath = Path.GetFullPath(TestDataDirPath + fileToSave);
				_mockDialogHelper.Setup(dialogHelper =>
					dialogHelper.SaveToXMLDialog(It.IsAny<string>())).Returns(filePath);
				_mockDialogHelper.Setup(dialogHelper =>
					dialogHelper.SaveToXMLDialog(null)).Returns(filePath);

				TestContext.WriteLine($"Created MOCKDIALOGHELPER, returns {filePath} for SaveToXMLFileDialog()");
			}


			return _mockDialogHelper;
		}

		protected Mock<IDialogHelper> getMockDialogHelper()
		{
			return _mockDialogHelper;
		}

		protected virtual string GetFullPath(string fileName)
		{
			var path = Path.GetFullPath(TestDataDirPath + fileName);
			Debug.WriteLine(path);
			return path;
		}
	}
}