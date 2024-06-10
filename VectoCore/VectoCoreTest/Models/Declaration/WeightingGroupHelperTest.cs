
namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	using NUnit.Framework;
	using TUGraz.VectoCommon.Models;
	using TUGraz.VectoCommon.Utils;
	using TUGraz.VectoCore.Models.Declaration;

	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class WeightingGroupHelperTest: WeightingGroups
	{
		[TestCase()]
		public void WeightingGroupHelper_ResourceId()
		{
			Assert.AreEqual("TUGraz.VectoCore.Resources.Declaration.CO2Standards.WeightingGroups.csv", ResourceId);
		}

		// Group 1s
		[TestCase(VehicleClass.Class1s, false, false, 100, null, WeightingGroup.Group1s)]
		[TestCase(VehicleClass.Class1s, false, true, 100, null, WeightingGroup.Group1s)]
		[TestCase(VehicleClass.Class1s, false, false, 100, 100, WeightingGroup.Group1s)]
		[TestCase(VehicleClass.Class1s, true, false, 100, 100, WeightingGroup.Group1s)]

		// Group 1
		[TestCase(VehicleClass.Class1, false, false, 100, null, WeightingGroup.Group1)]
		[TestCase(VehicleClass.Class1, false, true, 100, null, WeightingGroup.Group1)]
		[TestCase(VehicleClass.Class1, false, false, 100, 100, WeightingGroup.Group1)]
		[TestCase(VehicleClass.Class1, true, false, 100, 100, WeightingGroup.Group1)]

		// Group 2
		[TestCase(VehicleClass.Class2, false, false, 100, null, WeightingGroup.Group2)]
		[TestCase(VehicleClass.Class2, false, true, 100, null, WeightingGroup.Group2)]
		[TestCase(VehicleClass.Class2, false, false, 100, 100, WeightingGroup.Group2)]
		[TestCase(VehicleClass.Class2, true, false, 100, 100, WeightingGroup.Group2)]

		// Group 3
		[TestCase(VehicleClass.Class3, false, false, 100, null, WeightingGroup.Group3)]
		[TestCase(VehicleClass.Class3, false, true, 100, null, WeightingGroup.Group3)]
		[TestCase(VehicleClass.Class3, false, false, 100, 100, WeightingGroup.Group3)]
		[TestCase(VehicleClass.Class3, true, false, 100, 100, WeightingGroup.Group3)]
		[TestCase(VehicleClass.Class3, true, true, 100, 100, WeightingGroup.Group3)]

		// Group 4-UD
		[TestCase(VehicleClass.Class4, false, false, 169.9, null, WeightingGroup.Group4UD)]
		[TestCase(VehicleClass.Class4, false, true, 169.9, null, WeightingGroup.Group4UD)]
		[TestCase(VehicleClass.Class4, false, false, 169.9, 100, WeightingGroup.Group4UD)]
		[TestCase(VehicleClass.Class4, true, false, 169.9, 100, WeightingGroup.Group4UD)]
		[TestCase(VehicleClass.Class4, true, true, 100, 400, WeightingGroup.Group4UD)]

		// Group 4-RD
		[TestCase(VehicleClass.Class4, false, false, 170, null, WeightingGroup.Group4RD)]
		[TestCase(VehicleClass.Class4, false, false, 265, null, WeightingGroup.Group4RD)]
		[TestCase(VehicleClass.Class4, false, false, 170, 100, WeightingGroup.Group4RD)]
		[TestCase(VehicleClass.Class4, false, true, 170, null, WeightingGroup.Group4RD)]
		[TestCase(VehicleClass.Class4, false, true, 170, 351, WeightingGroup.Group4RD)]
		[TestCase(VehicleClass.Class4, false, true, 264, null, WeightingGroup.Group4RD)]
		[TestCase(VehicleClass.Class4, true, true, 270, 250, WeightingGroup.Group4RD)]
		[TestCase(VehicleClass.Class4, true, false, 170, 400, WeightingGroup.Group4RD)]
		[TestCase(VehicleClass.Class4, true, true, 170, 400, WeightingGroup.Group4RD)]

		// Group 4-LH
		[TestCase(VehicleClass.Class4, false, true, 270, 100, WeightingGroup.Group4LH)]
		[TestCase(VehicleClass.Class4, false, true, 265, null, WeightingGroup.Group4LH)]
		[TestCase(VehicleClass.Class4, true, true, 270, 351, WeightingGroup.Group4LH)]
		[TestCase(VehicleClass.Class4, true, true, 265, null, WeightingGroup.Group4LH)]

		//Group 5-RD
		[TestCase(VehicleClass.Class5, false, false, 0, null, WeightingGroup.Group5RD)]
		[TestCase(VehicleClass.Class5, false, false, 0, null, WeightingGroup.Group5RD)]
		[TestCase(VehicleClass.Class5, false, false, 0, 100, WeightingGroup.Group5RD)]
		[TestCase(VehicleClass.Class5, false, true, 264.9, null, WeightingGroup.Group5RD)]
		[TestCase(VehicleClass.Class5, false, true, 264.9, 351, WeightingGroup.Group5RD)]
		[TestCase(VehicleClass.Class5, false, true, 264.9, null, WeightingGroup.Group5RD)]
		[TestCase(VehicleClass.Class5, true, true, 265, 250, WeightingGroup.Group5RD)]
		[TestCase(VehicleClass.Class5, true, false, 265, 400, WeightingGroup.Group5RD)]

		//Group 5-LH
		[TestCase(VehicleClass.Class5, false, true, 270, 100, WeightingGroup.Group5LH)]
		[TestCase(VehicleClass.Class5, false, true, 265, null, WeightingGroup.Group5LH)]
		[TestCase(VehicleClass.Class5, true, true, 270, 351, WeightingGroup.Group5LH)]
		[TestCase(VehicleClass.Class5, true, true, 265, null, WeightingGroup.Group5LH)]

		//Group 9-RD
		[TestCase(VehicleClass.Class9, false, false, 0, null, WeightingGroup.Group9RD)]
		[TestCase(VehicleClass.Class9, true, false, 0, null, WeightingGroup.Group9RD)]
		[TestCase(VehicleClass.Class9, true, true, 0, 349, WeightingGroup.Group9RD)]

		////Group 9-LH
		[TestCase(VehicleClass.Class9, true, true, 0, null, WeightingGroup.Group9LH)]
		[TestCase(VehicleClass.Class9, false, true, 0, null, WeightingGroup.Group9LH)]
		[TestCase(VehicleClass.Class9, false, true, 0, 349, WeightingGroup.Group9LH)]
		[TestCase(VehicleClass.Class9, false, true, 0, null, WeightingGroup.Group9LH)]
		[TestCase(VehicleClass.Class9, false, true, 0, 350, WeightingGroup.Group9LH)]
		[TestCase(VehicleClass.Class9, true, true, 0, 350, WeightingGroup.Group9LH)]

		// Group 10-RD
		[TestCase(VehicleClass.Class10, false, false, 0, null, WeightingGroup.Group10RD)]
		[TestCase(VehicleClass.Class10, true, false, 0, null, WeightingGroup.Group10RD)]

		// Group 10-LH
		[TestCase(VehicleClass.Class10, false, true, 0, null, WeightingGroup.Group10LH)]
		[TestCase(VehicleClass.Class10, true, true, 0, null, WeightingGroup.Group10LH)]
		[TestCase(VehicleClass.Class10, false, true, 0, 100, WeightingGroup.Group10LH)]
		[TestCase(VehicleClass.Class10, true, true, 0, 100, WeightingGroup.Group10LH)]

		// Group 11
		[TestCase(VehicleClass.Class11, false, false, 0, null, WeightingGroup.Group11)]
		[TestCase(VehicleClass.Class11, false, true, 0, null, WeightingGroup.Group11)]
		[TestCase(VehicleClass.Class11, true, false, 0, null, WeightingGroup.Group11)]
		[TestCase(VehicleClass.Class11, true, true, 0, null, WeightingGroup.Group11)]

		// Group 12
		[TestCase(VehicleClass.Class12, false, false, 0, null, WeightingGroup.Group12)]
		[TestCase(VehicleClass.Class12, false, true, 0, null, WeightingGroup.Group12)]
		[TestCase(VehicleClass.Class12, true, false, 0, null, WeightingGroup.Group12)]
		[TestCase(VehicleClass.Class12, true, true, 0, null, WeightingGroup.Group12)]

		// Group 16
		[TestCase(VehicleClass.Class16, false, false, 0, null, WeightingGroup.Group16)]
		[TestCase(VehicleClass.Class16, false, true, 0, null, WeightingGroup.Group16)]
		[TestCase(VehicleClass.Class16, true, false, 0, null, WeightingGroup.Group16)]
		[TestCase(VehicleClass.Class16, true, true, 0, null, WeightingGroup.Group16)]
		public void WeightingGroupHelper_AssignsCorrectSubgroup(VehicleClass vehicleClass, bool isElectric, bool hasSleeperCabin, double enginePower, double? operationalRange, WeightingGroup expectedSubgroup)
		{
			double? range = operationalRange != null ? operationalRange.Value.SI(Unit.SI.Kilo.Meter).Cast<Meter>().Value() : null;
			var subgroup = Lookup(vehicleClass, hasSleeperCabin, enginePower.SI(Unit.SI.Kilo.Watt).Cast<Watt>(), isElectric, range);

			Assert.AreEqual(expectedSubgroup, subgroup);
		}
	}
}
