using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = System.Xml.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	public class XMLDeclarationReaderVersionsTest
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

		[TestCase(@"SchemaVersion1.0\Tractor_4x2_vehicle-class-5_5_t_0.xml")]
		public void TestReadingJobVersion_V10(string jobFile)
		{
			ReadDeclarationJob(jobFile);
		}

		[TestCase(@"SchemaVersion2.0\Tractor_4x2_vehicle-class-5_5_t_0.xml")]
		public void TestReadingJobVersion_V20(string jobFile)
		{
			ReadDeclarationJob(jobFile);
		}


		public void ReadDeclarationJob(string jobfile)
		{
			var filename = Path.Combine(@"TestData\XML\XMLReaderDeclaration", jobfile);

			var fileWriter = new FileOutputWriter(filename);
			//var sumWriter = new SummaryDataContainer(fileWriter);
			//var jobContainer = new JobContainer(sumWriter);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename), true);
			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = false,
				ActualModalData = false,
				Validate = false,
			};

			var runs = runsFactory.SimulationRuns().ToArray();
			Assert.IsTrue(runs.Length > 0);

			//var customerRecord = fileWriter.XMLCustomerReportName;
			//var manufacturerRecord = fileWriter.XMLFullReportName;

			//var validationMsg1 = new List<string> { customerRecord };

			//var validator1 = new XMLValidator(XmlReader.Create(customerRecord), validationErrorAction: (s, e) => {
			//	validationMsg1.Add(e.ValidationEventArgs.Message);
			//});
			//Assert.IsTrue(validator1.ValidateXML(VectoCore.Utils.XmlDocumentType.CustomerReport), string.Join("\n", validationMsg1));

			//var validationMsg2 = new List<string> { manufacturerRecord };
			//var validator2 = new XMLValidator(XmlReader.Create(manufacturerRecord), validationErrorAction: (s, e) => {
			//	validationMsg2.Add(e.ValidationEventArgs.Message);
			//});
			//Assert.IsTrue(validator2.ValidateXML(VectoCore.Utils.XmlDocumentType.ManufacturerReport), string.Join("\n", validationMsg2));
		}
	}
}
