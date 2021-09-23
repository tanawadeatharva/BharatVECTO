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
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.PCCStates;
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
			(4107, 5566, WithinSegment, Accelerate),      // len: 1459m
			(5566, 5834, UseCase1, Coast),                // len: 268m
			(5834, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4585, OutsideSegment, Accelerate),        // len: 4585m
			(4585, 5682, WithinSegment, Accelerate),      // len: 1097m
			(5682, 7327, UseCase1, Coast),                // len: 1645m
			(7327, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3955, OutsideSegment, Accelerate),        // len: 3955m
			(3955, 4982, WithinSegment, Accelerate),      // len: 1027m
			(4982, 6112, UseCase1, Coast),                // len: 1130m
			(6112, 6330, WithinSegment, Coast),           // len: 218m
			(6330, 7096, WithinSegment, Brake),           // len: 766m
			(7096, 7183, WithinSegment, Coast),           // len: 87m
			(7183, 7414, OutsideSegment, Coast),          // len: 231m
			(7414, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1949, WithinSegment, Accelerate),       // len: 1295m
			(1949, 2483, UseCase1, Coast),                // len: 534m
			(2483, 2495, OutsideSegment, Coast),          // len: 12m
			(2495, 4011, OutsideSegment, Accelerate),     // len: 1516m
			(4011, 5003, WithinSegment, Accelerate),      // len: 992m
			(5003, 6235, UseCase1, Coast),                // len: 1232m
			(6235, 6719, WithinSegment, Coast),           // len: 484m
			(6719, 6924, OutsideSegment, Coast),          // len: 205m
			(6924, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2112, WithinSegment, Accelerate),       // len: 1423m
			(2112, 2380, UseCase1, Coast),                // len: 268m
			(2380, 3033, WithinSegment, Accelerate),      // len: 653m
			(3033, 3840, UseCase1, Coast),                // len: 807m
			(3840, 3994, WithinSegment, Coast),           // len: 154m
			(3994, 4089, OutsideSegment, Coast),          // len: 95m
			(4089, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2124, WithinSegment, Accelerate),       // len: 1435m
			(2124, 2403, UseCase1, Coast),                // len: 279m
			(2403, 2741, WithinSegment, Accelerate),      // len: 338m
			(2741, 3286, UseCase1, Coast),                // len: 545m
			(3286, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3932, OutsideSegment, Accelerate),        // len: 3932m
			(3932, 5134, WithinSegment, Accelerate),      // len: 1202m
			(5134, 5917, UseCase1, Coast),                // len: 783m
			(5917, 6621, WithinSegment, Coast),           // len: 704m
			(6621, 7054, OutsideSegment, Coast),          // len: 433m
			(7054, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3792, OutsideSegment, Accelerate),        // len: 3792m
			(3792, 4982, WithinSegment, Accelerate),      // len: 1190m
			(4982, 6011, UseCase1, Coast),                // len: 1029m
			(6011, 1e6, OutsideSegment, Accelerate));

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
			//(4983, 4991, WithinSegment, Roll),            // len: 8m
			//(4991, 5015, WithinSegment, Accelerate),      // len: 24m
			//(5015, 5025, WithinSegment, Roll),            // len: 10m
			//(5025, 5053, WithinSegment, Accelerate),      // len: 28m
			//(5053, 5065, WithinSegment, Roll),            // len: 12m
			(5065, 5356, WithinSegment, Accelerate),      // len: 291m
			(5356, 5684, UseCase2, Coast),                // len: 328m
			(5684, 6147, WithinSegment, Coast),           // len: 463m
			(6147, 6354, OutsideSegment, Coast),          // len: 207m
			(6354, 1e6, OutsideSegment, Accelerate));

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
			(4107, 5566, WithinSegment, Accelerate),      // len: 1459m
			(5566, 5834, UseCase1, Coast),                // len: 268m
			(5834, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4585, OutsideSegment, Accelerate),        // len: 4585m
			(4585, 5682, WithinSegment, Accelerate),      // len: 1097m
			(5682, 7327, UseCase1, Coast),                // len: 1645m
			(7327, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3955, OutsideSegment, Accelerate),        // len: 3955m
			(3955, 4982, WithinSegment, Accelerate),      // len: 1027m
			(4982, 6112, UseCase1, Coast),                // len: 1130m
			(6112, 6183, WithinSegment, Coast),           // len: 71m
			(6183, 7108, WithinSegment, Brake),           // len: 925m
			(7108, 7181, WithinSegment, Coast),           // len: 73m
			(7181, 7371, OutsideSegment, Coast),          // len: 190m
			(7371, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1949, WithinSegment, Accelerate),       // len: 1295m
			(1949, 2483, UseCase1, Coast),                // len: 534m
			(2483, 2495, OutsideSegment, Coast),          // len: 12m
			(2495, 4011, OutsideSegment, Accelerate),     // len: 1516m
			(4011, 5003, WithinSegment, Accelerate),      // len: 992m
			(5003, 6235, UseCase1, Coast),                // len: 1232m
			(6235, 6366, WithinSegment, Coast),           // len: 131m
			(6366, 6643, WithinSegment, Brake),           // len: 277m
			(6643, 6715, WithinSegment, Coast),           // len: 72m
			(6715, 6905, OutsideSegment, Coast),          // len: 190m
			(6905, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2112, WithinSegment, Accelerate),       // len: 1423m
			(2112, 2380, UseCase1, Coast),                // len: 268m
			(2380, 3033, WithinSegment, Accelerate),      // len: 653m
			(3033, 3840, UseCase1, Coast),                // len: 807m
			(3840, 3994, WithinSegment, Coast),           // len: 154m
			(3994, 4089, OutsideSegment, Coast),          // len: 95m
			(4089, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2124, WithinSegment, Accelerate),       // len: 1435m
			(2124, 2403, UseCase1, Coast),                // len: 279m
			(2403, 2741, WithinSegment, Accelerate),      // len: 338m
			(2741, 3286, UseCase1, Coast),                // len: 545m
			(3286, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC12_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3932, OutsideSegment, Accelerate),        // len: 3932m
			(3932, 5134, WithinSegment, Accelerate),      // len: 1202m
			(5134, 5917, UseCase1, Coast),                // len: 783m
			(5917, 6036, WithinSegment, Coast),           // len: 119m
			(6036, 6468, WithinSegment, Brake),           // len: 432m
			(6468, 6624, WithinSegment, Coast),           // len: 156m
			(6624, 6898, OutsideSegment, Coast),          // len: 274m
			(6898, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3792, OutsideSegment, Accelerate),        // len: 3792m
			(3792, 4982, WithinSegment, Accelerate),      // len: 1190m
			(4982, 6011, UseCase1, Coast),                // len: 1029m
			(6011, 1e6, OutsideSegment, Accelerate));


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
			//(4983, 4991, WithinSegment, Roll),            // len: 8m
			//(4991, 5015, WithinSegment, Accelerate),      // len: 24m
			//(5015, 5025, WithinSegment, Roll),            // len: 10m
			//(5025, 5053, WithinSegment, Accelerate),      // len: 28m
			//(5053, 5065, WithinSegment, Roll),            // len: 12m
			(5065, 5356, WithinSegment, Accelerate),      // len: 291m
			(5356, 5684, UseCase2, Coast),                // len: 328m
			(5684, 5755, WithinSegment, Coast),           // len: 71m
			(5755, 6080, WithinSegment, Brake),           // len: 325m
			(6080, 6140, WithinSegment, Coast),           // len: 60m
			(6140, 6330, OutsideSegment, Coast),          // len: 190m
			(6330, 1e6, OutsideSegment, Accelerate));

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
			(4130, 5531, WithinSegment, Accelerate),      // len: 1401m
			(5531, 5810, UseCase1, Coast),                // len: 279m
			(5810, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5612, WithinSegment, Accelerate),      // len: 957m
			(5612, 7240, UseCase1, Coast),                // len: 1628m
			(7240, 7264, OutsideSegment, Coast),          // len: 24m
			(7264, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4971, WithinSegment, Accelerate),      // len: 992m
			(4971, 6090, UseCase1, Coast),                // len: 1119m
			(6090, 6296, WithinSegment, Coast),           // len: 206m
			(6296, 7112, WithinSegment, Brake),           // len: 816m
			(7112, 7149, WithinSegment, Coast),           // len: 37m
			(7149, 7430, OutsideSegment, Coast),          // len: 281m
			(7430, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1926, WithinSegment, Accelerate),       // len: 1272m
			(1926, 2479, UseCase1, Coast),                // len: 553m
			(2479, 4043, OutsideSegment, Accelerate),     // len: 1564m
			(4043, 4988, WithinSegment, Accelerate),      // len: 945m
			(4988, 6198, UseCase1, Coast),                // len: 1210m
			(6198, 6683, WithinSegment, Coast),           // len: 485m
			(6683, 6939, OutsideSegment, Coast),          // len: 256m
			(6939, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2101, WithinSegment, Accelerate),       // len: 1401m
			(2101, 2379, UseCase1, Coast),                // len: 278m
			(2379, 3009, WithinSegment, Accelerate),      // len: 630m
			(3009, 3868, UseCase1, Coast),                // len: 859m
			(3868, 3975, WithinSegment, Coast),           // len: 107m
			(3975, 4093, OutsideSegment, Coast),          // len: 118m
			(4093, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2112, WithinSegment, Accelerate),       // len: 1412m
			(2112, 2391, UseCase1, Coast),                // len: 279m
			(2391, 2718, WithinSegment, Accelerate),      // len: 327m
			(2718, 3262, UseCase1, Coast),                // len: 544m
			(3262, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5122, WithinSegment, Accelerate),      // len: 1178m
			(5122, 5893, UseCase1, Coast),                // len: 771m
			(5893, 6551, WithinSegment, Coast),           // len: 658m
			(6551, 7145, OutsideSegment, Coast),          // len: 594m
			(7145, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4947, WithinSegment, Accelerate),      // len: 1132m
			(4947, 5996, UseCase1, Coast),                // len: 1049m
			(5996, 6020, OutsideSegment, Coast),          // len: 24m
			(6020, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 5374, WithinSegment, Accelerate),      // len: 1815m
			(5374, 5713, UseCase2, Coast),                // len: 339m
			(5713, 6114, WithinSegment, Coast),           // len: 401m
			(6114, 6370, OutsideSegment, Coast),          // len: 256m
			(6370, 1e6, OutsideSegment, Accelerate));

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
			(4130, 5531, WithinSegment, Accelerate),      // len: 1401m
			(5531, 5810, UseCase1, Coast),                // len: 279m
			(5810, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_E4_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4644, OutsideSegment, Accelerate),        // len: 4644m
			(4644, 5601, WithinSegment, Accelerate),      // len: 957m
			(5601, 7252, UseCase1, Coast),                // len: 1651m
			(7252, 7417, OutsideSegment, Coast),          // len: 165m
			(7417, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4959, WithinSegment, Accelerate),      // len: 980m
			(4959, 6088, UseCase1, Coast),                // len: 1129m
			(6088, 6293, WithinSegment, Coast),           // len: 205m
			(6293, 7121, WithinSegment, Brake),           // len: 828m
			(7121, 7158, WithinSegment, Coast),           // len: 37m
			(7158, 7523, OutsideSegment, Coast),          // len: 365m
			(7523, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1926, WithinSegment, Accelerate),       // len: 1272m
			(1926, 2480, UseCase1, Coast),                // len: 554m
			(2480, 2503, OutsideSegment, Coast),          // len: 23m
			(2503, 4040, OutsideSegment, Accelerate),     // len: 1537m
			(4040, 4983, WithinSegment, Accelerate),      // len: 943m
			(4983, 6192, UseCase1, Coast),                // len: 1209m
			(6192, 6691, WithinSegment, Coast),           // len: 499m
			(6691, 7042, OutsideSegment, Coast),          // len: 351m
			(7042, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E4_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2101, WithinSegment, Accelerate),       // len: 1412m
			(2101, 2379, UseCase1, Coast),                // len: 278m
			(2379, 3008, WithinSegment, Accelerate),      // len: 629m
			(3008, 3888, UseCase1, Coast),                // len: 880m
			(3888, 3971, WithinSegment, Coast),           // len: 83m
			(3971, 4100, OutsideSegment, Coast),          // len: 129m
			(4100, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E4_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2112, WithinSegment, Accelerate),       // len: 1412m
			(2112, 2391, UseCase1, Coast),                // len: 279m
			(2391, 2706, WithinSegment, Accelerate),      // len: 315m
			(2706, 3273, UseCase1, Coast),                // len: 567m
			(3273, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3941, OutsideSegment, Accelerate),        // len: 3941m
			(3941, 5117, WithinSegment, Accelerate),      // len: 1176m
			(5117, 5888, UseCase1, Coast),                // len: 771m
			(5888, 6559, WithinSegment, Coast),           // len: 671m
			(6559, 7177, OutsideSegment, Coast),          // len: 618m
			(7177, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_E4_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4936, WithinSegment, Accelerate),      // len: 1121m
			(4936, 5994, UseCase1, Coast),                // len: 1058m
			(5994, 6065, OutsideSegment, Coast),          // len: 71m
			(6065, 1e6, OutsideSegment, Accelerate));

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

		private void TestPCC(string jobName, string cycleName, params (double start, double end, PCCStates pcc, DrivingAction action)[] data)
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
			var pccStates = mod.Columns["PCCState"].Values<PCCStates>();
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

			var pccStates = pccCol.Values<PCCStates>();
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
