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
			(4107, 5496, WithinSegment, Accelerate),      // len: 1389m
			(5496, 5832, UseCase1, Coast),                // len: 336m
			(5832, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4585, OutsideSegment, Accelerate),        // len: 4585m
			(4585, 5542, WithinSegment, Accelerate),      // len: 957m
			(5542, 7326, UseCase1, Coast),                // len: 1784m
			(7326, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3955, OutsideSegment, Accelerate),        // len: 3955m
			(3955, 4959, WithinSegment, Accelerate),      // len: 1004m
			(4959, 6139, UseCase1, Coast),                // len: 1180m
			(6139, 6345, WithinSegment, Coast),           // len: 206m
			(6345, 7099, WithinSegment, Brake),           // len: 754m
			(7099, 7185, WithinSegment, Coast),           // len: 86m
			(7185, 7417, OutsideSegment, Coast),          // len: 232m
			(7417, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1914, WithinSegment, Accelerate),       // len: 1260m
			(1914, 2487, UseCase1, Coast),                // len: 573m
			(2487, 4015, OutsideSegment, Accelerate),     // len: 1528m
			(4015, 4972, WithinSegment, Accelerate),      // len: 957m
			(4972, 6287, UseCase1, Coast),                // len: 1315m
			(6287, 6709, WithinSegment, Coast),           // len: 422m
			(6709, 6925, OutsideSegment, Coast),          // len: 216m
			(6925, 1e6, OutsideSegment, Accelerate));

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
			(0, 677, OutsideSegment, Accelerate),         // len: 677m
			(677, 2101, WithinSegment, Accelerate),       // len: 1424m
			(2101, 2425, UseCase1, Coast),                // len: 324m
			(2425, 2682, WithinSegment, Accelerate),      // len: 257m
			(2682, 3314, UseCase1, Coast),                // len: 632m
			(3314, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3932, OutsideSegment, Accelerate),        // len: 3932m
			(3932, 5111, WithinSegment, Accelerate),      // len: 1179m
			(5111, 5947, UseCase1, Coast),                // len: 836m
			(5947, 6624, WithinSegment, Coast),           // len: 677m
			(6624, 7032, OutsideSegment, Coast),          // len: 408m
			(7032, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3792, OutsideSegment, Accelerate),        // len: 3792m
			(3792, 4900, WithinSegment, Accelerate),      // len: 1108m
			(4900, 6014, UseCase1, Coast),                // len: 1114m
			(6014, 1e6, OutsideSegment, Accelerate));

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
			(4107, 5496, WithinSegment, Accelerate),      // len: 1389m
			(5496, 5832, UseCase1, Coast),                // len: 336m
			(5832, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4585, OutsideSegment, Accelerate),        // len: 4585m
			(4585, 5542, WithinSegment, Accelerate),      // len: 957m
			(5542, 7326, UseCase1, Coast),                // len: 1784m
			(7326, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3955, OutsideSegment, Accelerate),        // len: 3955m
			(3955, 4959, WithinSegment, Accelerate),      // len: 1004m
			(4959, 6139, UseCase1, Coast),                // len: 1180m
			(6139, 6210, WithinSegment, Coast),           // len: 71m
			(6210, 7112, WithinSegment, Brake),           // len: 902m
			(7112, 7184, WithinSegment, Coast),           // len: 72m
			(7184, 7374, OutsideSegment, Coast),          // len: 190m
			(7374, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1914, WithinSegment, Accelerate),       // len: 1260m
			(1914, 2487, UseCase1, Coast),                // len: 573m
			(2487, 4015, OutsideSegment, Accelerate),     // len: 1528m
			(4015, 4972, WithinSegment, Accelerate),      // len: 957m
			(4972, 6287, UseCase1, Coast),                // len: 1315m
			(6287, 6418, WithinSegment, Coast),           // len: 131m
			(6418, 6646, WithinSegment, Brake),           // len: 228m
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
			(689, 2101, WithinSegment, Accelerate),       // len: 1412m
			(2101, 2402, UseCase1, Coast),                // len: 301m
			(2402, 2682, WithinSegment, Accelerate),      // len: 280m
			(2682, 3291, UseCase1, Coast),                // len: 609m
			(3291, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC12_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3932, OutsideSegment, Accelerate),        // len: 3932m
			(3932, 5111, WithinSegment, Accelerate),      // len: 1179m
			(5111, 5947, UseCase1, Coast),                // len: 836m
			(5947, 6090, WithinSegment, Coast),           // len: 143m
			(6090, 6474, WithinSegment, Brake),           // len: 384m
			(6474, 6630, WithinSegment, Coast),           // len: 156m
			(6630, 6903, OutsideSegment, Coast),          // len: 273m
			(6903, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3792, OutsideSegment, Accelerate),        // len: 3792m
			(3792, 4900, WithinSegment, Accelerate),      // len: 1108m
			(4900, 6014, UseCase1, Coast),                // len: 1114m
			(6014, 1e6, OutsideSegment, Accelerate));


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
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5808, UseCase1, Coast),                // len: 336m
			(5808, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5484, WithinSegment, Accelerate),      // len: 829m
			(5484, 7239, UseCase1, Coast),                // len: 1755m
			(7239, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 6054, UseCase1, Coast),                // len: 1118m
			(6054, 6066, UseCase1, Brake),                // len: 12m
			(6066, 6090, UseCase1, Coast),                // len: 24m
			(6090, 6296, WithinSegment, Coast),           // len: 206m
			(6296, 7112, WithinSegment, Brake),           // len: 816m
			(7112, 7149, WithinSegment, Coast),           // len: 37m
			(7149, 7430, OutsideSegment, Coast),          // len: 281m
			(7430, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2486, UseCase1, Coast),                // len: 584m
			(2486, 4038, OutsideSegment, Accelerate),     // len: 1552m
			(4038, 4948, WithinSegment, Accelerate),      // len: 910m
			(4948, 6260, UseCase1, Coast),                // len: 1312m
			(6260, 6683, WithinSegment, Coast),           // len: 423m
			(6683, 6938, OutsideSegment, Coast),          // len: 255m
			(6938, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2077, WithinSegment, Accelerate),       // len: 1377m
			(2077, 2378, UseCase1, Coast),                // len: 301m
			(2378, 2996, WithinSegment, Accelerate),      // len: 618m
			(2996, 3931, UseCase1, Coast),                // len: 935m
			(3931, 3967, WithinSegment, Coast),           // len: 36m
			(3967, 4049, OutsideSegment, Coast),          // len: 82m
			(4049, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2089, WithinSegment, Accelerate),       // len: 1389m
			(2089, 2390, UseCase1, Coast),                // len: 301m
			(2390, 2646, WithinSegment, Accelerate),      // len: 256m
			(2646, 3265, UseCase1, Coast),                // len: 619m
			(3265, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5923, UseCase1, Coast),                // len: 824m
			(5923, 6555, WithinSegment, Coast),           // len: 632m
			(6555, 7123, OutsideSegment, Coast),          // len: 568m
			(7123, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5995, UseCase1, Coast),                // len: 1141m
			(5995, 1e6, OutsideSegment, Accelerate));

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
			(4130, 5449, WithinSegment, Accelerate),      // len: 1319m
			(5449, 5807, UseCase1, Coast),                // len: 358m
			(5807, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_E4_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4644, OutsideSegment, Accelerate),        // len: 4644m
			(4644, 5437, WithinSegment, Accelerate),      // len: 793m
			(5437, 7260, UseCase1, Coast),                // len: 1823m
			(7260, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4912, WithinSegment, Accelerate),      // len: 933m
			(4912, 6130, UseCase1, Coast),                // len: 1218m
			(6130, 6336, WithinSegment, Coast),           // len: 206m
			(6336, 7114, WithinSegment, Brake),           // len: 778m
			(7114, 7151, WithinSegment, Coast),           // len: 37m
			(7151, 7528, OutsideSegment, Coast),          // len: 377m
			(7528, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1879, WithinSegment, Accelerate),       // len: 1225m
			(1879, 2481, UseCase1, Coast),                // len: 602m
			(2481, 4041, OutsideSegment, Accelerate),     // len: 1560m
			(4041, 4926, WithinSegment, Accelerate),      // len: 885m
			(4926, 6266, UseCase1, Coast),                // len: 1340m
			(6266, 6689, WithinSegment, Coast),           // len: 423m
			(6689, 7002, OutsideSegment, Coast),          // len: 313m
			(7002, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E4_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2066, WithinSegment, Accelerate),       // len: 1377m
			(2066, 2377, UseCase1, Coast),                // len: 311m
			(2377, 2994, WithinSegment, Accelerate),      // len: 617m
			(2994, 3972, UseCase1, Coast),                // len: 978m
			(3972, 4054, OutsideSegment, Coast),          // len: 82m
			(4054, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E4_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2077, WithinSegment, Accelerate),       // len: 1377m
			(2077, 2401, UseCase1, Coast),                // len: 324m
			(2401, 2611, WithinSegment, Accelerate),      // len: 210m
			(2611, 3272, UseCase1, Coast),                // len: 661m
			(3272, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3941, OutsideSegment, Accelerate),        // len: 3941m
			(3941, 5082, WithinSegment, Accelerate),      // len: 1141m
			(5082, 5939, UseCase1, Coast),                // len: 857m
			(5939, 6559, WithinSegment, Coast),           // len: 620m
			(6559, 7150, OutsideSegment, Coast),          // len: 591m
			(7150, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_E4_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4807, WithinSegment, Accelerate),      // len: 992m
			(4807, 5995, UseCase1, Coast),                // len: 1188m
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
