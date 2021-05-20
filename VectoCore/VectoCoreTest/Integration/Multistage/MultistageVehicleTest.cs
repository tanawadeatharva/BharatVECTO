using System;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.Tests.Integration.Multistage
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class MultistageVehicleTest
	{
		const string VIFDirPath = @"TestData\XML\XMLReaderDeclaration\SchemaVersionMultistage.0.1\";
		const string InputDirPath = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.8\";
		
		const string InputFilePath = InputDirPath  + "vecto_vehicle-stage_input_full-sample.xml";
		const string VIFInputFile = VIFDirPath  + "vecto_multistage_primary_vehicle_stage_2_3.xml";


		const string InputFilePathGroup41 = InputDirPath + "vecto_vehicle-stage_input_full-sample_group41.xml";
		const string VIFInputFileGroup41 = VIFDirPath + "vecto_multistage_primary_vehicle_stage_2_3_group41.xml";

		private const string vifResult = VIFDirPath + "vif_vehicle-sample.VIF_Report_3.xml";


		

		protected IXMLInputDataReader xmlInputReader;
		protected IXMLInputDataReader xmlVIFInputReader;

		private IKernel _kernel;
        private string _generatedVIFFilepath;

        [OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
			xmlVIFInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase(VIFInputFile, InputFilePath, 1)]
		public void TestSimulationMultistageVehicle(string vifFilename, string inputFilename, int numRuns)
		{
			//Input files
			var inputReader = XmlReader.Create(inputFilename);
			var inputDataProvider = xmlInputReader.CreateDeclaration(inputReader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;

			var vifReader = XmlReader.Create(vifFilename);
			var vifDataProvider = xmlInputReader.Create(vifReader) as IMultistageBusInputDataProvider;

			var numberOfManufacturingStages = vifDataProvider?.JobInputData.ManufacturingStages?.Count ?? 0;
			var writer = new FileOutputVIFWriter(vifResult, numberOfManufacturingStages);
			_generatedVIFFilepath = writer.XMLMultistageReportFileName;
			
			var inputData = new XMLDeclarationVIFInputData(vifDataProvider, vehicle);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			
			var jobContainer = new JobContainer(new MockSumWriter());

			var runs = factory.SimulationRuns().ToList();
			Assert.AreEqual(numRuns, runs.Count);
			foreach (var run in runs)
			{
				jobContainer.AddRun(run);
			}

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));


			var validator = new XMLValidator(XmlReader.Create(writer.XMLMultistageReportFileName));
			Assert.IsTrue(validator.ValidateXML(VectoCore.Utils.XmlDocumentType.MultistageOutputData), validator.ValidationError);
		}


		[TestCase()]
		public void TestMultistageSimulationRun()
		{
			TestSimulationMultistageVehicle(VIFInputFileGroup41, InputFilePathGroup41, 1);
			
			var vifReader = XmlReader.Create(_generatedVIFFilepath);
			var vifDataProvider = xmlInputReader.Create(vifReader) as IMultistageBusInputDataProvider;

			var inputData = new XMLDeclarationVIFInputData(vifDataProvider, null);
			var writer = new FileOutputWriter("vif_vehicle-sample_test.xml");
			

			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer)
			{
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};

			var jobContainer = new JobContainer(new SummaryDataContainer(writer));
			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), String.Concat<Exception>(jobContainer.Runs.Select(r => r.ExecException)));
		}

		public const string PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.6_Buses\vecto_vehicle-primary_heavyBus-sample.xml";
		public const string PrimaryBus_SmartES =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.6_Buses\vecto_vehicle-primary_heavyBusSmartES-sample.xml";


		[TestCase(PrimaryBus, TestName = "Multistage Write VIF Primary"),
		TestCase(PrimaryBus_SmartES, TestName = "Multistage Write VIF Primary SmartES")
		]
		public void TestMultistageWritingVif(string primaryFile)
		{
			var inputData = xmlInputReader.Create(primaryFile);

			var writer = new FileOutputWriter("vif_writing_test.xml");
			
			var xmlreport = new XMLDeclarationReportMultistageBusVehicle(writer);
			//var xmlreport = new XMLDeclarationReportPrimaryVehicle(writer);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer, xmlreport) {
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};

			var jobContainer = new JobContainer(new SummaryDataContainer(writer));
			jobContainer.AddRuns(factory);

			xmlreport.DoWriteReport();
		}
	}
}
