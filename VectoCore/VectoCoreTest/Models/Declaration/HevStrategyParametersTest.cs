using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class HevStrategyParametersTest
    {
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}
		private const string HevStrategyParamFile = @"TestData\Cycles\HEV_Strategy_Parameters_fequiv_40soc_Lorries.csv";
        [TestCase]
		public void TestHevStrategyParametersInput()
		{
			var lookup = new HEVStrategyParameters();
			Assert.IsTrue(lookup.Entries.Count > 0);
		}

		[TestCase]
		public void TestHevStrategyLookup()
		{
			LookupAndAssert(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 2.0);

			LookupAndAssert(MissionType.UrbanDelivery, VehicleClass.Class4, LoadingType.ReferenceLoad, 2.50);
			LookupAndAssert(MissionType.UrbanDelivery, VehicleClass.Class4, LoadingType.LowLoading, 2.10);

			LookupAndAssert(MissionType.LongHaul, VehicleClass.Class3, LoadingType.LowLoading, 0);


		}

		private void LookupAndAssert(MissionType mission, VehicleClass hdvClass, LoadingType loading, double expected)
		{
			var feq = DeclarationData.HevStrategyParameters.LookupEquivalenceFactor(mission, hdvClass,
				loading);

			Assert.IsTrue(feq.IsEqual(expected));
		}
	}
}
