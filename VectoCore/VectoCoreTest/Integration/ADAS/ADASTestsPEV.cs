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

		#region E2

		[Test]
		public void Class5_E2_NoADAS_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
	(0, 5647, OutsideSegment, Accelerate),        // len: 5647m
	(5647, 5776, OutsideSegment, Coast),          // len: 129m
	(5776, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E2_NoADAS_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 6196, OutsideSegment, Accelerate),        // len: 6196m
			(6196, 7546, OutsideSegment, Coast),          // len: 1350m
			(7546, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_NoADAS_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5496, OutsideSegment, Accelerate),        // len: 5496m
			(5496, 5849, OutsideSegment, Coast),          // len: 353m
			(5849, 7111, OutsideSegment, Brake),          // len: 1262m
			(7111, 7373, OutsideSegment, Coast),          // len: 262m
			(7373, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E2_NoADAS_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2147, OutsideSegment, Accelerate),        // len: 2147m
			(2147, 2300, OutsideSegment, Coast),          // len: 153m
			(2300, 2469, OutsideSegment, Brake),          // len: 169m
			(2469, 2517, OutsideSegment, Coast),          // len: 48m
			(2517, 5550, OutsideSegment, Accelerate),     // len: 3033m
			(5550, 5916, OutsideSegment, Coast),          // len: 366m
			(5916, 6649, OutsideSegment, Brake),          // len: 733m
			(6649, 6899, OutsideSegment, Coast),          // len: 250m
			(6899, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_NoADAS_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2194, OutsideSegment, Accelerate),        // len: 2194m
			(2194, 2358, OutsideSegment, Coast),          // len: 164m
			(2358, 3349, OutsideSegment, Accelerate),     // len: 991m
			(3349, 3597, OutsideSegment, Coast),          // len: 248m
			(3597, 3946, OutsideSegment, Brake),          // len: 349m
			(3946, 4160, OutsideSegment, Coast),          // len: 214m
			(4160, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_NoADAS_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2206, OutsideSegment, Accelerate),        // len: 2206m
			(2206, 2381, OutsideSegment, Coast),          // len: 175m
			(2381, 2918, OutsideSegment, Accelerate),     // len: 537m
			(2918, 3329, OutsideSegment, Coast),          // len: 411m
			(3329, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_NoADAS_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5437, OutsideSegment, Accelerate),        // len: 5437m
			(5437, 5614, OutsideSegment, Coast),          // len: 177m
			(5614, 6467, OutsideSegment, Brake),          // len: 853m
			(6467, 6897, OutsideSegment, Coast),          // len: 430m
			(6897, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_NoADAS_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5332, OutsideSegment, Accelerate),        // len: 5332m
			(5332, 5722, OutsideSegment, Coast),          // len: 390m
			(5722, 5962, OutsideSegment, Brake),          // len: 240m
			(5962, 6177, OutsideSegment, Coast),          // len: 215m
			(6177, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_NoADAS_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_NoADAS_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4794, OutsideSegment, Accelerate),        // len: 4794m
			(5546, 6086, OutsideSegment, Brake),          // len: 540m
			(6086, 6325, OutsideSegment, Coast),          // len: 239m
			(6325, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_NoADAS_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 112, OutsideSegment, Accelerate),         // len: 112m
			(493, 3808, OutsideSegment, Accelerate),      // len: 3315m
			(3808, 4567, OutsideSegment, Coast),          // len: 759m
			(4567, 5010, OutsideSegment, Brake),          // len: 443m
			(5010, 5160, OutsideSegment, Coast),          // len: 150m
			(5160, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E2_NoADAS_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 112, OutsideSegment, Accelerate),         // len: 112m
			(493, 3613, OutsideSegment, Accelerate),      // len: 3120m
			(3613, 4246, OutsideSegment, Coast),          // len: 633m
			(4246, 4508, OutsideSegment, Brake),          // len: 262m
			(4508, 1e6, OutsideSegment, Accelerate));
		

		[Test]
		public void Class5_E2_PCC123_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4060, OutsideSegment, Accelerate),        // len: 4060m
			(4060, 5496, WithinSegment, Accelerate),      // len: 1436m
			(5496, 5878, UseCase1, Coast),                // len: 382m
			(5878, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4469, OutsideSegment, Accelerate),        // len: 4469m
			(4469, 5542, WithinSegment, Accelerate),      // len: 1073m
			(5542, 7430, UseCase1, Coast),                // len: 1888m
			(7430, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3909, OutsideSegment, Accelerate),        // len: 3909m
			(3909, 4947, WithinSegment, Accelerate),      // len: 1038m
			(4947, 6146, UseCase1, Coast),                // len: 1199m
			(6146, 6364, WithinSegment, Coast),           // len: 218m
			(6364, 7093, WithinSegment, Brake),           // len: 729m
			(7093, 7229, WithinSegment, Coast),           // len: 136m
			(7229, 7412, OutsideSegment, Coast),          // len: 183m
			(7412, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 642, OutsideSegment, Accelerate),         // len: 642m
			(642, 1914, WithinSegment, Accelerate),       // len: 1272m
			(1914, 2499, UseCase1, Coast),                // len: 585m
			(2499, 3969, OutsideSegment, Accelerate),     // len: 1470m
			(3969, 4972, WithinSegment, Accelerate),      // len: 1003m
			(4972, 6287, UseCase1, Coast),                // len: 1315m
			(6287, 6757, WithinSegment, Coast),           // len: 470m
			(6757, 6925, OutsideSegment, Coast),          // len: 168m
			(6925, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 665, OutsideSegment, Accelerate),         // len: 665m
			(665, 2089, WithinSegment, Accelerate),       // len: 1424m
			(2089, 2401, UseCase1, Coast),                // len: 312m
			(2401, 2996, WithinSegment, Accelerate),      // len: 595m
			(2996, 4022, UseCase1, Coast),                // len: 1026m
			(4022, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 677, OutsideSegment, Accelerate),         // len: 677m
			(677, 2101, WithinSegment, Accelerate),       // len: 1424m
			(2101, 2425, UseCase1, Coast),                // len: 324m
			(2425, 2682, WithinSegment, Accelerate),      // len: 257m
			(2682, 3314, UseCase1, Coast),                // len: 632m
			(3314, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3932, OutsideSegment, Accelerate),        // len: 3932m
			(3932, 5111, WithinSegment, Accelerate),      // len: 1179m
			(5111, 5947, UseCase1, Coast),                // len: 836m
			(5947, 6746, WithinSegment, Coast),           // len: 799m
			(6746, 7032, OutsideSegment, Coast),          // len: 286m
			(7032, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E2_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3757, OutsideSegment, Accelerate),        // len: 3757m
			(3757, 4900, WithinSegment, Accelerate),      // len: 1143m
			(4900, 6049, UseCase1, Coast),                // len: 1149m
			(6049, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC123_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC123_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4794, WithinSegment, Accelerate),      // len: 1247m
			(5437, 5619, UseCase2, Coast),                // len: 182m
			(5642, 5861, WithinSegment, Coast),           // len: 219m
			(5861, 6072, WithinSegment, Brake),           // len: 211m
			(6072, 6183, WithinSegment, Coast),           // len: 111m
			(6183, 6353, OutsideSegment, Coast),          // len: 170m
			(6353, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC123_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 112, OutsideSegment, Accelerate),         // len: 112m
			(493, 3808, OutsideSegment, Accelerate),      // len: 3315m
			(3808, 4567, OutsideSegment, Coast),          // len: 759m
			(4567, 5010, OutsideSegment, Brake),          // len: 443m
			(5010, 5160, OutsideSegment, Coast),          // len: 150m
			(5160, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E2_PCC123_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 112, OutsideSegment, Accelerate),         // len: 112m
			(493, 3613, OutsideSegment, Accelerate),      // len: 3120m
			(3613, 4246, OutsideSegment, Coast),          // len: 633m
			(4246, 4508, OutsideSegment, Brake),          // len: 262m
			(4508, 1e6, OutsideSegment, Accelerate));

		

		[Test]
		public void Class5_E2_PCC12_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4060, OutsideSegment, Accelerate),        // len: 4060m
			(4060, 5496, WithinSegment, Accelerate),      // len: 1436m
			(5496, 5878, UseCase1, Coast),                // len: 382m
			(5878, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC12_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4469, OutsideSegment, Accelerate),        // len: 4469m
			(4469, 5542, WithinSegment, Accelerate),      // len: 1073m
			(5542, 7430, UseCase1, Coast),                // len: 1888m
			(7430, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC12_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3909, OutsideSegment, Accelerate),        // len: 3909m
			(3909, 4947, WithinSegment, Accelerate),      // len: 1038m
			(4947, 6146, UseCase1, Coast),                // len: 1199m
			(6146, 6218, WithinSegment, Coast),           // len: 72m
			(6218, 7107, WithinSegment, Brake),           // len: 889m
			(7107, 7227, WithinSegment, Coast),           // len: 120m
			(7227, 7369, OutsideSegment, Coast),          // len: 142m
			(7369, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC12_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 642, OutsideSegment, Accelerate),         // len: 642m
			(642, 1914, WithinSegment, Accelerate),       // len: 1272m
			(1914, 2499, UseCase1, Coast),                // len: 585m
			(2499, 3969, OutsideSegment, Accelerate),     // len: 1470m
			(3969, 4972, WithinSegment, Accelerate),      // len: 1003m
			(4972, 6287, UseCase1, Coast),                // len: 1315m
			(6287, 6418, WithinSegment, Coast),           // len: 131m
			(6418, 6646, WithinSegment, Brake),           // len: 228m
			(6646, 6754, WithinSegment, Coast),           // len: 108m
			(6754, 6897, OutsideSegment, Coast),          // len: 143m
			(6897, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC12_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 665, OutsideSegment, Accelerate),         // len: 665m
			(665, 2089, WithinSegment, Accelerate),       // len: 1424m
			(2089, 2401, UseCase1, Coast),                // len: 312m
			(2401, 2996, WithinSegment, Accelerate),      // len: 595m
			(2996, 4022, UseCase1, Coast),                // len: 1026m
			(4022, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC12_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 677, OutsideSegment, Accelerate),         // len: 677m
			(677, 2101, WithinSegment, Accelerate),       // len: 1424m
			(2101, 2425, UseCase1, Coast),                // len: 324m
			(2425, 2682, WithinSegment, Accelerate),      // len: 257m
			(2682, 3314, UseCase1, Coast),                // len: 632m
			(3314, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC12_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3932, OutsideSegment, Accelerate),        // len: 3932m
			(3932, 5111, WithinSegment, Accelerate),      // len: 1179m
			(5111, 5947, UseCase1, Coast),                // len: 836m
			(5947, 6090, WithinSegment, Coast),           // len: 143m
			(6090, 6474, WithinSegment, Brake),           // len: 384m
			(6474, 6749, WithinSegment, Coast),           // len: 275m
			(6749, 6903, OutsideSegment, Coast),          // len: 154m
			(6903, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC12_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3757, OutsideSegment, Accelerate),        // len: 3757m
			(3757, 4900, WithinSegment, Accelerate),      // len: 1143m
			(4900, 6049, UseCase1, Coast),                // len: 1149m
			(6049, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E2_PCC12_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC12_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4794, WithinSegment, Accelerate),      // len: 1247m
			(5437, 5619, UseCase2, Coast),                // len: 182m
			(5678, 6087, WithinSegment, Brake),           // len: 409m
			(6087, 6183, WithinSegment, Coast),           // len: 96m
			(6183, 6325, OutsideSegment, Coast),          // len: 142m
			(6325, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E2_PCC12_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 112, OutsideSegment, Accelerate),         // len: 112m
			(112, 125, OutsideSegment, Roll),             // len: 13m
			(125, 405, OutsideSegment, Accelerate),       // len: 280m
			(405, 425, OutsideSegment, Roll),             // len: 20m
			(425, 473, OutsideSegment, Accelerate),       // len: 48m
			(473, 493, OutsideSegment, Roll),             // len: 20m
			(493, 3808, OutsideSegment, Accelerate),      // len: 3315m
			(3808, 4567, OutsideSegment, Coast),          // len: 759m
			(4567, 5010, OutsideSegment, Brake),          // len: 443m
			(5010, 5160, OutsideSegment, Coast),          // len: 150m
			(5160, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E2_PCC12_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 112, OutsideSegment, Accelerate),         // len: 112m
			(112, 125, OutsideSegment, Roll),             // len: 13m
			(125, 405, OutsideSegment, Accelerate),       // len: 280m
			(405, 425, OutsideSegment, Roll),             // len: 20m
			(425, 473, OutsideSegment, Accelerate),       // len: 48m
			(473, 493, OutsideSegment, Roll),             // len: 20m
			(493, 3613, OutsideSegment, Accelerate),      // len: 3120m
			(3613, 4246, OutsideSegment, Coast),          // len: 633m
			(4246, 4508, OutsideSegment, Brake),          // len: 262m
			(4508, 1e6, OutsideSegment, Accelerate));


		#endregion

		#region E3

		[Test]
		public void Class5_E3_PCC123_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4049, OutsideSegment, Accelerate),        // len: 4049m
			(4049, 5472, WithinSegment, Accelerate),      // len: 1423m
			(5472, 5889, UseCase1, Coast),                // len: 417m
			(5889, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E3_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4445, OutsideSegment, Accelerate),        // len: 4445m
			(4445, 5484, WithinSegment, Accelerate),      // len: 1039m
			(5484, 7459, UseCase1, Coast),                // len: 1975m
			(7459, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E3_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3897, OutsideSegment, Accelerate),        // len: 3897m
			(3897, 4936, WithinSegment, Accelerate),      // len: 1039m
			(4936, 6054, UseCase1, Coast),                // len: 1118m
			(6054, 6066, UseCase1, Brake),                // len: 12m
			(6066, 6090, UseCase1, Coast),                // len: 24m
			(6090, 6296, WithinSegment, Coast),           // len: 206m
			(6296, 7112, WithinSegment, Brake),           // len: 816m
			(7112, 7235, WithinSegment, Coast),           // len: 123m
			(7235, 7430, OutsideSegment, Coast),          // len: 195m
			(7430, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E3_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 642, OutsideSegment, Accelerate),         // len: 642m
			(642, 1902, WithinSegment, Accelerate),       // len: 1260m
			(1902, 2498, UseCase1, Coast),                // len: 596m
			(2498, 3956, OutsideSegment, Accelerate),     // len: 1458m
			(3956, 4948, WithinSegment, Accelerate),      // len: 992m
			(4948, 6260, UseCase1, Coast),                // len: 1312m
			(6260, 6769, WithinSegment, Coast),           // len: 509m
			(6769, 6938, OutsideSegment, Coast),          // len: 169m
			(6938, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E3_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 665, OutsideSegment, Accelerate),         // len: 665m
			(665, 2077, WithinSegment, Accelerate),       // len: 1412m
			(2077, 2401, UseCase1, Coast),                // len: 324m
			(2401, 2984, WithinSegment, Accelerate),      // len: 583m
			(2984, 4031, UseCase1, Coast),                // len: 1047m
			(4031, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E3_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 677, OutsideSegment, Accelerate),         // len: 677m
			(677, 2089, WithinSegment, Accelerate),       // len: 1412m
			(2089, 2425, UseCase1, Coast),                // len: 336m
			(2425, 2646, WithinSegment, Accelerate),      // len: 221m
			(2646, 3334, UseCase1, Coast),                // len: 688m
			(3334, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E3_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3920, OutsideSegment, Accelerate),        // len: 3920m
			(3920, 5099, WithinSegment, Accelerate),      // len: 1179m
			(5099, 5923, UseCase1, Coast),                // len: 824m
			(5923, 6788, WithinSegment, Coast),           // len: 865m
			(6788, 7123, OutsideSegment, Coast),          // len: 335m
			(7123, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E3_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3745, OutsideSegment, Accelerate),        // len: 3745m
			(3745, 4854, WithinSegment, Accelerate),      // len: 1109m
			(4854, 6065, UseCase1, Coast),                // len: 1211m
			(6065, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E3_PCC123_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E3_PCC123_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 5374, WithinSegment, Accelerate),      // len: 1827m
			(5374, 5643, UseCase2, Coast),                // len: 269m
			(5643, 5666, UseCase2, Brake),                // len: 23m
			(5666, 5909, WithinSegment, Coast),           // len: 243m
			(5909, 6082, WithinSegment, Brake),           // len: 173m
			(6082, 6194, WithinSegment, Coast),           // len: 112m
			(6194, 6364, OutsideSegment, Coast),          // len: 170m
			(6364, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E3_PCC123_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 3410, OutsideSegment, Accelerate),        // len: 3410m
			(3410, 4441, OutsideSegment, Coast),          // len: 1031m
			(4441, 5005, OutsideSegment, Brake),          // len: 564m
			(5005, 5206, OutsideSegment, Coast),          // len: 201m
			(5206, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E3_PCC123_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 3614, OutsideSegment, Accelerate),        // len: 3614m
			(3614, 4148, OutsideSegment, Coast),          // len: 534m
			(4148, 4510, OutsideSegment, Brake),          // len: 362m
			(4510, 1e6, OutsideSegment, Accelerate));

		#endregion


		#region E4

		[Test]
		public void Class5_E4_PCC123_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 271, OutsideSegment, Brake),              // len: 271m
			(271, 4025, OutsideSegment, Accelerate),      // len: 3754m
			(4025, 5902, WithinSegment, Accelerate),      // len: 1877m
			(5902, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E4_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 271, OutsideSegment, Brake),              // len: 271m
			(271, 4342, OutsideSegment, Accelerate),      // len: 4071m
			(4342, 7546, WithinSegment, Accelerate),      // len: 3204m
			(7546, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E4_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 271, OutsideSegment, Brake),              // len: 271m
			(271, 3850, OutsideSegment, Accelerate),      // len: 3579m
			(3850, 7279, WithinSegment, Accelerate),      // len: 3429m
			(7279, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E4_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 271, OutsideSegment, Brake),              // len: 271m
			(271, 630, OutsideSegment, Accelerate),       // len: 359m
			(630, 2499, WithinSegment, Accelerate),       // len: 1869m
			(2499, 3900, OutsideSegment, Accelerate),     // len: 1401m
			(3900, 6803, WithinSegment, Accelerate),      // len: 2903m
			(6803, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E4_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 271, OutsideSegment, Brake),              // len: 271m
			(271, 647, OutsideSegment, Accelerate),       // len: 376m
			(647, 4050, WithinSegment, Accelerate),       // len: 3403m
			(4050, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E4_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 271, OutsideSegment, Brake),              // len: 271m
			(271, 655, OutsideSegment, Accelerate),       // len: 384m
			(655, 3350, WithinSegment, Accelerate),       // len: 2695m
			(3350, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E4_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 271, OutsideSegment, Brake),              // len: 271m
			(271, 3909, OutsideSegment, Accelerate),      // len: 3638m
			(3909, 6903, WithinSegment, Accelerate),      // len: 2994m
			(6903, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E4_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 271, OutsideSegment, Brake),              // len: 271m
			(271, 3717, OutsideSegment, Accelerate),      // len: 3446m
			(3717, 6086, WithinSegment, Accelerate),      // len: 2369m
			(6086, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E4_PCC123_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 271, OutsideSegment, Brake),              // len: 271m
			(271, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E4_PCC123_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 271, OutsideSegment, Brake),              // len: 271m
			(271, 3542, OutsideSegment, Accelerate),      // len: 3271m
			(3542, 6222, WithinSegment, Accelerate),      // len: 2680m
			(6222, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_E4_PCC123_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 1e6, OutsideSegment, Accelerate));


		[Test]
		public void Class5_E4_PCC123_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 1e6, OutsideSegment, Accelerate));

		#endregion

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

			var distances = mod.Columns[ModalResultField.dist.GetName()].Values<Meter>();
			var pccStates = mod.Columns["PCCState"].Values<DefaultDriverStrategy.PCCStates>();
			var actions = mod.Columns["DriverAction"].Values<DrivingAction>();
			var vActs = mod.Columns[ModalResultField.v_act.GetName()].Values<MeterPerSecond>();

			using (var exp = expected.AsEnumerable().GetEnumerator()) {
				exp.MoveNext();
				foreach (var (dist, pcc, action, vAct) in distances.Zip(pccStates, actions, vActs)) {
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

			Console.WriteLine("Start-End Segments:");
			if (sections.Any()) {
				var start = 0d;
				foreach (var section in sections) {
					Console.WriteLine($"{$"({start}, {(int)section.Distance.Value()}, {section.Before.Item1}, {section.Before.Item2}),",-45} // len: {(int)section.Distance.Value() - start}m");
					start = (int)section.Distance.Value();
				}
				Console.WriteLine($"({(int)sections.Last().Distance.Value()}, 1e6, {sections.Last().After.Item1}, {sections.Last().After.Item2}));");
			} else {
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
