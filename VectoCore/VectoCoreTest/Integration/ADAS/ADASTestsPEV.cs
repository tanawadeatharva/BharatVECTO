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

		[TestCase]
		public void Class5_E2_NoADAS_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5647, OutsideSegment, Accelerate),        // len: 5647m
			(5647, 5776, OutsideSegment, Coast),          // len: 129m
			(5776, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_NoADAS_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 6196, OutsideSegment, Accelerate),        // len: 6196m
			(6196, 7546, OutsideSegment, Coast),          // len: 1350m
			(7546, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5496, OutsideSegment, Accelerate),        // len: 5496m
			(5496, 5849, OutsideSegment, Coast),          // len: 353m
			(5849, 7111, OutsideSegment, Brake),          // len: 1262m
			(7111, 7373, OutsideSegment, Coast),          // len: 262m
			(7373, 1e6, OutsideSegment, Accelerate));


		[TestCase]
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

		[TestCase]
		public void Class5_E2_NoADAS_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2194, OutsideSegment, Accelerate),        // len: 2194m
			(2194, 2358, OutsideSegment, Coast),          // len: 164m
			(2358, 3349, OutsideSegment, Accelerate),     // len: 991m
			(3349, 3597, OutsideSegment, Coast),          // len: 248m
			(3597, 3946, OutsideSegment, Brake),          // len: 349m
			(3946, 4160, OutsideSegment, Coast),          // len: 214m
			(4160, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2206, OutsideSegment, Accelerate),        // len: 2206m
			(2206, 2381, OutsideSegment, Coast),          // len: 175m
			(2381, 2918, OutsideSegment, Accelerate),     // len: 537m
			(2918, 3329, OutsideSegment, Coast),          // len: 411m
			(3329, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5437, OutsideSegment, Accelerate),        // len: 5437m
			(5437, 5614, OutsideSegment, Coast),          // len: 177m
			(5614, 6467, OutsideSegment, Brake),          // len: 853m
			(6467, 6897, OutsideSegment, Coast),          // len: 430m
			(6897, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5332, OutsideSegment, Accelerate),        // len: 5332m
			(5332, 5722, OutsideSegment, Coast),          // len: 390m
			(5722, 5962, OutsideSegment, Brake),          // len: 240m
			(5962, 6177, OutsideSegment, Coast),          // len: 215m
			(6177, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4794, OutsideSegment, Accelerate),        // len: 4794m
			(5546, 6086, OutsideSegment, Brake),          // len: 540m
			(6086, 6325, OutsideSegment, Coast),          // len: 239m
			(6325, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 11, OutsideSegment, Accelerate),          // len: 11m
			(302, 3808, OutsideSegment, Accelerate),      // len: 3506m
			(3808, 4567, OutsideSegment, Coast),          // len: 759m
			(4567, 5010, OutsideSegment, Brake),          // len: 443m
			(5010, 5160, OutsideSegment, Coast),          // len: 150m
			(5160, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_NoADAS_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 11, OutsideSegment, Accelerate),          // len: 11m
			(302, 3614, OutsideSegment, Accelerate),      // len: 3312m
			(3614, 4246, OutsideSegment, Coast),          // len: 632m
			(4246, 4508, OutsideSegment, Brake),          // len: 262m
			(4508, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC123_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4107, OutsideSegment, Accelerate),        // len: 4107m
			(4107, 5437, WithinSegment, Accelerate),      // len: 1330m
			(5437, 5840, UseCase1, Coast),                // len: 403m
			(5840, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4585, OutsideSegment, Accelerate),        // len: 4585m
			(4585, 5426, WithinSegment, Accelerate),      // len: 841m
			(5426, 7318, UseCase1, Coast),                // len: 1892m
			(7318, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3955, OutsideSegment, Accelerate),        // len: 3955m
			(3955, 4912, WithinSegment, Accelerate),      // len: 957m
			(4912, 5360, UseCase1, Coast),                // len: 448m
			(5360, 5381, PCCinterrupt, Accelerate),       // len: 21m
			(5381, 6123, UseCase1, Coast),                // len: 742m
			(6123, 6340, WithinSegment, Coast),           // len: 217m
			(6340, 7094, WithinSegment, Brake),           // len: 754m
			(7094, 7181, WithinSegment, Coast),           // len: 87m
			(7181, 7413, OutsideSegment, Coast),          // len: 232m
			(7413, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1879, WithinSegment, Accelerate),       // len: 1225m
			(1879, 2491, UseCase1, Coast),                // len: 612m
			(2491, 4019, OutsideSegment, Accelerate),     // len: 1528m
			(4019, 4929, WithinSegment, Accelerate),      // len: 910m
			(4929, 5410, UseCase1, Coast),                // len: 481m
			(5410, 5431, PCCinterrupt, Accelerate),       // len: 21m
			(5431, 6263, UseCase1, Coast),                // len: 832m
			(6263, 6709, WithinSegment, Coast),           // len: 446m
			(6709, 6926, OutsideSegment, Coast),          // len: 217m
			(6926, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2089, WithinSegment, Accelerate),       // len: 1400m
			(2089, 2390, UseCase1, Coast),                // len: 301m
			(2390, 2996, WithinSegment, Accelerate),      // len: 606m
			(2996, 3987, UseCase1, Coast),                // len: 991m
			(3987, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2400, UseCase1, Coast),                // len: 323m
			(2400, 2598, WithinSegment, Accelerate),      // len: 198m
			(2598, 3290, UseCase1, Coast),                // len: 692m
			(3290, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3932, OutsideSegment, Accelerate),        // len: 3932m
			(3932, 5076, WithinSegment, Accelerate),      // len: 1144m
			(5076, 5406, UseCase1, Coast),                // len: 330m
			(5406, 5427, PCCinterrupt, Accelerate),       // len: 21m
			(5427, 5911, UseCase1, Coast),                // len: 484m
			(5911, 6628, WithinSegment, Coast),           // len: 717m
			(6628, 7061, OutsideSegment, Coast),          // len: 433m
			(7061, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3792, OutsideSegment, Accelerate),        // len: 3792m
			(3792, 4795, WithinSegment, Accelerate),      // len: 1003m
			(4795, 6017, UseCase1, Coast),                // len: 1222m
			(6017, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4794, WithinSegment, Accelerate),      // len: 1247m
			//(4794, 4805, WithinSegment, Roll),            // len: 11m
			//(4805, 4837, WithinSegment, Accelerate),      // len: 32m
			//(4837, 4845, WithinSegment, Roll),            // len: 8m
			//(4845, 4881, WithinSegment, Accelerate),      // len: 36m
			//(4881, 4887, WithinSegment, Roll),            // len: 6m
			//(4887, 4903, WithinSegment, Accelerate),      // len: 16m
			//(4903, 4909, WithinSegment, Roll),            // len: 6m
			//(4909, 4955, WithinSegment, Accelerate),      // len: 46m
			//(4955, 4962, WithinSegment, Roll),            // len: 7m
			//(4962, 4983, WithinSegment, Accelerate),      // len: 21m
			//(4983, 4992, WithinSegment, Roll),            // len: 9m
			//(4992, 5015, WithinSegment, Accelerate),      // len: 23m
			//(5015, 5025, WithinSegment, Roll),            // len: 10m
			//(5025, 5053, WithinSegment, Accelerate),      // len: 28m
			//(5053, 5065, WithinSegment, Roll),            // len: 12m
			(5065, 5356, WithinSegment, Accelerate),      // len: 291m
			(5356, 5625, UseCase2, Coast),                // len: 269m
			(5625, 5649, UseCase2, Brake),                // len: 24m
			(5649, 5868, WithinSegment, Coast),           // len: 219m
			(5868, 6065, WithinSegment, Brake),           // len: 197m
			(6065, 6140, WithinSegment, Coast),           // len: 75m
			(6140, 6347, OutsideSegment, Coast),          // len: 207m
			(6347, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 11, OutsideSegment, Accelerate),          // len: 11m
			//(11, 15, OutsideSegment, Roll),               // len: 4m
			//(15, 29, OutsideSegment, Accelerate),         // len: 14m
			//(29, 35, OutsideSegment, Roll),               // len: 6m
			//(35, 55, OutsideSegment, Accelerate),         // len: 20m
			//(55, 63, OutsideSegment, Roll),               // len: 8m
			//(63, 87, OutsideSegment, Accelerate),         // len: 24m
			//(87, 97, OutsideSegment, Roll),               // len: 10m
			//(97, 175, OutsideSegment, Accelerate),        // len: 78m
			//(175, 188, OutsideSegment, Roll),             // len: 13m
			//(188, 286, OutsideSegment, Accelerate),       // len: 98m
			//(286, 302, OutsideSegment, Roll),             // len: 16m
			(302, 3808, OutsideSegment, Accelerate),      // len: 3506m
			(3808, 4567, OutsideSegment, Coast),          // len: 759m
			(4567, 5010, OutsideSegment, Brake),          // len: 443m
			(5010, 5160, OutsideSegment, Coast),          // len: 150m
			(5160, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC123_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 11, OutsideSegment, Accelerate),          // len: 11m
			//(11, 15, OutsideSegment, Roll),               // len: 4m
			//(15, 29, OutsideSegment, Accelerate),         // len: 14m
			//(29, 35, OutsideSegment, Roll),               // len: 6m
			//(35, 55, OutsideSegment, Accelerate),         // len: 20m
			//(55, 63, OutsideSegment, Roll),               // len: 8m
			//(63, 87, OutsideSegment, Accelerate),         // len: 24m
			//(87, 97, OutsideSegment, Roll),               // len: 10m
			//(97, 175, OutsideSegment, Accelerate),        // len: 78m
			//(175, 188, OutsideSegment, Roll),             // len: 13m
			//(188, 286, OutsideSegment, Accelerate),       // len: 98m
			//(286, 302, OutsideSegment, Roll),             // len: 16m
			(302, 3614, OutsideSegment, Accelerate),      // len: 3312m
			(3614, 4246, OutsideSegment, Coast),          // len: 632m
			(4246, 4508, OutsideSegment, Brake),          // len: 262m
			(4508, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_E2_PCC12_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4107, OutsideSegment, Accelerate),        // len: 4107m
			(4107, 5437, WithinSegment, Accelerate),      // len: 1330m
			(5437, 5840, UseCase1, Coast),                // len: 403m
			(5840, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4585, OutsideSegment, Accelerate),        // len: 4585m
			(4585, 5426, WithinSegment, Accelerate),      // len: 841m
			(5426, 7318, UseCase1, Coast),                // len: 1892m
			(7318, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3955, OutsideSegment, Accelerate),        // len: 3955m
			(3955, 4912, WithinSegment, Accelerate),      // len: 957m
			(4912, 5360, UseCase1, Coast),                // len: 448m
			(5360, 5381, PCCinterrupt, Accelerate),       // len: 21m
			(5381, 6123, UseCase1, Coast),                // len: 742m
			(6123, 6194, WithinSegment, Coast),           // len: 71m
			(6194, 7107, WithinSegment, Brake),           // len: 913m
			(7107, 7179, WithinSegment, Coast),           // len: 72m
			(7179, 7370, OutsideSegment, Coast),          // len: 191m
			(7370, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1879, WithinSegment, Accelerate),       // len: 1225m
			(1879, 2491, UseCase1, Coast),                // len: 612m
			(2491, 4019, OutsideSegment, Accelerate),     // len: 1528m
			(4019, 4929, WithinSegment, Accelerate),      // len: 910m
			(4929, 5410, UseCase1, Coast),                // len: 481m
			(5410, 5431, PCCinterrupt, Accelerate),       // len: 21m
			(5431, 6263, UseCase1, Coast),                // len: 832m
			(6263, 6394, WithinSegment, Coast),           // len: 131m
			(6394, 6646, WithinSegment, Brake),           // len: 252m
			(6646, 6718, WithinSegment, Coast),           // len: 72m
			(6718, 6897, OutsideSegment, Coast),          // len: 179m
			(6897, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2089, WithinSegment, Accelerate),       // len: 1400m
			(2089, 2390, UseCase1, Coast),                // len: 301m
			(2390, 2996, WithinSegment, Accelerate),      // len: 606m
			(2996, 3987, UseCase1, Coast),                // len: 991m
			(3987, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2400, UseCase1, Coast),                // len: 323m
			(2400, 2598, WithinSegment, Accelerate),      // len: 198m
			(2598, 3290, UseCase1, Coast),                // len: 692m
			(3290, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC12_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3932, OutsideSegment, Accelerate),        // len: 3932m
			(3932, 5076, WithinSegment, Accelerate),      // len: 1144m
			(5076, 5406, UseCase1, Coast),                // len: 330m
			(5406, 5427, PCCinterrupt, Accelerate),       // len: 21m
			(5427, 5911, UseCase1, Coast),                // len: 484m
			(5911, 6031, WithinSegment, Coast),           // len: 120m
			(6031, 6475, WithinSegment, Brake),           // len: 444m
			(6475, 6619, WithinSegment, Coast),           // len: 144m
			(6619, 6904, OutsideSegment, Coast),          // len: 285m
			(6904, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3792, OutsideSegment, Accelerate),        // len: 3792m
			(3792, 4795, WithinSegment, Accelerate),      // len: 1003m
			(4795, 6017, UseCase1, Coast),                // len: 1222m
			(6017, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC12_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4794, WithinSegment, Accelerate),      // len: 1247m
			//(4794, 4805, WithinSegment, Roll),            // len: 11m
			//(4805, 4837, WithinSegment, Accelerate),      // len: 32m
			//(4837, 4845, WithinSegment, Roll),            // len: 8m
			//(4845, 4881, WithinSegment, Accelerate),      // len: 36m
			//(4881, 4887, WithinSegment, Roll),            // len: 6m
			//(4887, 4903, WithinSegment, Accelerate),      // len: 16m
			//(4903, 4909, WithinSegment, Roll),            // len: 6m
			//(4909, 4955, WithinSegment, Accelerate),      // len: 46m
			//(4955, 4962, WithinSegment, Roll),            // len: 7m
			//(4962, 4983, WithinSegment, Accelerate),      // len: 21m
			//(4983, 4992, WithinSegment, Roll),            // len: 9m
			//(4992, 5015, WithinSegment, Accelerate),      // len: 23m
			//(5015, 5025, WithinSegment, Roll),            // len: 10m
			//(5025, 5053, WithinSegment, Accelerate),      // len: 28m
			//(5053, 5065, WithinSegment, Roll),            // len: 12m
			(5065, 5356, WithinSegment, Accelerate),      // len: 291m
			(5356, 5625, UseCase2, Coast),                // len: 269m
			(5625, 5649, UseCase2, Brake),                // len: 24m
			(5649, 5685, WithinSegment, Coast),           // len: 36m
			(5685, 6081, WithinSegment, Brake),           // len: 396m
			(6081, 6141, WithinSegment, Coast),           // len: 60m
			(6141, 6320, OutsideSegment, Coast),          // len: 179m
			(6320, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 11, OutsideSegment, Accelerate),          // len: 11m
			//(11, 15, OutsideSegment, Roll),               // len: 4m
			//(15, 29, OutsideSegment, Accelerate),         // len: 14m
			//(29, 35, OutsideSegment, Roll),               // len: 6m
			//(35, 55, OutsideSegment, Accelerate),         // len: 20m
			//(55, 63, OutsideSegment, Roll),               // len: 8m
			//(63, 87, OutsideSegment, Accelerate),         // len: 24m
			//(87, 97, OutsideSegment, Roll),               // len: 10m
			//(97, 175, OutsideSegment, Accelerate),        // len: 78m
			//(175, 188, OutsideSegment, Roll),             // len: 13m
			//(188, 286, OutsideSegment, Accelerate),       // len: 98m
			//(286, 302, OutsideSegment, Roll),             // len: 16m
			(302, 3808, OutsideSegment, Accelerate),      // len: 3506m
			(3808, 4567, OutsideSegment, Coast),          // len: 759m
			(4567, 5010, OutsideSegment, Brake),          // len: 443m
			(5010, 5160, OutsideSegment, Coast),          // len: 150m
			(5160, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC12_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 11, OutsideSegment, Accelerate),          // len: 11m
			//(11, 15, OutsideSegment, Roll),               // len: 4m
			//(15, 29, OutsideSegment, Accelerate),         // len: 14m
			//(29, 35, OutsideSegment, Roll),               // len: 6m
			//(35, 55, OutsideSegment, Accelerate),         // len: 20m
			//(55, 63, OutsideSegment, Roll),               // len: 8m
			//(63, 87, OutsideSegment, Accelerate),         // len: 24m
			//(87, 97, OutsideSegment, Roll),               // len: 10m
			//(97, 175, OutsideSegment, Accelerate),        // len: 78m
			//(175, 188, OutsideSegment, Roll),             // len: 13m
			//(188, 286, OutsideSegment, Accelerate),       // len: 98m
			//(286, 302, OutsideSegment, Roll),             // len: 16m
			(302, 3614, OutsideSegment, Accelerate),      // len: 3312m
			(3614, 4246, OutsideSegment, Coast),          // len: 632m
			(4246, 4508, OutsideSegment, Brake),          // len: 262m
			(4508, 1e6, OutsideSegment, Accelerate));


		#endregion

		#region E3

		[TestCase]
		public void Class5_E3_PCC123_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5426, WithinSegment, Accelerate),      // len: 1296m
			(5426, 5805, UseCase1, Coast),                // len: 379m
			(5805, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5391, WithinSegment, Accelerate),      // len: 736m
			(5391, 7250, UseCase1, Coast),                // len: 1859m
			(7250, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4889, WithinSegment, Accelerate),      // len: 910m
			(4889, 5316, UseCase1, Coast),                // len: 427m
			(5316, 5337, PCCinterrupt, Accelerate),       // len: 21m
			(5337, 6110, UseCase1, Coast),                // len: 773m
			(6110, 6315, WithinSegment, Coast),           // len: 205m
			(6315, 7106, WithinSegment, Brake),           // len: 791m
			(7106, 7156, WithinSegment, Coast),           // len: 50m
			(7156, 7437, OutsideSegment, Coast),          // len: 281m
			(7437, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1856, WithinSegment, Accelerate),       // len: 1202m
			(1856, 2487, UseCase1, Coast),                // len: 631m
			(2487, 4038, OutsideSegment, Accelerate),     // len: 1551m
			(4038, 4913, WithinSegment, Accelerate),      // len: 875m
			(4913, 5394, UseCase1, Coast),                // len: 481m
			(5394, 5416, PCCinterrupt, Accelerate),       // len: 22m
			(5416, 6225, UseCase1, Coast),                // len: 809m
			(6225, 6686, WithinSegment, Coast),           // len: 461m
			(6686, 6941, OutsideSegment, Coast),          // len: 255m
			(6941, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2054, WithinSegment, Accelerate),       // len: 1354m
			(2054, 2375, UseCase1, Coast),                // len: 321m
			(2375, 2982, WithinSegment, Accelerate),      // len: 607m
			(2982, 3969, UseCase1, Coast),                // len: 987m
			(3969, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2066, WithinSegment, Accelerate),       // len: 1366m
			(2066, 2388, UseCase1, Coast),                // len: 322m
			(2388, 2563, WithinSegment, Accelerate),      // len: 175m
			(2563, 3264, UseCase1, Coast),                // len: 701m
			(3264, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5064, WithinSegment, Accelerate),      // len: 1120m
			(5064, 5406, UseCase1, Coast),                // len: 342m
			(5406, 5427, PCCinterrupt, Accelerate),       // len: 21m
			(5427, 5888, UseCase1, Coast),                // len: 461m
			(5888, 6559, WithinSegment, Coast),           // len: 671m
			(6559, 7153, OutsideSegment, Coast),          // len: 594m
			(7153, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4749, WithinSegment, Accelerate),      // len: 934m
			(4749, 5986, UseCase1, Coast),                // len: 1237m
			(5986, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 5374, WithinSegment, Accelerate),      // len: 1815m
			(5374, 5643, UseCase2, Coast),                // len: 269m
			(5643, 5666, UseCase2, Brake),                // len: 23m
			(5666, 5909, WithinSegment, Coast),           // len: 243m
			(5909, 6082, WithinSegment, Brake),           // len: 173m
			(6082, 6120, WithinSegment, Coast),           // len: 38m
			(6120, 6364, OutsideSegment, Coast),          // len: 244m
			(6364, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 3410, OutsideSegment, Accelerate),        // len: 3410m
			(3410, 4441, OutsideSegment, Coast),          // len: 1031m
			(4441, 5005, OutsideSegment, Brake),          // len: 564m
			(5005, 5206, OutsideSegment, Coast),          // len: 201m
			(5206, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 3614, OutsideSegment, Accelerate),        // len: 3614m
			(3614, 4148, OutsideSegment, Coast),          // len: 534m
			(4148, 4510, OutsideSegment, Brake),          // len: 362m
			(4510, 1e6, OutsideSegment, Accelerate));

		#endregion


		#region E4

		[TestCase]
		public void Class5_E4_PCC123_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5379, WithinSegment, Accelerate),      // len: 1249m
			(5379, 5813, UseCase1, Coast),                // len: 434m
			(5813, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_E4_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4644, OutsideSegment, Accelerate),        // len: 4644m
			(4644, 5309, WithinSegment, Accelerate),      // len: 665m
			(5309, 7250, UseCase1, Coast),                // len: 1941m
			(7250, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4854, WithinSegment, Accelerate),      // len: 875m
			(4854, 5228, UseCase1, Coast),                // len: 374m
			(5228, 5249, PCCinterrupt, Accelerate),       // len: 21m
			(5249, 6144, UseCase1, Coast),                // len: 895m
			(6144, 6337, WithinSegment, Coast),           // len: 193m
			(6337, 7116, WithinSegment, Brake),           // len: 779m
			(7116, 7153, WithinSegment, Coast),           // len: 37m
			(7153, 7529, OutsideSegment, Coast),          // len: 376m
			(7529, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1821, WithinSegment, Accelerate),       // len: 1167m
			(1821, 2479, UseCase1, Coast),                // len: 658m
			(2479, 4038, OutsideSegment, Accelerate),     // len: 1559m
			(4038, 4864, WithinSegment, Accelerate),      // len: 826m
			(4864, 5259, UseCase1, Coast),                // len: 395m
			(5259, 5280, PCCinterrupt, Accelerate),       // len: 21m
			(5280, 5386, UseCase1, Coast),                // len: 106m
			(5386, 5407, PCCinterrupt, Accelerate),       // len: 21m
			(5407, 6236, UseCase1, Coast),                // len: 829m
			(6236, 6685, WithinSegment, Coast),           // len: 449m
			(6685, 7023, OutsideSegment, Coast),          // len: 338m
			(7023, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E4_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2042, WithinSegment, Accelerate),       // len: 1353m
			(2042, 2374, UseCase1, Coast),                // len: 332m
			(2374, 2968, WithinSegment, Accelerate),      // len: 594m
			(2968, 3241, UseCase1, Coast),                // len: 273m
			(3241, 3262, PCCinterrupt, Accelerate),       // len: 21m
			(3262, 3976, UseCase1, Coast),                // len: 714m
			(3976, 4047, OutsideSegment, Coast),          // len: 71m
			(4047, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E4_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2042, WithinSegment, Accelerate),       // len: 1342m
			(2042, 2397, UseCase1, Coast),                // len: 355m
			(2397, 2479, WithinSegment, Accelerate),      // len: 82m
			(2479, 3262, UseCase1, Coast),                // len: 783m
			(3262, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3941, OutsideSegment, Accelerate),        // len: 3941m
			(3941, 5036, WithinSegment, Accelerate),      // len: 1095m
			(5036, 5367, UseCase1, Coast),                // len: 331m
			(5367, 5399, PCCinterrupt, Accelerate),       // len: 32m
			(5399, 5892, UseCase1, Coast),                // len: 493m
			(5892, 6562, WithinSegment, Coast),           // len: 670m
			(6562, 7180, OutsideSegment, Coast),          // len: 618m
			(7180, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_E4_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4667, WithinSegment, Accelerate),      // len: 852m
			(4667, 5096, UseCase1, Coast),                // len: 429m
			(5096, 5118, PCCinterrupt, Accelerate),       // len: 22m
			(5118, 5223, UseCase1, Coast),                // len: 105m
			(5223, 5244, PCCinterrupt, Accelerate),       // len: 21m
			(5244, 5995, UseCase1, Coast),                // len: 751m
			(5995, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseJE4() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3430, OutsideSegment, Accelerate),        // len: 3430m
			(3430, 5236, WithinSegment, Accelerate),      // len: 1806m
			(5236, 6125, UseCase2, Coast),                // len: 889m
			(6125, 6424, WithinSegment, Coast),           // len: 299m
			(6424, 6531, OutsideSegment, Coast),          // len: 107m
			(6531, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 3417, OutsideSegment, Accelerate),        // len: 3417m
			(3417, 4400, OutsideSegment, Coast),          // len: 983m
			(4400, 5004, OutsideSegment, Brake),          // len: 604m
			(5004, 5383, OutsideSegment, Coast),          // len: 379m
			(5383, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 3612, OutsideSegment, Accelerate),        // len: 3612m
			(3612, 4126, OutsideSegment, Coast),          // len: 514m
			(4126, 4509, OutsideSegment, Brake),          // len: 383m
			(4509, 4687, OutsideSegment, Coast),          // len: 178m
			(4687, 1e6, OutsideSegment, Accelerate));

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
			PrintPCCSections(mod);
			Assert.IsTrue(run.FinishedWithoutErrors);

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
						Assert.AreEqual(exp.Current.pcc, pcc, $"dist {dist}: Wrong PCC state: expected {exp.Current.pcc} instead of {pcc}.");
						Assert.AreEqual(exp.Current.action, action, $"dist {dist}: Wrong DriverAction: expected {exp.Current.action} instead of {action}.");
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
