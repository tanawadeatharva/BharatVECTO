using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Algorithms
{
	[TestFixture]
	public class MeanShiftClusteringTest
	{
		
		[TestCase]
		public void TestClusteringClusters()
		{
			var centers = new[] { 100.0, 200, 300, 400 };
			var entries = new List<double>();
			var rnd = new Random(centers.Length);
			foreach (var center in centers) {
				entries.Add(center + rnd.NextDouble() * 80 - 40);
			}

			var clusterer = new MeanShiftClustering();
			var clusters = clusterer.FindClusters(entries.ToArray(), 0.1);

			Console.WriteLine(string.Join(", ",centers));
			Console.WriteLine(string.Join(", ", clusters));

			Assert.AreEqual(centers.Length, clusters.Length);
			foreach (var center in centers) {
				Assert.IsTrue(clusters.Any(x => Math.Abs(center - x) < 1));
			}
		}

		[TestCase]
		public void TestClusteringRandom()
		{
			var entries = new double[100];
			var rnd = new Random(0);
			for (var i = 0; i < entries.Length; i++) {
				entries[i] = rnd.NextDouble() * 100;
			}

			var clusterer = new MeanShiftClustering();
			var clusters = clusterer.FindClusters(entries.ToArray(), 0.1);

			Console.WriteLine(string.Join(", ", clusters));
		}
	}
}
