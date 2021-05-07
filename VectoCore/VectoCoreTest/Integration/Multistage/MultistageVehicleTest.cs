using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = System.Xml.XmlDocumentType;

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

		private const string vifResult = VIFDirPath + "vif_vehicle-sample.VIF_Report_3.xml";

		protected IXMLInputDataReader xmlInputReader;
		protected IXMLInputDataReader xmlVIFInputReader;

		private IKernel _kernel;


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

			var numberOfManufacturingStages = vifDataProvider.JobInputData.ManufacturingStages?.Count ?? 0;
			var writer = new FileOutputVIFWriter(vifResult, numberOfManufacturingStages);
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

	}
}
