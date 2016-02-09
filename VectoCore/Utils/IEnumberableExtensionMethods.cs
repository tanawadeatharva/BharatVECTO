/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace TUGraz.VectoCore.Utils
{
	public static class IEnumberableExtensionMethods
	{
		public static IEnumerable<double> ToDouble(this IEnumerable<string> self)
		{
			return self.Select(StringExtensionMethods.ToDouble);
		}

		public static IList<double> ToDouble(this IEnumerable<SI> self)
		{
			return self.Select(x => x.Value()).ToList();
		}

		/// <summary>
		/// Wraps this object instance into an IEnumerable.
		/// </summary>
		public static IEnumerable<T> ToEnumerable<T>(this T item)
		{
			yield return item;
		}

		public static IEnumerable<TResult> ZipAll<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first,
			IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
		{
			var firstEnum = first.GetEnumerator();
			var secondEnum = second.GetEnumerator();
			while (true) {
				var firstHadNext = firstEnum.MoveNext();
				var secondHadNext = secondEnum.MoveNext();
				if (firstHadNext && secondHadNext) {
					yield return resultSelector(firstEnum.Current, secondEnum.Current);
				} else if (firstHadNext != secondHadNext) {
					throw new IndexOutOfRangeException("The argument enumerables must have the same length.");
				} else {
					yield break;
				}
			}
		}

		public static T Sum<T>(this IEnumerable<T> values) where T : SIBase<T>
		{
			var valueList = values.ToList();
			return valueList.Any() ? valueList.Aggregate((sum, current) => sum + current) : null;
		}

		public static T Average<T>(this IEnumerable<T> values) where T : SIBase<T>
		{
			var valueList = values.ToList();
			return valueList.Any() ? valueList.Aggregate((sum, current) => sum + current) / valueList.Count : null;
		}

		public static SI Sum(this IEnumerable<SI> values)
		{
			var valueList = values.ToList();
			return valueList.Any() ? valueList.Aggregate((sum, current) => sum + current) : null;
		}

		public static Func<bool> Once()
		{
			var once = 0;
			return () => once++ == 0;
		}

		/// <summary>
		/// Get the first two adjacent items where the predicate changes from true to false.
		/// If the predicate never gets true, the last 2 elements are returned.
		/// </summary>
		public static Tuple<T, T> GetSection<T>(this IEnumerable<T> self, Func<T, bool> skip, out int index,
			string message = null)
		{
			var list = self.ToList();
			var skipList = list.Select((arg1, i) => new { skip = skip(arg1) && i < list.Count - 1, i, value = arg1 });
			var p = skipList.SkipWhile(x => x.skip).First();
			index = Math.Max(p.i - 1, 0);

			if (!string.IsNullOrWhiteSpace(message)) {
				if (!skip(list[index]) || skip(list[index + 1])) {
					var Log = LogManager.GetLogger(typeof(T).ToString());
					Log.Error(message);
				}
			}

			return Tuple.Create(list[index], list[index + 1]);
		}

		/// <summary>
		/// Get the first two adjacent items where the predicate changes from true to false.
		/// If the predicate never gets true, the last 2 elements are returned.
		/// </summary>
		/// <example>GetSection(data => data.X &lt; searchedX); //returns the pair where first &lt; searchedX and second &gt;= searchedX</example>>
		public static Tuple<T, T> GetSection<T>(this IEnumerable<T> self, Func<T, bool> predicate, string message = null)
		{
			int unused;
			return self.GetSection(predicate, out unused, message);
		}

		public static IEnumerable<T> Slice<T>(this IEnumerable<T> numerable, int from = 0, int to = int.MaxValue)
		{
			var s = numerable.ToList();
			from = Math.Min(Math.Max(from, -s.Count), s.Count);
			from = from < 0 ? from + s.Count : from;
			to = Math.Min(Math.Max(to, -s.Count), s.Count);
			to = to < 0 ? to + s.Count : to;
			return s.Skip(from).Take(Math.Max(to - from, 0));
		}

		public static TSource MinBy<TSource>(this IEnumerable<TSource> source,
			Func<TSource, IComparable> projectionToComparable)
		{
			using (var e = source.GetEnumerator()) {
				if (!e.MoveNext()) {
					throw new InvalidOperationException("Sequence is empty.");
				}
				var min = e.Current;
				var minProjection = projectionToComparable(e.Current);
				while (e.MoveNext()) {
					var currentProjection = projectionToComparable(e.Current);
					if (currentProjection.CompareTo(minProjection) < 0) {
						min = e.Current;
						minProjection = currentProjection;
					}
				}
				return min;
			}
		}
	}
}