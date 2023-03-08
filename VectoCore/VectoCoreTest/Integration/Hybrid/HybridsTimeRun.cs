using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;

namespace TUGraz.VectoCore.Tests.Integration.Hybrid
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class HybridsTimeRunTest
    {
        private const string P2_JOB = @"TestData/Integration/TimeRun/MeasuredSpeed/GenericVehicle_Group2_P2_EM/Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P3_JOB = @"TestData/Integration/TimeRun/MeasuredSpeed/GenericVehicle_Group2_P3_EM/Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P4_JOB = @"TestData/Integration/TimeRun/MeasuredSpeed/GenericVehicle_Group2_P4_EM/Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P2_5_JOB = @"TestData/Integration/TimeRun/MeasuredSpeed/GenericVehicle_Group5_P2.5/P2.5 Group 5 2.vecto";
        private const string P1_JOB = @"TestData/Integration/TimeRun/MeasuredSpeed/GenericVehicle_P1-APT/CityBus_AT_PS.vecto";
        private const string IHPC_6SPEED_JOB= @"TestData/Integration/TimeRun/MeasuredSpeed/GenericIHPC/6SpeedGbx/IHPC Group 5.vecto";
        private const string IHPC_12SPEED_JOB= @"TestData/Integration/TimeRun/MeasuredSpeed/GenericIHPC/12SpeedGbx/IHPC Group 5.vecto";

        [OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        /*
         * How to correct vdri data for time runs with gear
         * ---------------------------------------
         * 1. If VECTO fails with retry count exceeded and ResponseOverload, check the vdri at that time step and 
         * correct the gear by a) "unifying" consecutive gear shifts with the same gear, or b) lowering the gear at the
         * adjacent steps.
         * 
         * 2. If VECTO fails with retry count exceeded and ResponseOverload at the first step(s), check the vdri at the 
         * beginning for consecutive steps with zeroes in vehicle speed, gear, etc. Leave only one of those steps and 
         * delete the rest.
         */
        [Category("JRC")]
		[Category("LongRunning")]
		[Category("Integration")]
        [
        TestCase(P1_JOB, 0, 0, 496.8873, 10.2371, 10.2191, 51.3803, 49.0392, TestName = "P1 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P1_JOB, 1, 1, 497.4313, 14.9897, 15.072, 39.3967, 33.9238, TestName = "P1 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P1_JOB, 2, 2, 629.491, 44.975, 44.9965, 20.7555, 8.9994, TestName = "P1 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        
		TestCase(P1_JOB, 6, 0, 495.9084, 10.1283, 10.1104, 51.3742, 49.033, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P1_JOB, 7, 1, 496.4165, 14.922, 14.995, 39.393, 33.9201, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P1_JOB, 8, 2, 623.4541, 46.4335, 46.442, 20.6765, 8.9351, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),
		
        TestCase(P2_JOB, 0, 0, 580.2147, 9.5194, 8.3231, 56.8458, 54.8048, TestName = "P2 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_JOB, 1, 1, 561.5268, 11.9015, 11.7354, 42.7333, 37.796, TestName = "P2 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_JOB, 2, 2, 613.3183, 30.4515, 31.0469, 20.7829, 9.3229, TestName = "P2 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

		TestCase(P2_JOB, 6, 0, 581.2067, 9.4262, 8.2459, 56.8437, 54.8074, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_JOB, 7, 1, 564.2724, 11.608, 11.6023, 42.8042, 37.8908, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P2_JOB, 8, 2, 632.1931, 28.8383, 29.7576, 20.8321, 9.4629, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P2_5_JOB, 0, 0, 836.2705, 5.4536, 8.9433, 97.7988, 82.2871, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_5_JOB, 1, 1, 0, 0, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_5_JOB, 2, 2, 0, 0, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

		TestCase(P2_5_JOB, 6, 0, 847.8268, 5.6085, 9.0618, 99.882, 84.3774, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_5_JOB, 7, 1, 961.6919, 6.9354, 11.0504, 84.6331, 59.6807, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"), 
        TestCase(P2_5_JOB, 8, 2, 1667.4168, 15.0076, 19.6406, 57.5292, 18.443, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P3_JOB, 0, 0, 588.6082, 8.5587, 7.5518, 56.8412, 54.7998, TestName = "P3 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P3_JOB, 1, 1, 567.0846, 11.5216, 11.5927, 42.859, 37.8901, TestName = "P3 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P3_JOB, 2, 2, 609.2478, 28.7076, 29.6792, 20.8735, 9.3822, TestName = "P3 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

		TestCase(P3_JOB, 6, 0, 588.9964, 8.5552, 7.5504, 56.8402, 54.8026, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P3_JOB, 7, 1, 568.3154, 11.534, 11.602, 42.8641, 37.9198, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P3_JOB, 8, 2, 611.2123, 28.4337, 29.4137, 20.8631, 9.475, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P4_JOB, 0, 0, 574.8567, 3.0858, 2.1371, 56.5289, 54.4959, TestName = "P4 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P4_JOB, 1, 1, 568.708, 4.4516, 3.9854, 42.3484, 37.5006, TestName = "P4 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P4_JOB, 2, 2, 707.3661, 8.5436, 9.6855, 20.3519, 9.1005, TestName = "P4 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

		TestCase(P4_JOB, 6, 0, 575.3901, 3.0878, 2.1394, 56.5962, 54.5714, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P4_JOB, 7, 1, 569.7132, 4.4619, 3.9254, 42.391, 37.5597, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P4_JOB, 8, 2, 696.7515, 8.1464, 9.2446, 20.5068, 9.2941, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),
		
        TestCase(IHPC_6SPEED_JOB, 0, 0, 829.1772, 19.8093, 19.9646, 100.5286, 85.4752, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(IHPC_6SPEED_JOB, 1, 1, 938.3156, 27.6612, 27.9835, 88.1876, 62.2951, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IHPC_6SPEED_JOB, 2, 2, 1468.0661, 73.9576, 71.7535, 64.2939, 19.9436, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

		TestCase(IHPC_6SPEED_JOB, 6, 0, 816.2639, 21.5614, 21.5763, 101.0721, 86.0174, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IHPC_6SPEED_JOB, 7, 1, 898.2915, 29.4113, 29.6004, 88.2206, 62.3142, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IHPC_6SPEED_JOB, 8, 2, 1452.9933, 77.8354, 75.2896, 64.3481, 19.9521, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IHPC_12SPEED_JOB, 0, 0, 820.0406, 20.7663, 20.8432, 101.4742, 86.4469, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(IHPC_12SPEED_JOB, 1, 1, 920.0827, 29.6705, 30.0166, 87.8704, 62.2552, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IHPC_12SPEED_JOB, 2, 2, 1500.814, 67.7276, 66.4847, 63.5659, 19.8401, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        
		TestCase(IHPC_12SPEED_JOB, 6, 0, 811.4486, 21.1895, 21.2088, 101.5206, 86.4943, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IHPC_12SPEED_JOB, 7, 1, 899.1912, 30.1879, 30.5689, 87.7847, 62.1753, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IHPC_12SPEED_JOB, 8, 2, 1487.2193, 73.2166, 71.4632, 63.6414, 19.8749, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear UrbanDelivery")
		]
        public void TestHybridTimeRunCycle(string jobFile, int cycleIdx, int distanceCycleIdx, double CO2, double charge, double discharge, double pWheelpos,
            double pWheel)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				{ SummaryDataContainer.Fields.CO2_KM, CO2 },
                { SummaryDataContainer.Fields.E_REESS_T_chg, charge },
                { SummaryDataContainer.Fields.E_REESS_T_dischg, discharge },
                { SummaryDataContainer.Fields.P_WHEEL_POS, pWheelpos },
                { SummaryDataContainer.Fields.P_WHEEL, pWheel }
			};

            RunHybridTimeRunCycle(jobFile, cycleIdx, distanceCycleIdx, metrics);
        }

        [Category("JRC")]
        [Category("LongRunning")]
        [Category("Integration")]
        [
        TestCase(P1_JOB, 3, TestName = "P1 Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(P1_JOB, 4, TestName = "P1 Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(P1_JOB, 5, TestName = "P1 Hybrid DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(P2_JOB, 3, TestName = "P2 Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(P2_JOB, 4, TestName = "P2 Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_JOB, 5, TestName = "P2 Hybrid DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(P2_5_JOB, 3, TestName = "P2_5 Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(P2_5_JOB, 4, TestName = "P2_5 Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_5_JOB, 5, TestName = "P2_5 Hybrid DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(P3_JOB, 3, TestName = "P3 Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(P3_JOB, 4, TestName = "P3 Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(P3_JOB, 5, TestName = "P3 Hybrid DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(P4_JOB, 3, TestName = "P4 Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(P4_JOB, 4, TestName = "P4 Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(P4_JOB, 5, TestName = "P4 Hybrid DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(IHPC_6SPEED_JOB, 3, TestName = "IHPC_6SPEED IHPC Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(IHPC_6SPEED_JOB, 4, TestName = "IHPC_6SPEED IHPC Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(IHPC_6SPEED_JOB, 5, TestName = "IHPC_6SPEED IHPC Hybrid DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(IHPC_12SPEED_JOB, 3, TestName = "IHPC_12SPEED IHPC Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(IHPC_12SPEED_JOB, 4, TestName = "IHPC_12SPEED IHPC Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(IHPC_12SPEED_JOB, 5, TestName = "IHPC_12SPEED IHPC Hybrid DistanceRun MeasuredSpeed UrbanDelivery")
        ]
        public void TestHybridDistanceRunCycle(string jobFile, int cycleIdx)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				{ SummaryDataContainer.Fields.CO2_KM, double.NaN },
                { SummaryDataContainer.Fields.E_REESS_T_chg, double.NaN },
                { SummaryDataContainer.Fields.E_REESS_T_dischg, double.NaN }
			};

            RunHybridDistanceRunCycle(jobFile, cycleIdx, metrics);
        }

        public void RunHybridTimeRunCycle(string jobFile, int cycleIdx, int distanceCycleIdx, Dictionary<String, double> metrics)
        {
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			string outputFile = InputDataHelper.CreateUniqueSubfolder(jobFile);
			var writer = new FileOutputWriter(outputFile);

			var factory = new SimulatorFactoryEngineering(inputProvider, writer, false) { WriteModalResults = true };
			factory.SumData = new SummaryDataContainer(writer);

			var run = factory.SimulationRuns().ToArray()[cycleIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
			
            string distanceSumPath = Path.Combine(Path.GetDirectoryName(jobFile), "distance.vsum");
            AssertHelper.ReportDeviations(distanceSumPath, distanceCycleIdx, factory, metrics);
            
            AssertHelper.AssertMetrics(factory, metrics);

			Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
        }

        public void RunHybridDistanceRunCycle(string jobFile, int cycleIdx, Dictionary<String, double> metrics)
        { 
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			string outputFile = InputDataHelper.CreateUniqueSubfolder(jobFile);
			var writer = new FileOutputWriter(outputFile);

			var factory = new SimulatorFactoryEngineering(inputProvider, writer, false) { WriteModalResults = true };
			factory.SumData = new SummaryDataContainer(writer);

			var run = factory.SimulationRuns().ToArray()[cycleIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
			
            string distanceSumPath = Path.Combine(Path.GetDirectoryName(jobFile), "distance.vsum");

            const int DISTANCE_RUN_START_POSITION = 3;

            AssertHelper.ReadMetricsFromVSum(distanceSumPath, cycleIdx - DISTANCE_RUN_START_POSITION, metrics);

            AssertHelper.AssertMetrics(factory, metrics);

			Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
        }

    }
}
