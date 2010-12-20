namespace SkinnyDip.Tests.DummyTypes {
	public class DependencyChain {
		public DependencyChain(X x) {
			
		}
	}

	public class X {
		public X(Y y) {
			
		}

	}

	public class Y {

		public Z Z { get; set; }

		internal Deroute deroute;
	}

	public class Deroute {
		

		void Hej() {
			var z = new Z();

		}
	}

	public class Z {
		
	}
}