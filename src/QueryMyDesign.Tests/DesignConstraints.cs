using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using QueryMyDesign.Dsm;
using QueryMyDesign.Sugar;
using Xunit;

namespace SkinnyDip.Tests {
	public class DesignConstraints {

		[Fact]
		public void CheckDesignConstraintsOfSkinnyDip() {
			var sut = typeof(CecilExtensions).Assembly;

			Assert.True(Methods.In(sut).Any());

			// WARN IF Count > 0 IN SELECT METHODS WHERE NbILInstructions > 200 ORDER BY NbILInstructions DESC
			Assert.Empty(from m in Methods.In(sut) where m.NumberOfInstructions() > 200 select m);

			// WARN IF Count > 0 IN SELECT METHODS WHERE ILCyclomaticComplexity > 40 ORDER BY ILCyclomaticComplexity DESC
			Assert.Empty(from m in Methods.In(sut) where m.CyclomaticComplexity() > 40 select m);

			// WARN IF Count > 0 IN SELECT METHODS WHERE NbParameters > 5 ORDER BY NbParameters DESC
			Assert.Empty(from m in Methods.In(sut) where m.Parameters.Count > 5 select m);

			// WARN IF Count > 0 IN SELECT METHODS WHERE NbVariables > 15 ORDER BY NbVariables DESC
			Assert.Empty(from m in Methods.In(sut) where m.NumberOfVariables() > 15 select m);

			// SELECT METHODS WHERE IsUsing "System.Xml.XmlWriter..ctor()" ORDER BY DepthOfIsUsing
			var typeDefinitions =
				(from frameworkType in Types.In(sut)
				 from testType in Types.In(typeof(DesignConstraints).Assembly)
				 where frameworkType.FindUsesOf(testType).Any()
				 select new { frameworkType, testType }).ToArray();

			foreach (var pair in typeDefinitions)
				Console.WriteLine("{0} used by {1}", pair.testType, pair.frameworkType);
			Assert.Empty(typeDefinitions);

			Assert.Empty(from t in Types.In(sut) where t.FindUsesOf(typeof(DesignConstraints).GetTypeDefinition()).Any() select t);

			Assert.Empty(from t in Types.In(sut) where t.CountUsesOfNamespace("SkinnyDip.Tests") > 0 select t);
		}

		//[Fact]
		public void CheckDesignConstraintsOfQueryMyDesign() {
			var sut = new TypeDependencyStructureMatrix(new[] { typeof(CecilExtensions).Assembly });

			var problematicTypes =
				from t in sut.DefinedMembers
				let distanceFromMainSequence = sut.GetDistanceFromMainSequence(t)
				where distanceFromMainSequence > 0.5
				&& !t.Namespace.StartsWith("System.Diagnostics.Contracts")
				&& !t.Namespace.Contains("JetBrains")
				orderby distanceFromMainSequence descending 
				select new {t, Pain = sut.GetAmountOfPain(t), Uselesness = sut.GetUselesness(t), I = sut.GetInstability(t), A = sut.GetAbstractness(t)};

			problematicTypes.Each(Console.WriteLine);

			Assert.Empty(problematicTypes);
		}


		private void ReferencesToString() {
			string s = "";

			var sometype = typeof(string);

			bool x = string.IsNullOrEmpty("dfkhj");

			s.ToLower();

			new string('s', 3);

			new object().ToString();
		}
	}
}