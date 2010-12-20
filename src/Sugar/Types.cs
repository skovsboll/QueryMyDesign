using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using QueryMyDesign.DependencyFinders;

namespace QueryMyDesign.Sugar {
	public static class Types {
		private static readonly AbstractDependencyFinder<TypeDefinition, TypeReference> TypeDependencyFinder =
			new TypeDependencyFinder();

		public static IEnumerable<TypeDefinition> InAssemblyOf<T>() {
			return Types.InAssemblyOf(typeof(T));
		}

		public static IEnumerable<TypeDefinition> InAssemblyOf(Type type) {
			Contract.Requires(type != null);

			return Types.In(type.Assembly);
		}

		public static IEnumerable<TypeDefinition> In(Assembly assembly) {
			Contract.Requires(assembly != null);

			Contract.Requires(assembly != null);
			Contract.Ensures(Contract.Result<IEnumerable<TypeDefinition>>() != null);
			Contract.Ensures(Contract.Result<IEnumerable<TypeDefinition>>().All(t => t != null));

			var typeDefinitionCollection = AssemblyFactory.GetAssembly(new Uri(assembly.Location).AbsolutePath).MainModule.Types;

			Contract.Assert(typeDefinitionCollection != null);

			return typeDefinitionCollection.OfType<TypeDefinition>().Where(t => t != null);
		}

		public static IEnumerable<TypeDefinition> In(IEnumerable<Assembly> assemblies) {
			Contract.Requires(assemblies != null);
			Contract.Ensures(Contract.Result<IEnumerable<TypeDefinition>>() != null);
			
			return
				assemblies.SelectMany(
					a => GetAssemblyDefinition(a)
						.MainModule.Types.OfType<TypeDefinition>());
		}

		public static AssemblyDefinition GetAssemblyDefinition(this Assembly a) {
			return AssemblyFactory.GetAssembly(
				new Uri(a.Location).AbsolutePath);
		}

		public static IEnumerable<TypeReference> UsedBy<T>() {
			return Types.UsedBy(typeof(T));
		}

		public static IEnumerable<TypeReference> UsedBy(Type type) {
			Contract.Requires(type != null);

			return TypeDependencyFinder.GetUses(type.GetTypeDefinition());
		}

		public static IEnumerable<TypeReference> UsedBy(TypeDefinition type) {
			Contract.Requires(type != null);

			return TypeDependencyFinder.GetUses(type);
		}
	}
}