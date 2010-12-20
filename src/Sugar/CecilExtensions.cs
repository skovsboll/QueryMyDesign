using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Mono.Cecil;
using QueryMyDesign.DependencyFinders;
using QueryMyDesign.Dsm;

namespace QueryMyDesign.Sugar {
	public static class CecilExtensions {
		private static readonly TypeDependencyFinder TypeDependencyFinder = new TypeDependencyFinder();

		[Pure]
		public static int NumberOfInstructions(this MethodDefinition method) {
			Contract.Requires(method != null);

			return NumericalAssemblyAnalyzer.CountNumberOfInstructions(method);
		}

		[Pure]
		public static int NumberOfVariables(this MethodDefinition method) {
			Contract.Requires(method != null);

			return NumericalAssemblyAnalyzer.CountNumberOfVariables(method);
		}

		[Pure]
		public static int CyclomaticComplexity(this MethodDefinition method) {
			Contract.Requires(method != null);

			return NumericalAssemblyAnalyzer.CalcCyclomaticComplexity(method);
		}

		[Pure]
		public static IEnumerable<TypeReference> FindUsesOf(this TypeDefinition user, TypeDefinition used) {
			return TypeDependencyFinder.GetUsesOf(user, used);
		}

		public static bool HasCyclicDependency(this TypeDefinition type) {
			return FindDepthsOfUseOf(type, type).Any(depth => depth > 1);
		}

		public static IEnumerable<int> FindDepthsOfUseOf(this TypeDefinition user, TypeDefinition used) {
			return FindGraphsOfUseOf(user, used, new List<TypeReference>()).Select(depths => depths.Count());
		}

		public static IEnumerable<IEnumerable<TypeReference>> FindGraphsOfUseOf(this TypeDefinition user, TypeReference used, ICollection<TypeReference> visited) {
			foreach (TypeDefinition use in TypeDependencyFinder.GetUses(user).OfType<TypeDefinition>()) {

				if (TypeDependencyFinder.AreEquivalent(use, used))
					yield return visited;

				if (!visited.Contains(use)) {
					visited.Add(use);
					foreach (var subGraph in FindGraphsOfUseOf(use, used, visited)) {
						yield return subGraph;
					};
				}
			}
		}

		[Pure]
		public static IEnumerable<TypeReference> FindUsesOf(this TypeDefinition user, IEnumerable<TypeDefinition> used) {
			return
				from u1 in used
				from u2 in TypeDependencyFinder.GetUsesOf(user, u1)
				select u2;
		}

		[Pure]
		public static int CountUsesOfNamespace(this TypeDefinition user, string namespaze) {
			return TypeDependencyFinder.GetUses(user).Count(u => u.Namespace == namespaze);
		}

		[Pure]
		public static TypeDefinition GetTypeDefinition(this Type type) {
			Contract.Requires(type != null);
			Contract.Ensures(Contract.Result<TypeDefinition>() != null);

			return Types.In(type.Assembly).Single(t => t.FullName == type.FullName);
		}

		[Pure]
		public static int CyclomaticComplexity(this TypeDefinition type) {
			return
				type.Methods.OfType<MethodDefinition>()
					.Where(m => m.HasBody)
					.Sum(
						m => NumericalAssemblyAnalyzer.CalcCyclomaticComplexity(m));
		}
	}
}