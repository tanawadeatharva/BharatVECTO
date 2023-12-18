namespace VectoCommonTests
{
	using NUnit.Framework;
	using System;
	using System.Collections.Generic;
	using TUGraz.VectoCommon.Models;
	using TUGraz.VectoCommon.Utils;

	public class EnumerableExtensionMethodsTest
	{
		[Test]
		[TestCase(1, 2, 3, 4, 4)]
		[TestCase(1, 2, 3, -1, 3)]
		[TestCase(-1, -2, -3, -4, -1)]
		public void MaxBy_Returns_MaxValue(int value1, int value2, int value3, int value4, double result)
		{
			List<int> sut = new List<int>() { value1, value2, value3, value4 };
			Assert.That(sut.MaxBy(x => x), Is.EqualTo(result));
		}

		[Test]
		public void MaxBy_Returns_Null_WhenCollection_Empty()
		{
			List<HybridResultEntry> sut = new List<HybridResultEntry>();

			var result = sut.MaxBy(x => x.U);

			Assert.IsNull(result);
		}

		[Test]
		public void MaxBy_Throws_InvalidOperationException_WhenCollectionDefaultNotNull()
		{
			Dictionary<int, HybridResultEntry> sut = new Dictionary<int, HybridResultEntry>();

			Assert.Throws<InvalidOperationException>(() => sut.MaxBy(x => x.Value.Cost));
		}

		[Test]
		public void MinBy_Returns_Null_WhenCollection_Empty()
		{
			List<HybridResultEntry> sut = new List<HybridResultEntry>(); 

			var result = sut.MinBy(x => x.U);

			Assert.IsNull(result);
		}

		[Test]
		public void MinBy_Throws_InvalidOperationException_WhenCollectionDefaultNotNull()
		{
			Dictionary<int, HybridResultEntry> sut = new Dictionary<int, HybridResultEntry>();

			Assert.Throws<InvalidOperationException>(() => sut.MinBy(x => x.Value.Cost));
		}

		[Test]
		public void MinBy_Returns_MinValue()
		{
			List<int> sut = new List<int>() { 1, 2, 3, -1 };
			Assert.That(sut.MinBy(x => x), Is.EqualTo(-1));

			sut = new List<int>() { -1, -2, -3, -4 };
			Assert.That(sut.MinBy(x => x), Is.EqualTo(-4));

			sut = new List<int>() { 1, 2, 3, 4 };
			Assert.That(sut.MinBy(x => x), Is.EqualTo(1));
		}


		[Test]
		[TestCase(1, 2, 3, 4, 1)]
		[TestCase(1, 2, 3, -1, -1)]
		[TestCase(-1, -2, -3, -4, -4)]
		public void MinBy_Returns_MinValue_HybridResult(int value1, int value2, int value3, int value4, double result)
		{
			List<int> sut = new List<int>() { value1, value2, value3, value4 };
			Assert.That(sut.MinBy(x => x), Is.EqualTo(result));
		}
	}
}