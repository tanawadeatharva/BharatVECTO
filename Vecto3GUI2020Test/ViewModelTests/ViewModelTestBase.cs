using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.Ninject.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;
using Vecto3GUI2020Test.Utils;

namespace Vecto3GUI2020Test
{
	public class ViewModelTestBase
	{

		//protected IXMLInputDataReader xmlInputReader;
		protected IKernel _kernel;
		protected MockDialogHelper _mockDialogHelper;
		protected TestHelper _testHelper;

		[SetUp]
		public void SetUp()
		{
			_kernel = TestHelper.GetKernel(out _mockDialogHelper, out _);
			_testHelper = new TestHelper(_kernel.Get<IXMLInputDataReader>());
		}

		public virtual NewMultiStageJobViewModel LoadFileFromPath(string filePath)
		{
			AssertHelper.FileExists(filePath);
			var documentFactory = _kernel.Get<IDocumentViewModelFactory>();
			
			var newMultistageJobViewModel = _kernel.Get<NewMultiStageJobViewModel>();

			TestContext.WriteLine("Loading " + filePath);
			newMultistageJobViewModel.AddVifFile(filePath);


			Assert.NotNull(newMultistageJobViewModel.MultiStageJobViewModel);

			var manstageVehicleViewModel = newMultistageJobViewModel.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as InterimStageBusVehicleViewModel;
			Assert.NotNull(manstageVehicleViewModel);

			//Assert.AreEqual(GetTestDataPath(fileName), newMultistageJobViewModel.VifPath);

			if (!manstageVehicleViewModel.ExemptedVehicle)
			{
				var auxiliariesViewModel = manstageVehicleViewModel.MultistageAuxiliariesViewModel;
				Assert.NotNull(auxiliariesViewModel);

				var airdragViewModel = (manstageVehicleViewModel as InterimStageBusVehicleViewModel)?.MultistageAirdragViewModel;
				Assert.NotNull(airdragViewModel);
			}
			return newMultistageJobViewModel;
		}
	}
}