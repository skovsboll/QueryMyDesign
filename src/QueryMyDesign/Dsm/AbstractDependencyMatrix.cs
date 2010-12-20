using System;
using System.Diagnostics.Contracts;
using System.Linq;
using QueryMyDesign.DependencyFinders;

namespace QueryMyDesign.Dsm {
	public abstract class AbstractDependencyMatrix<TDef, TRef>
		where TDef : class, TRef
		where TRef : class {

		public abstract TDef[] DefinedMembers { get; }
		public abstract Dependency<TRef>[] Dependencies { get; }
		public abstract AbstractDependencyFinder<TDef, TRef> DependencyFinder { get; }

		protected Dependency<TRef>[] FindDependencies() {
			return (
				 from source in DefinedMembers
				 from target in DependencyFinder.GetUses(source)
				 where DefinedMembers.Contains(target)
				 group target by new { source, target }
					 into g
					 select new Dependency<TRef>(g.Key.source, g.Key.target, g.Count())).ToArray();
		}

		public virtual int GetNumberOfIncomingDependencies(TDef definition) {
			Contract.Requires(definition != null);

			return
				(from d in Dependencies
				 where DependencyFinder.AreEquivalent(d.To, definition)
				 select d.Count
					 ).Sum();
		}

		public virtual int GetNumberOfOutgoingDependencies(TDef type) {
			Contract.Requires(type != null);

			return
				(from d in Dependencies
				 where DependencyFinder.AreEquivalent(d.From, type)
				 select d.Count
					 ).Sum();
		}

		public double GetInstability(TDef definition) {
			Contract.Requires(definition != null);

			double ce = GetNumberOfOutgoingDependencies(definition);
			double ca = GetNumberOfIncomingDependencies(definition);

			if (ca + ce == 0) {
				return 0.0;
			}

			return ce / (ca + ce);
		}

		public double GetAbstractness(TDef definition) {
			Contract.Requires(definition != null);

			int memberCount = NumberOfMembers(definition);

			if (memberCount == 0) {
				return 1.0;
			}

			return (double)NumberOfAbstractMembers(definition) / memberCount;
		}

		public double GetDistanceFromMainSequence(TDef definition) {
			Contract.Requires(definition != null);

			double a = GetAbstractness(definition);
			double i = GetInstability(definition);

			return Math.Abs(a + i - 1);
		}

		public double GetUselesness(TDef definition) {
			Contract.Requires(definition != null);

			double a = GetAbstractness(definition);
			double i = GetInstability(definition);

			double uselessNess = a + i - 1;
			return uselessNess > 0 ? uselessNess : 0;
		}

		public double GetAmountOfPain(TDef definition) {
			Contract.Requires(definition != null);

			double a = GetAbstractness(definition);
			double i = GetInstability(definition);

			double pain = a + i - 1;
			return pain < 0 ? -pain : 0;
		}

		public abstract int NumberOfAbstractMembers(TDef definition);

		public abstract int NumberOfMembers(TDef definition);
		}
}