using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace QueryMyDesign.DependencyFinders {
	public class MethodDependencyFinder : AbstractDependencyFinder<MethodDefinition, MethodReference> {
		public override IEnumerable<MethodReference> GetUses(MethodDefinition method) {
			Contract.Requires(method != null);

			var ops = new[] { "call", "newobj" };

			if (method.HasBody) {
				foreach (
					Instruction instruction in method.Body.Instructions.OfType<Instruction>().Where(i => ops.Contains(i.OpCode.Name))) {
					yield return ((MethodReference)instruction.Operand);
				}
			}
		}

		public override bool AreEquivalent(MethodReference typeA, MethodReference typeB) {
			return typeA.Equals(typeB);			
		}
	}
}