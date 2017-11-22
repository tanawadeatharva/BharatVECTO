/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

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