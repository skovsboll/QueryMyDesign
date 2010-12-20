using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Cci;

namespace SkinnyDip.Tests.Obsolete {

	public static class CciReflectionExtensions {

		public static int NumberOfInstructions(this IMethodDefinition method) {
			return method.Body.Operations.Count();
		}

		public static int NumberOfVariables(this IMethodDefinition method) {
			return method.Body.LocalVariables.Count();
		}

		public static int CyclomaticComplexity(this IMethodDefinition method) {
			var plusOneOpcodes = new[] { "Br", "Beq", "Bge", "Bgt", "Ble", "Blt" };
			var count = method.Body.Operations.Count(o => plusOneOpcodes.Any(code => o.OperationCode.ToString().StartsWith(code)));
			return count + 1;
		}

		public static int CountUsesOf(this INamedTypeDefinition user, INamedTypeDefinition used) {
			int fields = user.Fields.Count(f => f.Type == used);
			int props = user.Properties.Count(f => f.Type == used);
			int returningMethods = user.Methods.Count(f => f.Type == used);
			int methodsUsingVariables = user.Methods.Sum(m => m.Body.LocalVariables.Count(v => v.Type == used));
			var ops = new[] { OperationCode.Call, OperationCode.Calli, OperationCode.Callvirt, OperationCode.Newobj };
			int methodsCallingMethods = user.Methods.Sum(m => m.Body.Operations.Count(o => ops.Contains(o.OperationCode) && ((IMethodReference)o.Value).ContainingType == used));

			return fields + props + returningMethods + methodsUsingVariables + methodsCallingMethods;
		}

		public static int CountUsesOf(this INamedTypeDefinition user, IEnumerable<INamedTypeDefinition> used) {
			return used.Sum(u => user.CountUsesOf(u));
		}

		public static INamedTypeDefinition GetNamedType(this object o) {
			Contract.Requires(o != null);
			Contract.Ensures(Contract.Result<INamedTypeDefinition>() != null);

			return All.TypesIn(o.GetType().Assembly).Single(t => t.ToString() == o.GetType().FullName);
		}
	}

	public static class All {

		private static readonly PeReader.DefaultHost DefaultHost = new PeReader.DefaultHost();
		static All() {
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.GlobalAssemblyCache)) {
				var location = new Uri(assembly.CodeBase);
				var path = location.AbsolutePath;

				Debug.WriteLine("Loading " + path);
				if (File.Exists(path))
					DefaultHost.LoadUnitFrom(path);
			}
		}


		public static IEnumerable<INamedTypeDefinition> Types {
			get {
				return DefaultHost.LoadedUnits.OfType<IModule>().SelectMany(m => m.GetAllTypes());
			}
		}

		public static IEnumerable<IMethodDefinition> Methods {
			get { return Types.SelectMany(t => t.Methods); }
		}

		public static IEnumerable<INamedTypeDefinition> TypesIn(Assembly assembly) {
			Contract.Requires(assembly != null);
			Contract.Requires(assembly.GlobalAssemblyCache == false);
			Contract.Ensures(Contract.Result<IEnumerable<INamedTypeDefinition>>().Any());

			var location = new Uri(assembly.CodeBase);
			var path = location.AbsolutePath;
			var unit = DefaultHost.LoadUnitFrom(path);

			return ((IModule)unit).GetAllTypes();
		}

		public static IEnumerable<IMethodDefinition> MethodsIn(Assembly assembly) {
			Contract.Requires(assembly != null);
			Contract.Ensures(Contract.Result<IEnumerable<IMethodDefinition>>().Any());

			return TypesIn(assembly).SelectMany(t => t.Methods);
		}

		public static IEnumerable<string> NamespacesIn(Assembly assembly) {
			return All.TypesIn(assembly).Select(t => t.Name.ToString().Substring(0, t.Name.ToString().LastIndexOf(".")));

		}
	}
}