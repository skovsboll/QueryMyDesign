using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace QueryMyDesign.DependencyFinders {
	public class TypeDependencyFinder : AbstractDependencyFinder<TypeDefinition, TypeReference> {
		public override IEnumerable<TypeReference> GetUses(TypeDefinition type) {
			Contract.Requires(type != null);

			IList<TypeReference> bases = new List<TypeReference>();

			TypeReference baze = type.BaseType;
			while (baze != null) {
				bases.Add(baze);
				var baseDefinition = baze as TypeDefinition;
				baze = baseDefinition != null ? baseDefinition.BaseType : null;
			}

			IEnumerable<TypeReference> interfaces = type.Interfaces.Cast<TypeReference>();

			IEnumerable<TypeReference> attributeTypes =
				type.CustomAttributes.Cast<CustomAttribute>().Select(a => a.Constructor.DeclaringType);

			IEnumerable<TypeReference> constructorParameters =
				type.Constructors.Cast<MethodDefinition>().SelectMany(
					c => c.Parameters.Cast<ParameterDefinition>().Select(p => p.ParameterType));

			IEnumerable<TypeReference> fields = type.Fields.Cast<FieldDefinition>().Select(f => (f.FieldType));

			IEnumerable<TypeReference> properties =
				type.Properties.Cast<PropertyDefinition>().Select(p => (p.PropertyType));

			IEnumerable<TypeReference> events = type.Events.Cast<EventDefinition>().Select(p => (p.EventType));

			var methodsAndConstructors = type.Constructors.Cast<MethodDefinition>().Union(type.Methods.Cast<MethodDefinition>());
			IEnumerable<TypeReference> methods = methodsAndConstructors.SelectMany(GetUses);

			IEnumerable<TypeReference> nestedTypes =
				type.NestedTypes.Cast<TypeDefinition>().SelectMany(GetUses);

			return
				FlattenGenericTypeChain(
					interfaces.Union(attributeTypes).Union(bases)
						.Union(fields).Union(properties).Union(events)
						.Union(methods).Union(nestedTypes).Union(constructorParameters));
		}

		protected IEnumerable<TypeReference> GetUses(MethodDefinition method) {
			Contract.Requires(method != null);

			string[] ops = new[] { "call", "newobj" };

			IEnumerable<TypeReference> dependenciesInMethodBody = new TypeReference[0];

			if (method.HasBody) {
				dependenciesInMethodBody =
					method.Body.Variables.OfType<VariableDefinition>().Select(v => v.VariableType)
						.Union(
						method.Body.Instructions.OfType<Instruction>().Where(i => ops.Contains(i.OpCode.Name)).Select(i => ((MethodReference)i.Operand).DeclaringType)
						);
			}

			var returnType = new[] { method.ReturnType.ReturnType };
			IEnumerable<TypeReference> parameterTypes = method.Parameters.OfType<ParameterDefinition>().Select(p => p.ParameterType);
			IEnumerable<TypeReference> attributes = method.CustomAttributes.OfType<CustomAttribute>().Select(a => a.Constructor.DeclaringType);
			
			return
					returnType
				.Union(
					parameterTypes
				).Union(
					dependenciesInMethodBody
				).Union(
					attributes
				);
		}

		public override bool AreEquivalent(TypeReference typeA, TypeReference typeB) {
			if (typeA.FullName != typeB.FullName) {
				return false;
			}

			var moduleA = typeA.Scope as ModuleDefinition;
			var assemblyA = typeA.Scope as AssemblyNameReference;
			var moduleB = typeB.Scope as ModuleDefinition;
			var assemblyB = typeB.Scope as AssemblyNameReference;

			string scopeNameA = moduleA != null ? moduleA.Assembly.Name.FullName : assemblyA.FullName;
			string scopeNameB = moduleB != null ? moduleB.Assembly.Name.FullName : assemblyB.FullName;

			return scopeNameA == scopeNameB;
		}
	}
}