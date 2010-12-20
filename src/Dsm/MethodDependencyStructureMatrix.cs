using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using QueryMyDesign.DependencyFinders;
using QueryMyDesign.Sugar;

namespace QueryMyDesign.Dsm {
	public class MethodDependencyStructureMatrix : AbstractDependencyMatrix<MethodDefinition, MethodReference> {
		private readonly MethodDefinition[] _definedMembers;
		private readonly Dependency<MethodReference>[] _dependencies;
		private readonly AbstractDependencyFinder<MethodDefinition, MethodReference> _dependencyFinder;

		public MethodDependencyStructureMatrix(IEnumerable<Assembly> assemblies) {
			_dependencyFinder = new MethodDependencyFinder();
			_definedMembers = Enumerable.ToArray<MethodDefinition>(assemblies.SelectMany(Methods.In));
			_dependencies = FindDependencies();
			FindDependencies();
		}

		public override MethodDefinition[] DefinedMembers {
			get { return _definedMembers; }
		}

		public override Dependency<MethodReference>[] Dependencies {
			get { return _dependencies; }
		}

		public override AbstractDependencyFinder<MethodDefinition, MethodReference> DependencyFinder {
			get { return _dependencyFinder; }
		}

		public override int NumberOfAbstractMembers(MethodDefinition definition) {
			return _dependencyFinder.GetUses(definition).OfType<MethodDefinition>().Count(m => m.HasBody);
		}

		public override int NumberOfMembers(MethodDefinition definition) {
			return _dependencyFinder.GetUses(definition).Count();
		}
	}
}