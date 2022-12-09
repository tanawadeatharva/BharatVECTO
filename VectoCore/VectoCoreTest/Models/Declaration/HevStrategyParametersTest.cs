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
		public void TestHevStrategyParametersInputLorry()
		{
			Assert.DoesNotThrow(() => new HEVStrategyParametersLorry());
			
		}
		[TestCase]
		public void TestHevStrategyParametersInputBus()
		{
			Assert.DoesNotThrow(() => new HEVStrategyParametersBus());

		}

		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 2.00, 40)]
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 2.20, 40)]
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 1.9, 20)]
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 2.40, 20)]
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 0.10, 10)]
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 0.10, 10)]


		//Lookup in nearest table
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 2.00, 90)]
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 2.20, 90)]
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 1.9, 21)]
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 2.40, 21)]


		//Currently looks up in the soc 20 csv, change if needed and remove the other testcase
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 2.00, 30)]
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 2.20, 30)]
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 1.9, 30)]
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 2.40, 30)]



		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 0.10, 10)]
		[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 0.10, 10)]


		public void TestHevStrategyLookup(MissionType missionType, VehicleClass vehicleClass, LoadingType loadingType, double expected, int socRange)
		{
			LookupAndAssert(missionType, vehicleClass, loadingType, expected,socRange);
		}

		private void LookupAndAssert(MissionType mission, VehicleClass hdvClass, LoadingType loading, double expected, int socRange)
		{
			var feq = DeclarationData.InitEquivalenceFactors.LookupEquivalenceFactor(mission, hdvClass,
				loading, socRange);

			Assert.IsTrue(feq.IsEqual(expected));
		}
	}
}
