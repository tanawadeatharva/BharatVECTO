using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using VECTO3GUI2020.ViewModel;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class JobListViewModelTests : ViewModelTestBase
	{
		private string finalVIF = "final.VIF_Report_4.xml";



		[Test]
		public async Task CancelSimulationWhileLoadingFiles()
		{
			var jobListViewModel = _kernel.Get<IJobListViewModel>() as JobListViewModel;

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

		}


		[Test]
		public async Task CancelSimulationWhenJobContainerIsRunning()
		{
			var jobListViewModel = _kernel.Get<IJobListViewModel>() as JobListViewModel;

			//load final vif
			var loadedFile = await jobListViewModel.AddJobAsync(GetFullPath(finalVIF)).ConfigureAwait(false);

			//select vif for simulation
			Assert.AreNotEqual(0, jobListViewModel.Jobs.Count);
			jobListViewModel.Jobs[0].Selected = true;


			jobListViewModel.RunSimulationExecute();



			//Simulate for a while
			var outputVm = _kernel.Get<IOutputViewModel>(); // SINGLETON
			var constraint = Is.True.After(delayInMilliseconds: 100000, pollingInterval: 100);
			Assert.That(() => outputVm.Progress >= 25, constraint);
			



			TestContext.Write("Canceling Simulation ... ");
			Assert.IsTrue(jobListViewModel.SimulationRunning);
			jobListViewModel.CancelSimulation.Execute(null);
			Assert.That(() => jobListViewModel.SimulationRunning == false, constraint);
			TestContext.WriteLine("Done!");
		}
	}
}