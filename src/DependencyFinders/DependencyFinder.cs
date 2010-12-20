using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinqToDesign {
	public class DependencyFinder {

		[Pure]
		public static IEnumerable<TypeReference> FindUses(ModuleDefinition user, TypeDefinition used) {
			Contract.Requires(used != null);
			Contract.Requires(user != null);

			return GetTypesUsedBy(user).Where(t => IsEquivalentOrInnerTypeOf(used, t));
		}

		[Pure]
		public static IEnumerable<TypeReference> FindUses(ModuleDefinition user, IEnumerable<TypeDefinition> used) {
			Contract.Requires(used != null);
			Contract.Requires(user != null);

			return used.SelectMany(t => FindUses(user, t));
		}



		[Pure]
		public static IEnumerable<TypeReference> FindUses(TypeDefinition user, TypeDefinition used) {
			Contract.Requires(used != null);
			Contract.Requires(user != null);

			return GetTypesUsedBy(user).Where(t => IsEquivalentOrInnerTypeOf(used, t));
		}

		[Pure]
		public static IEnumerable<TypeReference> FindUses(TypeDefinition user, IEnumerable<TypeDefinition> used) {
			Contract.Requires(used != null);
			Contract.Requires(user != null);

			return used.SelectMany(t => FindUses(user, t));
		}



		[Pure]
		public static IEnumerable<TypeReference> FindUses(MethodDefinition user, TypeReference used) {
			Contract.Requires(used != null);
			Contract.Requires(user != null);

			return GetTypesUsedBy(user).Where(t => IsEquivalentOrInnerTypeOf(used, t));
		}
		
		[Pure]
		public static IEnumerable<TypeReference> FindUses(MethodDefinition user, IEnumerable<TypeReference> used) {
			Contract.Requires(used != null);
			Contract.Requires(user != null);

			return used.SelectMany(t => FindUses(user, t));
		}



		[Pure]
		public static IEnumerable<TypeReference> FindUsesOfNameSpace(ModuleDefinition user, string namespaze) {
			Contract.Requires(namespaze != null);
			Contract.Requires(user != null);

			return GetTypesUsedBy(user).Where(u => u.Namespace.StartsWith(namespaze));
		}

		[Pure]
		public static IEnumerable<TypeReference> FindUsesOfNameSpaces(ModuleDefinition user, IEnumerable<string> namespaces) {
			Contract.Requires(namespaces != null);
			Contract.Requires(user != null);

			return namespaces.SelectMany(ns => FindUsesOfNameSpace(user, ns));
		}

		[Pure]
		public static IEnumerable<TypeReference> FindUsesOfNameSpace(TypeDefinition user, string namespaze) {
			Contract.Requires(namespaze != null);
			Contract.Requires(user != null);

			return GetTypesUsedBy(user).Where(u => u.Namespace.StartsWith(namespaze));
		}

		[Pure]
		public static IEnumerable<TypeReference> FindUsesOfNameSpaces(TypeDefinition user, IEnumerable<string> namespaces) {
			Contract.Requires(namespaces != null);
			Contract.Requires(user != null);

			return namespaces.SelectMany(ns => FindUsesOfNameSpace(user, ns));
		}


		[Pure]
		public static IEnumerable<TypeReference> GetTypesUsedBy(ModuleDefinition module) {
			Contract.Requires(module != null);

			return module.Types.OfType<TypeDefinition>().SelectMany(GetTypesUsedBy);
		}

	

		[Pure]
		public static IEnumerable<MethodReference> GetMethodsUsedBy(ModuleDefinition module) {
			Contract.Requires(module != null);
			Contract.Requires(module.Types != null);
			
			return module.Types.OfType<TypeDefinition>().SelectMany(GetMethodsUsedBy);
		}

		[Pure]
		public static IEnumerable<MethodReference> GetMethodsUsedBy(TypeDefinition type) {
			Contract.Requires(type != null);
			Contract.Requires(type.Methods != null);

			return type.Methods.OfType<MethodDefinition>().SelectMany(GetMethodsUsedBy);
		}

		[Pure]
		public static IEnumerable<MethodReference> GetMethodsUsedBy(MethodDefinition method) {
			Contract.Requires(method != null);

			var ops = new[] { "call", "newobj" };

			if (method.HasBody) {
				foreach (
					Instruction instruction in method.Body.Instructions.OfType<Instruction>().Where(i => ops.Contains(i.OpCode.Name))) {
					yield return ((MethodReference)instruction.Operand);
				}
			}
		}



		[Pure]
		public static TypeDefinition GetTypeDefinition(Type type) {
			Contract.Requires(type != null);
			Contract.Ensures(Contract.Result<TypeDefinition>() != null);

			return Types.In(type.Assembly).Single(t => t.FullName == type.FullName);
		}

		[Pure]
		public static TypeDefinition GetTypeDefinition(object o) {
			Contract.Requires(o != null);
			Contract.Ensures(Contract.Result<TypeDefinition>() != null);

			return Types.In(o.GetType().Assembly).Single(t => t.FullName == o.GetType().FullName);
		}
	}
}