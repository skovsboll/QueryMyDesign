using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using B;
using C;
using Mono.Cecil;
using QueryMyDesign.Dsm;
using QueryMyDesign.Sugar;
using SkinnyDip.Tests.DummyTypes;
using Xunit;

namespace SkinnyDip.Tests {

	public class CecilAnalyzerTests {
		protected readonly Assembly Sut;

		public CecilAnalyzerTests() {
			Sut = typeof(CecilAnalyzerTests).Assembly;
		}

		[Fact(Timeout = 5000)]
		public void CatchesMethodsWithHighCyclomaticComplexity() {
			var methodsWithHighComplexity =
				(from m in Methods.InAssemblyOf<CecilAnalyzerTests>()
				 where m != null && m.DeclaringType.Namespace == "A"
				 let c = m.CyclomaticComplexity()
				 where c > 40
				 select new { m, c }).ToArray();

			foreach (var methodDefinition in methodsWithHighComplexity) {
				if (methodDefinition.m.DeclaringType != null) {
					Debug.WriteLine(methodDefinition.m.DeclaringType.Name + "." + methodDefinition.m.Name + ": " + methodDefinition.c);
				}
			}

			Assert.Equal(1, methodsWithHighComplexity.Length);
		}

		[Fact]
		public void CatchesUsesOfOtherNamespace() {
			var dependenciesFromAtoB = from t in Types.In(Sut) where t.Namespace.StartsWith("A") && t.CountUsesOfNamespace("B") > 0 select t;
			Assert.True(dependenciesFromAtoB.Any());

			foreach (var dependency in dependenciesFromAtoB)
				Debug.WriteLine(dependency.FullName + " uses B");
		}

		[Fact]
		public void CanCountUsesOfOneTypeToAnother() {
			
			TypeDefinition typeD = typeof(D).GetTypeDefinition();
			TypeDefinition typeE = typeof(E).GetTypeDefinition();

			IEnumerable<TypeReference> dependenciesFromDtoE = typeD.FindUsesOf(typeE).ToArray();

			Assert.Equal(15, dependenciesFromDtoE.Count());
		}

		[Fact]
		public void CanFindDepthOfUse() {
			TypeDefinition fromType = typeof(DependencyChain).GetTypeDefinition();
			TypeDefinition toType = typeof(Z).GetTypeDefinition();

			IEnumerable<int> depthsOfUse = fromType.FindDepthsOfUseOf(toType).ToArray();
			int minDepthOfUse = depthsOfUse.Min();
			int maxDepthOfUse = depthsOfUse.Max();

			Assert.Equal(3, minDepthOfUse);
			Assert.Equal(4, maxDepthOfUse);
		}


		[Fact]
		public void CanFindDependencyCycles() {
			TypeDefinition fromType = typeof(DependencyCycle).GetTypeDefinition();

			var visited = new List<TypeReference>();
			foreach (var subGraph in fromType.FindGraphsOfUseOf(fromType, visited)) {
				Console.WriteLine("Found!");
				foreach(var typeReference in subGraph)
					Console.WriteLine(typeReference);
				Console.WriteLine(Environment.NewLine);
			}

			Assert.True(fromType.HasCyclicDependency());
		}

		[Fact]
		public void SyntacticSugarCanFindDependenciesInANamespace() {

			var x =
				(from t in Types.InAssemblyOf<CecilAnalyzerTests>()
				 from u in Types.UsedBy(t)
				 where u.Namespace == "B"
				 select u).Count();

			// Assert
			Assert.Equal(1, x);
		}

		[Fact]
		public void SyntacticSugarCanOrderByCyclomaticComplexity() {

			var y =
				(from t in Types.InAssemblyOf<CecilAnalyzerTests>()
				 orderby t.CyclomaticComplexity() descending
				 select t).Take(20);

			y.Each(Console.WriteLine);

			Assert.Equal(20, y.Count());

			var elementsWithHigherCcThanThePrevious = y.Where((t, i) => i > 0 && t.CyclomaticComplexity() > y.ElementAt(i - 1).CyclomaticComplexity());
			Assert.False(elementsWithHigherCcThanThePrevious.Any());
		}


		[Fact]
		public void DsmCanCalculateTypeInstability() {

			var sut = new TypeDependencyStructureMatrix(new[] { typeof(A.A1).Assembly });

			double aInstability = sut.GetInstability<A.A1>();

			Assert.Equal(1, aInstability);

			double eInstability = sut.GetInstability<E>();

			Assert.Equal(0, eInstability);
		}

		[Fact]
		public void DsmCanCalculateTypeInstabilityForIsolatedType() {

			var sut = new TypeDependencyStructureMatrix(new[] { typeof(A.A1).Assembly });

			double instability = sut.GetInstability<DontUseMe>();

			Assert.Equal(0, instability);
		}

		[Fact]
		public void DsmCanCalculateTypeAbstractness() {

			var sut = new TypeDependencyStructureMatrix(new[] { typeof(A.A1).Assembly });

			double aAbstractness = sut.GetAbstractness<A.A1>();

			Assert.Equal(0, aAbstractness);

			double fAbstractness = sut.GetAbstractness<F>();

			Assert.Equal(0.5, fAbstractness);

			double gAbstractness = sut.GetAbstractness<G>();

			Assert.Equal(1.0, gAbstractness);
		}

		[Fact]
		public void DsmCanCalculateTypeAbstractnessForEmptyType() {

			var sut = new TypeDependencyStructureMatrix(new[] { typeof(A.A1).Assembly });

			double abstractness = sut.GetAbstractness<DontUseMe>();

			Assert.Equal(1.0, abstractness);
		}

		[Fact]
		public void DsmCanCalculateTypesDistanceFromMainSequence() {

			var sut = new TypeDependencyStructureMatrix(new[] { typeof(A.A1).Assembly });

			double aDistance = sut.GetDistanceFromMainSequence<A.A1>();

			Assert.Equal(0, aDistance);

			double fDistance = sut.GetDistanceFromMainSequence<F>();

			Assert.Equal(0.5, fDistance);

			double gDistance = sut.GetDistanceFromMainSequence<G>();

			Assert.Equal(0.0, gDistance);

			var tenWorstTypes =
				(from t in sut.DefinedMembers
				 let distanceFromMainSequence = sut.GetDistanceFromMainSequence(t)
				 where t.DeclaringType == null
				 orderby distanceFromMainSequence descending
				 select new { t, distanceFromMainSequence, A=sut.GetAbstractness(t), I=sut.GetInstability(t) }).Take(10);

			tenWorstTypes.Each(t => Console.WriteLine(t.t + "\t: " + t.distanceFromMainSequence + "(A=" + t.A + ", I=" + t.I + ")"));
		}

		[Fact]
		public void DsmCanCalculateAmountOfPainAndUselesnessOfAUselessType() {
			// Arrange
			var sut = new TypeDependencyStructureMatrix(new[] { typeof(A.A1).Assembly });

			// Act
			double pain = sut.GetAmountOfPain<UselessType>();
			double uselessNess = sut.GetUselesness<UselessType>();

			// Assert
			Assert.Equal(0, pain);
			Assert.Equal(1, uselessNess);
		}

		[Fact]
		public void DsmCanCalculateAmountOfPainAndUselesnessOfAPainfulType() {
			// Arrange
			var sut = new TypeDependencyStructureMatrix(new[] { typeof(A.A1).Assembly });

			// Act
			double pain = sut.GetAmountOfPain<PainfulType>();
			double uselessNess = sut.GetUselesness<PainfulType>();

			// Assert
			Assert.Equal(1, pain);
			Assert.Equal(0, uselessNess);
		}

	}
}

