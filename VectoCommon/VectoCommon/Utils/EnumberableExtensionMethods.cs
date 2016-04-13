/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCommon.Utils
{
	public static class EnumberableExtensionMethods
	{
		public static IEnumerable<double> ToDouble(this IEnumerable<string> self)
		{
			return self.Select(StringExtensionMethods.ToDouble);
		}

		public static bool SequenceEqualFast<T>(this IEnumerable<T> self, IEnumerable<T> other) where T : IComparable
		{
			return self.ToArray().SequenceEqualFast(other.ToArray());
		}

		public static bool SequenceEqualFast<T>(this T[] self, T[] other) where T : IComparable
		{
			if (self.Equals(other)) {
				return true;
			}

			if (self.Length != other.Length) {
				return false;
			}

			for (var i = 0; i < self.Length; i++) {
				if (self[i].CompareTo(other[i]) != 0) {
					return self.OrderBy(x => x).SequenceEqual(other.OrderBy(x => x));
				}
			}
			return true;
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

		/// <summary>
		/// Zips all elements of two enumerable together. If the enumerables dont have the same length an exception is thrown.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Enumeration already finished. Thrown if the enumerables dont have the same length.</exception>
		public static IEnumerable<TResult> ZipAll<TFirst, TSecond, TResult>(this IEnumerable<TFirst> firstEnumerable,
			IEnumerable<TSecond> secondEnumerable, Func<TFirst, TSecond, TResult> resultSelector)
		{
			using (var first = firstEnumerable.GetEnumerator()) {
				using (var second = secondEnumerable.GetEnumerator()) {
					while (first.MoveNext() | second.MoveNext()) {
						yield return resultSelector(first.Current, second.Current);
					}
				}
			}
		}

		/// <summary>
		/// Sums up the values of selector.
		/// </summary>
		/// <typeparam name="TU"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="values"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public static TResult Sum<TU, TResult>(this IEnumerable<TU> values, Func<TU, TResult> selector)
			where TResult : SIBase<TResult>
		{
			return values.Select(selector).DefaultIfEmpty().Aggregate((sum, current) => sum + current);
		}

		public static T Average<T>(this IEnumerable<T> values) where T : SIBase<T>
		{
			var valueList = values.ToList();
			return valueList.Any() ? valueList.Aggregate((sum, current) => sum + current) / valueList.Count : null;
		}

		public static SI Sum(this IEnumerable<SI> values)
		{
			return values.DefaultIfEmpty().Aggregate((sum, current) => sum + current);
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
					LogManager.GetLogger(typeof(T).ToString()).Warn(message);
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