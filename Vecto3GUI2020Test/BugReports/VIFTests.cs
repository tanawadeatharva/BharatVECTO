using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces;
using Vecto3GUI2020Test.Utils;

namespace Vecto3GUI2020Test.BugReports
{
	[TestFixture]
	public class VifTests
	{
		[Ignore("DoesNothing")]
		[TestCase(TestData.primaryDecimalTestFile, TestName="PneumaticsCompressorDrive")]
		public async Task CreateAndLoadVifWithWrongDecimalCount(string fileName)
		{
			var path = Path.GetFullPath(fileName);
			AssertHelper.FileExists(path);
			var kernel = TestHelper.GetKernel();
			var dialogHelper = kernel.Get<IDialogHelper>() as MockDialogHelper;
			
            //Load JobFile 
			var jobListViewModel = kernel.Get<IJobListViewModel>() as JobListViewModel;
			await jobListViewModel.AddJobAsync(fileName);
			dialogHelper.AssertNoErrorDialogs();
			Assert.AreEqual(1, jobListViewModel.Jobs.Count);
			jobListViewModel.Jobs[0].Selected = true;

			//Start Simulation
			TestContext.Write("Starting simulation ... ");
			Stopwatch stop = Stopwatch.StartNew();
			await jobListViewModel.RunSimulationExecute();
			stop.Stop();
			TestContext.WriteLine($"Done! ({stop.Elapsed.TotalSeconds}s)");


			var vifpath = path.Replace(".xml", ".RSLT_VIF.xml");
			TestContext.WriteLine($"Trying to add {path} to JobList");
			await jobListViewModel.AddJobAsync(path);
			Assert.AreEqual(2, jobListViewModel.Jobs.Count);

			foreach (var documentViewModel in jobListViewModel.Jobs) {
				TestContext.WriteLine($"{documentViewModel.DataSource.SourcePath}");
			}

			return;
		}





	}
}
