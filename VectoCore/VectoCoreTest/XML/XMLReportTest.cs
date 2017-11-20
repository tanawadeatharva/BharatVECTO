using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.XML
{
	public class XMLReportTest
	{
		const string SampleVehicleDecl = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample.xml";
		
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(SampleVehicleDecl)]
		public void RunDeclarationJob(string filename)
		{
			var fileWriter = new FileOutputWriter(filename);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = new XMLDeclarationInputDataProvider(XmlReader.Create(filename), true);
			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = false,
				ActualModalData = false,
				Validate = false,
			};
			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var customerRecord = fileWriter.XMLCustomerReportName;
			var manufacturerRecord = fileWriter.XMLFullReportName;

			Assert.IsTrue(DoValidation(XmlReader.Create(customerRecord)));
			Assert.IsTrue(DoValidation(XmlReader.Create(manufacturerRecord)));
		}

		private bool DoValidation(XmlReader hashedComponent)
		{
			var settings = new XmlReaderSettings {
				ValidationType = ValidationType.Schema,
				ValidationFlags = //XmlSchemaValidationFlags.ProcessInlineSchema |
					//XmlSchemaValidationFlags.ProcessSchemaLocation |
					XmlSchemaValidationFlags.ReportValidationWarnings
			};
			//settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
			settings.Schemas.Add(GetXMLSchema(""));

			var vreader = XmlReader.Create(hashedComponent, settings);
			var doc = new XmlDocument();
			doc.Load(vreader);
			doc.Validate(ValidationCallBack);
			return true;
		}

		private void ValidationCallBack(object sender, ValidationEventArgs args)
		{
			throw new Exception("Validation failed");
		}

		private static XmlSchemaSet GetXMLSchema(string version)
		{
			var xset = new XmlSchemaSet() { XmlResolver = new XmlResourceResolver() };
			foreach (var schema in new[] { "VectoComponent.xsd", "VectoInput.xsd", "VectoOutputManufacturer.xsd", "VectoOutputCustomer.xsd" }) {
				var resource = RessourceHelper.LoadResourceAsStream(RessourceHelper.ResourceType.XMLSchema, schema);

				var reader = XmlReader.Create(resource, new XmlReaderSettings(), "schema://");
				xset.Add(XmlSchema.Read(reader, null));
			}
			xset.Compile();
			return xset;
		}
	}
}