using System.Diagnostics;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using VECTO3GUI2020.ViewModel;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using Vecto3GUI2020Test.BugReports;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class JobListViewModelTests : ViewModelTestBase
	{
		private string finalVIF = "final.VIF_Report_4.xml";


		[Test]
		public async Task LoadPrimaryFile()
		{
			var jobListViewModel = _kernel.Get<IJobListViewModel>() as JobListViewModel;
			Write("Trying to load {}");


		}


		[Test]
		public async Task CancelSimulationWhileLoadingFiles()
		{
			var jobListViewModel = _kernel.Get<IJobListViewModel>() as JobListViewModel;
			var watch = new Stopwatch();
			watch.Start();
			//load final vif
			var loadedFile = await jobListViewModel.AddJobAsync(GetFullPath(finalVIF)).ConfigureAwait(false);

			//select vif for simulation
			Assert.AreNotEqual(0, jobListViewModel.Jobs.Count);
			jobListViewModel.Jobs[0].Selected = true;


			jobListViewModel.RunSimulationExecute();
			TestContext.Write("Canceling Simulation ... ");
			Assert.IsTrue(jobListViewModel.SimulationRunning);
			jobListViewModel.CancelSimulation.Execute(null);


			//Wait 
			var constraint = Is.True.After(delayInMilliseconds: 100000, pollingInterval: 100);
			Assert.That(() => jobListViewModel.SimulationRunning == false, constraint);
			TestContext.WriteLine("Done!");

			watch.Stop();
			TestContext.WriteLine($"ExecutionTime {watch.Elapsed.TotalSeconds}s");
		}

		[TestCase(VIFTests.exempted, TestName="Exempted")]

		public async Task AddJobAsyncTest(string fileName)
		{
			var path = GetFullPath(fileName);
			var jobListViewModel = _kernel.Get<IJobListViewModel>() as JobListViewModel;
			Assert.AreEqual(0, jobListViewModel.Jobs.Count);

			await jobListViewModel.AddJobAsync(path);
			Assert.AreEqual(1, jobListViewModel.Jobs.Count);

			Assert.AreEqual(path, jobListViewModel.Jobs[0].DataSource.SourceFile);
		}

		[TestCase(true, TestName = "Exempted")]
		[TestCase(false, TestName = "NotExempted")]
		public void addNewFilesToJobList(bool exempted)
		{
			var jobListVm = _kernel.Get<IJobListViewModel>() as JobListViewModel;
			if (exempted)
			{
				jobListVm.NewExemptedCompletedInputCommand.Execute(null);
			}
			else
			{
				jobListVm.NewCompletedInputCommand.Execute(null);
			}

			Assert.AreEqual(1, jobListVm.Jobs.Count);
		}


		[Test]
		public async Task CancelSimulationWhenJobContainerIsRunning()
		{
			var jobListViewModel = _kernel.Get<IJobListViewModel>() as JobListViewModel;
			var watch = new Stopwatch();
			watch.Start();

			//load final vif
			var loadedFile = await jobListViewModel.AddJobAsync(GetFullPath(finalVIF)).ConfigureAwait(false);

			//select vif for simulation
			Assert.AreNotEqual(0, jobListViewModel.Jobs.Count);
			jobListViewModel.Jobs[0].Selected = true;


			//Simulate for a while
			var outputVm = _kernel.Get<IOutputViewModel>(); // SINGLETON
			var simulationTask = jobListViewModel.RunSimulationExecute();
			Assert.That(() => outputVm.Progress, Is.GreaterThanOrEqualTo(25).After(1 * 60 * 1000, 1),
				() => $"Simulation reached {outputVm.Progress}%");

			TestContext.Write("Canceling Simulation ... ");
			Assert.IsTrue(jobListViewModel.SimulationRunning);
			jobListViewModel.CancelSimulation.Execute(null);
			Assert.That(() => jobListViewModel.SimulationRunning, Is.False.After(20*1000, 50) );
			TestContext.WriteLine("Done!");

			watch.Stop();
			TestContext.WriteLine($"ExecutionTime {watch.Elapsed.TotalSeconds}s");
        }

        [Test]
        public async Task LoadStageInputOnly()
        {
            var jobListVm = _kernel.Get<IJobListViewModel>();
            var documentViewModel = await jobListVm.AddJobAsync(GetFullPath(stageInputFullSample));
            Assert.AreEqual(typeof(StageInputViewModel), documentViewModel.GetType());

            var stageInputDocumentViewModel = documentViewModel.EditViewModel as StageInputViewModel;
            Assert.NotNull(stageInputDocumentViewModel);



        }
    }
}