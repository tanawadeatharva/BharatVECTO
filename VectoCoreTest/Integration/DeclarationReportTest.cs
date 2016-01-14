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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestClass]
	public class DeclarationReportTest
	{
		[TestMethod, TestCategory("LongRunning")]
		public void DeclarationReport_Test()
		{
			const string jobFile = @"TestData\Jobs\job-report.vecto";

			if (File.Exists(@"TestData\Jobs\job-report.vsum")) {
				File.Delete(@"TestData\Jobs\job-report.vsum");
			}

			if (File.Exists(@"TestData\Jobs\job-report.pdf")) {
				File.Delete(@"TestData\Jobs\job-report.pdf");
			}

			var fileWriter = new FileOutputWriter(jobFile);
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);

			jobContainer.AddRuns(factory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(File.Exists(@"TestData\Jobs\job-report.vsum"));
			Assert.IsTrue(File.Exists(@"TestData\Jobs\job-report.pdf"));
		}
	}
}