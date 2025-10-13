using NUnit.Framework;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
    [TestFixture]
    internal class FindClosestElementSearchTest
    {

        [TestCase(new int[] { 100, 200, 250, 800, 900 }, 10, 0, TestName="Lower than lowest")]
		[TestCase(new int[] { 100, 200, 230, 250, 800, 900 }, 1000, 5, TestName = "Higher than highest")]
		[TestCase(new int[] { 100, 200, 250, 800, 900 }, 150, 1, TestName = "In between")]
		[TestCase(new int[] { 100, 200, 250, 800, 900 }, 110, 0, TestName = "In between II ")]
        [TestCase(new int[] { 100, 200, 250, 800, 900 }, 800, 3, TestName = "Exact match")]
		public void FindClosestElementSearch(int[] array, int value, int expected)
		{
            Assert.AreEqual(expected, SearchAlgorithm.FindClosest(array,  value, t => t.a - t.b, out var _));
		}

		public void FindClosestElementDouble()
		{
			Assert.Fail("We should add more testcases with different datatypes");
		}
	}
}
