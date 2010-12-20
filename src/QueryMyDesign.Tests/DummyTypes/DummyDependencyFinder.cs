using System;
using System.Collections.Generic;
using System.Linq;
using C;
using Mono.Cecil;
using QueryMyDesign.DependencyFinders;
using QueryMyDesign.Sugar;

namespace SkinnyDip.Tests.DummyTypes {
	public class DummyDependencyFinder : AbstractDependencyFinder<TypeDefinition, TypeReference> {

		public override IEnumerable<TypeReference> GetUses(TypeDefinition definition) {
			return FlattenGenericTypeChain(typeof(C.D).GetTypeDefinition().Events.OfType<EventDefinition>().First().EventType);
		}

		public override bool AreEquivalent(TypeReference typeA, TypeReference typeB) {
			return typeA == typeB;
		}
	}
}