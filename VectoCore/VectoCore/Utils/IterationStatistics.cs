using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	public static class IterationStatistics
	{
		private static readonly ThreadLocal<List<DataEntry>> DataLocal =
			new ThreadLocal<List<DataEntry>>(() => new List<DataEntry>());

		public static List<DataEntry> Data
		{
			get { return DataLocal.Value; }
		}

		private static readonly ThreadLocal<Stopwatch> TimerLocal = new ThreadLocal<Stopwatch>(Stopwatch.StartNew);

		private static Stopwatch Timer
		{
			get { return TimerLocal.Value; }
		}

		private static readonly ThreadLocal<Dictionary<string, Dictionary<string, double>>> CurrentLocal =
			new ThreadLocal<Dictionary<string, Dictionary<string, double>>>(
				() => new Dictionary<string, Dictionary<string, double>>());

		private static Dictionary<string, Dictionary<string, double>> Current
		{
			get { return CurrentLocal.Value; }
			set { CurrentLocal.Value = value; }
		}

		[Conditional("TRACE")]
		public static void Increment<T>(T o, string key, double? value = null)
		{
			var t = typeof(T).Name;

			if (!Current.ContainsKey(t))
				Current[t] = new Dictionary<string, double>();

			if (Current[t].ContainsKey(key))
				if (value.HasValue)
					Current[t][key] += value.Value;
				else
					Current[t][key]++;

			else {
				if (value.HasValue)
					Current[t][key] = value.Value;
				else
					Current[t][key] = 1;
			}
		}

		public static void StartIteration()
		{
			Timer.Restart();
		}

		[Conditional("TRACE"), MethodImpl(MethodImplOptions.Synchronized)]
		public static void FinishIteration(Second absTime)
		{
			Timer.Stop();
			Data.Add(new DataEntry(absTime, Timer.Elapsed, Current));
			Current = new Dictionary<string, Dictionary<string, double>>();
		}

		[Conditional("TRACE")]
		public static void FinishSimulation(string runName)
		{
			var table = new DataTable();
			table.Columns.Add("Distance", typeof(double));
			table.Columns.Add("Time", typeof(double));
			table.Columns.Add("StepDuration", typeof(double));
			table.Columns.Add("StepIterationCount", typeof(double));
			table.Columns.Add("NumDriverRequests", typeof(double));
			table.Columns.Add("NumAccelActions", typeof(double));
			table.Columns.Add("NumBrakeActions", typeof(double));
			table.Columns.Add("NumCoastActions", typeof(double));
			table.Columns.Add("NumRollActions", typeof(double));
			table.Columns.Add("SearchOPIterations", typeof(double));
			table.Columns.Add("SearchBrakeIterations", typeof(double));
			table.Columns.Add("NumGearboxRequests", typeof(double));
			table.Columns.Add("NumEngineRequests", typeof(double));

			foreach (var entry in Data) {
				var row = table.NewRow();
				row["StepDuration"] = entry.Duration.TotalMilliseconds;

				if (entry.Values.ContainsKey("DistanceRun")) {
					row["Distance"] = entry.Values["DistanceRun"].ContainsKey("Distance") ? entry.Values["DistanceRun"]["Distance"] : 0;
					row["Time"] = entry.Values["DistanceRun"].ContainsKey("Time") ? entry.Values["DistanceRun"]["Time"] : 0;
					row["StepIterationCount"] = entry.Values["DistanceRun"].ContainsKey("Iterations")
						? entry.Values["DistanceRun"]["Iterations"]
						: 0;
				}
				if (entry.Values.ContainsKey("Driver")) {
					row["NumDriverRequests"] = entry.Values["Driver"].ContainsKey("Requests") ? entry.Values["Driver"]["Requests"] : 0;
					row["NumAccelActions"] = entry.Values["Driver"].ContainsKey("Accelerate")
						? entry.Values["Driver"]["Accelerate"]
						: 0;
					row["NumBrakeActions"] = entry.Values["Driver"].ContainsKey("Brake") ? entry.Values["Driver"]["Brake"] : 0;
					row["NumCoastActions"] = entry.Values["Driver"].ContainsKey("Coast") ? entry.Values["Driver"]["Coast"] : 0;
					row["NumRollActions"] = entry.Values["Driver"].ContainsKey("Roll") ? entry.Values["Driver"]["Roll"] : 0;
					row["SearchOPIterations"] = entry.Values["Driver"].ContainsKey("SearchOperatingPoint")
						? entry.Values["Driver"]["SearchOperatingPoint"]
						: 0;
					row["SearchBrakeIterations"] = entry.Values["Driver"].ContainsKey("SearchBrakingPower")
						? entry.Values["Driver"]["SearchBrakingPower"]
						: 0;
				}
				if (entry.Values.ContainsKey("Gearbox")) {
					row["NumGearboxRequests"] = entry.Values["Gearbox"].ContainsKey("Requests")
						? entry.Values["Gearbox"]["Requests"]
						: 0;
				}
				if (entry.Values.ContainsKey("CombustionEngine")) {
					row["NumEngineRequests"] = entry.Values["CombustionEngine"].ContainsKey("Requests")
						? entry.Values["CombustionEngine"]["Requests"]
						: 0;
				}
				table.Rows.Add(row);
			}
			var writer = new StreamWriter("statistics_" + runName + ".csv");
			VectoCSVFile.Write(writer, table);
		}

		public sealed class DataEntry
		{
			public readonly TimeSpan Duration;
			public readonly Second Time;
			public readonly Dictionary<string, Dictionary<string, double>> Values;

			public DataEntry(Second time, TimeSpan duration, Dictionary<string, Dictionary<string, double>> values)
			{
				Time = time;
				Duration = duration;
				Values = values;
			}
		}
	}
}