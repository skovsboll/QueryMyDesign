using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SkinnyDip {
	public static class EnumerableExtensions {
		public static void AddRange<T>(this ICollection<T> list,  IEnumerable<T> items) {
			Contract.Requires(list != null);
			Contract.Requires(items != null);
			foreach (var t in items)
				list.Add(t);
		}

		public static IEnumerable<Tuple<T1, T2>> Zip<T1, T2>(this IEnumerable<T1> t1s, IEnumerable<T2> t2s) {
			var enumerator1 = t1s.GetEnumerator();
			var enumerator2 = t2s.GetEnumerator();

			while (enumerator1.MoveNext() && enumerator2.MoveNext())
				yield return new Tuple<T1, T2>(enumerator1.Current, enumerator2.Current);
		}

		public static void Each<T>(this IEnumerable<T> collection, Action<T> action) {
			foreach(var t in collection) {
				action(t);
			}
		}
	}
}