using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using QueryMyDesign.DependencyFinders;

namespace QueryMyDesign.Sugar {
	public static class Methods {

		private static readonly AbstractDependencyFinder<MethodDefinition, MethodReference> DependencyFinder =
			new MethodDependencyFinder();

		public static IEnumerable<MethodDefinition> In<T>() {
			return Methods.In(typeof(T));
		}

		public static IEnumerable<MethodDefinition> In(Type type) {
			Contract.Requires(type != null);

			return type.GetTypeDefinition().Methods.OfType<MethodDefinition>();
		}

		public static IEnumerable<MethodReference> CalledBy<T>() {
			return Methods.CalledBy(typeof(T));
		}

		public static IEnumerable<MethodReference> CalledBy(Type type) {
			Contract.Requires(type != null);

			return
				from m in type.GetTypeDefinition().Methods.OfType<MethodDefinition>()
				from use in DependencyFinder.GetUses(m)
				select use;
		}

		public static IEnumerable<MethodDefinition> InAssemblyOf<T>() {
			return Methods.InAssemblyOf(typeof(T));
		}

		public static IEnumerable<MethodDefinition> InAssemblyOf(Type type) {
			Contract.Requires(type != null);

			return Methods.In(type.Assembly);
		}

		public static IEnumerable<MethodDefinition> In(Assembly assembly) {

			Contract.Requires(assembly != null);
			Contract.Ensures(Contract.Result<IEnumerable<MethodDefinition>>() != null);

			return Types.In(assembly).SelectMany(t => t.Methods.OfType<MethodDefinition>().Where(m => m.HasBody));
		}
	}
}