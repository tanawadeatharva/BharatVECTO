using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	public class MeanShiftClustering
	{
		
		public int MaxIterations { get; set; }

		public double UpdateTolerance { get; set; }

		public int ClusterCount { get; set; }

		public MeanShiftClustering()
		{
			MaxIterations = 20;
			UpdateTolerance = 1e-3;
			ClusterCount = 50;
		}

		public double[] FindClusters(double[] numbers, double clusterTolerance)
		{
			var min = numbers.Min();
			var max = numbers.Max();
			var clusterDistance = (max - min) / ClusterCount;
			var clusters = new Cluster[ClusterCount];
			for (var i = 0; i < ClusterCount; i++) {
				clusters[i] = new Cluster(min + (i + 0.5) * clusterDistance);
			}

			IterationCount = 0;
			var updated = true;
			while (IterationCount++ < MaxIterations && updated) {
				for (var clusterIdx = 0; clusterIdx < ClusterCount; clusterIdx++) {
					var minDist = double.MaxValue;
					var minIdx = -1;
					for (var valueIdx = 0; valueIdx < numbers.Length; valueIdx++) {
						if (clusters[clusterIdx].Distance(numbers[valueIdx]) < minDist) {
							minDist = clusters[clusterIdx].Distance(numbers[valueIdx]);
							minIdx = valueIdx;
						}
					}

					if (minIdx >= 0) {
						clusters[clusterIdx].AddValue(numbers[minIdx]);
					}
				}

				updated = false;
				for (int i = 0; i < ClusterCount; i++) {
					updated |= clusters[i].Update(UpdateTolerance);
				}
			}

			return clusters.Select(c => c.Center).Distinct(new ClusterComparer(clusterTolerance)).ToArray();
		}

		public int IterationCount { get; protected set; }

		public struct Cluster
		{
			private int _count;
			private double _sum;

			public Cluster(double center)
			{
				Center = center;
				_sum = 0.0;
				_count = 0;
			}

			public double Center { get; private set; }

			public double Distance(double val)
			{
				return Math.Abs(Center - val);
			}

			public void AddValue(double val)
			{
				_sum += val;
				_count++;
			}

			public double? Mean { get { return _count > 0 ? _sum / _count : (double?)null; } }

			public override string ToString()
			{
				return string.Format("[{0},{2}]", Center, Mean);
			}

			public bool Update(double tolerance)
			{
				if (Mean == null)
					return false;
				var retVal = Math.Abs(Center - Mean.Value) > tolerance;
				Center = Mean.Value;
				_count = 0;
				_sum = 0;
				return retVal;
			}
		}
	}

	public class ClusterComparer : IEqualityComparer<double>
	{
		protected readonly double Tolerance;

		public ClusterComparer(double updateTolerance)
		{
			Tolerance = updateTolerance;
		}

		#region Implementation of IEqualityComparer<in double>

		public bool Equals(double x, double y)
		{
			return x.IsEqual(y, Tolerance);
		}

		public int GetHashCode(double obj)
		{
			return 0;
		}

		#endregion
	}
}
