using System;
using System.IO;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	public class ExemptedVehicleTest
	{
		const string ExemptedVehicle = @"Testdata\Integration\DeclarationMode\ExemptedVehicle\vecto_vehicle-sample_exempted.xml";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(ExemptedVehicle, 1)]
		public void TestSimulationExemptedVehicle(string filename, int numRuns)
		{
			var writer = new FileOutputWriter(filename);

			var customerFile = writer.XMLCustomerReportName;
			var manufactuerFile = writer.XMLFullReportName;
			if (File.Exists(customerFile)) {
				File.Delete(customerFile);
			}
			if (File.Exists(manufactuerFile)) {
				File.Delete(manufactuerFile);
			}

			var inputData = new XMLDeclarationInputDataProvider(filename, true); //.ReadJsonJob(relativeJobPath);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				ActualModalData = true
			};
			var jobContainer = new JobContainer(new MockSumWriter());

			var runs = factory.SimulationRuns().ToList();
			Assert.AreEqual(numRuns, runs.Count);
			foreach (var run in runs) {
				jobContainer.AddRun(run);
			}
			//jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));

			Assert.IsTrue(File.Exists(manufactuerFile));
			Assert.IsTrue(File.Exists(customerFile));

			var validator = new XMLValidator(XmlReader.Create(manufactuerFile));
			Assert.IsTrue(validator.ValidateXML(XMLValidator.XmlDocumentType.ManufacturerReport));

			var val2 = new XMLValidator(XmlReader.Create(customerFile));
			Assert.IsTrue(val2.ValidateXML(XMLValidator.XmlDocumentType.CustomerReport));
		}
	}
}
