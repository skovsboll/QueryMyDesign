using System.Diagnostics.Contracts;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace QueryMyDesign.Dsm {
	public static class NumericalAssemblyAnalyzer {
		public static int CountNumberOfInstructions(MethodDefinition method) {
			Contract.Requires(method != null);
			Contract.Requires(method.Body != null);
			Contract.Requires(method.Body.Instructions != null);

			return method.Body.Instructions.Count;
		}

		public static int CountNumberOfVariables(MethodDefinition method) {
			Contract.Requires(method != null);
			Contract.Requires(method.Body != null);
			Contract.Requires(method.Body.Variables != null);

			return method.Body.Variables.Count;
		}

		public static int CalcCyclomaticComplexity(MethodDefinition method) {
			Contract.Requires(method != null);
			Contract.Requires(method.Body != null);
			Contract.Requires(method.Body.Instructions != null);

			var plusOneOpcodes = new[] { "br", "be", "bg", "bl" };

			int count =
				method.Body.Instructions.OfType<Instruction>().Count(
					o => plusOneOpcodes.Contains(o.OpCode.ToString().Substring(0, 2)));

			return count + 1;
		}
	}
}