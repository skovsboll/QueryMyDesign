using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Mono.Cecil;

namespace QueryMyDesign.DependencyFinders {
	public class ModuleDependencyFinder : AbstractDependencyFinder<ModuleDefinition, ModuleReference> {
		private readonly AbstractDependencyFinder<TypeDefinition, TypeReference> _typeDependencyFinder;

		public ModuleDependencyFinder(AbstractDependencyFinder<TypeDefinition, TypeReference> typeDependencyFinder) {
			_typeDependencyFinder = typeDependencyFinder;
		}

		public override IEnumerable<ModuleReference> GetUses(ModuleDefinition definition) {
			Contract.Requires(definition != null);
			return
				from t in definition.Types.OfType<TypeDefinition>()
				from use in _typeDependencyFinder.GetUses(t)
				select t.Module;
		}

		public override bool AreEquivalent(ModuleReference typeA, ModuleReference typeB) {
			Contract.Requires(typeA != null);
			Contract.Requires(typeB != null);
			return typeA.Name == typeB.Name;
		}
	}
}