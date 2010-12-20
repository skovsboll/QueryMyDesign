namespace SkinnyDip.Tests.DummyTypes {

	public class PainfulClient {
		PainfulType p = new PainfulType(89);

		public PainfulClient() {
			p.DoStuff(23);
		}

		public void Mjallo() {
			p.DoStuff(909);
		}
	}

	public class PainfulType {
		private int _i;

		public PainfulType(int i) {
			_i = i;
		}

		public void DoStuff(int j) {
			_i = j;
		}
	}
}