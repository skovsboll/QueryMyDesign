using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace QueryMyDesign.DependencyFinders {
	public abstract class AbstractDependencyFinder<TDef, TRef> where TDef : class, TRef where TRef : class {
		public abstract IEnumerable<TRef> GetUses(TDef definition);

		public virtual IEnumerable<TRef> GetUsesOf(TDef definition, TRef target) {
			return from use in GetUses(definition)
			       where AreEquivalent(use, target)
			       select use;
		}

		public abstract bool AreEquivalent(TRef typeA, TRef typeB);

		protected IEnumerable<TypeReference> FlattenGenericTypeChain(IEnumerable<TypeReference> types) {
			return types.SelectMany(FlattenGenericTypeChain);
		}

		protected IEnumerable<TypeReference> FlattenGenericTypeChain(TypeReference type) {

			yield return type;

			var genericInstanceType = type as GenericInstanceType;

			if(genericInstanceType != null && genericInstanceType.GenericArguments != null) {
				foreach(TypeReference genericArgument in genericInstanceType.GenericArguments.OfType<TypeReference>()) {
					foreach(TypeReference innerType in FlattenGenericTypeChain(genericArgument)) {
						yield return innerType;
					}
				}
			}
		}
	}
}