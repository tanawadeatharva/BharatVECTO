/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestClass]
	public class DeclarationReportTest
	{
		[TestMethod]
		public void DeclarationReport_Test()
		{
			if (File.Exists("job-report.vsum")) {
				File.Delete("job-report.vsum");
			}

			if (File.Exists("job-report.pdf")) {
				File.Delete("job-report.pdf");
			}

			var sumWriter = new SummaryFileWriter(@"job-report.vsum");
			var jobContainer = new JobContainer(sumWriter);
			var factory = new SimulatorFactory(SimulatorFactory.FactoryMode.DeclarationMode, @"TestData\Jobs\job-report.vecto");

			jobContainer.AddRuns(factory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(File.Exists(@"job-report.vsum"));
			Assert.IsTrue(File.Exists(@"TestData\Jobs\job-report.pdf"));
		}
	}
}