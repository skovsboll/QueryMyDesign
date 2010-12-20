using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using QueryMyDesign.DependencyFinders;
using QueryMyDesign.Sugar;

namespace QueryMyDesign.Dsm {

	public class TypeDependencyStructureMatrix : AbstractDependencyMatrix<TypeDefinition, TypeReference> {

		private readonly Dependency<TypeReference>[] _typeDependencies;
		private readonly TypeDefinition[] _definedTypes;
		private readonly IEnumerable<Assembly> _assemblies;
		private readonly AbstractDependencyFinder<TypeDefinition, TypeReference> _dependencyFinder;

		public TypeDependencyStructureMatrix(IEnumerable<Assembly> assemblies) {
			Contract.Requires(assemblies != null);
			Contract.Requires(assemblies.Any());

			_dependencyFinder = new TypeDependencyFinder();
			_assemblies = assemblies;

			_definedTypes = Enumerable.ToArray<TypeDefinition>(assemblies.SelectMany(Types.In));

			_typeDependencies = FindDependencies();
		}

		public override TypeDefinition[] DefinedMembers {
			get { return _definedTypes; }
		}

		public override Dependency<TypeReference>[] Dependencies {
			get { return _typeDependencies; }
		}

		public override AbstractDependencyFinder<TypeDefinition, TypeReference> DependencyFinder {
			get { return _dependencyFinder; }
		}


		public double GetInstability(Type type) {
			Contract.Requires(type != null);

			return GetInstability(type.GetTypeDefinition());
		}

		public double GetInstability<T>() {
			return GetInstability(typeof(T));
		}


		public double GetAbstractness(Type type) {
			Contract.Requires(type != null);

			return GetAbstractness(type.GetTypeDefinition());
		}

		public double GetAbstractness<T>() {
			return GetAbstractness(typeof(T));
		}


		public override int NumberOfMembers(TypeDefinition definition) {
			return definition.Methods.Count;
		}

		public override int NumberOfAbstractMembers(TypeDefinition definition) {
			return definition.Methods.OfType<MethodDefinition>().Count(m => !m.HasBody);
		}


		public double GetDistanceFromMainSequence(Type type) {
			Contract.Requires(type != null);

			return GetDistanceFromMainSequence(type.GetTypeDefinition());
		}

		public double GetDistanceFromMainSequence<T>() {
			return GetDistanceFromMainSequence(typeof(T));
		}

		public double GetAmountOfPain<T>() {
			return GetAmountOfPain(typeof(T));
		}

		private double GetAmountOfPain(Type type) {
			return GetAmountOfPain(type.GetTypeDefinition());
		}

		public double GetUselesness<T>() {
			return GetUselesness(typeof(T));
		}

		private double GetUselesness(Type type) {
			return GetUselesness(type.GetTypeDefinition());
		}
	}
}