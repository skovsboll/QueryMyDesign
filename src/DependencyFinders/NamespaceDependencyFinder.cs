using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace QueryMyDesign.DependencyFinders {
	public class NamespaceDependencyFinder : AbstractDependencyFinder<string, string> {
		private readonly AssemblyDefinition[] _assemblies;

		public NamespaceDependencyFinder(AssemblyDefinition[] assemblies) {
			_assemblies = assemblies;
		}

		public override IEnumerable<string> GetUses(string definition) {
			return
				from a in _assemblies
				from m in a.Modules.OfType<ModuleDefinition>()
				from t in m.Types.OfType<TypeDefinition>()
				select t.Namespace;
		}

		public override bool AreEquivalent(string typeA, string typeB) {
			return typeA == typeB;
		}
	}
}