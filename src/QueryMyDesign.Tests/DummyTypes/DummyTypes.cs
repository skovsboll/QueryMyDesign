using System;
using System.Collections.Generic;
using B;

namespace A {

	public class TypeWithHighCyclomaticComplexity {
		private void MethodWithHighCyclomaticComplexity(bool x, bool y, bool z) {
			bool hellFreezesOver = true;

			if (x ^ y) {
				while (y != z) {
					x = !y;
					foreach (var i in new[] { 1, 2, 3, 4, 5, 6 }) {
						var j = i * 2;
					}
					if (z && x) {
						switch (x) {
							case true:
								return;
							case false:
								if (hellFreezesOver)
									z = y ? y : x;
								break;
							default:
								if (x || y) {
									while (y != z) {
										x = !y;
										foreach (var i in new[] { 1, 2, 3, 4, 5, 6 }) {
											var j = i * 2;
										}
										if (z || y) {
											switch (x) {
												case true:
													return;
												case false:
													if (hellFreezesOver)
														z = y ? y : x;
													break;
											}
										}
										else {
											x = !z;
										}
									}
								}
								break;
						}
					}
					else {
						x = !z;
						if (x || y) {
							while (y != z) {
								x = !y;
								foreach (var i in new[] { 1, 2, 3, 4, 5, 6 }) {
									var j = i * 2;
								}
								if (z || y) {
									switch (x) {
										case true:
											return;
										case false:
											if (hellFreezesOver)
												z = y ? y : x;
											break;
									}
								}
								else {
									x = !z;
								}
							}
						}

					}
				}
			}
		}

	}

	public class A1 {

		private void DoStuff() {
			var b1 = new B1();
		}
	}

}

namespace B {
	public class B1 {


	}
}

namespace C {
	public class D {

		public D() {
			var e = new E(); //1
			e.DoStuff(); // 2

		}

		public E TheE; // 3
		protected E TheOtherE; // 4
		private E theThirdE { //5
			get;
			set;
		}

		[E] // 6
		public void UsesE() {
			var collection = new List<E>(); //7
			E.DoStaticStuff(); //8

			var invoker = SomethingHappened;
			if (invoker != null) invoker(this, new EventArgs<E>()); // 9
		}

		public event EventHandler<EventArgs<E>> SomethingHappened; // 10

	}

	public class E : Attribute {
		public void DoStuff() { }

		public static void DoStaticStuff() { }
	}

	public class EventArgs<T> : EventArgs { }

	public abstract class F {
		private G g;

		protected virtual void Hejsa() {

		}

		public abstract int GetIt();
	}

	public interface G {

		void DoStuff();
	}

	public class DontUseMe { }
}
