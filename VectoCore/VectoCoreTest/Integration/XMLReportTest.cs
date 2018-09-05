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

using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;


namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	public class XMLReportTest
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase]
		public void TestXMLReportMetaInformation()
		{
			var jobfile = @"Testdata\XML\XMLReaderDeclaration\vecto_vehicle-sample.xml";
			var dataProvider = new XMLDeclarationInputDataProvider(XmlReader.Create(jobfile), true);
			var writer = new FileOutputWriter(jobfile);
			var xmlReport = new XMLDeclarationReport(writer);
			var sumData = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumData);

			if (File.Exists(writer.SumFileName)) {
				File.Delete(writer.SumFileName);
			}

			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, writer, xmlReport) {
				WriteModalResults = false,
				Validate = false,
			};
			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var manufacturerReport = xmlReport.FullReport;

			Assert.AreEqual(5, manufacturerReport.XPathSelectElement("//*[local-name()='VehicleGroup']")?.Value.ToInt());

			var reportWheels = manufacturerReport.XPathSelectElements("//*[local-name()='TyreCertificationNumber']").ToList();
			var i = 0;
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Axles.Count, reportWheels.Count);
			foreach (var axleDeclarationInputData in dataProvider.JobInputData.Vehicle.Axles) {
				Assert.AreEqual(axleDeclarationInputData.Tyre.CertificationNumber, reportWheels[i++].Value);
			}

			var digestWheels = manufacturerReport.XPathSelectElements("//*[local-name()='Axle']/*[local-name()='DigestValue']").ToArray();
			Assert.NotNull(digestWheels);
			Assert.AreEqual(2, digestWheels.Count());
			foreach (var digestWheel in digestWheels) {
				Assert.IsFalse(string.IsNullOrWhiteSpace(digestWheel.Value));
			}
		}
	}
}
