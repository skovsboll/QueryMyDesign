namespace SkinnyDip.Tests.DummyTypes {
	public class DependencyCycle {
		

		void Djallo() {
			AA aa;

		}
	}

	public class AA {
		void Mjallo(BB bb) {
			
		}

	}


	public class  BB {
		public readonly CC CC;
		public BB(CC cc) {
			CC = cc;
		}
	}

	public class CC {

		public CC() {
			new DependencyCycle();
		}
	}
}