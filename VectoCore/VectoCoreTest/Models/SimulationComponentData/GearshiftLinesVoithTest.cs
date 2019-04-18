using System.IO;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestFixture]
	public class GearshiftLinesVoithTest
	{
		public const string File = @"TestData\Components\GearshiftLinesVoith.vgsv";


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(File, 1, 1, 0.0, 0.0, 680),
		TestCase(File, 1, 1, 0.0, 1.0, 680),
		TestCase(File, 1, 1, 0.0, 0.5, 680),

		TestCase(File, 1, 1, 5.0, 0.0, 725),
		TestCase(File, 1, 1, 5.0, 1.0, 725),
		TestCase(File, 1, 1, 5.0, 0.5, 725),


		TestCase(File, 1, 1, -5.0, 0.0, 650),
		TestCase(File, 1, 1, -5.0, 1.0, 650),
		TestCase(File, 1, 1, -5.0, 0.5, 650),

		TestCase(File, 1, 1, 2.5, 0.0, 702.5),
		TestCase(File, 1, 1, 2.5, 1.0, 702.5),
		TestCase(File, 1, 1, 2.5, 0.5, 702.5),

		TestCase(File, 1, 2, 0.0, 1.0, 680),
		TestCase(File, 1, 2, 5.0, 1.0, 725),
		TestCase(File, 1, 2, -5.0, 1.0, 650),
		TestCase(File, 1, 2, 2.5, 1.0, 702.5),

		TestCase(File, 1, 4, 0.0, 0.5, 690),
		TestCase(File, 1, 4, 5.0, 0.5, 735),
		TestCase(File, 1, 4, -5.0, 0.5, 660),
		TestCase(File, 1, 4, 2.5, 0.0, 702.5),
		TestCase(File, 1, 4, 2.5, 0.5, 712.5),
		TestCase(File, 1, 4, 2.5, 1.0, 722.5),

		TestCase(File, 1, 5, 0.0, 0.5, 700),
		TestCase(File, 1, 5, 5.0, 0.5, 745),
		TestCase(File, 1, 5, -5.0, 0.5, 670),
		TestCase(File, 1, 5, 2.5, 0.0, 712.5),
		TestCase(File, 1, 5, 2.5, 0.5, 722.5),
		TestCase(File, 1, 5, 2.5, 1.0, 732.5),

		TestCase(File, 1, 6, 0.0, 0.5, 710),
		TestCase(File, 1, 6, 5.0, 0.5, 755),
		TestCase(File, 1, 6, -5.0, 0.5, 680),
		TestCase(File, 1, 6, 2.5, 0.0, 722.5),
		TestCase(File, 1, 6, 2.5, 0.5, 732.5),
		TestCase(File, 1, 6, 2.5, 1.0, 742.5),

			]
		public void TestShiftlinesLookupUpshift(string filename,int gear, int loadStage, double gradient, double acc, double expectedRpm)
		{
			var data = VectoCSVFile.Read(filename);

			var runData = new VectoRunData() {
				GearshiftParameters = new ShiftStrategyParameters() {
					GearshiftLines = data
				}
			};

			var strategy = new ATShiftStrategyVoith(runData, null);
			var slope = VectoMath.InclinationToAngle(gradient / 100.0);

			var upshiftSpeed = strategy.UpshiftLines[gear].LookupShiftSpeed(
				loadStage, slope, acc.SI<MeterPerSquareSecond>(), 0.1.SI<MeterPerSquareSecond>(), 0.9.SI<MeterPerSquareSecond>());

			Assert.AreEqual(expectedRpm, upshiftSpeed.AsRPM, 0.1);
		}
	}
}
