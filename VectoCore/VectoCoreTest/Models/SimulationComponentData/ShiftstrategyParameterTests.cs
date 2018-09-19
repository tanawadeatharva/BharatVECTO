using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCore.InputData.Reader.ShiftStrategy;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestFixture]
	public class ShiftstrategyParameterTests
	{

		[TestCase(0.3, 1.0),
		TestCase(0.8, 1.0),
		TestCase(0.9, 0.5),
		TestCase(1.5, 0.5),
		TestCase(0.85, 0.75)
		]
		public void TestPredictionIntervalLookup(double speedRatio, double expected)
		{
			var lookup = PredictionDurationLookupReader.ReadFromStream(
				RessourceHelper.ReadStream(
					DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.PredictionTimeLookup.csv"));
			var val = lookup.Lookup(speedRatio);

			Assert.AreEqual(expected, val, 1e-6);
		}
	}
}
