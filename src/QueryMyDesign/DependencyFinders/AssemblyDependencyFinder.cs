using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace QueryMyDesign.DependencyFinders {
	public class AssemblyDependencyFinder : AbstractDependencyFinder<AssemblyDefinition, AssemblyDefinition> {
		
		private readonly AbstractDependencyFinder<ModuleDefinition, ModuleReference> _moduleDependencyFinder;

		public AssemblyDependencyFinder(AbstractDependencyFinder<ModuleDefinition, ModuleReference> moduleDependencyFinder) {
			_moduleDependencyFinder = moduleDependencyFinder;
		}

		public override IEnumerable<AssemblyDefinition> GetUses(AssemblyDefinition definition) {
			return
				from m in definition.Modules.OfType<ModuleDefinition>()
				from t in _moduleDependencyFinder.GetUses(m).OfType<ModuleDefinition>()
				select t.Assembly;
		}

		public override bool AreEquivalent(AssemblyDefinition typeA, AssemblyDefinition typeB) {
			return typeA.Name == typeB.Name;
		}
	}
}