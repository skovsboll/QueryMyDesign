using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using QueryMyDesign.Sugar;
using SkinnyDip.Tests.DummyTypes;
using Xunit;

namespace SkinnyDip.Tests {
	public class AbstractDependencyFinderTests {
		[Fact]
		public void AbstractDependencyFinderCanFlattenGenericTypeChain() {
			// Arrange
			var sut = new DummyDependencyFinder();

			// Act
			IEnumerable<TypeReference> uses = sut.GetUses(typeof(AbstractDependencyFinderTests).GetTypeDefinition());

			// Assert
			Assert.Equal(3, uses.Count());
		}
	}
}