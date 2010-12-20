using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using QueryMyDesign.DependencyFinders;
using QueryMyDesign.Sugar;

namespace QueryMyDesign.Dsm {
	public class AssemblyDependencyStructureMatrix : AbstractDependencyMatrix<AssemblyDefinition, AssemblyDefinition> {
		private readonly AssemblyDependencyFinder _dependencyFinder;
		private readonly AssemblyDefinition[] _definedTypes;
		private readonly Dependency<AssemblyDefinition>[] _dependencies;

		public AssemblyDependencyStructureMatrix(IEnumerable<Assembly> assemblies) {
			Contract.Requires(assemblies != null);
			Contract.Requires(assemblies.Any());

			_dependencyFinder = new AssemblyDependencyFinder(new ModuleDependencyFinder(new TypeDependencyFinder()));

			_definedTypes = assemblies.Select(a => a.GetAssemblyDefinition()).ToArray<AssemblyDefinition>();

			_dependencies = FindDependencies();
		}

		public override AssemblyDefinition[] DefinedMembers {
			get { return _definedTypes; }
		}

		public override Dependency<AssemblyDefinition>[] Dependencies {
			get { return _dependencies; }
		}

		public override AbstractDependencyFinder<AssemblyDefinition, AssemblyDefinition> DependencyFinder {
			get { return _dependencyFinder; }
		}

		public override int NumberOfAbstractMembers(AssemblyDefinition definition) {
			return definition.Modules.OfType<ModuleDefinition>().Sum(m => m.Types.OfType<TypeDefinition>().Count(t => t.IsAbstract));
		}

		public override int NumberOfMembers(AssemblyDefinition definition) {
			return definition.Modules.OfType<ModuleDefinition>().Sum(m => m.Types.OfType<TypeDefinition>().Count());			
		}
	}
}