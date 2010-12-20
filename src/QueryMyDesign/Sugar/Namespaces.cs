using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using QueryMyDesign.DependencyFinders;

namespace QueryMyDesign.Sugar {
	public static class Namespaces {

		private static readonly AbstractDependencyFinder<TypeDefinition, TypeReference> DependencyFinder = new TypeDependencyFinder();

		public static IEnumerable<string> InAssemblyOf<T>() {
			return Namespaces.InAssemblyOf(typeof(T));
		}

		public static IEnumerable<string> InAssemblyOf(Type type) {
			Contract.Requires(type != null);

			return In(type.Assembly);
		}

		public static IEnumerable<string> In(Assembly assembly) {
			Contract.Requires(assembly != null);

			return Types.In(assembly).Select(t => t.Namespace);
		}

		public static IEnumerable<string> NamespacesIn(Assembly assembly) {
			Contract.Requires(assembly != null);
			Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

			return Types.In(assembly).Select(t => t.Namespace);
		}

		public static IEnumerable<string> UsedBy<T>() {
			return Namespaces.UsedBy(typeof(T));
		}

		public static IEnumerable<string> UsedBy(Type type) {
			Contract.Requires(type != null);

			return DependencyFinder.GetUses(type.GetTypeDefinition()).Select(t => t.Namespace);
		}
	}
}