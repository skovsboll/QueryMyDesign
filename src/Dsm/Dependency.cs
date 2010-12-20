using System.Diagnostics.Contracts;

namespace QueryMyDesign.Dsm {

	public class Dependency<T> {
		public readonly T From;
		public readonly T To;
		public readonly int Count;

		public Dependency(T from, T to, int count) {
			Contract.Requires(from != null);
			Contract.Requires(to != null);

			Count = count;
			From = from;
			To = to;
		}
	}
}