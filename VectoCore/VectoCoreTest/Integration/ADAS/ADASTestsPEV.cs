using System.IO;
using System.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.DefaultDriverStrategy.PCCStates;
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.DrivingAction;

namespace TUGraz.VectoCore.Tests.Integration.ADAS
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ADASTestsPEV
	{
		private const string BasePath = @"TestData\Integration\ADAS-PEV\Group5PCCEng\";
		private const double tolerance = 1; //seconds of tolerance. Tolerance distance is calculated dynamically based on speed.

		[OneTimeSetUp]
		public void RunBeforeAnyTests() => Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

		[Test]
		public void Class5_PCC123_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 4060m
			(4060, WithinSegment, Accelerate),       // len: 1436m
			(5496, UseCase1, Coast),                 // len: 382m
			(5878, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 4469m
			(4469, WithinSegment, Accelerate),       // len: 1073m
			(5542, UseCase1, Coast),                 // len: 1888m
			(7430, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 3909m
			(3909, WithinSegment, Accelerate),       // len: 1038m
			(4947, UseCase1, Coast),                 // len: 1199m
			(6146, WithinSegment, Coast),            // len: 218m
			(6364, WithinSegment, Brake),            // len: 729m
			(7093, WithinSegment, Coast),            // len: 136m
			(7229, OutsideSegment, Coast),           // len: 183m
			(7412, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 642m
			(642, WithinSegment, Accelerate),        // len: 1272m
			(1914, UseCase1, Coast),                 // len: 585m
			(2499, OutsideSegment, Accelerate),      // len: 1470m
			(3969, WithinSegment, Accelerate),       // len: 1003m
			(4972, UseCase1, Coast),                 // len: 1315m
			(6287, WithinSegment, Coast),            // len: 470m
			(6757, OutsideSegment, Coast),           // len: 168m
			(6925, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 665m
			(665, WithinSegment, Accelerate),        // len: 1424m
			(2089, UseCase1, Coast),                 // len: 312m
			(2401, WithinSegment, Accelerate),       // len: 595m
			(2996, UseCase1, Coast),                 // len: 1026m
			(4022, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 677m
			(677, WithinSegment, Accelerate),        // len: 1424m
			(2101, UseCase1, Coast),                 // len: 324m
			(2425, WithinSegment, Accelerate),       // len: 257m
			(2682, UseCase1, Coast),                 // len: 632m
			(3314, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 3932m
			(3932, WithinSegment, Accelerate),       // len: 1179m
			(5111, UseCase1, Coast),                 // len: 836m
			(5947, WithinSegment, Coast),            // len: 799m
			(6746, OutsideSegment, Coast),           // len: 286m
			(7032, OutsideSegment, Accelerate));


		[Test]
		public void Class5_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 3757m
			(3757, WithinSegment, Accelerate),       // len: 1143m
			(4900, UseCase1, Coast),                 // len: 1149m
			(6049, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4794, WithinSegment, Accelerate),      // len: 1247m
			(5437, 5619, UseCase2, Coast),                // len: 182m
			(5642, 5861, WithinSegment, Coast),           // len: 219m
			(5861, 6072, WithinSegment, Brake),           // len: 211m
			(6072, 6183, WithinSegment, Coast),           // len: 111m
			(6183, 6353, OutsideSegment, Coast),          // len: 170m
			(6353, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Halt),               // len: 0m
			(0, OutsideSegment, Accelerate),         // len: 112m
			(112, OutsideSegment, Roll),             // len: 13m
			(125, OutsideSegment, Accelerate),       // len: 280m
			(405, OutsideSegment, Roll),             // len: 20m
			(425, OutsideSegment, Accelerate),       // len: 48m
			(473, OutsideSegment, Roll),             // len: 20m
			(493, OutsideSegment, Accelerate),       // len: 3315m
			(3808, OutsideSegment, Coast),           // len: 759m
			(4567, OutsideSegment, Brake),           // len: 443m
			(5010, OutsideSegment, Coast),           // len: 150m
			(5160, OutsideSegment, Accelerate));


		[Test]
		public void Class5_PCC123_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Halt),               // len: 0m
			(0, OutsideSegment, Accelerate),         // len: 112m
			(112, OutsideSegment, Roll),             // len: 13m
			(125, OutsideSegment, Accelerate),       // len: 280m
			(405, OutsideSegment, Roll),             // len: 20m
			(425, OutsideSegment, Accelerate),       // len: 48m
			(473, OutsideSegment, Roll),             // len: 20m
			(493, OutsideSegment, Accelerate),       // len: 3120m
			(3613, OutsideSegment, Coast),           // len: 633m
			(4246, OutsideSegment, Brake),           // len: 262m
			(4508, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 4060m
			(4060, WithinSegment, Accelerate),       // len: 1436m
			(5496, UseCase1, Coast),                 // len: 382m
			(5878, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 4469m
			(4469, WithinSegment, Accelerate),       // len: 1073m
			(5542, UseCase1, Coast),                 // len: 1888m
			(7430, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 3909m
			(3909, WithinSegment, Accelerate),       // len: 1038m
			(4947, UseCase1, Coast),                 // len: 1199m
			(6146, WithinSegment, Coast),            // len: 72m
			(6218, WithinSegment, Brake),            // len: 889m
			(7107, WithinSegment, Coast),            // len: 120m
			(7227, OutsideSegment, Coast),           // len: 142m
			(7369, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 642m
			(642, WithinSegment, Accelerate),        // len: 1272m
			(1914, UseCase1, Coast),                 // len: 585m
			(2499, OutsideSegment, Accelerate),      // len: 1470m
			(3969, WithinSegment, Accelerate),       // len: 1003m
			(4972, UseCase1, Coast),                 // len: 1315m
			(6287, WithinSegment, Coast),            // len: 131m
			(6418, WithinSegment, Brake),            // len: 228m
			(6646, WithinSegment, Coast),            // len: 108m
			(6754, OutsideSegment, Coast),           // len: 143m
			(6897, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 665m
			(665, WithinSegment, Accelerate),        // len: 1424m
			(2089, UseCase1, Coast),                 // len: 312m
			(2401, WithinSegment, Accelerate),       // len: 595m
			(2996, UseCase1, Coast),                 // len: 1026m
			(4022, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 677m
			(677, WithinSegment, Accelerate),        // len: 1424m
			(2101, UseCase1, Coast),                 // len: 324m
			(2425, WithinSegment, Accelerate),       // len: 257m
			(2682, UseCase1, Coast),                 // len: 632m
			(3314, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 3932m
			(3932, WithinSegment, Accelerate),       // len: 1179m
			(5111, UseCase1, Coast),                 // len: 836m
			(5947, WithinSegment, Coast),            // len: 143m
			(6090, WithinSegment, Brake),            // len: 384m
			(6474, WithinSegment, Coast),            // len: 275m
			(6749, OutsideSegment, Coast),           // len: 154m
			(6903, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 3757m
			(3757, WithinSegment, Accelerate),       // len: 1143m
			(4900, UseCase1, Coast),                 // len: 1149m
			(6049, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4794, WithinSegment, Accelerate),      // len: 1247m
			(5437, 5619, UseCase2, Coast),                // len: 182m
			(5678, 6087, WithinSegment, Brake),           // len: 409m
			(6087, 6183, WithinSegment, Coast),           // len: 96m
			(6183, 6325, OutsideSegment, Coast),          // len: 142m
			(6325, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Halt),               // len: 0m
			(0, OutsideSegment, Accelerate),         // len: 112m
			(112, OutsideSegment, Roll),             // len: 13m
			(125, OutsideSegment, Accelerate),       // len: 280m
			(405, OutsideSegment, Roll),             // len: 20m
			(425, OutsideSegment, Accelerate),       // len: 48m
			(473, OutsideSegment, Roll),             // len: 20m
			(493, OutsideSegment, Accelerate),       // len: 3315m
			(3808, OutsideSegment, Coast),           // len: 759m
			(4567, OutsideSegment, Brake),           // len: 443m
			(5010, OutsideSegment, Coast),           // len: 150m
			(5160, OutsideSegment, Accelerate));


		[Test]
		public void Class5_PCC12_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Halt),               // len: 0m
			(0, OutsideSegment, Accelerate),         // len: 112m
			(112, OutsideSegment, Roll),             // len: 13m
			(125, OutsideSegment, Accelerate),       // len: 280m
			(405, OutsideSegment, Roll),             // len: 20m
			(425, OutsideSegment, Accelerate),       // len: 48m
			(473, OutsideSegment, Roll),             // len: 20m
			(493, OutsideSegment, Accelerate),       // len: 3120m
			(3613, OutsideSegment, Coast),           // len: 633m
			(4246, OutsideSegment, Brake),           // len: 262m
			(4508, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 5647m
			(5647, OutsideSegment, Coast),           // len: 129m
			(5776, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 6196m
			(6196, OutsideSegment, Coast),           // len: 1350m
			(7546, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 5496m
			(5496, OutsideSegment, Coast),           // len: 353m
			(5849, OutsideSegment, Brake),           // len: 1262m
			(7111, OutsideSegment, Coast),           // len: 262m
			(7373, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 2147m
			(2147, OutsideSegment, Coast),           // len: 153m
			(2300, OutsideSegment, Brake),           // len: 169m
			(2469, OutsideSegment, Coast),           // len: 48m
			(2517, OutsideSegment, Accelerate),      // len: 3033m
			(5550, OutsideSegment, Coast),           // len: 366m
			(5916, OutsideSegment, Brake),           // len: 733m
			(6649, OutsideSegment, Coast),           // len: 250m
			(6899, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 2194m
			(2194, OutsideSegment, Coast),           // len: 164m
			(2358, OutsideSegment, Accelerate),      // len: 991m
			(3349, OutsideSegment, Coast),           // len: 248m
			(3597, OutsideSegment, Brake),           // len: 349m
			(3946, OutsideSegment, Coast),           // len: 214m
			(4160, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 2206m
			(2206, OutsideSegment, Coast),           // len: 175m
			(2381, OutsideSegment, Accelerate),      // len: 537m
			(2918, OutsideSegment, Coast),           // len: 411m
			(3329, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 5437m
			(5437, OutsideSegment, Coast),           // len: 177m
			(5614, OutsideSegment, Brake),           // len: 853m
			(6467, OutsideSegment, Coast),           // len: 430m
			(6897, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),         // len: 5332m
			(5332, OutsideSegment, Coast),           // len: 390m
			(5722, OutsideSegment, Brake),           // len: 240m
			(5962, OutsideSegment, Coast),           // len: 215m
			(6177, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4794, OutsideSegment, Accelerate),        // len: 4794m
			(5546, 6086, OutsideSegment, Brake),          // len: 540m
			(6086, 6325, OutsideSegment, Coast),          // len: 239m
			(6325, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Halt),               // len: 0m
			(0, OutsideSegment, Accelerate),         // len: 112m
			(112, OutsideSegment, Roll),             // len: 13m
			(125, OutsideSegment, Accelerate),       // len: 280m
			(405, OutsideSegment, Roll),             // len: 20m
			(425, OutsideSegment, Accelerate),       // len: 48m
			(473, OutsideSegment, Roll),             // len: 20m
			(493, OutsideSegment, Accelerate),       // len: 3315m
			(3808, OutsideSegment, Coast),           // len: 759m
			(4567, OutsideSegment, Brake),           // len: 443m
			(5010, OutsideSegment, Coast),           // len: 150m
			(5160, OutsideSegment, Accelerate));


		[Test]
		public void Class5_NoADAS_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Halt),               // len: 0m
			(0, OutsideSegment, Accelerate),         // len: 112m
			(112, OutsideSegment, Roll),             // len: 13m
			(125, OutsideSegment, Accelerate),       // len: 280m
			(405, OutsideSegment, Roll),             // len: 20m
			(425, OutsideSegment, Accelerate),       // len: 48m
			(473, OutsideSegment, Roll),             // len: 20m
			(493, OutsideSegment, Accelerate),       // len: 3120m
			(3613, OutsideSegment, Coast),           // len: 633m
			(4246, OutsideSegment, Brake),           // len: 262m
			(4508, OutsideSegment, Accelerate));

		private void TestPCC(string jobName, string cycleName, params (double distance, DefaultDriverStrategy.PCCStates pcc, DrivingAction action)[] data)
		{
			var expected = new List<(double start, double end, DefaultDriverStrategy.PCCStates pcc, DrivingAction action)>(
					data.Pairwise().Select(x => (x.Item1.distance, x.Item2.distance, x.Item1.pcc, x.Item1.action)))
				{ (data.Last().distance, 1e6d, data.Last().pcc, data.Last().action) };
			TestPCC(jobName, cycleName, expected.ToArray());
		}

		private void TestPCC(string jobName, string cycleName, params (double start, double end, DefaultDriverStrategy.PCCStates pcc, DrivingAction action)[] data)
		{
			jobName = Path.Combine(BasePath, jobName + ".vecto");

			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));
			var sumContainer = new SummaryDataContainer(writer);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) { WriteModalResults = true, Validate = false, SumData = sumContainer };

			var run = factory.SimulationRuns().First(r => r.CycleName == cycleName);
			var mod = (run.GetContainer().ModalData as ModalDataContainer).Data;
			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			PrintPCCSections(mod);

			var expected = data;

			var segmentWasTested = false;

			var dists = mod.Columns[ModalResultField.dist.GetName()].Values<Meter>();
			var pccs = mod.Columns["PCCState"].Values<DefaultDriverStrategy.PCCStates>();
			var actions = mod.Columns["DriverAction"].Values<DrivingAction>();
			var vActs = mod.Columns[ModalResultField.v_act.GetName()].Values<MeterPerSecond>();

			using (var exp = expected.AsEnumerable().GetEnumerator()) {
				exp.MoveNext();
				foreach (var (dist, pcc, action, vAct) in dists.Zip(pccs, actions, vActs)) {
					if (dist > exp.Current.end) {
						Assert.IsTrue(segmentWasTested, $"dist {dist}: Expected Segment was not tested. Maybe distance range to narrow?");
						if (!exp.MoveNext())
							break;
						segmentWasTested = false;
					}

					if (dist.IsBetween(exp.Current.start, exp.Current.end)) {
						// if the segment is very short, at least one of the entries should have the expected values
						if (exp.Current.pcc == pcc && exp.Current.action == action)
							segmentWasTested = true;
					}

					if (dist.IsBetween(exp.Current.start.SI<Meter>() + vAct * tolerance.SI<Second>(), exp.Current.end.SI<Meter>() - vAct * tolerance.SI<Second>())) {
						Assert.AreEqual(exp.Current.pcc, pcc, $"dist {dist}: Wrong PCC state: {pcc} instead of {exp.Current.pcc}.");
						Assert.AreEqual(exp.Current.action, action, $"dist {dist}: Wrong DriverAction: {action} instead of {exp.Current.action}.");
						segmentWasTested = true;
					}
				}
			}

			Assert.IsTrue(segmentWasTested);
		}


		private void PrintPCCSections(ModalResults mod)
		{
			var sCol = mod.Columns[ModalResultField.dist.GetName()];
			var pccCol = mod.Columns["PCCState"];
			var driverActionCol = mod.Columns["DriverAction"];

			var pccStates = pccCol.Values<DefaultDriverStrategy.PCCStates>();
			var driverAction = driverActionCol.Values<DrivingAction>();
			var distances = sCol.Values<Meter>();
			var sections = GetDistancesOfStateChanges(pccStates.Zip(driverAction), distances).ToArray();

			if (sections.Any()) {
				var start = 0d;
				Console.WriteLine("Found Segments:");
				foreach (var section in sections) {
					Console.WriteLine($"{$"({start}, {section.Before.Item1}, {section.Before.Item2}),",-40} // len: {(int)section.Distance.Value() - start}m");
					start = (int)section.Distance.Value();
				}
				Console.WriteLine($"({start}, {sections.Last().After.Item1}, {sections.Last().After.Item2}));");
				Console.WriteLine();

				Console.WriteLine("Start-End Segments:");
				start = 0;
				foreach (var section in sections) {
					Console.WriteLine($"{$"({start}, {(int)section.Distance.Value()}, {section.Before.Item1}, {section.Before.Item2}),",-45} // len: {(int)section.Distance.Value() - start}m");
					start = (int)section.Distance.Value();
				}
				Console.WriteLine($"({(int)sections.Last().Distance.Value()}, 1e6, {sections.Last().After.Item1}, {sections.Last().After.Item2}));");
			} else {
				Console.WriteLine("Found Segments:");
				Console.WriteLine("(0, OutsideSegment, Accelerate));");
				Console.WriteLine();
				Console.WriteLine("Start-End Segments:");
				Console.WriteLine("(0, 1e6, OutsideSegment, Accelerate));");
			}
		}

		IEnumerable<(T2 Distance, T1 Before, T1 After, T2[] SegmentValues)> GetDistancesOfStateChanges<T1, T2>(IEnumerable<T1> states, IEnumerable<T2> distances)
		{
			using (var values = states.GetEnumerator()) {
				using (var distance = distances.GetEnumerator()) {
					distance.MoveNext();
					values.MoveNext();
					var value = values.Current;
					var segmentValues = new List<T2> { distance.Current };
					while (values.MoveNext() | distance.MoveNext()) {
						if (!value.Equals(values.Current)) {
							yield return (distance.Current, value, values.Current, segmentValues.ToArray());
							segmentValues.Clear();
							value = values.Current;
							segmentValues.Add(distance.Current);
						}
					}
				}
			}
		}
	}
}
