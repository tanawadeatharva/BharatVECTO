using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
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
	}
}
