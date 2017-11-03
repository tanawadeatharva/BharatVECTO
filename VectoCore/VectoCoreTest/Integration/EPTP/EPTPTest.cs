using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration.EPTP
{
    [TestFixture]
    public class EPTPTest
    {
        [TestCase()]
        public void RunEPTP()
        {
            var jobFile = @"E:\QUAM\Workspace\VECTO_quam\EPTP\MAN_EPTP.vecto";

            var fileWriter = new FileOutputWriter(jobFile);
            var sumWriter = new SummaryDataContainer(fileWriter);
            var jobContainer = new JobContainer(sumWriter);
            var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
            var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, dataProvider, fileWriter) {
                ModalResults1Hz = false,
                WriteModalResults = true,
                ActualModalData = false,
                Validate = false,
            };
            
            jobContainer.AddRuns(runsFactory);
            jobContainer.Execute();

            Assert.AreEqual(true, jobContainer.AllCompleted);
            
        }

    }
}